using System.Text.Json;
using System.Text.Json.Serialization;

namespace TrainingArchitect.Core.Models;

/// <summary>
/// Request payload for the athlete data quality check.
/// </summary>
/// <param name="AthleteDataJson">Consolidated athlete dataset as returned by <c>prepare_week_data</c>.</param>
public sealed record AthleteDataCheckRequest(string AthleteDataJson);

/// <summary>
/// Result of a single area checked by the MCP <c>check_athlete_data</c> tool.
/// </summary>
public sealed record AthleteDataCheckItem
{
    /// <summary>Gets the checked area key, e.g. <c>season_plan</c>.</summary>
    [JsonPropertyName("area")]
    public string Area { get; init; } = string.Empty;

    /// <summary>Gets the area status: <c>OK</c>, <c>Warning</c>, or <c>Missing</c>.</summary>
    [JsonPropertyName("status")]
    public string Status { get; init; } = string.Empty;

    /// <summary>Gets the human readable explanation of the status.</summary>
    [JsonPropertyName("explanation")]
    public string Explanation { get; init; } = string.Empty;

    /// <summary>Gets the raw evidence object backing the status.</summary>
    [JsonPropertyName("evidence")]
    public JsonElement? Evidence { get; init; }

    /// <summary>Gets additional context notes that do not affect the status.</summary>
    [JsonPropertyName("context")]
    public JsonElement? Context { get; init; }
}

/// <summary>
/// Full payload returned by the MCP <c>check_athlete_data</c> tool.
/// </summary>
public sealed record AthleteDataCheckResult
{
    /// <summary>Gets the reference date of the checked dataset.</summary>
    [JsonPropertyName("current_date")]
    public string? CurrentDate { get; init; }

    /// <summary>Gets the Monday of the checked week.</summary>
    [JsonPropertyName("week_starting")]
    public string? WeekStarting { get; init; }

    /// <summary>Gets the activity lookback window in days.</summary>
    [JsonPropertyName("lookback_days")]
    public int? LookbackDays { get; init; }

    /// <summary>Gets the per-area check results.</summary>
    [JsonPropertyName("checks")]
    public IReadOnlyList<AthleteDataCheckItem> Checks { get; init; } = [];

    /// <summary>Gets the overall completeness rating: <c>Good</c>, <c>Partial</c>, or <c>Limited</c>.</summary>
    [JsonPropertyName("overall")]
    public string Overall { get; init; } = string.Empty;

    /// <summary>Gets the number of areas whose status is not <c>OK</c>.</summary>
    [JsonPropertyName("non_ok_count")]
    public int NonOkCount { get; init; }

    /// <summary>Gets the single highest-priority data fix, if any.</summary>
    [JsonPropertyName("suggested_fix")]
    public string? SuggestedFix { get; init; }

    /// <summary>Gets the plain-text report with one line per area plus a summary line.</summary>
    [JsonPropertyName("report_text")]
    public string? ReportText { get; init; }
}
