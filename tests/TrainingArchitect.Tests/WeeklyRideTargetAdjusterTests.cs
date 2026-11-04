using System.Text.Json.Nodes;
using TrainingArchitect.Core.Models;
using TrainingArchitect.Core.Services;

namespace TrainingArchitect.Tests;

public class WeeklyRideTargetAdjusterTests
{
    private static PlanRequest Request(double target, double? hours = null, string constraints = "[]") => new(
        FormattableString.Invariant($$"""
        {"week_starting":"2026-11-02","week_summary":{"training_plan":[
          {"week":"2026-11-02","weekly_load_target":{{target}},
           "weekly_time_target_hours":{{(hours?.ToString(System.Globalization.CultureInfo.InvariantCulture) ?? "null")}},
           "day_constraints":{{constraints}}}
        ]} }
        """), "fitness", "en", PlanningScope.CurrentWeek, new PlanConstraints(null, []));

    private static string Plan(int seconds, string tags = """["aerobic-threshold-moderate"]""", string extra = "") =>
        PlanUploadNormalizer.Normalize($$"""
        {"workouts":[{"date":"2026-11-04","activity_type":"Ride","duration_minutes":60,"tss":999,
          "description":"Endurance. TSS: 999.","tags":{{tags}}{{extra}},
          "steps":[{"duration_seconds":300,"power_pct_ftp":50},
                   {"duration_seconds":{{seconds}},"power_pct_ftp":70},
                   {"duration_seconds":300,"power_pct_ftp":50}]}]}
        """);

    [Theory]
    [InlineData(3000, 60, 3900, 57, 75)]
    [InlineData(3900, 50, 3600, 53, 70)]
    public void Adjust_TopUpOrTrim_ReachesToleranceInFiveMinuteIncrements(
        int seconds, double target, int expectedSeconds, int expectedTss, int expectedMinutes)
    {
        var original = Plan(seconds);

        var result = WeeklyRideTargetAdjuster.Adjust(original, Request(target), out var adjusted);
        var workout = JsonNode.Parse(result)!["workouts"]![0]!;

        Assert.True(adjusted);
        Assert.Equal(expectedSeconds, workout["steps"]![1]!["duration_seconds"]!.GetValue<double>());
        Assert.Equal(300, workout["steps"]![0]!["duration_seconds"]!.GetValue<int>());
        Assert.Equal(300, workout["steps"]![2]!["duration_seconds"]!.GetValue<int>());
        Assert.Equal(expectedTss, workout["tss"]!.GetValue<double>());
        Assert.Equal(expectedMinutes, workout["duration_minutes"]!.GetValue<double>());
        Assert.Equal($"Endurance. TSS: {expectedTss}.", workout["description"]!.GetValue<string>());
        Assert.InRange(workout["tss"]!.GetValue<double>(), target * 0.9, target * 1.1);
        Assert.Equal(result, WeeklyRideTargetAdjuster.Adjust(result, Request(target), out var readjusted));
        Assert.False(readjusted);
    }

    [Theory]
    [InlineData(1.25, true)]
    [InlineData(1, false)]
    public void Adjust_TopUp_RespectsWeeklyHours(double hours, bool expected)
    {
        var original = Plan(3000);
        var result = WeeklyRideTargetAdjuster.Adjust(original, Request(60, hours), out var adjusted);

        Assert.Equal(expected, adjusted);
        if (adjusted)
            Assert.InRange(JsonNode.Parse(result)!["workouts"]![0]!["duration_minutes"]!.GetValue<double>() / 60,
                hours * 0.9, hours * 1.1);
        else
            Assert.Equal(original, result);
    }

    [Theory]
    [InlineData(1.25, true)]
    [InlineData(1, false)]
    public void Adjust_TopUp_RespectsDailyMaximum(double maximum, bool expected)
    {
        var original = Plan(3000);
        var request = Request(60, constraints:
            FormattableString.Invariant($$"""[{"date":"2026-11-04","training_allowed":true,"max_training_time_hours":{{maximum}}}]"""));

        var result = WeeklyRideTargetAdjuster.Adjust(original, request, out var adjusted);

        Assert.Equal(expected, adjusted);
        if (!expected)
            Assert.Equal(original, result);
    }

    [Theory]
    [InlineData("""["vo2max-low"]""")]
    [InlineData("""["aerobic-threshold-moderate","lactate-threshold-low"]""")]
    [InlineData("[]")]
    public void Adjust_DoesNotChangeKeyOrUntaggedSessions(string tags)
    {
        var original = Plan(3000, tags);

        Assert.Equal(original, WeeklyRideTargetAdjuster.Adjust(original, Request(60), out var adjusted));
        Assert.False(adjusted);
    }

