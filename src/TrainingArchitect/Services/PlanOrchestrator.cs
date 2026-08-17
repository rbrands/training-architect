using System.Runtime.CompilerServices;
using TrainingArchitect.Core.Interfaces;
using TrainingArchitect.Core.Models;
using TrainingArchitect.Core.Services;

namespace TrainingArchitect.Services;

/// <summary>
/// Generates a weekly plan, validates it, and asks the coaching agent to correct remaining findings.
/// </summary>
public sealed class PlanOrchestrator(
    ICoachingAgent agent,
    IPlanValidator validator,
    IUsageCounterRepository usageCounterRepository,
    ILogger<PlanOrchestrator> logger) : IPlanOrchestrator
{
    private const int MaxCorrectionRounds = 2;

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
                logger.LogError(ex, "Plan agent call failed in round {Round} for athlete {AthleteId}.", round, intervalsAthleteId);
            }

            if (response is null)
            {
                yield return new PlanProgressEvent(
                    PlanProgressStage.Failed,
                    "The coaching model request failed. Please try again.",
                    round);
                yield break;
            }

            await RecordUsageBestEffortAsync(response, round == 0 ? "plan_create" : "plan_correct", intervalsAthleteId);

            lastResponse = response;
            previousResponseId = response.ResponseId;
            totalTokens += response.TotalTokens ?? 0;
            var uploadJson = PlanResponseParser.Split(response.Content).UploadJson;

            yield return new PlanProgressEvent(PlanProgressStage.Generated, "Plan received.", round);
            yield return new PlanProgressEvent(PlanProgressStage.Validating, "Validating the plan.", round);

            var validation = await validator.ValidateAsync(uploadJson, request, intervalsAthleteId, intervalsApiKey, ct);
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
            prompt = PromptBuilder.BuildPlanCorrectionPrompt(findings);
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
