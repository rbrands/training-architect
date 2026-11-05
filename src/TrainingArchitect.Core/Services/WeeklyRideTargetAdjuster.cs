using System.Text.Json.Nodes;
using TrainingArchitect.Core.Models;

namespace TrainingArchitect.Core.Services;

/// <summary>Adjusts generated endurance rides to close small weekly target deviations.</summary>
public static class WeeklyRideTargetAdjuster
{
    /// <summary>
    /// Tries five-minute changes to the longest Z2 step of support rides. Changes are committed
    /// only when both weekly targets can be met without changing anchors, library workouts, or key sessions.
    /// </summary>
    /// <param name="normalizedJson">The plan after step-based duration and TSS normalization.</param>
    /// <param name="request">The weekly planning request and effective athlete dataset.</param>
    /// <param name="adjusted">Whether an adjusted plan was returned.</param>
    /// <returns>The adjusted normalized JSON, or the original JSON when adjustment is not possible.</returns>
    public static string Adjust(string normalizedJson, PlanRequest request, out bool adjusted) =>
        Adjust(normalizedJson, request, out adjusted, out _);

    /// <summary>
    /// Tries five-minute changes to the longest Z2 step of support rides and reports why no
    /// adjustment was made. When the week is short and no generated support ride exists, library
    /// aerobic-threshold rides outside anchor dates are converted to generated rides
    /// (warm-up, Z2 block, cool-down) so the Z2 block can be extended.
    /// </summary>
    /// <param name="normalizedJson">The plan after step-based duration and TSS normalization.</param>
    /// <param name="request">The weekly planning request and effective athlete dataset.</param>
    /// <param name="adjusted">Whether an adjusted plan was returned.</param>
    /// <param name="skipReason">
    /// Why the top-up did not run or did not succeed; <see langword="null"/> when the plan was adjusted
    /// or the weekly TSS is already within the ±10 % tolerance.
    /// </param>
    /// <returns>The adjusted normalized JSON, or the original JSON when adjustment is not possible.</returns>
    public static string Adjust(string normalizedJson, PlanRequest request, out bool adjusted, out string? skipReason)
    {
        adjusted = false;
        skipReason = null;
        if (request.Mode != PlanningMode.Week)
        {
            skipReason = "not a week plan";
            return normalizedJson;
        }

        var budget = WeeklyRideBudget.Resolve(request);
        if (budget.TssTarget is not { } target || !double.IsFinite(target))
        {
            skipReason = "no weekly TSS target";
            return normalizedJson;
        }

        var root = JsonNode.Parse(normalizedJson);
        if (root?["workouts"] is not JsonArray workouts)
        {
            skipReason = "no workouts array";
            return normalizedJson;
        }
        var rides = workouts.OfType<JsonObject>().Where(WeeklyRideBudget.IsRide).ToArray();
        if (rides.Any(ride => WeeklyRideBudget.ReadNumber(ride, "tss") is null
            || WeeklyRideBudget.ReadNumber(ride, "duration_minutes") is null))
        {
            skipReason = "a ride lacks tss or duration_minutes";
            return normalizedJson;
        }

        var totalTss = (budget.CompletedTss ?? 0) + rides.Sum(ride => WeeklyRideBudget.ReadNumber(ride, "tss")!.Value);
        var minimumTss = target * 0.9;
        var maximumTss = target * 1.1;
        if (totalTss >= minimumTss && totalTss <= maximumTss)
            return normalizedJson;
        if (totalTss < target * 0.75 || totalTss > target * 1.25)
        {
            skipReason = "deviation outside the ±25 % top-up range";
            return normalizedJson;
        }
        if (totalTss < minimumTss && WeeklyLoadTargetResolver.IsRaceWeek(request.WeekDataJson, request.Scope))
        {
            skipReason = "race week";
            return normalizedJson;
        }

        var data = JsonNode.Parse(request.WeekDataJson);
        var completedActivities = request.Scope == PlanningScope.CurrentWeek && data?["activities"] is JsonArray activities
            ? activities.OfType<JsonObject>().ToArray()
            : [];
        var anchors = WeeklyRideBudget.ReadAnchors(data, request.Scope);
        var week = WeeklyRideBudget.TargetWeekStart(data, request.Scope);
        if (week is null || rides.Any(ride => WeeklyRideBudget.ReadDate(ride) is not { } date
            || date < week || date >= week.Value.AddDays(7)))
        {
            skipReason = week is null ? "target week unknown" : "ride outside target week";
            return normalizedJson;
        }
        var constraints = ReadDayConstraints(data, week.Value);
        var direction = totalTss < minimumTss ? 1 : -1;
        var freeRides = rides.Where(ride => !anchors.Any(anchor => SameAnchorDate(ride, anchor))).ToArray();
        var candidates = freeRides
            .Select(CreateCandidate)
            .OfType<Candidate>()
            .OrderBy(candidate => WeeklyRideBudget.ReadDate(candidate.Workout))
            .ToArray();
        if (candidates.Length == 0 && direction > 0)
        {
            var converted = freeRides.Where(IsLibraryAerobicRide).ToArray();
            foreach (var ride in converted)
            {
                var libraryTss = ride["tss"]!.GetValue<double>();
                ConvertLibraryRide(ride);
                totalTss += ride["tss"]!.GetValue<double>() - libraryTss;
            }
            candidates = converted
                .Select(CreateCandidate)
                .OfType<Candidate>()
                .OrderBy(candidate => WeeklyRideBudget.ReadDate(candidate.Workout))
                .ToArray();
        }
        if (candidates.Length == 0)
        {
            skipReason = "no eligible generated aerobic/recovery ride";
            return normalizedJson;
        }
        var totalHours = budget.CompletedHours + rides.Sum(WeeklyRideBudget.ReadHours);
        var minimumHours = budget.HoursTarget * 0.9 ?? 0;
        var maximumHours = budget.HoursTarget * 1.1 ?? double.PositiveInfinity;

        while (true)
        {
            if (totalTss >= minimumTss && totalTss <= maximumTss
                && totalHours >= minimumHours && totalHours <= maximumHours
                && constraints.All(constraint => WeeklyRideBudget.ReadNumber(constraint, "max_training_time_hours") is not { } maximum
                    || workouts.OfType<JsonObject>()
                        .Where(entry => WeeklyRideBudget.ReadDate(entry) == WeeklyRideBudget.ReadDate(constraint))
                        .Sum(WeeklyRideBudget.ReadHours)
                        + completedActivities.Where(entry => WeeklyRideBudget.ReadDate(entry) == WeeklyRideBudget.ReadDate(constraint))
                            .Sum(WeeklyRideBudget.ReadHours) <= maximum))
            {
                adjusted = true;
                return PlanUploadNormalizer.Normalize(root!.ToJsonString(), request: request);
            }

            var changed = false;
            foreach (var candidate in candidates)
            {
                var workout = candidate.Workout;
                var date = WeeklyRideBudget.ReadDate(workout)!.Value;
                var constraint = constraints.FirstOrDefault(entry => WeeklyRideBudget.ReadDate(entry) == date);
                if (constraint?["training_allowed"] is JsonValue allowed && allowed.TryGetValue<bool>(out var canTrain) && !canTrain
                    || request.Constraints?.DayConstraints.Any(day => day.Day == date.DayOfWeek
                        && day.Availability is DayAvailability.Unavailable or DayAvailability.Race) == true)
                    continue;

                var step = LongestZ2Step(candidate.Steps)!;
                var oldSeconds = step["duration_seconds"]!.GetValue<double>();
                step["duration_seconds"] = oldSeconds + direction * 300;
                var valid = PlanUploadNormalizer.TryGetStepTotals(candidate.Steps, out var seconds, out var tss);
                var minutes = Math.Round(seconds / 60);
                var nextTss = totalTss - workout["tss"]!.GetValue<double>() + Math.Round(tss ?? 0);
                var nextHours = totalHours + (minutes - workout["duration_minutes"]!.GetValue<double>()) / 60;
                var dailyHours = workouts.OfType<JsonObject>()
                    .Where(entry => WeeklyRideBudget.ReadDate(entry) == date)
                    .Sum(WeeklyRideBudget.ReadHours)
                    + completedActivities.Where(entry => WeeklyRideBudget.ReadDate(entry) == date)
                        .Sum(WeeklyRideBudget.ReadHours)
                    + (minutes - workout["duration_minutes"]!.GetValue<double>()) / 60;
                var dailyMaximum = WeeklyRideBudget.ReadNumber(constraint, "max_training_time_hours") ?? 24;
                if (!valid || tss is null || seconds < 1800
                    || direction > 0 && dailyHours > dailyMaximum
                    || direction > 0 && (nextTss > maximumTss || nextHours > maximumHours)
                    || direction < 0 && (nextTss < minimumTss || nextHours < minimumHours))
                {
                    step["duration_seconds"] = oldSeconds;
                    continue;
                }

                workout["tss"] = Math.Round(tss.Value);
                workout["duration_minutes"] = minutes;
                totalTss = nextTss;
                totalHours = nextHours;
                changed = true;
                break;
            }
            if (!changed)
            {
                skipReason = "time budget, day cap or weekly bounds prevent reaching the target";
                return normalizedJson;
            }
        }
    }

