using System.Reflection;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;
using TrainingArchitect.Core.Constants;
using TrainingArchitect.Core.Models;
using TrainingArchitect.Core.Services;
using TrainingArchitect.Endpoints;
using TrainingArchitect.Services;

namespace TrainingArchitect.Tests;

public sealed class NextTrainingPlanTests
{
    private const string PlanJson = """
        {"workouts":[{"date":"2026-10-19","duration_minutes":45,"tss":25,
        "description":"Easy ride.\n- 45m 60%","steps":[{"duration_seconds":2700,"power_pct_ftp":60}]}]}
        """;
    private static readonly PlanRequest Request = new(
        "{}", "fitness", "en", PlanningScope.CurrentWeek,
        new PlanConstraints(500, []), Mode: PlanningMode.NextTraining);

    [Fact]
    public async Task ValidateAsync_SingleSession_DoesNotTryToFillWeeklyTarget()
    {
        var service = new RecordingAthleteDataService();
        var validator = new TssPlanValidator(service, NullLogger<TssPlanValidator>.Instance);

        var result = await validator.ValidateAsync(PlanJson, Request, "test-athlete", "test-key");

        Assert.True(result.IsValid);
        Assert.Contains("weekly TSS target check does not apply", result.Summary);
        Assert.Empty(service.Calls);
    }

    [Theory]
    [InlineData("""{"workouts":[{},{}]}""", "plan.single_workout_required")]
    [InlineData("""{"workouts":[]}""", "plan.workouts_missing")]
    [InlineData("""{"workouts":{}}""", "plan.workouts_missing")]
    [InlineData("[]", "plan.upload_json_not_an_object")]
    [InlineData("{broken", "plan.upload_json_invalid")]
    [InlineData("", "plan.upload_json_missing")]
    public async Task ValidateAsync_NextTraining_RejectsInvalidWorkoutCountOrJson(string json, string code)
    {
        var service = new RecordingAthleteDataService();
        var validator = new TssPlanValidator(service, NullLogger<TssPlanValidator>.Instance);

        var result = await validator.ValidateAsync(json, Request, "test-athlete", "test-key");

        Assert.False(result.IsValid);
        Assert.Equal(code, Assert.Single(result.Findings).Code);
        Assert.Empty(service.Calls);
    }

    [Fact]
    public async Task ValidateAsync_WeeklyMode_StillChecksWeeklyTarget()
    {
        var service = new RecordingAthleteDataService();
        var validator = new TssPlanValidator(service, NullLogger<TssPlanValidator>.Instance);

        await validator.ValidateAsync(PlanJson, Request with { Mode = PlanningMode.Week }, "test-athlete", "test-key");

        Assert.Equal(["tss"], service.Calls);
        Assert.Equal(500d, service.LoadTarget);
    }

    [Theory]
    [InlineData("valid", null, 200)]
    [InlineData("invalid", null, 422)]
    [InlineData("valid", "Validation service failed.", 502)]
    public async Task UploadAsync_ValidatesBeforeUploadAndUsesSameJson(string status, string? error, int expectedStatus)
    {
        var service = new RecordingAthleteDataService
        {
            Validation = new PlanValidationCheckResult { Status = status, Error = error }
        };

        var result = await InvokeUploadAsync(service, PlanJson);

        Assert.Equal(expectedStatus, Assert.IsAssignableFrom<IStatusCodeHttpResult>(result).StatusCode);
        var normalizedJson = PlanUploadNormalizer.Normalize(PlanJson);
        Assert.NotEqual(PlanJson, normalizedJson);
        Assert.Equal(normalizedJson, service.ValidatedJson);
        Assert.Equal(normalizedJson, PlanUploadNormalizer.Normalize(normalizedJson));
        if (expectedStatus == 200)
        {
            Assert.Equal(["validate", "upload"], service.Calls);
            Assert.Equal(service.ValidatedJson, service.UploadedJson);
        }
        else
        {
            Assert.Equal(["validate"], service.Calls);
            Assert.Null(service.UploadedJson);
        }
    }

    [Theory]
    [InlineData("{broken")]
    [InlineData("[]")]
    [InlineData("null")]
    public async Task UploadAsync_InvalidJson_DoesNotCallValidationOrUpload(string json)
    {
        var service = new RecordingAthleteDataService();

        var result = await InvokeUploadAsync(service, json);

        Assert.Equal(400, Assert.IsAssignableFrom<IStatusCodeHttpResult>(result).StatusCode);
        Assert.Empty(service.Calls);
    }

    private static async Task<IResult> InvokeUploadAsync(IAthleteDataService service, string planJson)
    {
        var context = new DefaultHttpContext();
        context.Request.Headers[IntervalsHeaders.AthleteId] = "test-athlete";
        context.Request.Headers[IntervalsHeaders.ApiKey] = "test-key";
        var requestType = typeof(CoachEndpoints).GetNestedType("PlanUploadRequest", BindingFlags.NonPublic)!;
        var request = Activator.CreateInstance(requestType, planJson, true)!;
        var method = typeof(CoachEndpoints).GetMethod("UploadPlanAsync", BindingFlags.NonPublic | BindingFlags.Static)!;

        return await (Task<IResult>)method.Invoke(null,
            [context, request, service, NullLoggerFactory.Instance, CancellationToken.None])!;
    }

    private sealed class RecordingAthleteDataService : IAthleteDataService
    {
        public List<string> Calls { get; } = [];
        public double? LoadTarget { get; private set; }
        public string? ValidatedJson { get; private set; }
        public string? UploadedJson { get; private set; }
        public PlanValidationCheckResult Validation { get; init; } = new() { Status = "valid" };

        public Task<AthleteDataResponse> GetAsync(string athleteId, string apiKey, CancellationToken ct) =>
            throw new NotSupportedException();

        public Task UploadWeekPlanAsync(string athleteId, string apiKey, string weekPlanJson, CancellationToken ct)
        {
            Calls.Add("upload");
            UploadedJson = weekPlanJson;
            return Task.CompletedTask;
        }

        public Task<PlanTssCheckResult> CheckPlanTssAsync(
            string athleteId, string apiKey, string planJson, double loadTarget, double? tolerancePct, CancellationToken ct)
        {
            Calls.Add("tss");
            LoadTarget = loadTarget;
            return Task.FromResult(new PlanTssCheckResult { Valid = true });
        }

        public Task<PlanValidationCheckResult> ValidateWeekPlanAsync(
            string athleteId, string apiKey, string planJson, int maxErrors, CancellationToken ct)
        {
            Calls.Add("validate");
            ValidatedJson = planJson;
            return Task.FromResult(Validation);
        }

        public Task<AthleteDataCheckResult> CheckAthleteDataAsync(
            string athleteId, string apiKey, string athleteDataJson, CancellationToken ct) =>
            throw new NotSupportedException();
    }
}
