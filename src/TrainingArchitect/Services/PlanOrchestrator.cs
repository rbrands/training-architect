using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using TrainingArchitect.Core.Interfaces;
using TrainingArchitect.Core.Models;
using TrainingArchitect.Core.Services;

namespace TrainingArchitect.Services;

/// <summary>
/// Generates a weekly plan or next-session recommendation, validates workouts, and corrects remaining findings.
/// </summary>
public sealed class PlanOrchestrator(
    ICoachingAgent agent,
    IPlanValidator validator,
    IUsageCounterRepository usageCounterRepository,
    ILogger<PlanOrchestrator> logger) : IPlanOrchestrator
{
    private const int MaxCorrectionRounds = 1;

    /// <inheritdoc />
    public async IAsyncEnumerable<PlanProgressEvent> RunAsync(
        PlanRequest request,
        string intervalsAthleteId,
        string intervalsApiKey,
        [EnumeratorCancellation] CancellationToken ct = default)
    {
        yield return new PlanProgressEvent(PlanProgressStage.Started, "Preparing plan request.");

        var prompt = PromptBuilder.BuildPlanPrompt(request);
        string? previousResponseId = null;
        CoachingAgentResponse? lastResponse = null;
        IReadOnlyList<PlanValidationFinding> findings = [];
        long totalTokens = 0;
        var lastRound = 0;
        CoachingAgentResponse? bestResponse = null;
        IReadOnlyList<PlanValidationFinding> bestFindings = [];
        var bestRound = 0;

        for (var round = 0; round <= MaxCorrectionRounds; round++)
        {
            lastRound = round;

            yield return new PlanProgressEvent(
                round == 0 ? PlanProgressStage.Generating : PlanProgressStage.Correcting,
                round == 0
                    ? "Creating the training plan."
                    : $"Correcting the plan (round {round} of {MaxCorrectionRounds}).",
                round);

            CoachingAgentResponse? response = null;
            string? failureMessage = null;
            try
            {
                response = await agent.PromptAsync(
                    prompt,
                    request.DisciplineType,
                    request.Language,
                    ct,
                    intervalsAthleteId,
                    intervalsApiKey,
                    previousResponseId);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var agentException = ex as CoachingAgentException;
                var diagnosticId = agentException?.DiagnosticId
                    ?? System.Diagnostics.Activity.Current?.TraceId.ToString()
                    ?? Guid.NewGuid().ToString("N");
                logger.LogError(
                    "Plan agent call failed in round {Round}. DiagnosticId={DiagnosticId}; ExceptionType={ExceptionType}; StackTrace={StackTrace}",
                    round, diagnosticId, ex.GetType().FullName, ex.StackTrace);
                failureMessage = $"{agentException?.Message ?? "The coaching model request failed. Please try again."} Diagnostic ID: {diagnosticId}.";
            }

            if (response is null && bestResponse is not null)
            {
                logger.LogWarning("Plan correction round {Round} failed structurally ({Reason}); falling back to the plan from round {BestRound}.",
                    round, failureMessage ?? "no response", bestRound);
                foreach (var fallbackEvent in FallbackEvents(bestResponse, bestFindings, totalTokens, round))
                {
                    yield return fallbackEvent;
                }
                yield break;
            }

            if (response is null)
            {
                yield return new PlanProgressEvent(
                    PlanProgressStage.Failed,
                    failureMessage ?? "The coaching model returned no response. Please try again.",
                    round);
                yield break;
            }

            var action = request.Mode == PlanningMode.NextTraining
                ? (round == 0 ? "next_training_create" : "next_training_correct")
                : (round == 0 ? "plan_create" : "plan_correct");
            await RecordUsageBestEffortAsync(response, action, intervalsAthleteId);

            lastResponse = response;
            previousResponseId = response.ResponseId;
            totalTokens += response.TotalTokens ?? 0;
            var (readableText, uploadJson) = PlanResponseParser.Split(response.Content);
            var adjusted = false;
            var normalizationFailed = false;
            if (!string.IsNullOrWhiteSpace(uploadJson))
            {
                try
                {
                    var normalizedJson = PlanUploadNormalizer.Normalize(uploadJson,
                        (previous, normalized) => logger.LogDebug(
                            "Normalized workout duration from {PreviousMinutes} to {DurationMinutes} minutes.",
                            previous, normalized),
                        request);
                    normalizedJson = WeeklyRideTargetAdjuster.Adjust(normalizedJson, request, out adjusted, out var topUpSkipReason);
                    if (topUpSkipReason is not null)
                    {
                        logger.LogInformation("Weekly TSS top-up skipped in round {Round}: {Reason}", round, topUpSkipReason);
                    }
                    if (adjusted)
                    {
                        logger.LogInformation("Adjusted endurance rides to reach the weekly target in round {Round}.", round);
                    }
                    var finalReadableText = string.IsNullOrWhiteSpace(readableText)
                        ? BuildFinalizedPlanText(normalizedJson)
                        : readableText + BuildMetricAdjustments(uploadJson, normalizedJson);
                    lastResponse = response with
                    {
                        Content = finalReadableText != readableText
                            ? $"{finalReadableText}\nBEGIN_UPLOAD_JSON\n{normalizedJson}\nEND_UPLOAD_JSON"
                            : response.Content.Replace(uploadJson, normalizedJson, StringComparison.Ordinal)
                    };
                    uploadJson = normalizedJson;
                }
                catch (System.Text.Json.JsonException ex)
                {
                    logger.LogDebug(ex, "Plan JSON could not be normalized; passing it to validation.");
                    normalizationFailed = true;
                }
            }

            yield return new PlanProgressEvent(PlanProgressStage.Generated, "Plan received.", round);
            if (adjusted)
            {
                yield return new PlanProgressEvent(PlanProgressStage.Validating,
                    "Adjusted endurance rides to reach the weekly target", round);
            }
            var isProseOnlyRecommendation = request.Mode == PlanningMode.NextTraining
                && !string.IsNullOrWhiteSpace(readableText)
                && string.IsNullOrWhiteSpace(uploadJson)
                && !response.Content.Contains("BEGIN_UPLOAD_JSON", StringComparison.Ordinal);
            var structurallyValid = isProseOnlyRecommendation
                || (!string.IsNullOrWhiteSpace(uploadJson) && !normalizationFailed);

            if (!structurallyValid && round > 0 && bestResponse is not null)
            {
                logger.LogWarning("Plan correction round {Round} returned no valid upload JSON; falling back to the plan from round {BestRound}.",
                    round, bestRound);
                foreach (var fallbackEvent in FallbackEvents(bestResponse, bestFindings, totalTokens, round))
                {
                    yield return fallbackEvent;
                }
                yield break;
            }

            yield return new PlanProgressEvent(PlanProgressStage.Validating, "Validating the plan.", round);

            PlanValidationResult validation;
            if (request.Mode == PlanningMode.NextTraining
                && readableText.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries).Length > 100)
            {
                validation = PlanValidationResult.FromFinding(
                    "plan.recommendation_too_long",
                    "Limit the recommendation to at most 100 words of prose, keeping the reason, easier alternative, and downgrade/stop criterion.");
            }
            else
            {
                validation = isProseOnlyRecommendation
                    ? PlanValidationResult.Valid
                    : await validator.ValidateAsync(uploadJson, request, intervalsAthleteId, intervalsApiKey, ct);
            }
            findings = validation.Findings;
            if (structurallyValid)
            {
                bestResponse = lastResponse;
                bestFindings = findings;
                bestRound = round;
            }

            if (!string.IsNullOrWhiteSpace(validation.Summary))
            {
                yield return new PlanProgressEvent(PlanProgressStage.Validating, validation.Summary, round);
            }

            if (validation.IsValid)
            {
                break;
            }

            logger.LogInformation(
                "Plan validation found {FindingCount} issue(s) in round {Round} for athlete {AthleteId}: {Codes}",
                findings.Count,
                round,
                intervalsAthleteId,
                string.Join(", ", findings.Select(finding => finding.Code)));

            // The follow-up turn continues the conversation, so only the findings need to be sent.
            prompt = PromptBuilder.BuildPlanCorrectionPrompt(findings, request.StrengthPhase);
            if (request.Mode == PlanningMode.NextTraining)
            {
                prompt += "\nKeep the next-training contract: at most 100 words of prose, "
                    + (request.StrengthPhase == "none"
                        ? "one main workout and no strength session, "
                        : "one main workout and, when strength is due and placement/readiness allow it, one same-day strength session; do not omit an eligible due session, ")
                    + "or prose only if rest is recommended or essential workout data is missing. "
                    + "Keep the original recommendation date; do not fill the weekly TSS target or upload.";
            }
        }

        if (lastResponse is null)
        {
            yield return new PlanProgressEvent(PlanProgressStage.Failed, "The coaching model returned no plan.");
            yield break;
        }

        var warnings = findings.Count == 0
            ? null
            : findings.Select(finding => finding.Message).ToArray();

        yield return new PlanProgressEvent(
            PlanProgressStage.Completed,
            warnings is null ? "Plan created." : "Plan created with open issues.",
            lastRound,
            new PlanResponse(lastResponse.Content, totalTokens, lastResponse.ResponseId),
            warnings);
    }

    private static IEnumerable<PlanProgressEvent> FallbackEvents(
        CoachingAgentResponse bestResponse,
        IReadOnlyList<PlanValidationFinding> bestFindings,
        long totalTokens,
        int round)
    {
        var openIssues = bestFindings.Select(finding => finding.Message).ToArray();
        var message = openIssues.Length == 0
            ? "Correction failed – showing the original plan."
            : $"Correction failed – showing the original plan; open issue: {string.Join("; ", openIssues)}";
        yield return new PlanProgressEvent(PlanProgressStage.Validating, message, round);
        yield return new PlanProgressEvent(
            PlanProgressStage.Completed,
            openIssues.Length == 0 ? "Plan created." : "Plan created with open issues.",
            round,
            new PlanResponse(bestResponse.Content, totalTokens, bestResponse.ResponseId),
            openIssues.Length == 0 ? null : openIssues);
    }

    private async Task RecordUsageBestEffortAsync(CoachingAgentResponse response, string action, string athleteId)
    {
        try
        {
            await usageCounterRepository.RecordUsageAsync(
                athleteId,
                action,
                ToInt32NonNegative(response.InputTokens),
                ToInt32NonNegative(response.CachedInputTokens),
                ToInt32NonNegative(response.OutputTokens));
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Usage recording failed for coach action {Action}.", action);
        }
    }

    private static int ToInt32NonNegative(long? value)
    {
        if (!value.HasValue || value.Value <= 0)
        {
            return 0;
        }

        return value.Value > int.MaxValue ? int.MaxValue : (int)value.Value;
    }

    private static string BuildFinalizedPlanText(string planJson)
    {
        using var document = JsonDocument.Parse(planJson);
        if (!document.RootElement.TryGetProperty("workouts", out var workouts)
            || workouts.ValueKind != JsonValueKind.Array)
        {
            return string.Empty;
        }

        var text = new StringBuilder("Final workout details:");
        foreach (var workout in workouts.EnumerateArray())
        {
            var name = GetString(workout, "name")
                ?? GetString(workout, "activity_type")
                ?? "Workout";
            text.AppendLine().AppendLine().Append("- ");

            var date = GetString(workout, "date");
            if (date is not null)
            {
                text.Append(date).Append(": ");
            }

            text.Append(name);
            var duration = GetNumber(workout, "duration_minutes");
            if (duration is { } minutes)
            {
                text.Append(" — ")
                    .Append(minutes.ToString("0.##", CultureInfo.InvariantCulture))
                    .Append(" min");
            }

            var tss = GetNumber(workout, "tss");
            if (tss is { } workoutTss)
            {
                text.Append(", TSS ")
                    .Append(workoutTss.ToString("0.##", CultureInfo.InvariantCulture));
            }

            var description = GetString(workout, "description");
            if (!string.IsNullOrWhiteSpace(description))
            {
                text.AppendLine().Append("  ").Append(description);
            }
        }

        return text.ToString();
    }

    private static string BuildMetricAdjustments(string originalJson, string normalizedJson)
    {
        using var originalDocument = JsonDocument.Parse(originalJson);
        using var normalizedDocument = JsonDocument.Parse(normalizedJson);
        if (!originalDocument.RootElement.TryGetProperty("workouts", out var originalWorkouts)
            || !normalizedDocument.RootElement.TryGetProperty("workouts", out var normalizedWorkouts)
            || originalWorkouts.ValueKind != JsonValueKind.Array
            || normalizedWorkouts.ValueKind != JsonValueKind.Array)
        {
            return string.Empty;
        }

        var originalEntries = originalWorkouts.EnumerateArray().ToArray();
        var normalizedEntries = normalizedWorkouts.EnumerateArray().ToArray();
        var text = new StringBuilder();
        // Normalization and top-up preserve workout order and count.
        for (var index = 0; index < originalEntries.Length; index++)
        {
            var original = originalEntries[index];
            var final = normalizedEntries[index];
            var previousMinutes = GetNumber(original, "duration_minutes");
            var minutes = GetNumber(final, "duration_minutes");
            var previousTss = GetNumber(original, "tss");
            var tss = GetNumber(final, "tss");
            var durationChanged = minutes.HasValue && minutes != previousMinutes;
            var tssChanged = tss.HasValue
                && (!previousTss.HasValue || Math.Abs(tss.Value - previousTss.Value) > 2);
            if (!durationChanged && !tssChanged)
            {
                continue;
            }

            if (text.Length == 0)
            {
                text.Append("\n\nAdjusted to the final values:");
            }
            text.Append('\n').Append("- ");
            var date = GetString(final, "date");
            if (DateOnly.TryParseExact(date, "yyyy-MM-dd", CultureInfo.InvariantCulture,
                DateTimeStyles.None, out var workoutDate))
            {
                text.Append(workoutDate.ToString("ddd dd MMM", CultureInfo.InvariantCulture)).Append(" · ");
            }
            else if (!string.IsNullOrWhiteSpace(date))
            {
                text.Append(date).Append(" · ");
            }
            text.Append(GetString(final, "name") ?? GetString(final, "activity_type") ?? "Workout");
            if (durationChanged)
            {
                text.Append(" · ").Append(FormatNumber(minutes)).Append(" min (was ")
                    .Append(FormatNumber(previousMinutes)).Append(')');
            }
            if (tssChanged)
            {
                text.Append(" · TSS ").Append(FormatNumber(tss)).Append(" (was ")
                    .Append(FormatNumber(previousTss)).Append(')');
            }
        }

        return text.ToString();

        static string FormatNumber(double? value) =>
            value?.ToString("0.##", CultureInfo.InvariantCulture) ?? "not supplied";
    }

    private static string? GetString(JsonElement element, string propertyName) =>
        element.TryGetProperty(propertyName, out var value) && value.ValueKind == JsonValueKind.String
            ? value.GetString()
            : null;

    private static double? GetNumber(JsonElement element, string propertyName) =>
        element.TryGetProperty(propertyName, out var value) && value.ValueKind == JsonValueKind.Number
            && value.TryGetDouble(out var number)
                ? number
                : null;
}
