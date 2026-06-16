namespace BrandsAdvisory.Client.Models;

/// <summary>
/// Non-sensitive client configuration fetched from the server at startup via /api/config.
/// </summary>
public record ClientConfig
{
    public string SyncfusionLicenseKey { get; init; } = string.Empty;
    public string ImageContainerUrl { get; init; } = string.Empty;
}
