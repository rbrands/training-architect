using TrainingArchitect.Client.Models;
using TrainingArchitect.Client.Services;
using TrainingArchitect.Core.Interfaces;
using TrainingArchitect.Core.Services;
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
builder.Services.AddScoped<IUsageCounterRepository, HttpUsageCounterRepository>();
// Creditals store: persists Athlete-ID + API-Key in localStorage (opt-in via "remember")
builder.Services.AddScoped<ICredentialStore, CredentialStore>(); // ersetzt NullCredentialStore
// Coach-Session-State: Scoped = one instance per user (in WASM effectively singleton)
builder.Services.AddScoped<IAthleteSession, AthleteSession>();
builder.Services.AddScoped<CoachPageState>();
builder.Services.AddScoped<IAthleteDataClient, HttpAthleteDataClient>();

// OwnerService: same logic as server-side — checks user.IsInRole("SiteAdmin")
builder.Services.AddScoped<IOwnerService, OwnerService>();

// Training Architect stubs — replace with real implementations once the
// orchestration layer is connected.
// TODO: IChatService should be scoped (one conversation per WASM circuit/session).
builder.Services.AddScoped<IChatService, StubChatService>();
// TODO: IIntervalsDataProvider should call the real intervals.icu API server-side.
builder.Services.AddScoped<IIntervalsDataProvider, StubIntervalsDataProvider>();

await builder.Build().RunAsync();
