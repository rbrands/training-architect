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
