using BrandsAdvisory.Client.Models;
using BrandsAdvisory.Client.Services;
using BrandsAdvisory.Core.Interfaces;
using BrandsAdvisory.Core.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Syncfusion.Blazor;
using System.Net.Http.Json;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

// Fetch non-sensitive configuration from the server before initializing services.
// This keeps secrets out of wwwroot/appsettings.json (which is publicly accessible).
var http = new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) };
var clientConfig = await http.GetFromJsonAsync<ClientConfig>("/api/config");

if (!string.IsNullOrEmpty(clientConfig?.SyncfusionLicenseKey) &&
    !clientConfig.SyncfusionLicenseKey.StartsWith("__"))
{
    Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense(clientConfig.SyncfusionLicenseKey);
}

builder.Services.AddSyncfusionBlazor();
builder.Services.AddSingleton(clientConfig ?? new ClientConfig());
builder.Services.AddAuthorizationCore();
builder.Services.AddCascadingAuthenticationState();

// HttpClient pointing to the host server (same origin — cookies are sent automatically)
builder.Services.AddScoped(sp =>
    new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

// Authentication state: reads current user from /api/user on the host server
builder.Services.AddScoped<AuthenticationStateProvider, ApiAuthenticationStateProvider>();

// Repositories: call minimal API endpoints on the host server
builder.Services.AddScoped<IAboutRepository, HttpAboutRepository>();
builder.Services.AddScoped<IProjectRepository, HttpProjectRepository>();
builder.Services.AddScoped<IArticleRepository, HttpArticleRepository>();

// OwnerService: same logic as server-side — checks user.IsInRole("SiteAdmin")
builder.Services.AddScoped<IOwnerService, OwnerService>();

await builder.Build().RunAsync();
