using System.Text.Json.Serialization;

namespace TrainingArchitect.Core.Models;

/// <summary>
/// Result of the MCP <c>validate_week_plan</c> tool: a formal schema check of the upload JSON.
/// </summary>
public sealed record PlanValidationCheckResult
{
    /// <summary>Gets the validation status reported by the tool ("valid" or "invalid").</summary>
    [JsonPropertyName("status")]
    public string? Status { get; init; }

    /// <summary>Gets the schema path the plan was validated against.</summary>
    [JsonPropertyName("schema")]
    public string? Schema { get; init; }

    /// <summary>Gets the human-readable validation output (empty on success, schema errors on failure).</summary>
    [JsonPropertyName("details")]
    public string? Details { get; init; }

    /// <summary>Gets a technical error message when the tool call itself failed (for example invalid JSON).</summary>
    [JsonPropertyName("error")]
    public string? Error { get; init; }

    /// <summary>Gets a value indicating whether the plan passed the formal schema check.</summary>
    public bool IsValid => string.Equals(Status, "valid", StringComparison.OrdinalIgnoreCase);
}
