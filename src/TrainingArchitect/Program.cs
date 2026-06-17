using Azure.Extensions.AspNetCore.Configuration.Secrets;
using Azure.Identity;
using TrainingArchitect.Components;
using TrainingArchitect.Endpoints;
using TrainingArchitect.Models;
using Microsoft.AspNetCore.DataProtection;
using TrainingArchitect.Core.Interfaces;
using TrainingArchitect.Core.Services;
using TrainingArchitect.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Azure.Cosmos;
using Microsoft.Identity.Web;
using Syncfusion.Blazor;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// ---------------------------------------------------------------------------
// Key Vault Configuration (production only)
// In development, secrets are set via dotnet user-secrets.
// In production, secrets are stored in Azure Key Vault and loaded here.
// Secret naming: "Section--Key" in KV maps to "Section:Key" in config.
// Example: "Syncfusion--LicenseKey" → configuration["Syncfusion:LicenseKey"]
// Uses System-Assigned Managed Identity in production.
// Requires: Key Vault Secrets User role on the Key Vault for the Managed Identity.
// ---------------------------------------------------------------------------
if (!builder.Environment.IsDevelopment())
{
    var keyVaultUrl = builder.Configuration["KeyVault:Url"];
    if (!string.IsNullOrEmpty(keyVaultUrl))
    {
        builder.Configuration.AddAzureKeyVault(
            new Uri(keyVaultUrl),
            new DefaultAzureCredential());
    }
}

// Register Syncfusion Community License key.
// Set via user-secrets locally: dotnet user-secrets set "Syncfusion:LicenseKey" "..."
// Set via App Service Configuration in production: Syncfusion__LicenseKey
var syncfusionKey = builder.Configuration["Syncfusion:LicenseKey"];
if (!string.IsNullOrEmpty(syncfusionKey))
    Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense(syncfusionKey);

// Application Insights
// Automatically tracks requests, dependencies (Cosmos DB, Key Vault, Storage),
// exceptions, and performance metrics.
// Connection string is set via App Service configuration:
// APPLICATIONINSIGHTS_CONNECTION_STRING
// In version 3.x the SDK throws when no connection string is present,
// so we only register it when the value is actually configured.
// Authentication uses DefaultAzureCredential (Managed Identity in production,
// developer identity locally) instead of the instrumentation key.
// Requires: Monitoring Metrics Publisher role on the Application Insights resource.
if (!string.IsNullOrEmpty(builder.Configuration["APPLICATIONINSIGHTS_CONNECTION_STRING"]))
{
    builder.Services.AddApplicationInsightsTelemetry();
    builder.Services.Configure<Microsoft.ApplicationInsights.Extensibility.TelemetryConfiguration>(config =>
        config.SetAzureTokenCredential(new DefaultAzureCredential()));
}

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();
builder.Services.AddSyncfusionBlazor();
builder.Services.AddHttpContextAccessor();
builder.Services.AddCascadingAuthenticationState();

// Required for GitHub Codespaces and other reverse proxy environments:
// Trust X-Forwarded-Proto and X-Forwarded-Host so ASP.NET Core knows
// the request is HTTPS, which is needed for Secure cookies to work.
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor
        | ForwardedHeaders.XForwardedProto
        | ForwardedHeaders.XForwardedHost;
    // Clear default restrictions so the Codespaces proxy IP is trusted
    options.KnownIPNetworks.Clear();
    options.KnownProxies.Clear();
});

var dataProtectionBuilder = builder.Services.AddDataProtection()
    .SetApplicationName("training-architect");

// Persist DataProtection keys to disk in development so that
// dotnet watch restarts don't invalidate in-flight OIDC correlation cookies.
if (builder.Environment.IsDevelopment())
{
    var keysDir = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
        ".aspnet", "DataProtection-Keys", "training-architect");
    Directory.CreateDirectory(keysDir);
    dataProtectionBuilder.PersistKeysToFileSystem(new DirectoryInfo(keysDir));
}

builder.Services.AddMicrosoftIdentityWebAppAuthentication(builder.Configuration);

// Use PostConfigure so our settings run AFTER Microsoft.Identity.Web's
// own PostConfigure, which would otherwise override Configure calls.
//
// SameSite=None + SecurePolicy=Always is required in reverse proxy environments
// (production, GitHub Codespaces) where the OIDC callback crosses a site boundary.
// On Windows localhost (direct HTTPS), browser defaults apply — no override needed.
var isReverseProxy = !builder.Environment.IsDevelopment() ||
    Environment.GetEnvironmentVariable("CODESPACES") == "true";

builder.Services.PostConfigure<CookieAuthenticationOptions>(
    CookieAuthenticationDefaults.AuthenticationScheme, options =>
    {
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
    });

builder.Services.PostConfigure<OpenIdConnectOptions>(
    OpenIdConnectDefaults.AuthenticationScheme, options =>
    {
        if (isReverseProxy)
        {
            options.NonceCookie.SameSite = SameSiteMode.None;
            options.NonceCookie.SecurePolicy = CookieSecurePolicy.Always;
            options.CorrelationCookie.SameSite = SameSiteMode.None;
            options.CorrelationCookie.SecurePolicy = CookieSecurePolicy.Always;
        }

        // Fix for GitHub Codespaces: ASP.NET Core binds to 127.0.0.1
        // internally but Entra ID only accepts localhost for http URIs.
        // Chain after Microsoft.Identity.Web's existing handler (if any).
        options.Events ??= new OpenIdConnectEvents();
        var existingRedirectHandler = options.Events.OnRedirectToIdentityProvider;
        options.Events.OnRedirectToIdentityProvider = async context =>
        {
            if (existingRedirectHandler != null)
                await existingRedirectHandler(context);

            if (context.ProtocolMessage.RedirectUri?.Contains("127.0.0.1") == true)
            {
                context.ProtocolMessage.RedirectUri =
                    context.ProtocolMessage.RedirectUri
                        .Replace("127.0.0.1", "localhost");
            }
        };
    });

