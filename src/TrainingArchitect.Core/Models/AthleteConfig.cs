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
    [JsonPropertyName("plan_create")]
    public AthleteLimitWindow PlanCreate { get; set; } = new();

    [JsonPropertyName("assess_metrics")]
    public AthleteLimitWindow AssessMetrics { get; set; } = new();

    [JsonPropertyName("assess_last_training")]
    public AthleteLimitWindow AssessLastTraining { get; set; } = new();

    [JsonPropertyName("assess_week")]
    public AthleteLimitWindow AssessWeek { get; set; } = new();
}

/// <summary>
/// Weekly and monthly allowance for one action.
/// </summary>
public sealed class AthleteLimitWindow
{
    [JsonPropertyName("weekly")]
    public int Weekly { get; set; }

    [JsonPropertyName("monthly")]
    public int Monthly { get; set; }
}