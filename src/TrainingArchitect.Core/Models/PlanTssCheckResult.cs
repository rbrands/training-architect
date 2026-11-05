using System.Text.Json.Serialization;

namespace TrainingArchitect.Core.Models;

/// <summary>
/// Result of the MCP <c>check_plan_tss</c> tool for a single workout.
/// </summary>
public sealed record PlanTssWorkoutCheck
{
    /// <summary>Gets the workout date in ISO format.</summary>
    [JsonPropertyName("date")]
    public string Date { get; init; } = string.Empty;

    /// <summary>Gets the TSS declared by the generated plan.</summary>
    [JsonPropertyName("stated_tss")]
    public double StatedTss { get; init; }

    /// <summary>Gets the TSS computed from the workout structure.</summary>
    [JsonPropertyName("computed_tss")]
    public double ComputedTss { get; init; }

    /// <summary>Gets a value indicating whether stated and computed TSS differ.</summary>
    [JsonPropertyName("mismatch")]
    public bool Mismatch { get; init; }

    /// <summary>Gets whether this workout contributes to the Ride TSS check.</summary>
    [JsonPropertyName("counted")]
    public bool? Counted { get; init; }
}

/// <summary>
/// Weekly aggregate returned by the MCP <c>check_plan_tss</c> tool.
/// </summary>
public sealed record PlanTssWeekCheck
{
    /// <summary>Gets the total weekly Ride TSS of the plan.</summary>
    [JsonPropertyName("total_tss")]
    public double TotalTss { get; init; }

    /// <summary>Gets the weekly Ride TSS target the plan was checked against.</summary>
    [JsonPropertyName("load_target")]
    public double LoadTarget { get; init; }

    /// <summary>Gets the deviation from the load target in percent.</summary>
    [JsonPropertyName("deviation_pct")]
    public double DeviationPct { get; init; }

    /// <summary>Gets a value indicating whether the deviation is within tolerance.</summary>
    [JsonPropertyName("within_tolerance")]
    public bool WithinTolerance { get; init; }

    /// <summary>Gets the planned TSS from ride entries.</summary>
    [JsonPropertyName("planned_ride_tss")]
    public double? PlannedRideTss { get; init; }

    /// <summary>Gets the planned TSS from non-ride entries.</summary>
    [JsonPropertyName("planned_other_tss")]
    public double? PlannedOtherTss { get; init; }

    /// <summary>Gets the planned duration in hours from ride entries.</summary>
    [JsonPropertyName("planned_ride_hours")]
    public double? PlannedRideHours { get; init; }

    /// <summary>Gets the planned duration in hours from non-ride entries.</summary>
    [JsonPropertyName("planned_other_hours")]
    public double? PlannedOtherHours { get; init; }
}

/// <summary>
/// Full payload returned by the MCP <c>check_plan_tss</c> tool.
/// </summary>
public sealed record PlanTssCheckResult
{
    /// <summary>Gets the per-workout TSS comparison.</summary>
    [JsonPropertyName("workouts")]
    public IReadOnlyList<PlanTssWorkoutCheck> Workouts { get; init; } = [];

    /// <summary>Gets the weekly aggregate.</summary>
    [JsonPropertyName("week")]
    public PlanTssWeekCheck? Week { get; init; }

    /// <summary>Gets a value indicating whether the plan passed the TSS check.</summary>
    [JsonPropertyName("valid")]
    public bool Valid { get; init; }

    /// <summary>Gets the human readable issues describing why the check failed.</summary>
    [JsonPropertyName("issues")]
    public IReadOnlyList<string> Issues { get; init; } = [];
}
