using System.Text.Json.Nodes;
using TrainingArchitect.Core.Models;
using TrainingArchitect.Core.Services;
using TrainingArchitect.Services;

namespace TrainingArchitect.Tests;

public class WeeklyRideBudgetTests
{
    private const string Dataset = """
        {
          "week_starting":"2026-11-02",
          "week_summary":{
            "ride_load":100,"total_training_load":150,"ride_time_hours":2,
            "training_plan":[
              {"week":"2026-11-02","weekly_load_target":500,"weekly_time_target_hours":10},
              {"week":"2026-11-09","weekly_load_target":600,"weekly_time_target_hours":12}
            ]
          },
          "planned_workouts":{
            "current_week":{"planned_workouts":[
              {"date":"2026-11-05","type":"Ride","planned_load":80,"duration_hours":1.5},
              {"date":"2026-11-06","type":"WeightTraining","planned_load":90,"duration_hours":1},
              {"date":"2026-10-30","type":"Ride","planned_load":999,"duration_hours":99}
            ]},
            "next_week":{"planned_workouts":[
              {"start_date_local":"2026-11-10T09:00:00","activity_type":"VirtualRide","tss":60,"duration_minutes":60}
            ]}
          }
        }
        """;

    [Theory]
    [InlineData(PlanningScope.CurrentWeek, false, 320, 270, 370, 6.5, 5.5, 7.5)]
    [InlineData(PlanningScope.CurrentWeek, true, 400, 350, 450, 8, 7, 9)]
    [InlineData(PlanningScope.NextWeek, false, 540, 480, 600, 11, 9.8, 12.2)]
    [InlineData(PlanningScope.NextWeek, true, 600, 540, 660, 12, 10.8, 13.2)]
    public void BuildPlanPrompt_SubtractsOnlyTargetWeekCompletedRidesAndRetainedAnchors(
        PlanningScope scope, bool ignoreAnchors, double tss, double minTss, double maxTss,
        double hours, double minHours, double maxHours)
    {
        var data = ignoreAnchors ? PlannedWorkoutOverrideApplier.Clear(Dataset) : Dataset;
        var request = new PlanRequest(data, "fitness", "en", scope, new PlanConstraints(null, []));

        var prompt = PromptBuilder.BuildPlanPrompt(request);

        Assert.Contains(FormattableString.Invariant(
            $"Remaining ride TSS budget for the days you plan: {tss:0.##} (allowed range {minTss:0.##}–{maxTss:0.##}). Choose workouts whose TSS sum lands in this range."), prompt);
        Assert.Contains(FormattableString.Invariant(
            $"Remaining ride hours budget for the days you plan: {hours:0.##} (allowed range {minHours:0.##}–{maxHours:0.##}). Choose workouts whose duration sum lands in this range."), prompt);
        Assert.DoesNotContain("{{remaining_", prompt);
    }

    [Fact]
    public void Resolve_WithoutCompletedSummary_SumsOnlyRidesInCurrentWeek()
    {
        var data = JsonNode.Parse(Dataset)!;
        data["week_summary"]!.AsObject().Remove("ride_load");
        data["week_summary"]!.AsObject().Remove("ride_time_hours");
        data["activities"] = JsonNode.Parse("""
            [
              {"date":"2026-11-02","type":"Ride","training_load":40,"duration_hours":1},
              {"date":"2026-11-03","type":"VirtualRide","training_load":60,"duration_hours":1.5},
              {"date":"2026-11-03","type":"Run","training_load":90,"duration_hours":2},
              {"date":"2026-11-03","type":"WeightTraining","training_load":80,"duration_hours":1},
              {"date":"2026-11-01","type":"Ride","training_load":999,"duration_hours":99},
              {"date":"2026-11-09","type":"Ride","training_load":999,"duration_hours":99}
            ]
            """);

        var budget = WeeklyRideBudget.Resolve(new PlanRequest(data.ToJsonString(), "fitness", "en",
            PlanningScope.CurrentWeek, new PlanConstraints(null, [])));

        Assert.Equal(100, budget.CompletedTss);
        Assert.Equal(2.5, budget.CompletedHours);
    }

    [Fact]
    public void Resolve_NoCompletedRidesOrAnchors_UsesFullTargets()
    {
        var data = JsonNode.Parse(PlannedWorkoutOverrideApplier.Clear(Dataset))!;
        data["week_summary"]!.AsObject().Remove("ride_load");
        data["week_summary"]!.AsObject().Remove("ride_time_hours");
        data["activities"] = new JsonArray();
        var request = new PlanRequest(data.ToJsonString(), "fitness", "en", PlanningScope.CurrentWeek,
            new PlanConstraints(null, []));

        var prompt = PromptBuilder.BuildPlanPrompt(request);

        Assert.Contains("days you plan: 500 (allowed range 450–550)", prompt);
        Assert.Contains("days you plan: 10 (allowed range 9–11)", prompt);
    }

    [Theory]
    [InlineData("{}", false, false)]
    [InlineData("""{"week_starting":"2026-11-02","week_summary":{"training_plan":[{"week":"2026-11-02","weekly_time_target_hours":5}]}}""", false, true)]
    [InlineData("""{"week_starting":"2026-11-02","week_summary":{"training_plan":[{"week":"2026-11-02","weekly_load_target":250}]}}""", true, false)]
    public void BuildPlanPrompt_OmitsEachMissingTargetLine(string data, bool tss, bool hours)
    {
        var prompt = PromptBuilder.BuildPlanPrompt(new PlanRequest(data, "fitness", "en",
            PlanningScope.CurrentWeek, new PlanConstraints(null, [])));

        Assert.Equal(tss, prompt.Contains("Remaining ride TSS budget", StringComparison.Ordinal));
        Assert.Equal(hours, prompt.Contains("Remaining ride hours budget", StringComparison.Ordinal));
        Assert.DoesNotContain("{{remaining_", prompt);
    }

    [Fact]
    public void Resolve_ExplicitTssConstraintOverridesDataset()
    {
        var budget = WeeklyRideBudget.Resolve(new PlanRequest(Dataset, "fitness", "en",
            PlanningScope.CurrentWeek, new PlanConstraints(550, [])));

        Assert.Equal(550, budget.TssTarget);
    }
}
