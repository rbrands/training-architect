using System.Globalization;
using System.Text.Json.Nodes;
using TrainingArchitect.Core.Models;

namespace TrainingArchitect.Core.Services;

/// <summary>Ride-only targets and fixed load for the week being planned.</summary>
public sealed record WeeklyRideBudget(
    double? TssTarget,
    double? HoursTarget,
    double? CompletedTss,
    double CompletedHours,
    double AnchoredTss,
    double AnchoredHours)
{
    /// <summary>Resolves targets, completed rides, and anchors from the effective planning dataset.</summary>
    /// <param name="request">The request, with ignored planned workouts already removed from its dataset.</param>
    /// <returns>The ride budget for the requested week.</returns>
    public static WeeklyRideBudget Resolve(PlanRequest request)
    {
        var tssTarget = request.Constraints?.WeeklyTssTarget is > 0
            ? request.Constraints.WeeklyTssTarget
            : WeeklyLoadTargetResolver.Resolve(request.WeekDataJson, request.Scope);
        var hoursTarget = WeeklyTimeTargetResolver.Resolve(request.WeekDataJson, request.Scope);
        var data = JsonNode.Parse(string.IsNullOrWhiteSpace(request.WeekDataJson) ? "{}" : request.WeekDataJson);
        var summary = data?["week_summary"];
        var completedTss = request.Scope == PlanningScope.CurrentWeek
            ? ReadNumber(summary, "ride_load")
            : null;
        var completedHours = request.Scope == PlanningScope.CurrentWeek
            ? ReadNumber(summary, "ride_time_hours")
            : null;

        if (request.Scope == PlanningScope.CurrentWeek)
        {
            if (data?["activities"] is JsonArray activities && TargetWeekStart(data, request.Scope) is { } week)
            {
                var rides = activities.OfType<JsonObject>()
                    .Where(IsRide)
                    .Where(activity => ReadDate(activity) is { } date && date >= week && date < week.AddDays(7))
                    .ToArray();
                completedTss ??= rides.Sum(activity => ReadNumber(activity, "training_load") ?? 0);
                completedHours ??= rides.Sum(ReadHours);
            }
            else if (summary?["ride_load"] is null)
            {
                // Older datasets have only the aggregate load, as used by check_plan_tss.
                completedTss ??= ReadNumber(summary, "total_training_load");
            }
        }

        var anchors = ReadAnchors(data, request.Scope);
        return new WeeklyRideBudget(tssTarget, hoursTarget, completedTss, completedHours ?? 0,
            anchors.Where(IsRide).Sum(anchor => ReadNumber(anchor, "tss") ?? ReadNumber(anchor, "planned_load") ?? 0),
            anchors.Where(IsRide).Sum(ReadHours));
    }

    internal static IReadOnlyList<JsonObject> ReadAnchors(JsonNode? data, PlanningScope scope)
    {
        var block = scope == PlanningScope.CurrentWeek ? "current_week" : "next_week";
        if (data?["planned_workouts"]?[block]?["planned_workouts"] is not JsonArray anchors)
            return [];

        var week = TargetWeekStart(data, scope);
        return anchors.OfType<JsonObject>()
            .Where(anchor => week is null || ReadDate(anchor) is { } date && date >= week && date < week.Value.AddDays(7))
            .ToArray();
    }

    internal static DateOnly? TargetWeekStart(JsonNode? data, PlanningScope scope) =>
        DateOnly.TryParseExact(data?["week_starting"]?.GetValue<string>(), "yyyy-MM-dd",
            CultureInfo.InvariantCulture, DateTimeStyles.None, out var week)
            ? scope == PlanningScope.NextWeek ? week.AddDays(7) : week
            : null;

    internal static DateOnly? ReadDate(JsonObject entry)
    {
        var text = (entry["date"] ?? entry["start_date_local"])?.GetValue<string>();
        return text is { Length: >= 10 } && DateOnly.TryParseExact(text[..10], "yyyy-MM-dd",
            CultureInfo.InvariantCulture, DateTimeStyles.None, out var date) ? date : null;
    }

    internal static bool IsRide(JsonObject entry) =>
        ActivityTypeCategories.IsRide((entry["activity_type"] ?? entry["type"])?.GetValue<string>());

    internal static double? ReadNumber(JsonNode? entry, string name) =>
        entry?[name] is JsonValue value && value.TryGetValue<double>(out var number)
            && double.IsFinite(number) && number >= 0 ? number : null;

    internal static double ReadHours(JsonObject entry) =>
        ReadNumber(entry, "duration_minutes") is { } minutes
            ? minutes / 60 : ReadNumber(entry, "duration_hours") ?? 0;
}
