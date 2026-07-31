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
            var syncfusionLicenseKey = NormalizeSyncfusionLicenseKey(config["Syncfusion:LicenseKey"]);

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

    private static string NormalizeSyncfusionLicenseKey(string? rawValue)
    {
        if (string.IsNullOrWhiteSpace(rawValue))
        {
            return string.Empty;
        }

        var normalized = rawValue.Trim();

        if (normalized.Length >= 2
            && normalized.StartsWith('"')
            && normalized.EndsWith('"'))
        {
            normalized = normalized[1..^1].Trim();
        }

        normalized = new string(normalized.Where(c => !char.IsWhiteSpace(c)).ToArray());

        return normalized;
    }
}

public record ClientConfig
{
    public string SyncfusionLicenseKey { get; init; } = string.Empty;
    public string ImageContainerUrl { get; init; } = string.Empty;
}
