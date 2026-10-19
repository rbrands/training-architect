using System.Globalization;
using System.Text.Json;
using TrainingArchitect.Core.Models;

namespace TrainingArchitect.Core.Services;

/// <summary>
/// Resolves the weekly TSS target for a plan request from the intervals.icu dataset.
/// </summary>
public static class WeeklyLoadTargetResolver
{
    /// <summary>
    /// Reads the weekly load target of the target week from <c>week_summary.training_plan</c>.
    /// </summary>
    /// <param name="weekDataJson">The athlete dataset as JSON text.</param>
    /// <param name="scope">The planning scope that selects the target week.</param>
    /// <returns>The weekly TSS target, or <see langword="null"/> when the dataset has no target.</returns>
    public static double? Resolve(string? weekDataJson, PlanningScope scope)
    {
        if (string.IsNullOrWhiteSpace(weekDataJson))
        {
            return null;
        }

        try
        {
            using var document = JsonDocument.Parse(weekDataJson);
            var root = document.RootElement;

            if (root.ValueKind != JsonValueKind.Object
                || !TryGetDate(root, "week_starting", out var currentWeekStart)
                || !root.TryGetProperty("week_summary", out var weekSummary)
                || weekSummary.ValueKind != JsonValueKind.Object
                || !weekSummary.TryGetProperty("training_plan", out var trainingPlan)
                || trainingPlan.ValueKind != JsonValueKind.Array)
            {
                return null;
            }

            var targetWeekStart = scope == PlanningScope.NextWeek
                ? currentWeekStart.AddDays(7)
                : currentWeekStart;

            foreach (var entry in trainingPlan.EnumerateArray())
            {
                if (entry.ValueKind == JsonValueKind.Object
                    && TryGetDate(entry, "week", out var entryWeek)
                    && entryWeek == targetWeekStart
                    && entry.TryGetProperty("weekly_load_target", out var target)
                    && target.ValueKind == JsonValueKind.Number
                    && target.TryGetDouble(out var loadTarget)
                    && loadTarget > 0)
                {
                    return loadTarget;
                }
            }
        }
        catch (JsonException)
        {
            return null;
        }

        return null;
    }

    /// <summary>
    /// Determines whether the target week contains a race, either as <c>week_type</c> RACE in
    /// <c>week_summary.training_plan</c> or as an entry in <c>planned_workouts.{current|next}_week.race_events</c>.
    /// </summary>
    /// <param name="weekDataJson">The athlete dataset as JSON text.</param>
    /// <param name="scope">The planning scope that selects the target week.</param>
    /// <returns><see langword="true"/> when a race is scheduled in the target week.</returns>
    public static bool IsRaceWeek(string? weekDataJson, PlanningScope scope)
    {
        if (string.IsNullOrWhiteSpace(weekDataJson))
        {
            return false;
        }

        try
        {
            using var document = JsonDocument.Parse(weekDataJson);
            var root = document.RootElement;

            if (root.ValueKind != JsonValueKind.Object)
            {
                return false;
            }

            var weekBlock = scope == PlanningScope.NextWeek ? "next_week" : "current_week";
            if (root.TryGetProperty("planned_workouts", out var plannedWorkouts)
                && plannedWorkouts.ValueKind == JsonValueKind.Object
                && plannedWorkouts.TryGetProperty(weekBlock, out var block)
                && block.ValueKind == JsonValueKind.Object
                && block.TryGetProperty("race_events", out var raceEvents)
                && raceEvents.ValueKind == JsonValueKind.Array
                && raceEvents.GetArrayLength() > 0)
            {
                return true;
            }

            if (!TryGetDate(root, "week_starting", out var currentWeekStart)
                || !root.TryGetProperty("week_summary", out var weekSummary)
                || weekSummary.ValueKind != JsonValueKind.Object
                || !weekSummary.TryGetProperty("training_plan", out var trainingPlan)
                || trainingPlan.ValueKind != JsonValueKind.Array)
            {
                return false;
            }

            var targetWeekStart = scope == PlanningScope.NextWeek
                ? currentWeekStart.AddDays(7)
                : currentWeekStart;

            foreach (var entry in trainingPlan.EnumerateArray())
            {
                if (entry.ValueKind == JsonValueKind.Object
                    && TryGetDate(entry, "week", out var entryWeek)
                    && entryWeek == targetWeekStart)
                {
                    return entry.TryGetProperty("week_type", out var weekType)
                        && weekType.ValueKind == JsonValueKind.String
                        && string.Equals(weekType.GetString(), "RACE", StringComparison.OrdinalIgnoreCase);
                }
            }
        }
        catch (JsonException)
        {
            return false;
        }

        return false;
    }

    private static bool TryGetDate(JsonElement parent, string propertyName, out DateOnly value)
    {
        value = default;

        return parent.TryGetProperty(propertyName, out var property)
            && property.ValueKind == JsonValueKind.String
            && DateOnly.TryParseExact(
                property.GetString(),
                "yyyy-MM-dd",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out value);
    }
}
