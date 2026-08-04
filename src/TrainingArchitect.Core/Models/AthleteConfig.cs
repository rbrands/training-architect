using System.Text.Json.Serialization;

namespace TrainingArchitect.Core.Models;

/// <summary>
/// Configuration and entitlement document for a single athlete.
/// </summary>
public sealed class AthleteConfig : CosmosDocument
{
    public const string DocumentType = "athlete_config";

    [JsonPropertyName("athleteId")]
    public string AthleteId { get; set; } = string.Empty;

    [JsonPropertyName("level")]
    public string Level { get; set; } = "basic";

    [JsonPropertyName("limits")]
    public AthleteLimits Limits { get; set; } = new();

    [JsonPropertyName("locked")]
    public bool Locked { get; set; }

    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;

    public override string Type => DocumentType;
}

/// <summary>
/// Per-action usage limits for an athlete.
/// </summary>
public sealed class AthleteLimits
{
    [JsonPropertyName("weeklyToken")]
    public int WeeklyToken { get; set; }

    [JsonPropertyName("monthlyToken")]
    public int MonthlyToken { get; set; }
}