    private static Candidate? CreateCandidate(JsonObject workout)
    {
        if (workout["library_workout_id"] is not null
            || workout["tags"] is not JsonArray { Count: > 0 } tags
            || !tags.All(tag => tag is JsonValue value && value.TryGetValue<string>(out var text)
                && (text.StartsWith("aerobic-threshold-", StringComparison.Ordinal)
                    || text.StartsWith("recovery-", StringComparison.Ordinal)))
            || workout["steps"] is not JsonArray { Count: > 0 } steps
            || !PlanUploadNormalizer.TryGetStepTotals(steps, out _, out var tss) || tss is null
            || steps.OfType<JsonObject>().Any(step => step["power_pct_ftp"]!.GetValue<double>() > 75))
            return null;

        return LongestZ2Step(steps) is null ? null : new Candidate(workout, steps);
    }

    private static bool IsLibraryAerobicRide(JsonObject workout) =>
        workout["library_workout_id"] is not null
        && workout["tags"] is JsonArray { Count: > 0 } tags
        && tags.All(tag => tag is JsonValue value && value.TryGetValue<string>(out var text)
            && text.StartsWith("aerobic-threshold-", StringComparison.Ordinal))
        && WeeklyRideBudget.ReadNumber(workout, "duration_minutes") is > 0;

