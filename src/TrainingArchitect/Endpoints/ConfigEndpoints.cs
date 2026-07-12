namespace TrainingArchitect.Endpoints;

/// <summary>
/// Provides safe client configuration from server-side settings.
/// Only non-sensitive values that the Blazor WebAssembly client needs
/// at startup should be exposed here.
/// </summary>
public static class ConfigEndpoints
{
    public static void MapConfigEndpoints(this WebApplication app)
    {
        app.MapGet("/api/config", (IConfiguration config) =>
        {
            var blobEndpoint = config["Storage:BlobEndpoint"] ?? string.Empty;
            var imageContainerUrl = blobEndpoint.TrimEnd('/') + "/images/";
            var syncfusionLicenseKey = config["Syncfusion:LicenseKey"] ?? string.Empty;

            if (syncfusionLicenseKey.StartsWith("__", StringComparison.Ordinal)
                || syncfusionLicenseKey.StartsWith("@Microsoft.KeyVault(", StringComparison.OrdinalIgnoreCase))
            {
                syncfusionLicenseKey = string.Empty;
            }

            return Results.Ok(new ClientConfig
            {
                SyncfusionLicenseKey = syncfusionLicenseKey,
                ImageContainerUrl = imageContainerUrl
            });
        })
        .AllowAnonymous()
        .WithName("GetClientConfig");
    }
}

public record ClientConfig
{
    public string SyncfusionLicenseKey { get; init; } = string.Empty;
    public string ImageContainerUrl { get; init; } = string.Empty;
}