builder.Services.AddAuthorization();

// Cosmos DB client (singleton, thread-safe)
// Authentication via DefaultAzureCredential:
//   - locally: Azure CLI credentials (az login)
//   - production: System-Assigned Managed Identity
builder.Services.AddSingleton(sp =>
{
    var cfg = sp.GetRequiredService<IConfiguration>();
    return new CosmosClient(
        cfg["CosmosDb:EndpointUri"]!,
        new DefaultAzureCredential(),
        new CosmosClientOptions
        {
            SerializerOptions = new CosmosSerializationOptions
            {
                PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase
            }
        });
});

// Repositories
builder.Services.AddSingleton<IArticleRepository, ArticleRepository>();
builder.Services.AddSingleton<IProjectRepository, ProjectRepository>();
builder.Services.AddSingleton<IAboutRepository, AboutRepository>();

builder.Services.AddScoped<IOwnerService, OwnerService>();

// Training Architect stubs — replace with real implementations once the
// orchestration layer is connected.
// IAuthContext: resolved server-side; tier must never come from client input.
// TODO: Replace StubAuthContext with an OIDC + entitlement-store backed implementation.
builder.Services.AddScoped<IAuthContext, StubAuthContext>();
// TODO: Replace StubChatService with the AI orchestration pipeline.
builder.Services.AddScoped<IChatService, StubChatService>();
// TODO: Replace StubIntervalsDataProvider with the real intervals.icu API client.
builder.Services.AddScoped<IIntervalsDataProvider, StubIntervalsDataProvider>();

// ClientConfig must be available in server-side DI for Blazor WASM prerender.
// The WASM client fetches this at startup via /api/config; during SSR prerender
// the server resolves the same values directly from IConfiguration.
builder.Services.AddSingleton(sp =>
{
    var cfg = sp.GetRequiredService<IConfiguration>();
    var blobEndpoint = cfg["Storage:BlobEndpoint"] ?? string.Empty;
    return new TrainingArchitect.Client.Models.ClientConfig
    {
        SyncfusionLicenseKey = cfg["Syncfusion:LicenseKey"] ?? string.Empty,
        ImageContainerUrl = blobEndpoint.TrimEnd('/') + "/article-images/"
    };
});

var app = builder.Build();

// Configure the HTTP request pipeline.

// Must be first in the pipeline so all subsequent middleware
// (including HTTPS redirect and authentication) sees the correct scheme.
app.UseForwardedHeaders();

// Normalize 127.0.0.1 → localhost so the OIDC correlation cookie
// is set and sent back with the same host in both the login request
// and the Entra ID callback. Without this, the browser stores the
// cookie for 127.0.0.1 but the callback arrives at localhost (or
// vice versa), causing "Correlation failed".
app.Use(async (context, next) =>
{
    if (context.Request.Host.Host == "127.0.0.1")
    {
        context.Request.Host = new HostString(
            "localhost",
            context.Request.Host.Port ?? 5000);
    }
    await next();
});

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseAuthentication();
app.UseAuthorization();

app.UseAntiforgery();

// blazor.web.js (even in Release builds) dynamically imports the Hot Reload
// bridge module. In production the file doesn't exist → 404 → unhandled
// promise rejection → WASM never activates on Safari / Edge (Mac).
// Return an empty JS module so the import() resolves cleanly.
app.MapGet("/_content/Microsoft.DotNet.HotReload.WebAssembly.Browser/{**rest}",
    () => Results.Content(string.Empty, "application/javascript"))
    .AllowAnonymous();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(TrainingArchitect.Client._Imports).Assembly);

// User info endpoint for WASM auth state
app.MapGet("/api/user", (HttpContext context) =>
{
    if (context.User.Identity?.IsAuthenticated != true)
        return Results.Ok(new UserInfo { IsAuthenticated = false });

    var roles = context.User.Claims
        .Where(c => c.Type is ClaimTypes.Role or "roles")
        .Select(c => c.Value)
        .Distinct()
        .ToList();

    return Results.Ok(new UserInfo
    {
        IsAuthenticated = true,
        Name = context.User.Identity.Name ?? string.Empty,
        Roles = roles
    });
}).AllowAnonymous();

// Admin API endpoints (require SiteAdmin role; called by WASM client)
app.MapAboutEndpoints();
app.MapProjectEndpoints();
app.MapArticleEndpoints();
app.MapImageEndpoints();

// Client config endpoint (non-sensitive values for WASM startup)
app.MapConfigEndpoints();

// Login: redirect to Microsoft login
app.MapGet("/login", (string? returnUrl) =>
{
    var redirectUri = string.IsNullOrEmpty(returnUrl) ? "/" : returnUrl;
    return Results.Challenge(
        new AuthenticationProperties { RedirectUri = redirectUri },
        [OpenIdConnectDefaults.AuthenticationScheme]);
});

// Logout: sign out locally and from Azure AD
app.MapPost("/logout", async (HttpContext context) =>
{
    await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    await context.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme,
        new AuthenticationProperties { RedirectUri = "/" });
});

app.Run();
