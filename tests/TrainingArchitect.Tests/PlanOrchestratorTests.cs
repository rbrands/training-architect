using System.Linq.Expressions;
using Microsoft.Extensions.Logging.Abstractions;
using TrainingArchitect.Core.Interfaces;
using TrainingArchitect.Core.Models;
using TrainingArchitect.Services;

namespace TrainingArchitect.Tests;

public class PlanOrchestratorTests
{
    private static readonly PlanRequest Request = new(
        "{}",
        "roadrace",
        "English",
        PlanningScope.NextWeek,
        new PlanConstraints(null, []));

    [Fact]
    public async Task RunAsync_WhenFirstPlanIsValid_CallsAgentOnceAndCompletes()
    {
        var agent = new FakeCoachingAgent();
        var orchestrator = CreateOrchestrator(agent, new FakeValidator());

        var events = await CollectAsync(orchestrator);
        var completed = events.Single(progressEvent => progressEvent.Stage == PlanProgressStage.Completed);

        Assert.Equal(1, agent.CallCount);
        Assert.Null(completed.Warnings);
        Assert.NotNull(completed.Result);
        Assert.DoesNotContain(events, progressEvent => progressEvent.Stage == PlanProgressStage.Correcting);
    }

    [Fact]
    public async Task RunAsync_WhenFirstPlanIsInvalid_CorrectsOnceAndCompletesWithoutWarnings()
    {
        var agent = new FakeCoachingAgent();
        var orchestrator = CreateOrchestrator(agent, new FakeValidator(invalidRounds: 1));

        var events = await CollectAsync(orchestrator);
        var completed = events.Single(progressEvent => progressEvent.Stage == PlanProgressStage.Completed);

        Assert.Equal(2, agent.CallCount);
        Assert.Null(completed.Warnings);
        Assert.Single(events, progressEvent => progressEvent.Stage == PlanProgressStage.Correcting);
    }

    [Fact]
    public async Task RunAsync_WhenPlanStaysInvalid_StopsAfterTwoCorrectionsAndReportsWarnings()
    {
        var agent = new FakeCoachingAgent();
        var orchestrator = CreateOrchestrator(agent, new FakeValidator(invalidRounds: int.MaxValue));

        var events = await CollectAsync(orchestrator);
        var completed = events.Single(progressEvent => progressEvent.Stage == PlanProgressStage.Completed);

        Assert.Equal(3, agent.CallCount);
        Assert.NotNull(completed.Warnings);
        Assert.Single(completed.Warnings!);
        Assert.NotNull(completed.Result);
    }

    [Fact]
    public async Task RunAsync_SumsTokenUsageAcrossAllTurns()
    {
        var agent = new FakeCoachingAgent(totalTokensPerCall: 100);
        var orchestrator = CreateOrchestrator(agent, new FakeValidator(invalidRounds: 1));

        var events = await CollectAsync(orchestrator);
        var completed = events.Single(progressEvent => progressEvent.Stage == PlanProgressStage.Completed);

        Assert.Equal(200, completed.Result!.TotalTokens);
    }

    [Fact]
    public async Task RunAsync_PassesPreviousResponseIdToCorrectionTurn()
    {
        var agent = new FakeCoachingAgent();
        var orchestrator = CreateOrchestrator(agent, new FakeValidator(invalidRounds: 1));

        await CollectAsync(orchestrator);

        Assert.Null(agent.PreviousResponseIds[0]);
        Assert.Equal("response-1", agent.PreviousResponseIds[1]);
    }

    [Fact]
    public async Task RunAsync_WhenAgentThrows_YieldsFailedEvent()
    {
        var agent = new FakeCoachingAgent { ThrowOnCall = true };
        var orchestrator = CreateOrchestrator(agent, new FakeValidator());

        var events = await CollectAsync(orchestrator);

        Assert.Equal(PlanProgressStage.Failed, events[^1].Stage);
        Assert.DoesNotContain(events, progressEvent => progressEvent.Stage == PlanProgressStage.Completed);
    }

    private static PlanOrchestrator CreateOrchestrator(ICoachingAgent agent, IPlanValidator validator) =>
        new(agent, validator, new FakeUsageCounterRepository(), NullLogger<PlanOrchestrator>.Instance);

    private static async Task<List<PlanProgressEvent>> CollectAsync(IPlanOrchestrator orchestrator)
    {
        var events = new List<PlanProgressEvent>();

        await foreach (var progressEvent in orchestrator.RunAsync(Request, "12345", "api-key"))
        {
            events.Add(progressEvent);
        }

        return events;
    }

    private sealed class FakeCoachingAgent(long? totalTokensPerCall = null) : ICoachingAgent
    {
        public int CallCount { get; private set; }

        public List<string?> PreviousResponseIds { get; } = [];

        public bool ThrowOnCall { get; init; }

        public Task<CoachingAgentResponse> PromptAsync(
            string prompt,
            string discipline,
            string language,
            CancellationToken ct = default,
            string? intervalsAthleteId = null,
            string? intervalsApiKey = null,
            string? previousResponseId = null)
        {
            if (ThrowOnCall)
            {
                throw new InvalidOperationException("agent unavailable");
            }

            CallCount++;
            PreviousResponseIds.Add(previousResponseId);

            var content = $"Plan {CallCount}\n\nBEGIN_UPLOAD_JSON\n"
                + """{"workouts":[{"date":"2026-08-17"}]}"""
                + "\nEND_UPLOAD_JSON";

            return Task.FromResult(new CoachingAgentResponse(
                content,
                totalTokensPerCall,
                $"response-{CallCount}"));
        }
    }

    private sealed class FakeValidator(int invalidRounds = 0) : IPlanValidator
    {
        private int _round;

        public Task<PlanValidationResult> ValidateAsync(
            string uploadJson,
            PlanRequest request,
            string? intervalsAthleteId,
            string? intervalsApiKey,
            CancellationToken ct = default)
        {
            var isInvalid = _round < invalidRounds;
            _round++;

            return Task.FromResult(isInvalid
                ? PlanValidationResult.FromFinding("test.finding", "Weekly load target missed.")
                : PlanValidationResult.Valid);
        }
    }

    private sealed class FakeUsageCounterRepository : IUsageCounterRepository
    {
        public Task RecordUsageAsync(string athleteId, string action, int inputTokens, int cachedTokens, int outputTokens) =>
            Task.CompletedTask;

        public Task<IReadOnlyList<UsageCounter>> GetByAthleteIdAsync(string athleteId) =>
            throw new NotSupportedException();

        public Task<(UsageCounter? Monthly, UsageCounter? Weekly)> GetByAthleteAndPeriodsAsync(
            string athleteId,
            string monthlyPeriodKey,
            string weeklyPeriodKey) =>
            throw new NotSupportedException();

        public Task<UsageCounter?> GetByIdAsync(string id) => throw new NotSupportedException();

        public Task<IReadOnlyList<UsageCounter>> GetAllAsync() => throw new NotSupportedException();

        public Task<UsageCounter> UpsertAsync(UsageCounter document) => throw new NotSupportedException();

        public Task DeleteAsync(string id) => throw new NotSupportedException();

        public Task<IReadOnlyList<UsageCounter>> QueryAsync(Expression<Func<UsageCounter, bool>> predicate) =>
            throw new NotSupportedException();

        public Task RefreshGlobalCountersAsync(CancellationToken ct = default) => throw new NotSupportedException();
    }
}
