using System.Globalization;
using System.Text.Json;
using System.Text.RegularExpressions;
using TrainingArchitect.Core.Models;
using TrainingArchitect.Core.Services;

namespace TrainingArchitect.Services;

/// <summary>
/// Validates the structure of the upload JSON and verifies the weekly TSS via the
/// <c>check_plan_tss</c> MCP tool. The TSS check is a backstop: technical failures never
/// block plan generation. Next-training recommendations may contain one main workout
/// and one same-day strength session, and are not compared with a full week's TSS target.
/// </summary>
public sealed class TssPlanValidator(
    IAthleteDataService athleteDataService,
    ILogger<TssPlanValidator> logger) : IPlanValidator
{
    private static readonly Regex StrengthTagRegex = new(
        @"\Astrength-(adaptation|transition|maximum|maintenance)\z",
        RegexOptions.CultureInvariant);

    /// <inheritdoc />
    public async Task<PlanValidationResult> ValidateAsync(
        string uploadJson,
        PlanRequest request,
        string? intervalsAthleteId,
        string? intervalsApiKey,
        CancellationToken ct = default)
    {
        var structuralResult = ValidateStructure(uploadJson);
        if (!structuralResult.IsValid)
        {
            return structuralResult;
        }

        var strengthPhaseFinding = ValidateRequestedStrengthPhase(uploadJson, request.StrengthPhase);
        if (strengthPhaseFinding is not null)
        {
            return strengthPhaseFinding;
        }

        if (request.Mode == PlanningMode.NextTraining)
        {
            if (!HasValidNextTrainingWorkoutCombination(uploadJson))
            {
                return PlanValidationResult.FromFinding(
                    "plan.single_workout_required",
                    "Return one main workout, optionally plus one strength session on the same day. Keep the easier alternative in prose only.");
            }

            var strengthFinding = ValidateStrengthEntries(uploadJson);
            return strengthFinding
                ?? WithSummary(PlanValidationResult.Valid, "Next-training structure checked; weekly TSS target check does not apply.");
        }

        var weeklyStrengthFinding = ValidateStrengthEntries(uploadJson);
        if (weeklyStrengthFinding is not null)
        {
            return weeklyStrengthFinding;
        }

        var isRaceWeek = WeeklyLoadTargetResolver.IsRaceWeek(request.WeekDataJson, request.Scope);
        var duplicateDateFinding = ValidateOneWorkoutPerDay(uploadJson, request.WeekDataJson, request.Scope);
        if (duplicateDateFinding is not null)
        {
            return duplicateDateFinding;
        }

        var timeTarget = WeeklyTimeTargetResolver.Resolve(request.WeekDataJson, request.Scope);
        if (timeTarget is > 0)
        {
            var plannedHours = ResolvePlannedHours(uploadJson)
                + ResolveCompletedHours(request.WeekDataJson, request.Scope);
            // Race weeks are tapered and the race itself is not part of the plan, so only the upper bound applies.
            if ((!isRaceWeek && plannedHours < timeTarget.Value * 0.9) || plannedHours > timeTarget.Value * 1.1)
            {
                return PlanValidationResult.FromFinding(
                    "plan.duration_deviation",
                    $"The planned weekly duration of {plannedHours:0.#} hours is outside the ±10% range for the target of {timeTarget.Value:0.#} hours. Adjust workout durations to bring the weekly total within range.");
            }
        }

        var loadTarget = request.Constraints?.WeeklyTssTarget is > 0
            ? request.Constraints.WeeklyTssTarget.Value
            : WeeklyLoadTargetResolver.Resolve(request.WeekDataJson, request.Scope);

        if (loadTarget is null or <= 0)
        {
            logger.LogWarning("Skipping TSS validation because no weekly load target could be resolved.");
            var skipSummary = timeTarget is > 0
                ? "Duration check passed; TSS check skipped: no weekly TSS target in the dataset."
                : "TSS check skipped: no weekly TSS target in the dataset.";
            return WithSummary(PlanValidationResult.Valid, skipSummary);
        }

        if (string.IsNullOrWhiteSpace(intervalsAthleteId) || string.IsNullOrWhiteSpace(intervalsApiKey))
        {
            logger.LogWarning("Skipping TSS validation because intervals.icu credentials are missing.");
            return WithSummary(PlanValidationResult.Valid, "TSS check skipped: intervals.icu credentials are missing.");
        }

        PlanTssCheckResult checkResult;

        try
        {
            checkResult = await athleteDataService.CheckPlanTssAsync(
                intervalsAthleteId,
                intervalsApiKey,
                uploadJson,
                loadTarget.Value,
                completedTss: request.Scope == PlanningScope.CurrentWeek
                    ? ResolveCompletedTss(request.WeekDataJson)
                    : null,
                tolerancePct:  null,
                ct);
        }
        catch (OperationCanceledException) when (ct.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "TSS validation could not be performed; accepting the plan without a TSS check.");
            return WithSummary(PlanValidationResult.Valid, "TSS check unavailable: the plan was accepted without verification.");
        }

        var onlyUncountedMismatches = checkResult.Workouts.Any(workout => workout.Counted == false && workout.Mismatch)
            && checkResult.Workouts.All(workout => workout.Counted == false || !workout.Mismatch)
            && checkResult.Week is not { WithinTolerance: false };
        var summary = BuildSummary(
            onlyUncountedMismatches ? checkResult with { Valid = true } : checkResult);

        if (checkResult.Valid || onlyUncountedMismatches)
        {
            return WithSummary(PlanValidationResult.Valid, summary);
        }

        if (isRaceWeek
            && checkResult.Week is { DeviationPct: < 0 }
            && checkResult.Workouts.All(workout => workout.Counted == false || !workout.Mismatch))
        {
            return WithSummary(PlanValidationResult.Valid, $"{summary} Accepted: tapered race week below target.");
        }

        var issues = checkResult.Issues
            .Where(issue => !string.IsNullOrWhiteSpace(issue))
            .Where(issue => !IsIssueForUncountedWorkout(issue, checkResult.Workouts))
            .ToArray();
        var findings = issues.Length == 0
            ? [new PlanValidationFinding("plan.tss_deviation", BuildFallbackMessage(checkResult))]
            : issues.Select(issue => new PlanValidationFinding("plan.tss_deviation", issue)).ToArray();

        logger.LogInformation(
            "TSS validation reported {FindingCount} finding(s) for the generated plan.",
            findings.Length);

        return new PlanValidationResult(findings) { Summary = summary };
    }

    private static bool IsIssueForUncountedWorkout(
        string issue,
        IReadOnlyList<PlanTssWorkoutCheck> workouts) =>
        workouts.Any(workout =>
            workout.Counted == false
            && workout.Mismatch
            && !string.IsNullOrWhiteSpace(workout.Date)
            && issue.Contains(workout.Date, StringComparison.OrdinalIgnoreCase)
            && (issue.Contains("mismatch", StringComparison.OrdinalIgnoreCase)
                || issue.Contains("stated", StringComparison.OrdinalIgnoreCase)
                || issue.Contains("computed", StringComparison.OrdinalIgnoreCase)));

    private static bool HasValidNextTrainingWorkoutCombination(string uploadJson)
    {
        using var document = JsonDocument.Parse(uploadJson);
        var workouts = document.RootElement.GetProperty("workouts");
        var count = workouts.GetArrayLength();
        if (count is < 1 or > 2)
        {
            return false;
        }

        var entries = workouts.EnumerateArray().ToArray();
        if (entries.Any(workout => workout.ValueKind != JsonValueKind.Object
            || !TryGetDate(workout, "date", out _)))
        {
            return false;
        }

        if (count == 1)
        {
            return true;
        }

        if (!TryGetDate(entries[0], "date", out var firstDate)
            || !TryGetDate(entries[1], "date", out var secondDate)
            || firstDate != secondDate)
        {
            return false;
        }

        var firstType = ReadActivityType(entries[0]);
        var secondType = ReadActivityType(entries[1]);
        return ActivityTypeCategories.IsRide(firstType) && ActivityTypeCategories.IsStrength(secondType)
            || ActivityTypeCategories.IsStrength(firstType) && ActivityTypeCategories.IsRide(secondType);
    }

    /// <summary>
    /// Weekly plans may schedule at most one ride and one strength session per calendar day.
    /// Days that already carry several anchored <c>planned_workouts</c> entries are tolerated
    /// up to that anchor count for rides, because those sessions must be reproduced verbatim.
    /// </summary>
    private static PlanValidationResult? ValidateOneWorkoutPerDay(string uploadJson, string? weekDataJson, PlanningScope scope)
    {
        var workoutsPerDate = new Dictionary<DateOnly, (int Rides, int Strength)>();
        using (var document = JsonDocument.Parse(uploadJson))
        {
            foreach (var workout in document.RootElement.GetProperty("workouts").EnumerateArray())
            {
                if (workout.ValueKind == JsonValueKind.Object && TryGetDate(workout, "date", out var date))
                {
                    var counts = workoutsPerDate.GetValueOrDefault(date);
                    var activityType = ReadActivityType(workout);
                    if (ActivityTypeCategories.IsRide(activityType))
                    {
                        counts.Rides++;
                    }
                    else if (ActivityTypeCategories.IsStrength(activityType))
                    {
                        counts.Strength++;
                    }

                    workoutsPerDate[date] = counts;
                }
            }
        }

        var anchorsPerDate = ResolvePlannedWorkoutsPerDate(weekDataJson, scope);
        var duplicateDates = workoutsPerDate
            .Where(entry =>
                entry.Value.Rides > Math.Max(1, anchorsPerDate.GetValueOrDefault(entry.Key))
                || entry.Value.Strength > 1)
            .Select(entry => entry.Key)
            .OrderBy(date => date)
            .ToArray();

        if (duplicateDates.Length == 0)
        {
            return null;
        }

        var dateList = string.Join(", ", duplicateDates.Select(date => date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)));
        return PlanValidationResult.FromFinding(
            "plan.duplicate_date",
            $"More than one ride or more than one strength session is scheduled on {dateList}. Schedule at most one ride and one strength session per day; one ride plus one strength session is valid.");
    }

    private static PlanValidationResult? ValidateRequestedStrengthPhase(string uploadJson, string strengthPhase)
    {
        var canonicalPhase = strengthPhase.Trim().ToLowerInvariant();
        using var document = JsonDocument.Parse(uploadJson);
        var strengthEntries = document.RootElement.GetProperty("workouts").EnumerateArray()
            .Where(workout => workout.ValueKind == JsonValueKind.Object
                && ActivityTypeCategories.IsStrength(ReadActivityType(workout)))
            .ToArray();

        if (canonicalPhase == "none" && strengthEntries.Length > 0)
        {
            return PlanValidationResult.FromFinding(
                "plan.strength_not_requested",
                "The requested strength phase is none. Remove all WeightTraining entries and do not mention strength in the plan.");
        }

        if (canonicalPhase is not ("adaptation" or "transition" or "maximum" or "maintenance")
            || strengthEntries.Length == 0)
        {
            return null;
        }

        var expectedTag = $"strength-{canonicalPhase}";
        foreach (var workout in strengthEntries)
        {
            var strengthTags = workout.TryGetProperty("tags", out var tags)
                && tags.ValueKind == JsonValueKind.Array
                    ? tags.EnumerateArray()
                        .Where(tag => tag.ValueKind == JsonValueKind.String)
                        .Select(tag => tag.GetString()!)
                        .Where(tag => StrengthTagRegex.IsMatch(tag))
                        .ToArray()
                    : [];

            if (strengthTags.Length != 1 || !string.Equals(strengthTags[0], expectedTag, StringComparison.Ordinal))
            {
                return PlanValidationResult.FromFinding(
                    "plan.strength_phase_mismatch",
                    $"Every WeightTraining entry must carry exactly the requested tag `{expectedTag}`.");
            }
        }

        return null;
    }

    private static PlanValidationResult? ValidateStrengthEntries(string uploadJson)
    {
        var invalidDates = new HashSet<string>(StringComparer.Ordinal);
        using var document = JsonDocument.Parse(uploadJson);
        foreach (var workout in document.RootElement.GetProperty("workouts").EnumerateArray())
        {
            if (workout.ValueKind != JsonValueKind.Object
                || !ActivityTypeCategories.IsStrength(ReadActivityType(workout)))
            {
                continue;
            }

            var matchingStrengthTags = workout.TryGetProperty("tags", out var tags)
                && tags.ValueKind == JsonValueKind.Array
                    ? tags.EnumerateArray()
                        .Where(tag => tag.ValueKind == JsonValueKind.String)
                        .Select(tag => tag.GetString()!)
                        .Count(tag => StrengthTagRegex.IsMatch(tag))
                    : 0;
            var hasNonEmptySteps = workout.TryGetProperty("steps", out var steps)
                && steps.ValueKind != JsonValueKind.Null
                && (steps.ValueKind != JsonValueKind.Array || steps.GetArrayLength() > 0);
            var hasValidDuration = workout.TryGetProperty("duration_minutes", out var duration)
                && duration.ValueKind == JsonValueKind.Number
                && duration.TryGetDouble(out var minutes)
                && double.IsFinite(minutes)
                && minutes is >= 30 and <= 75;

            if (matchingStrengthTags != 1
                || hasNonEmptySteps
                || workout.TryGetProperty("library_workout_id", out _)
                || !hasValidDuration)
            {
                invalidDates.Add(GetDateLabel(workout));
            }
        }

        if (invalidDates.Count == 0)
        {
            return null;
        }

        var dateList = string.Join(", ", invalidDates.Order(StringComparer.Ordinal));
        return PlanValidationResult.FromFinding(
            "plan.strength_entry_invalid",
            $"WeightTraining entries on {dateList} must have exactly one canonical strength phase tag, no steps or library_workout_id, and a duration of 30–75 minutes.");
    }

    private static string GetDateLabel(JsonElement workout) =>
        TryGetDate(workout, "date", out var date)
            ? date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)
            : workout.TryGetProperty("date", out var property) && property.ValueKind == JsonValueKind.String
                ? property.GetString() ?? "unknown date"
                : "unknown date";

    private static string? ReadActivityType(JsonElement workout) =>
        workout.TryGetProperty("activity_type", out var activityType)
            && activityType.ValueKind == JsonValueKind.String
                ? activityType.GetString()
                : workout.TryGetProperty("type", out var type)
                    && type.ValueKind == JsonValueKind.String
                        ? type.GetString()
                        : null;

    private static Dictionary<DateOnly, int> ResolvePlannedWorkoutsPerDate(string? weekDataJson, PlanningScope scope)
    {
        var result = new Dictionary<DateOnly, int>();
        if (string.IsNullOrWhiteSpace(weekDataJson))
        {
            return result;
        }

        try
        {
            using var document = JsonDocument.Parse(weekDataJson);
            var root = document.RootElement;
            var weekBlock = scope == PlanningScope.NextWeek ? "next_week" : "current_week";
            if (root.ValueKind != JsonValueKind.Object
                || !root.TryGetProperty("planned_workouts", out var plannedWorkouts)
                || plannedWorkouts.ValueKind != JsonValueKind.Object
                || !plannedWorkouts.TryGetProperty(weekBlock, out var block)
                || block.ValueKind != JsonValueKind.Object
                || !block.TryGetProperty("planned_workouts", out var anchors)
                || anchors.ValueKind != JsonValueKind.Array)
            {
                return result;
            }

            foreach (var anchor in anchors.EnumerateArray())
            {
                if (anchor.ValueKind == JsonValueKind.Object
                    && (TryGetDate(anchor, "date", out var date) || TryGetDatePrefix(anchor, "start_date_local", out date)))
                {
                    result[date] = result.GetValueOrDefault(date) + 1;
                }
            }
        }
        catch (JsonException)
        {
            return result;
        }

        return result;
    }

    private static bool TryGetDatePrefix(JsonElement parent, string propertyName, out DateOnly value)
    {
        value = default;
        if (!parent.TryGetProperty(propertyName, out var property) || property.ValueKind != JsonValueKind.String)
        {
            return false;
        }

        var text = property.GetString();
        return text is { Length: >= 10 }
            && DateOnly.TryParseExact(text[..10], "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out value);
    }

    private static double ResolvePlannedHours(string uploadJson)
    {
        using var document = JsonDocument.Parse(uploadJson);
        var totalMinutes = 0d;

        foreach (var workout in document.RootElement.GetProperty("workouts").EnumerateArray())
        {
            if (workout.ValueKind == JsonValueKind.Object
                && ActivityTypeCategories.IsRide(ReadActivityType(workout))
                && workout.TryGetProperty("duration_minutes", out var duration)
                && duration.ValueKind == JsonValueKind.Number
                && duration.TryGetDouble(out var minutes)
                && double.IsFinite(minutes)
                && minutes > 0)
            {
                totalMinutes += minutes;
            }
        }

        return totalMinutes / 60;
    }

    private static double ResolveCompletedHours(string? weekDataJson, PlanningScope scope)
    {
        if (scope != PlanningScope.CurrentWeek || string.IsNullOrWhiteSpace(weekDataJson))
        {
            return 0;
        }

        try
        {
            using var document = JsonDocument.Parse(weekDataJson);
            var root = document.RootElement;
            if (root.ValueKind != JsonValueKind.Object)
            {
                return 0;
            }

            if (root.TryGetProperty("week_summary", out var weekSummary)
                && weekSummary.ValueKind == JsonValueKind.Object
                && TryGetNonNegativeDouble(weekSummary, "ride_time_hours", out var rideTimeHours))
            {
                return rideTimeHours;
            }

            if (!TryGetDate(root, "week_starting", out var weekStart)
                || !root.TryGetProperty("activities", out var activities)
                || activities.ValueKind != JsonValueKind.Array)
            {
                return 0;
            }

            var weekEnd = weekStart.AddDays(7);
            var totalHours = 0d;
            foreach (var activity in activities.EnumerateArray())
            {
                if (activity.ValueKind == JsonValueKind.Object
                    && ActivityTypeCategories.IsRide(ReadActivityType(activity))
                    && TryGetDate(activity, "date", out var activityDate)
                    && activityDate >= weekStart
                    && activityDate < weekEnd
                    && activity.TryGetProperty("duration_hours", out var duration)
                    && duration.ValueKind == JsonValueKind.Number
                    && duration.TryGetDouble(out var hours)
                    && double.IsFinite(hours)
                    && hours > 0)
                {
                    totalHours += hours;
                }
            }

            return totalHours;
        }
        catch (JsonException)
        {
            return 0;
        }
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

    private static PlanValidationResult WithSummary(PlanValidationResult result, string summary) =>
        result with { Summary = summary };

    private static double? ResolveCompletedTss(string? weekDataJson)
    {
        if (string.IsNullOrWhiteSpace(weekDataJson))
        {
            return null;
        }

        try
        {
            using var document = JsonDocument.Parse(weekDataJson);
            if (document.RootElement.ValueKind != JsonValueKind.Object
                || !document.RootElement.TryGetProperty("week_summary", out var weekSummary)
                || weekSummary.ValueKind != JsonValueKind.Object)
            {
                return null;
            }

            if (weekSummary.TryGetProperty("ride_load", out _))
            {
                return TryGetNonNegativeDouble(weekSummary, "ride_load", out var rideLoad)
                    ? rideLoad
                    : null;
            }

            return TryGetNonNegativeDouble(weekSummary, "total_training_load", out var totalTrainingLoad)
                ? totalTrainingLoad
                : null;
        }
        catch (JsonException)
        {
            return null;
        }
    }

    private static bool TryGetNonNegativeDouble(JsonElement parent, string propertyName, out double value)
    {
        value = default;
        return parent.TryGetProperty(propertyName, out var property)
            && property.ValueKind == JsonValueKind.Number
            && property.TryGetDouble(out value)
            && double.IsFinite(value)
            && value >= 0;
    }

    private static string BuildSummary(PlanTssCheckResult checkResult)
    {
        if (checkResult.Week is not { } week)
        {
            return checkResult.Valid ? "TSS check passed." : "TSS check failed.";
        }

        var outcome = checkResult.Valid ? "TSS check passed" : "TSS check failed";
        var otherTssSummary = week.PlannedOtherTss is { } plannedOtherTss
            && double.IsFinite(plannedOtherTss)
            && plannedOtherTss > 0
            ? $" (+{plannedOtherTss:0.#} TSS strength/run, not counted)"
            : string.Empty;
        return $"{outcome}: {week.TotalTss:0} TSS vs. target {week.LoadTarget:0} ({week.DeviationPct:+0.#;-0.#;0}%).{otherTssSummary}";
    }

    private static PlanValidationResult ValidateStructure(string uploadJson)
    {
        if (string.IsNullOrWhiteSpace(uploadJson))
        {
            return PlanValidationResult.FromFinding(
                "plan.upload_json_missing",
                "The response did not contain an upload JSON block between BEGIN_UPLOAD_JSON and END_UPLOAD_JSON.");
        }

        try
        {
            using var document = JsonDocument.Parse(uploadJson);

            if (document.RootElement.ValueKind != JsonValueKind.Object)
            {
                return PlanValidationResult.FromFinding(
                    "plan.upload_json_not_an_object",
                    "The upload JSON must be a single JSON object.");
            }

            if (!document.RootElement.TryGetProperty("workouts", out var workouts)
                || workouts.ValueKind != JsonValueKind.Array
                || workouts.GetArrayLength() == 0)
            {
                return PlanValidationResult.FromFinding(
                    "plan.workouts_missing",
                    "The upload JSON must contain a non-empty \"workouts\" array.");
            }
        }
        catch (JsonException ex)
        {
            return PlanValidationResult.FromFinding(
                "plan.upload_json_invalid",
                $"The upload JSON could not be parsed: {ex.Message}");
        }

        return PlanValidationResult.Valid;
    }

    private static string BuildFallbackMessage(PlanTssCheckResult checkResult) =>
        checkResult.Week is { } week
            ? $"The weekly TSS of {week.TotalTss:0.#} deviates by {week.DeviationPct:0.#}% from the target of {week.LoadTarget:0.#}."
            : "The weekly TSS check failed without further details.";
}