    // Library steps are stripped during normalization, so the ride is rebuilt as warm-up, Z2 block and cool-down
    // with the same planned duration; the Z2 block can then be extended like any generated support ride.
    private static void ConvertLibraryRide(JsonObject workout)
    {
        var totalSeconds = WeeklyRideBudget.ReadNumber(workout, "duration_minutes")!.Value * 60;
        var steps = new JsonArray(
            new JsonObject { ["duration_seconds"] = 600d, ["power_pct_ftp"] = 50d },
            new JsonObject { ["duration_seconds"] = Math.Max(totalSeconds - 900, 600), ["power_pct_ftp"] = 65d },
            new JsonObject { ["duration_seconds"] = 300d, ["power_pct_ftp"] = 50d });
        PlanUploadNormalizer.TryGetStepTotals(steps, out var seconds, out var tss);
        workout.Remove("library_workout_id");
        workout["steps"] = steps;
        workout["duration_minutes"] = Math.Round(seconds / 60);
        workout["tss"] = Math.Round(tss!.Value);
    }

    private static JsonObject? LongestZ2Step(JsonArray steps) =>
        steps.OfType<JsonObject>()
            .Where(step => step["power_pct_ftp"]!.GetValue<double>() is > 55 and <= 75)
            .OrderByDescending(step => step["duration_seconds"]!.GetValue<double>())
            .FirstOrDefault();

    // Conservatively protect every ride on an anchored date, including anchors without IDs.
    private static bool SameAnchorDate(JsonObject workout, JsonObject anchor) =>
        WeeklyRideBudget.ReadDate(workout) == WeeklyRideBudget.ReadDate(anchor);

    private static JsonObject[] ReadDayConstraints(JsonNode? data, DateOnly week)
    {
        if (data?["week_summary"]?["training_plan"] is not JsonArray entries)
            return [];
        var target = entries.OfType<JsonObject>().FirstOrDefault(entry => entry["week"]?.GetValue<string>() == week.ToString("yyyy-MM-dd"));
        return target?["day_constraints"] is JsonArray constraints ? constraints.OfType<JsonObject>().ToArray() : [];
    }

    private sealed record Candidate(JsonObject Workout, JsonArray Steps);
}
