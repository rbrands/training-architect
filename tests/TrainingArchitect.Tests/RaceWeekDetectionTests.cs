using TrainingArchitect.Core.Models;
using TrainingArchitect.Core.Services;

namespace TrainingArchitect.Tests;

public class RaceWeekDetectionTests
{
    private const string RaceInCurrentWeek = """
        {
          "week_starting": "2026-10-19",
          "planned_workouts": {
            "current_week": { "planned_workouts": [], "race_events": [ { "date": "2026-10-24", "priority": "RACE_A" } ] },
            "next_week": { "planned_workouts": [], "race_events": [] }
          }
        }
        """;

    [Fact]
    public void IsRaceWeek_RaceEventInCurrentWeek_IsTrueOnlyForCurrentWeekScope()
    {
        Assert.True(WeeklyLoadTargetResolver.IsRaceWeek(RaceInCurrentWeek, PlanningScope.CurrentWeek));
        Assert.False(WeeklyLoadTargetResolver.IsRaceWeek(RaceInCurrentWeek, PlanningScope.NextWeek));
    }

    [Fact]
    public void IsRaceWeek_WeekTypeRace_IsTrue()
    {
        const string json = """
            { "week_starting": "2026-10-19",
              "week_summary": { "training_plan": [ { "week": "2026-10-19", "week_type": "RACE" } ] } }
            """;

        Assert.True(WeeklyLoadTargetResolver.IsRaceWeek(json, PlanningScope.CurrentWeek));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("not json")]
    [InlineData("""{ "week_starting": "2026-10-19" }""")]
    public void IsRaceWeek_NoRaceData_IsFalse(string? json) =>
        Assert.False(WeeklyLoadTargetResolver.IsRaceWeek(json, PlanningScope.CurrentWeek));
}
