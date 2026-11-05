using System.Reflection;
using System.Text.Json;
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
    private const string StrengthPlanJson = """
        {"workouts":[{"date":"2026-10-19","activity_type":"WeightTraining",
        "duration_minutes":45,"tags":["strength-adaptation"]}]}
        """;
    private static readonly PlanRequest Request = new(
        "{}", "fitness", "en", PlanningScope.CurrentWeek,
        new PlanConstraints(500, []),
        Mode: PlanningMode.NextTraining,
        StrengthPhase: "adaptation");

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
    [InlineData(0.75, true)]
    [InlineData(1, false)]
    public async Task ValidateAsync_WeeklyMode_ChecksDurationAgainstHoursOnlyTarget(double targetHours, bool expectedValid)
    {
        var athleteData = JsonSerializer.Serialize(new
        {
            week_starting = "2026-10-12",
            week_summary = new
            {
                training_plan = new[]
                {
                    new { week = "2026-10-19", weekly_time_target_hours = targetHours }
                }
            }
        });
        var request = Request with
        {
            WeekDataJson = athleteData,
            Constraints = new PlanConstraints(null, []),
            Scope = PlanningScope.NextWeek,
            Mode = PlanningMode.Week
        };
        var validator = new TssPlanValidator(new RecordingAthleteDataService(), NullLogger<TssPlanValidator>.Instance);

        var result = await validator.ValidateAsync(PlanJson, request, null, null);

        Assert.Equal(expectedValid, result.IsValid);
        if (!expectedValid)
            Assert.Equal("plan.duration_deviation", Assert.Single(result.Findings).Code);
        else
            Assert.Contains("Duration check passed", result.Summary);
    }

    [Fact]
    public async Task ValidateAsync_CurrentWeekDurationIncludesCompletedActivities()
    {
        const string athleteData = """
            {"week_starting":"2026-10-19","week_summary":{"training_plan":[
              {"week":"2026-10-19","weekly_time_target_hours":1}
            ]},"activities":[
              {"date":"2026-10-19","type":"Ride","duration_hours":0.25},
              {"date":"2026-10-19","type":"WeightTraining","duration_hours":4},
              {"date":"2026-10-19","type":"Run","duration_hours":2},
              {"date":"2026-10-18","duration_hours":4}
            ]}
            """;
        var request = Request with
        {
            WeekDataJson = athleteData,
            Constraints = new PlanConstraints(null, []),
            Scope = PlanningScope.CurrentWeek,
            Mode = PlanningMode.Week
        };
        var validator = new TssPlanValidator(new RecordingAthleteDataService(), NullLogger<TssPlanValidator>.Instance);

        var result = await validator.ValidateAsync(PlanJson, request, null, null);

        Assert.True(result.IsValid);
        Assert.Contains("Duration check passed", result.Summary);
    }

    [Fact]
    public async Task ValidateAsync_PlannedHoursIgnoreStrengthEntries()
    {
        const string athleteData = """
            {"week_starting":"2026-10-19","week_summary":{"training_plan":[
              {"week":"2026-10-19","weekly_time_target_hours":0.75}
            ]}}
            """;
        const string planJson = """
            {"workouts":[
              {"date":"2026-10-19","activity_type":"Ride","duration_minutes":45},
              {"date":"2026-10-19","activity_type":"WeightTraining","duration_minutes":60,
               "tags":["strength-adaptation"]}
            ]}
            """;
        var request = Request with
        {
            WeekDataJson = athleteData,
            Constraints = new PlanConstraints(null, []),
            Mode = PlanningMode.Week
        };
        var validator = new TssPlanValidator(new RecordingAthleteDataService(), NullLogger<TssPlanValidator>.Instance);

        var result = await validator.ValidateAsync(planJson, request, null, null);

        Assert.True(result.IsValid);
        Assert.Contains("Duration check passed", result.Summary);
    }

    [Fact]
    public async Task ValidateAsync_CompletedHoursPreferRideTimeHoursSummary()
    {
        const string athleteData = """
            {"week_starting":"2026-10-19","week_summary":{
              "ride_time_hours":0.25,
              "training_plan":[{"week":"2026-10-19","weekly_time_target_hours":1}]
            },"activities":[
              {"date":"2026-10-19","type":"WeightTraining","duration_hours":4}
            ]}
            """;
        var request = Request with
        {
            WeekDataJson = athleteData,
            Constraints = new PlanConstraints(null, []),
            Mode = PlanningMode.Week
        };
        var validator = new TssPlanValidator(new RecordingAthleteDataService(), NullLogger<TssPlanValidator>.Instance);

        var result = await validator.ValidateAsync(PlanJson, request, null, null);

        Assert.True(result.IsValid);
        Assert.Contains("Duration check passed", result.Summary);
    }

    [Theory]
    [InlineData(PlanningScope.CurrentWeek, 135d)]
    [InlineData(PlanningScope.NextWeek, null)]
    public async Task ValidateAsync_WeeklyMode_PassesCompletedTssOnlyForCurrentWeek(
        PlanningScope scope,
        double? expectedCompletedTss)
    {
        const string athleteData = """
            {"week_summary":{"total_training_load":135}}
            """;
        var service = new RecordingAthleteDataService();
        var validator = new TssPlanValidator(service, NullLogger<TssPlanValidator>.Instance);

        await validator.ValidateAsync(
            PlanJson,
            Request with { WeekDataJson = athleteData, Scope = scope, Mode = PlanningMode.Week },
            "test-athlete",
            "test-key");

        Assert.Equal(expectedCompletedTss, service.CompletedTss);
    }

    [Fact]
    public async Task ValidateAsync_WeeklyMode_PrefersRideLoadForCompletedTss()
    {
        const string athleteData = """
            {"week_summary":{"ride_load":92,"total_training_load":135}}
            """;
        var service = new RecordingAthleteDataService();
        var validator = new TssPlanValidator(service, NullLogger<TssPlanValidator>.Instance);

        await validator.ValidateAsync(
            PlanJson,
            Request with { WeekDataJson = athleteData, Mode = PlanningMode.Week },
            "test-athlete",
            "test-key");

        Assert.Equal(92d, service.CompletedTss);
    }

    [Fact]
    public async Task ValidateAsync_WeeklyMode_DoesNotFindMismatchForUncountedWorkout()
    {
        var service = new RecordingAthleteDataService
        {
            TssResult = new PlanTssCheckResult
            {
                Valid = false,
                Workouts =
                [
                    new PlanTssWorkoutCheck
                    {
                        Date = "2026-10-19",
                        StatedTss = 25,
                        ComputedTss = 40,
                        Mismatch = true,
                        Counted = false
                    }
                ],
                Week = new PlanTssWeekCheck
                {
                    TotalTss = 0,
                    LoadTarget = 500,
                    WithinTolerance = true,
                    PlannedOtherTss = 25
                },
                Issues = ["TSS mismatch for workout on 2026-10-19."]
            }
        };
        var validator = new TssPlanValidator(service, NullLogger<TssPlanValidator>.Instance);

        var result = await validator.ValidateAsync(
            PlanJson,
            Request with { Mode = PlanningMode.Week },
            "test-athlete",
            "test-key");

        Assert.True(result.IsValid);
        Assert.Contains("(+25 TSS strength/run, not counted)", result.Summary);
    }

    [Fact]
    public void PlanTssCheckResult_DeserializesOldAndExtendedPayloads()
    {
        const string oldPayload = """
            {"workouts":[{"date":"2026-10-19","stated_tss":25,"computed_tss":25,"mismatch":false}],
             "week":{"total_tss":25,"load_target":25,"deviation_pct":0,"within_tolerance":true},
             "valid":true,"issues":[]}
            """;
        const string extendedPayload = """
            {"workouts":[{"date":"2026-10-19","stated_tss":25,"computed_tss":25,"mismatch":false,"counted":false}],
             "week":{"total_tss":25,"load_target":25,"deviation_pct":0,"within_tolerance":true,
             "planned_ride_tss":20,"planned_other_tss":5,"planned_ride_hours":1,"planned_other_hours":0.5},
             "valid":true,"issues":[]}
            """;

        var oldResult = JsonSerializer.Deserialize<PlanTssCheckResult>(oldPayload)!;
        var extendedResult = JsonSerializer.Deserialize<PlanTssCheckResult>(extendedPayload)!;

        Assert.Null(Assert.Single(oldResult.Workouts).Counted);
        Assert.Null(oldResult.Week!.PlannedOtherTss);
        Assert.False(Assert.Single(extendedResult.Workouts).Counted);
        Assert.Equal(5d, extendedResult.Week!.PlannedOtherTss);
        Assert.Equal(1d, extendedResult.Week.PlannedRideHours);
    }

    [Fact]
    public async Task ValidateAsync_NextTraining_AllowsValidStrengthSession()
    {
        var service = new RecordingAthleteDataService();
        var validator = new TssPlanValidator(service, NullLogger<TssPlanValidator>.Instance);

        var result = await validator.ValidateAsync(StrengthPlanJson, Request, null, null);

        Assert.True(result.IsValid);
        Assert.Empty(service.Calls);
    }

    [Theory]
    [InlineData("""{"workouts":[{"date":"2026-10-19","activity_type":"Ride"},{"date":"2026-10-19","activity_type":"WeightTraining","duration_minutes":45,"tags":["strength-adaptation"]}]}""", true)]
    [InlineData("""{"workouts":[{"date":"2026-10-19","activity_type":"WeightTraining","duration_minutes":45,"tags":["strength-adaptation"]}]}""", true)]
    [InlineData("""{"workouts":[{"date":"2026-10-19","activity_type":"Ride"},{"date":"2026-10-19","activity_type":"VirtualRide"}]}""", false)]
    [InlineData("""{"workouts":[{"date":"2026-10-19","activity_type":"WeightTraining","duration_minutes":45,"tags":["strength-adaptation"]},{"date":"2026-10-19","activity_type":"WeightTraining","duration_minutes":45,"tags":["strength-adaptation"]}]}""", false)]
    [InlineData("""{"workouts":[{"date":"2026-10-19","activity_type":"Ride"},{"date":"2026-10-20","activity_type":"WeightTraining","duration_minutes":45,"tags":["strength-adaptation"]}]}""", false)]
    [InlineData("""{"workouts":[{"date":"2026-10-19","activity_type":"Ride"},{"date":"2026-10-19","activity_type":"Run"}]}""", false)]
    public async Task ValidateAsync_NextTraining_ValidatesMainAndOptionalStrengthCombination(string planJson, bool expectedValid)
    {
        var service = new RecordingAthleteDataService();
        var validator = new TssPlanValidator(service, NullLogger<TssPlanValidator>.Instance);

        var result = await validator.ValidateAsync(planJson, Request, null, null);

        Assert.Equal(expectedValid, result.IsValid);
        if (!expectedValid)
        {
            Assert.Equal("plan.single_workout_required", Assert.Single(result.Findings).Code);
            Assert.Equal(
                "Return one main workout, optionally plus one strength session on the same day. Keep the easier alternative in prose only.",
                result.Findings[0].Message);
        }

        Assert.Empty(service.Calls);
    }

    [Fact]
    public async Task ValidateAsync_NextTraining_ValidatesStrengthEntryInRideAndStrengthPlan()
    {
        const string planJson = """
            {"workouts":[
              {"date":"2026-10-19","activity_type":"Ride"},
              {"date":"2026-10-19","activity_type":"WeightTraining","duration_minutes":20,"tags":["strength-adaptation"]}
            ]}
            """;
        var validator = new TssPlanValidator(
            new RecordingAthleteDataService(),
            NullLogger<TssPlanValidator>.Instance);

        var result = await validator.ValidateAsync(planJson, Request, null, null);

        Assert.False(result.IsValid);
        Assert.Equal("plan.strength_entry_invalid", Assert.Single(result.Findings).Code);
    }

    [Fact]
    public async Task ValidateAsync_RejectsStrengthEntryWithDifferentRequestedPhase()
    {
        const string planJson = """
            {"workouts":[{"date":"2026-10-19","activity_type":"WeightTraining",
            "duration_minutes":45,"tags":["strength-adaptation"]}]}
            """;
        var validator = new TssPlanValidator(
            new RecordingAthleteDataService(),
            NullLogger<TssPlanValidator>.Instance);

        var result = await validator.ValidateAsync(
            planJson,
            Request with { StrengthPhase = "maximum" },
            null,
            null);

        Assert.Equal("plan.strength_phase_mismatch", Assert.Single(result.Findings).Code);
    }

    [Fact]
    public async Task ValidateAsync_RejectsStrengthWhenNoneWasRequested()
    {
        const string planJson = """
            {"workouts":[{"date":"2026-10-19","activity_type":"WeightTraining",
            "duration_minutes":45,"tags":["strength-adaptation"]}]}
            """;
        var validator = new TssPlanValidator(
            new RecordingAthleteDataService(),
            NullLogger<TssPlanValidator>.Instance);

        var result = await validator.ValidateAsync(
            planJson,
            Request with { StrengthPhase = "none" },
            null,
            null);

        Assert.Equal("plan.strength_not_requested", Assert.Single(result.Findings).Code);
    }

    [Theory]
    [InlineData("""{"workouts":[{"date":"2026-10-19","activity_type":"WeightTraining","duration_minutes":45,"tags":["vo2max-high"]}]}""", "plan.strength_phase_mismatch")]
    [InlineData("""{"workouts":[{"date":"2026-10-19","activity_type":"WeightTraining","duration_minutes":45,"tags":["strength-adaptation"],"steps":[{"duration_seconds":60}]}]}""", "plan.strength_entry_invalid")]
    [InlineData("""{"workouts":[{"date":"2026-10-19","activity_type":"WeightTraining","duration_minutes":20,"tags":["strength-adaptation"]}]}""", "plan.strength_entry_invalid")]
    [InlineData("""{"workouts":[{"date":"2026-10-19","activity_type":"WeightTraining","duration_minutes":90,"tags":["strength-adaptation"]}]}""", "plan.strength_entry_invalid")]
    public async Task ValidateAsync_NextTraining_RejectsInvalidStrengthEntry(string planJson, string findingCode)
    {
        var service = new RecordingAthleteDataService();
        var validator = new TssPlanValidator(service, NullLogger<TssPlanValidator>.Instance);

        var result = await validator.ValidateAsync(planJson, Request, null, null);

        Assert.False(result.IsValid);
        Assert.Equal(findingCode, Assert.Single(result.Findings).Code);
        if (findingCode == "plan.strength_entry_invalid")
        {
            Assert.Contains("2026-10-19", result.Findings[0].Message);
        }
        Assert.Empty(service.Calls);
    }

    [Fact]
    public async Task ValidateAsync_WeeklyMode_AllowsRideAndStrengthOnSameDate()
    {
        const string planJson = """
            {"workouts":[
              {"date":"2026-10-19","activity_type":"Ride","duration_minutes":45,
               "steps":[{"duration_seconds":2700,"power_pct_ftp":60}]},
              {"date":"2026-10-19","activity_type":"WeightTraining","duration_minutes":45,
               "tags":["strength-adaptation"]}
            ]}
            """;
        var service = new RecordingAthleteDataService();
        var validator = new TssPlanValidator(service, NullLogger<TssPlanValidator>.Instance);

        var result = await validator.ValidateAsync(
            planJson,
            Request with { Mode = PlanningMode.Week },
            "test-athlete",
            "test-key");

        Assert.True(result.IsValid);
        Assert.Equal(["tss"], service.Calls);
    }

    [Fact]
    public async Task ValidateAsync_WeeklyMode_RejectsTwoStrengthEntriesOnSameDate()
    {
        const string planJson = """
            {"workouts":[
              {"date":"2026-10-19","activity_type":"WeightTraining","duration_minutes":45,
               "tags":["strength-adaptation"]},
              {"date":"2026-10-19","activity_type":"WeightTraining","duration_minutes":45,
               "tags":["strength-adaptation"]}
            ]}
            """;
        var service = new RecordingAthleteDataService();
        var validator = new TssPlanValidator(service, NullLogger<TssPlanValidator>.Instance);

        var result = await validator.ValidateAsync(planJson, Request with { Mode = PlanningMode.Week }, null, null);

        Assert.False(result.IsValid);
        Assert.Equal("plan.duplicate_date", Assert.Single(result.Findings).Code);
        Assert.Empty(service.Calls);
    }

    [Fact]
    public async Task ValidateAsync_WeeklyMode_RejectsTwoRidesOnSameDate()
    {
        const string planJson = """
            {"workouts":[
              {"date":"2026-10-30","activity_type":"Ride","duration_minutes":75,"tss":57,"library_workout_id":82},
              {"date":"2026-10-30","activity_type":"Ride","duration_minutes":30,"tss":13,"steps":[{"duration_seconds":1800,"power_pct_ftp":50}]},
              {"date":"2026-10-31","activity_type":"Ride","duration_minutes":47,"tss":45,"library_workout_id":51}
            ]}
            """;
        var service = new RecordingAthleteDataService();
        var validator = new TssPlanValidator(service, NullLogger<TssPlanValidator>.Instance);

        var result = await validator.ValidateAsync(planJson, Request with { Mode = PlanningMode.Week }, "test-athlete", "test-key");

        Assert.False(result.IsValid);
        var finding = Assert.Single(result.Findings);
        Assert.Equal("plan.duplicate_date", finding.Code);
        Assert.Contains("2026-10-30", finding.Message);
        Assert.Contains("at most one ride and one strength session per day", finding.Message);
        Assert.DoesNotContain("2026-10-31", finding.Message);
        Assert.Empty(service.Calls);
    }

    [Fact]
    public async Task ValidateAsync_WeeklyMode_AllowsAnchoredPlannedWorkoutsOnSameDate()
    {
        const string athleteData = """
            {"planned_workouts":{"next_week":{"planned_workouts":[
              {"date":"2026-10-30","name":"Morning spin"},
              {"date":"2026-10-30","name":"Evening ride"}
            ],"race_events":[]}}}
            """;
        const string planJson = """
            {"workouts":[
              {"date":"2026-10-30","duration_minutes":30,"tss":13,"steps":[{"duration_seconds":1800,"power_pct_ftp":50}]},
              {"date":"2026-10-30","duration_minutes":75,"tss":57,"library_workout_id":82}
            ]}
            """;
        var service = new RecordingAthleteDataService();
        var validator = new TssPlanValidator(service, NullLogger<TssPlanValidator>.Instance);

        var result = await validator.ValidateAsync(
            planJson,
            Request with { WeekDataJson = athleteData, Scope = PlanningScope.NextWeek, Mode = PlanningMode.Week },
            "test-athlete",
            "test-key");

        Assert.Equal(["tss"], service.Calls);
        Assert.DoesNotContain(result.Findings, finding => finding.Code == "plan.duplicate_date");
    }

    [Fact]
    public async Task ValidateAsync_WeeklyMode_DoesNotCountStrengthAnchorsAsRides()
    {
        const string athleteData = """
            {"planned_workouts":{"next_week":{"planned_workouts":[
              {"date":"2026-10-30","type":"Ride"},
              {"date":"2026-10-30","type":"WeightTraining"}
            ],"race_events":[]}}}
            """;
        const string planJson = """
            {"workouts":[
              {"date":"2026-10-30","activity_type":"Ride","duration_minutes":30,"tss":13,"steps":[{"duration_seconds":1800,"power_pct_ftp":50}]},
              {"date":"2026-10-30","activity_type":"Ride","duration_minutes":75,"tss":57,"library_workout_id":82}
            ]}
            """;
        var service = new RecordingAthleteDataService();
        var validator = new TssPlanValidator(service, NullLogger<TssPlanValidator>.Instance);

        var result = await validator.ValidateAsync(
            planJson,
            Request with { WeekDataJson = athleteData, Scope = PlanningScope.NextWeek, Mode = PlanningMode.Week },
            "test-athlete",
            "test-key");

        Assert.False(result.IsValid);
        Assert.Equal("plan.duplicate_date", Assert.Single(result.Findings).Code);
        Assert.Empty(service.Calls);
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
    [InlineData(true)]
    [InlineData(false)]
    public async Task UploadAsync_NormalizesDurationBeforeValidationAndUpload(bool validate)
    {
        var service = new RecordingAthleteDataService();
        const string planJson = """
            {"workouts":[{"activity_type":"Ride","duration_minutes":70,
            "steps":[{"duration_seconds":600},{"duration_seconds":4080}]}]}
            """;

        var result = await InvokeUploadAsync(service, planJson, validate);

        Assert.Equal(200, Assert.IsAssignableFrom<IStatusCodeHttpResult>(result).StatusCode);
        using var uploaded = JsonDocument.Parse(service.UploadedJson!);
        Assert.Equal(78, uploaded.RootElement.GetProperty("workouts")[0].GetProperty("duration_minutes").GetDouble());
        if (validate)
            Assert.Equal(service.ValidatedJson, service.UploadedJson);
        else
            Assert.Null(service.ValidatedJson);
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

    private static async Task<IResult> InvokeUploadAsync(IAthleteDataService service, string planJson, bool validate = true)
    {
        var context = new DefaultHttpContext();
        context.Request.Headers[IntervalsHeaders.AthleteId] = "test-athlete";
        context.Request.Headers[IntervalsHeaders.ApiKey] = "test-key";
        var requestType = typeof(CoachEndpoints).GetNestedType("PlanUploadRequest", BindingFlags.NonPublic)!;
        var request = Activator.CreateInstance(requestType, planJson, validate)!;
        var method = typeof(CoachEndpoints).GetMethod("UploadPlanAsync", BindingFlags.NonPublic | BindingFlags.Static)!;

        return await (Task<IResult>)method.Invoke(null,
            [context, request, service, NullLoggerFactory.Instance, CancellationToken.None])!;
    }

    private sealed class RecordingAthleteDataService : IAthleteDataService
    {
        public List<string> Calls { get; } = [];
        public double? LoadTarget { get; private set; }
        public double? CompletedTss { get; private set; }
        public string? ValidatedJson { get; private set; }
        public string? UploadedJson { get; private set; }
        public PlanValidationCheckResult Validation { get; init; } = new() { Status = "valid" };
        public PlanTssCheckResult TssResult { get; init; } = new() { Valid = true };

        public Task<AthleteDataResponse> GetAsync(string athleteId, string apiKey, CancellationToken ct) =>
            throw new NotSupportedException();

        public Task UploadWeekPlanAsync(string athleteId, string apiKey, string weekPlanJson, CancellationToken ct)
        {
            Calls.Add("upload");
            UploadedJson = weekPlanJson;
            return Task.CompletedTask;
        }

        public Task<PlanTssCheckResult> CheckPlanTssAsync(
            string athleteId, string apiKey, string planJson, double loadTarget, double? completedTss, double? tolerancePct, CancellationToken ct)
        {
            Calls.Add("tss");
            LoadTarget = loadTarget;
            CompletedTss = completedTss;
            return Task.FromResult(TssResult);
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
