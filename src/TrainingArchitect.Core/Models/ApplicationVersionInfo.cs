namespace TrainingArchitect.Core.Models;

/// <summary>
/// Represents an application release with semantic version and release date.
/// </summary>
public sealed record ApplicationVersion(string SemVer, DateOnly ReleaseDate)
{
    /// <summary>
    /// Returns a compact display string for UI surfaces.
    /// </summary>
    public string Display => $"v{SemVer} ({ReleaseDate:yyyy-MM-dd})";
}

/// <summary>
/// Central source of truth for the currently running application version.
/// </summary>
public static class ApplicationVersionInfo
{
    /// <summary>
    /// Current release shown in the client UI and aligned with CHANGELOG.md.
    /// </summary>
    public static readonly ApplicationVersion Current = new("0.1.0", new DateOnly(2026, 6, 27));
}
