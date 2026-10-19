using System.Globalization;
using System.Text.Json;
using TrainingArchitect.Core.Models;
using TrainingArchitect.Core.Services;

namespace TrainingArchitect.Services;

/// <summary>
/// Validates the structure of the upload JSON and verifies the weekly TSS via the
/// <c>check_plan_tss</c> MCP tool. The TSS check is a backstop: technical failures never
/// block plan generation. Single-session recommendations must contain exactly one workout
/// and are not compared with a full week's TSS target.
/// </summary>
public sealed class TssPlanValidator(
    IAthleteDataService athleteDataService,
    ILogger<TssPlanValidator> logger) : IPlanValidator
{
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

        if (request.Mode == PlanningMode.NextTraining)
        {
            using var document = JsonDocument.Parse(uploadJson);
            return document.RootElement.GetProperty("workouts").GetArrayLength() == 1
                ? WithSummary(PlanValidationResult.Valid, "Single-workout structure checked; weekly TSS target check does not apply.")
                : PlanValidationResult.FromFinding(
                    "plan.single_workout_required",
                    "Return exactly one workout for the main recommendation. Keep the easier alternative in prose only.");
        }

        var isRaceWeek = WeeklyLoadTargetResolver.IsRaceWeek(request.WeekDataJson, request.Scope);
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

        var summary = BuildSummary(checkResult);

        if (checkResult.Valid)
        {
            return WithSummary(PlanValidationResult.Valid, summary);
        }

        if (isRaceWeek
            && checkResult.Week is { DeviationPct: < 0 }
            && checkResult.Workouts.All(workout => !workout.Mismatch))
        {
            return WithSummary(PlanValidationResult.Valid, $"{summary} Accepted: tapered race week below target.");
        }

        var issues = checkResult.Issues.Where(issue => !string.IsNullOrWhiteSpace(issue)).ToArray();
        var findings = issues.Length == 0
            ? [new PlanValidationFinding("plan.tss_deviation", BuildFallbackMessage(checkResult))]
            : issues.Select(issue => new PlanValidationFinding("plan.tss_deviation", issue)).ToArray();

        logger.LogInformation(
            "TSS validation reported {FindingCount} finding(s) for the generated plan.",
            findings.Length);

        return new PlanValidationResult(findings) { Summary = summary };
    }

    private static double ResolvePlannedHours(string uploadJson)
    {
        using var document = JsonDocument.Parse(uploadJson);
        var totalMinutes = 0d;

        foreach (var workout in document.RootElement.GetProperty("workouts").EnumerateArray())
        {
            if (workout.ValueKind == JsonValueKind.Object
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
            if (root.ValueKind != JsonValueKind.Object
                || !TryGetDate(root, "week_starting", out var weekStart)
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
            return document.RootElement.ValueKind == JsonValueKind.Object
                && document.RootElement.TryGetProperty("week_summary", out var weekSummary)
                && weekSummary.ValueKind == JsonValueKind.Object
                && weekSummary.TryGetProperty("total_training_load", out var totalTrainingLoad)
                && totalTrainingLoad.ValueKind == JsonValueKind.Number
                && totalTrainingLoad.TryGetDouble(out var completedTss)
                && double.IsFinite(completedTss)
                && completedTss >= 0
                    ? completedTss
                    : null;
        }
        catch (JsonException)
        {
            return null;
        }
    }

    private static string BuildSummary(PlanTssCheckResult checkResult)
    {
        if (checkResult.Week is not { } week)
        {
            return checkResult.Valid ? "TSS check passed." : "TSS check failed.";
        }

        var outcome = checkResult.Valid ? "TSS check passed" : "TSS check failed";
        return $"{outcome}: {week.TotalTss:0} TSS vs. target {week.LoadTarget:0} ({week.DeviationPct:+0.#;-0.#;0}%).";
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