    [Fact]
    public void Adjust_DoesNotChangeLibraryWorkout()
    {
        var original = """
            {"workouts":[{"date":"2026-11-04","activity_type":"Ride","tss":45,"duration_minutes":60,
            "library_workout_id":7,"tags":["aerobic-threshold-moderate"]}]}
            """;

        Assert.Equal(original, WeeklyRideTargetAdjuster.Adjust(original, Request(60), out var adjusted));
        Assert.False(adjusted);
    }

    [Fact]
    public void Adjust_DoesNotChangeAnchors()
    {
        var original = Plan(3000);
        var data = JsonNode.Parse(Request(60).WeekDataJson)!;
        data["planned_workouts"] = JsonNode.Parse("""
            {"current_week":{"planned_workouts":[{"date":"2026-11-04","type":"Ride","planned_load":45,"duration_hours":1}]}}
            """);
        var request = Request(60) with { WeekDataJson = data.ToJsonString() };

        Assert.Equal(original, WeeklyRideTargetAdjuster.Adjust(original, request, out var adjusted));
        Assert.False(adjusted);
    }

    [Fact]
    public void Adjust_DoesNotNormalizeAnchorsWhenOtherRidesAreAdjusted()
    {
        var data = JsonNode.Parse(Request(120).WeekDataJson)!;
        data["planned_workouts"] = JsonNode.Parse("""
            {"current_week":{"planned_workouts":[{"date":"2026-11-05","type":"Ride","planned_load":45,"duration_hours":1}]}}
            """);
        var plan = JsonNode.Parse(Plan(3000))!;
        plan["workouts"]!.AsArray().Add(JsonNode.Parse("""
            {"date":"2026-11-05","activity_type":"Ride","duration_minutes":60,"tss":45,
             "description":"Anchored ride. TSS: 45.","steps":[{"duration_seconds":4680,"power_pct_ftp":50}]}
            """));
        var anchor = plan["workouts"]![1]!.DeepClone();

        var result = WeeklyRideTargetAdjuster.Adjust(plan.ToJsonString(),
            Request(120) with { WeekDataJson = data.ToJsonString() }, out var adjusted);

        Assert.True(adjusted);
        var workouts = JsonNode.Parse(result)!["workouts"]!.AsArray();
        Assert.True(JsonNode.DeepEquals(anchor, workouts[1]));
    }

    [Theory]
    [InlineData(70)]
    [InlineData(30)]
    public void Adjust_DeviationLargerThanFifteenPointsBeyondTolerance_IsNotAdjusted(double target)
    {
        var original = Plan(3000);

        Assert.Equal(original, WeeklyRideTargetAdjuster.Adjust(original, Request(target), out var adjusted));
        Assert.False(adjusted);
    }

    [Fact]
    public void Adjust_Trim_DoesNotShortenRideBelowThirtyMinutes()
    {
        var original = Plan(1200);

        Assert.Equal(original, WeeklyRideTargetAdjuster.Adjust(original, Request(18), out var adjusted));
        Assert.False(adjusted);
    }

    [Fact]
    public void Adjust_AccountsForCompletedRidesWithoutDoubleCountingAnchors()
    {
        var data = JsonNode.Parse(Request(170).WeekDataJson)!;
        data["week_summary"]!["ride_load"] = 60;
        data["planned_workouts"] = JsonNode.Parse("""
            {"current_week":{"planned_workouts":[{"date":"2026-11-05","type":"Ride","planned_load":40,"duration_hours":1}]}}
            """);
        var plan = JsonNode.Parse(Plan(3000))!;
        plan["workouts"]!.AsArray().Add(JsonNode.Parse("""
            {"date":"2026-11-05","activity_type":"Ride","tss":40,"duration_minutes":60,"library_workout_id":7}
            """));

        var result = WeeklyRideTargetAdjuster.Adjust(plan.ToJsonString(),
            Request(170) with { WeekDataJson = data.ToJsonString() }, out var adjusted);

        Assert.True(adjusted);
        var workouts = JsonNode.Parse(result)!["workouts"]!.AsArray();
        Assert.InRange(60 + workouts.Sum(workout => workout!["tss"]!.GetValue<double>()), 153, 187);
        Assert.True(JsonNode.DeepEquals(plan["workouts"]![1], workouts[1]));
    }

    [Fact]
    public void Adjust_Trim_CanBringAnOverlongDayBackWithinItsLimit()
    {
        var result = WeeklyRideTargetAdjuster.Adjust(Plan(3900), Request(50, constraints:
            """[{"date":"2026-11-04","max_training_time_hours":1.17}]"""), out var adjusted);

        Assert.True(adjusted);
        Assert.Equal(70, JsonNode.Parse(result)!["workouts"]![0]!["duration_minutes"]!.GetValue<double>());
    }

