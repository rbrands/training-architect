namespace BrandsAdvisory.Endpoints;

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
            var imageContainerUrl = blobEndpoint.TrimEnd('/') + "/article-images/";

            return Results.Ok(new ClientConfig
            {
                SyncfusionLicenseKey = config["Syncfusion:LicenseKey"] ?? string.Empty,
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
