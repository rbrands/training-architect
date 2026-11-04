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
            if (!string.IsNullOrWhiteSpace(uploadJson))
            {
                try
                {
                    var normalizedJson = PlanUploadNormalizer.Normalize(uploadJson,
                        (previous, normalized) => logger.LogDebug(
                            "Normalized workout duration from {PreviousMinutes} to {DurationMinutes} minutes.",
                            previous, normalized),
                        request);
                    normalizedJson = WeeklyRideTargetAdjuster.Adjust(normalizedJson, request, out adjusted);
                    var workoutMetricsChanged = WorkoutMetricsChanged(uploadJson, normalizedJson);
                    if (adjusted)
                    {
                        logger.LogInformation("Adjusted endurance rides to reach the weekly target in round {Round}.", round);
                    }
                    var finalReadableText = workoutMetricsChanged
                        ? BuildFinalizedPlanText(normalizedJson)
                        : readableText;
                    lastResponse = response with
                    {
                        Content = workoutMetricsChanged
                            ? $"{finalReadableText}\nBEGIN_UPLOAD_JSON\n{normalizedJson}\nEND_UPLOAD_JSON"
                            : response.Content.Replace(uploadJson, normalizedJson, StringComparison.Ordinal)
                    };
                    uploadJson = normalizedJson;
                }
                catch (System.Text.Json.JsonException ex)
                {
                    logger.LogDebug(ex, "Plan JSON could not be normalized; passing it to validation.");
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

        static string? GetString(JsonElement element, string propertyName) =>
            element.TryGetProperty(propertyName, out var value) && value.ValueKind == JsonValueKind.String
                ? value.GetString()
                : null;

        static double? GetNumber(JsonElement element, string propertyName) =>
            element.TryGetProperty(propertyName, out var value) && value.TryGetDouble(out var number)
                ? number
                : null;
    }

    private static bool WorkoutMetricsChanged(string originalJson, string normalizedJson)
    {
        using var originalDocument = JsonDocument.Parse(originalJson);
        using var normalizedDocument = JsonDocument.Parse(normalizedJson);
        if (!originalDocument.RootElement.TryGetProperty("workouts", out var originalWorkouts)
            || !normalizedDocument.RootElement.TryGetProperty("workouts", out var normalizedWorkouts)
            || originalWorkouts.ValueKind != JsonValueKind.Array
            || normalizedWorkouts.ValueKind != JsonValueKind.Array)
        {
            return false;
        }

        var originalEntries = originalWorkouts.EnumerateArray().ToArray();
        var normalizedEntries = normalizedWorkouts.EnumerateArray().ToArray();
        if (originalEntries.Length != normalizedEntries.Length)
        {
            return true;
        }

        for (var index = 0; index < originalEntries.Length; index++)
        {
            if (GetNumber(originalEntries[index], "duration_minutes")
                    != GetNumber(normalizedEntries[index], "duration_minutes")
                || GetNumber(originalEntries[index], "tss")
                    != GetNumber(normalizedEntries[index], "tss"))
            {
                return true;
            }
        }

        return false;

        static double? GetNumber(JsonElement element, string propertyName) =>
            element.TryGetProperty(propertyName, out var value) && value.TryGetDouble(out var number)
                ? number
                : null;
    }
}