    [Fact]
    public void Adjust_Trim_RespectsMinimumWeeklyHours()
    {
        var original = Plan(3900);

        Assert.Equal(original, WeeklyRideTargetAdjuster.Adjust(original, Request(50, 1.4), out var adjusted));
        Assert.False(adjusted);
    }

    [Fact]
    public void Adjust_TopUp_CountsSameDayStrengthAgainstDailyLimit()
    {
        var plan = JsonNode.Parse(Plan(3000))!;
        var original = plan.ToJsonString();
        var request = Request(60, constraints:
            """[{"date":"2026-11-04","max_training_time_hours":1.5}]""");
        var data = JsonNode.Parse(request.WeekDataJson)!;
        data["activities"] = JsonNode.Parse("""
            [{"date":"2026-11-04","activity_type":"WeightTraining","duration_minutes":30}]
            """);
        request = request with { WeekDataJson = data.ToJsonString() };

        Assert.Equal(original, WeeklyRideTargetAdjuster.Adjust(original, request, out var adjusted));
        Assert.False(adjusted);
    }

    [Fact]
    public void Adjust_Trim_UsesCompletedActivitiesForFinalDailyLimitCheck()
    {
        var request = Request(50, constraints:
            """[{"date":"2026-11-04","max_training_time_hours":1.5}]""");
        var data = JsonNode.Parse(request.WeekDataJson)!;
        data["activities"] = JsonNode.Parse("""
            [{"date":"2026-11-04","activity_type":"WeightTraining","duration_minutes":30}]
            """);
        request = request with { WeekDataJson = data.ToJsonString() };

        var result = WeeklyRideTargetAdjuster.Adjust(Plan(3900), request, out var adjusted);

        Assert.True(adjusted);
        Assert.Equal(60, JsonNode.Parse(result)!["workouts"]![0]!["duration_minutes"]!.GetValue<double>());
    }

    [Fact]
    public void Adjust_RecoveryWithZ2Step_CanBeAdjusted()
    {
        WeeklyRideTargetAdjuster.Adjust(Plan(3000, """["recovery-low"]"""), Request(60), out var adjusted);

        Assert.True(adjusted);
    }

    [Theory]
    [InlineData(50)]
    [InlineData(100)]
    public void Adjust_NoZ2OrHighIntensitySteps_AreNotChanged(double power)
    {
        var plan = JsonNode.Parse(Plan(3000))!;
        plan["workouts"]![0]!["steps"]![1]!["power_pct_ftp"] = power;
        var original = PlanUploadNormalizer.Normalize(plan.ToJsonString());
        var tss = JsonNode.Parse(original)!["workouts"]![0]!["tss"]!.GetValue<double>();

        Assert.Equal(original, WeeklyRideTargetAdjuster.Adjust(original, Request(tss / 0.8), out var adjusted));
        Assert.False(adjusted);
    }

    [Fact]
    public void Adjust_UnavailableDay_IsNotLengthened()
    {
        var original = Plan(3000);

        Assert.Equal(original, WeeklyRideTargetAdjuster.Adjust(original, Request(60, constraints:
            """[{"date":"2026-11-04","training_allowed":false}]"""), out var adjusted));
        Assert.False(adjusted);
    }

    [Fact]
    public void Adjust_RaceWeekBelowTarget_IsNotToppedUp()
    {
        var data = JsonNode.Parse(Request(60).WeekDataJson)!;
        data["week_summary"]!["training_plan"]![0]!["week_type"] = "RACE";
        var original = Plan(3000);

        Assert.Equal(original, WeeklyRideTargetAdjuster.Adjust(original,
            Request(60) with { WeekDataJson = data.ToJsonString() }, out var adjusted));
        Assert.False(adjusted);
    }

    [Fact]
    public void Adjust_TrimsTheLongestZ2StepAtEachIncrement()
    {
        var original = PlanUploadNormalizer.Normalize("""
            {"workouts":[{"date":"2026-11-04","tags":["aerobic-threshold-moderate"],"steps":[
              {"duration_seconds":1800,"power_pct_ftp":70},
              {"duration_seconds":1800,"power_pct_ftp":70}
            ]}]}
            """);

        var result = WeeklyRideTargetAdjuster.Adjust(original, Request(40), out var adjusted);
        var steps = JsonNode.Parse(result)!["workouts"]![0]!["steps"]!;

        Assert.True(adjusted);
        Assert.Equal(1500, steps[0]!["duration_seconds"]!.GetValue<double>());
        Assert.Equal(1500, steps[1]!["duration_seconds"]!.GetValue<double>());
    }

    [Fact]
    public void Adjust_NextTraining_IsNotAdjustedToWeeklyTarget()
    {
        var original = Plan(3000);

        Assert.Equal(original, WeeklyRideTargetAdjuster.Adjust(original,
            Request(60) with { Mode = PlanningMode.NextTraining }, out var adjusted));
        Assert.False(adjusted);
    }
}
