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
    public async Task RunAsync_WhenPlanStaysInvalid_StopsAfterOneCorrectionAndReportsWarnings()
    {
        var agent = new FakeCoachingAgent();
        var orchestrator = CreateOrchestrator(agent, new FakeValidator(invalidRounds: int.MaxValue));

        var events = await CollectAsync(orchestrator);
        var completed = events.Single(progressEvent => progressEvent.Stage == PlanProgressStage.Completed);

        Assert.Equal(2, agent.CallCount);
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

    [Fact]
    public async Task RunAsync_AgentFailure_PreservesSafeMessageAndDiagnosticId()
    {
        var agent = new FakeCoachingAgent
        {
            ExceptionToThrow = new CoachingAgentException("Upstream HTTP 500. Please try again later.", "test-diagnostic")
        };

        var events = await CollectAsync(CreateOrchestrator(agent, new FakeValidator()));

        Assert.Equal(PlanProgressStage.Failed, events[^1].Stage);
        Assert.Equal("Upstream HTTP 500. Please try again later. Diagnostic ID: test-diagnostic.", events[^1].Message);
    }

    [Fact]
    public async Task RunAsync_UnexpectedFailure_DoesNotExposeExceptionMessage()
    {
        var agent = new FakeCoachingAgent { ExceptionToThrow = new InvalidOperationException("private-prompt-and-api-key") };

        var events = await CollectAsync(CreateOrchestrator(agent, new FakeValidator()));

        Assert.Equal(PlanProgressStage.Failed, events[^1].Stage);
        Assert.Contains("Diagnostic ID:", events[^1].Message);
        Assert.DoesNotContain("private-prompt-and-api-key", events[^1].Message);
    }

    [Fact]
    public async Task RunAsync_Cancellation_IsNotConvertedToFailure()
    {
        var agent = new FakeCoachingAgent { ExceptionToThrow = new OperationCanceledException() };

        await Assert.ThrowsAsync<OperationCanceledException>(() => CollectAsync(CreateOrchestrator(agent, new FakeValidator())));
    }

    [Theory]
    [InlineData("Rest today: readiness is red. Re-check in the morning.")]
    [InlineData("Essential workout data is missing; no workout can safely be prescribed.")]
    public async Task RunAsync_NextTraining_ProseOnlyCompletesWithoutCorrections(string content)
    {
        var agent = new FakeCoachingAgent { Content = content };
        var validator = new FakeValidator(invalidRounds: int.MaxValue);

        var events = await CollectAsync(CreateOrchestrator(agent, validator),
            Request with { Mode = PlanningMode.NextTraining });

        Assert.Equal(1, agent.CallCount);
        Assert.Equal(0, validator.CallCount);
        var completed = Assert.Single(events, item => item.Stage == PlanProgressStage.Completed);
        Assert.Null(completed.Warnings);
        Assert.Equal(content, completed.Result!.Content);
    }

    [Fact]
    public async Task RunAsync_WeeklyProseOnly_StillRequiresUploadJson()
    {
        var agent = new FakeCoachingAgent { Content = "Rest." };
        var validator = new FakeValidator(invalidRounds: int.MaxValue);

        var events = await CollectAsync(CreateOrchestrator(agent, validator));

        Assert.Equal(2, agent.CallCount);
        Assert.Equal(2, validator.CallCount);
        Assert.NotNull(events[^1].Warnings);
    }

    [Theory]
    [InlineData("Rest.\nBEGIN_UPLOAD_JSON\nEND_UPLOAD_JSON")]
    [InlineData("Rest.\nBEGIN_UPLOAD_JSON\n{}")]
    public async Task RunAsync_NextTraining_MalformedUploadBlockIsNotTreatedAsRest(string content)
    {
        var agent = new FakeCoachingAgent { Content = content };
        var validator = new FakeValidator(invalidRounds: int.MaxValue);

        await CollectAsync(CreateOrchestrator(agent, validator),
            Request with { Mode = PlanningMode.NextTraining });

        Assert.Equal(2, validator.CallCount);
    }

    [Fact]
    public async Task RunAsync_NextTraining_CorrectionKeepsSingleSessionContract()
    {
        var agent = new FakeCoachingAgent();
        var validator = new FakeValidator(invalidRounds: 1);

        await CollectAsync(CreateOrchestrator(agent, validator),
            Request with { Mode = PlanningMode.NextTraining });

        Assert.Equal(2, agent.CallCount);
        Assert.Contains("exactly one workout", agent.Prompts[1]);
        Assert.Contains("do not fill the weekly TSS target or upload", agent.Prompts[1]);
        Assert.Equal("response-1", agent.PreviousResponseIds[1]);
    }

    [Theory]
    [InlineData(100, false, 1)]
    [InlineData(101, false, 2)]
    [InlineData(100, true, 1)]
    [InlineData(101, true, 2)]
    public async Task RunAsync_NextTraining_EnforcesProseWordLimit(int wordCount, bool includeWorkout, int expectedCalls)
    {
        var prose = string.Join(" ", Enumerable.Repeat("word", wordCount));
        var content = includeWorkout
            ? prose + "\nBEGIN_UPLOAD_JSON\n{\"workouts\":[{}]}\nEND_UPLOAD_JSON"
            : prose;
        var agent = new FakeCoachingAgent { Content = content };
        var validator = new FakeValidator();

        var events = await CollectAsync(CreateOrchestrator(agent, validator),
            Request with { Mode = PlanningMode.NextTraining });

        Assert.Equal(expectedCalls, agent.CallCount);
        var completed = events[^1];
        Assert.Equal(PlanProgressStage.Completed, completed.Stage);
        if (wordCount > 100)
        {
            Assert.Contains("at most 100 words", Assert.Single(completed.Warnings!));
            Assert.Equal(0, validator.CallCount);
        }
        else
        {
            Assert.Null(completed.Warnings);
            Assert.Equal(includeWorkout ? 1 : 0, validator.CallCount);
        }
    }

    private static async Task<List<PlanProgressEvent>> CollectAsync(IPlanOrchestrator orchestrator, PlanRequest? request = null)
    {
        var events = new List<PlanProgressEvent>();

        await foreach (var progressEvent in orchestrator.RunAsync(request ?? Request, "12345", "api-key"))
        {
            events.Add(progressEvent);
        }

        return events;
    }

    private sealed class FakeCoachingAgent(long? totalTokensPerCall = null) : ICoachingAgent
    {
        public int CallCount { get; private set; }

        public List<string?> PreviousResponseIds { get; } = [];
        public List<string> Prompts { get; } = [];
        public string? Content { get; init; }

        public bool ThrowOnCall { get; init; }

        public Exception? ExceptionToThrow { get; init; }

        public Task<CoachingAgentResponse> PromptAsync(
            string prompt,
            string discipline,
            string language,
            CancellationToken ct = default,
            string? intervalsAthleteId = null,
            string? intervalsApiKey = null,
            string? previousResponseId = null)
        {
            if (ExceptionToThrow is not null)
            {
                throw ExceptionToThrow;
            }
            if (ThrowOnCall)
            {
                throw new InvalidOperationException("agent unavailable");
            }

            CallCount++;
            PreviousResponseIds.Add(previousResponseId);
            Prompts.Add(prompt);

            var content = Content ?? $"Plan {CallCount}\n\nBEGIN_UPLOAD_JSON\n"
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
        public int CallCount => _round;

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
