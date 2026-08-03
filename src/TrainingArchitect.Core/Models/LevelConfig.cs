using System.Text.Json.Serialization;

namespace TrainingArchitect.Core.Models;

/// <summary>
/// Configuration template for one athlete level.
/// </summary>
public sealed class LevelConfig : CosmosDocument
{
    public const string DocumentType = "level";

    [JsonPropertyName("level")]
    public string Level { get; set; } = "basic";

    [JsonPropertyName("label")]
    public string Label { get; set; } = "Basic";

    [JsonPropertyName("limits")]
    public AthleteLimits Limits { get; set; } = new();

    public override string Type => DocumentType;
}