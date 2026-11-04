using System.Runtime.CompilerServices;
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
                    if (adjusted)
                    {
                        logger.LogInformation("Adjusted endurance rides to reach the weekly target in round {Round}.", round);
                    }
                    lastResponse = response with
                    {
                        Content = response.Content.Replace(uploadJson, normalizedJson, StringComparison.Ordinal)
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
}
