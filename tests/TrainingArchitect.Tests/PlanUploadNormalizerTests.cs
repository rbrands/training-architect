using System.Text.Json;
using TrainingArchitect.Core.Services;

namespace TrainingArchitect.Tests;

public class PlanUploadNormalizerTests
{
    [Fact]
    public void Normalize_WhenWorkoutContainsExplicitSteps_ConvertsStepsForIntervalsUpload()
    {
        const string planJson = """
            {
              "workouts": [
                {
                  "duration_minutes": 90,
                  "description": "Aerobic ride. TSS: 58. Fueling: 30-50 g carbs per hour.",
                  "steps": [
                    { "duration_seconds": 600, "power_pct_ftp": 50 },
                    { "duration_seconds": 4200, "power_pct_ftp": 65 },
                    { "duration_seconds": 600, "power_pct_ftp": 50 }
                  ]
                }
              ]
            }
            """;

        var normalized = PlanUploadNormalizer.Normalize(planJson);
        using var document = JsonDocument.Parse(normalized);
        var workout = document.RootElement.GetProperty("workouts")[0];

        Assert.False(workout.TryGetProperty("steps", out _));
  Assert.Equal(90, workout.GetProperty("duration_minutes").GetInt32());
        Assert.Equal(
          "Aerobic ride. TSS: 58. Fueling: 30-50 g carbs per hour.\n- 10m 50%\n- 70m 65%\n- 10m 50%",
          workout.GetProperty("description").GetString());
    }

    [Fact]
    public void Normalize_WhenDescriptionAlreadyContainsStructure_ReplacesItFromExplicitSteps()
    {
        const string planJson = """
            {
              "workouts": [{
                "description": "Recovery ride.\n- 30m 80%",
                "steps": [{ "duration_seconds": 2700, "power_pct_ftp": 50 }]
              }]
            }
            """;

        var normalized = PlanUploadNormalizer.Normalize(planJson);
        using var document = JsonDocument.Parse(normalized);

        Assert.Equal(
            "Recovery ride.\n- 45m 50%",
            document.RootElement.GetProperty("workouts")[0].GetProperty("description").GetString());
    }

    [Fact]
    public void Normalize_WhenWorkoutHasNoSteps_PreservesDescription()
    {
        const string planJson = """
            { "workouts": [{ "description": "- 45m 55%" }] }
            """;

        var normalized = PlanUploadNormalizer.Normalize(planJson);
        using var document = JsonDocument.Parse(normalized);

        Assert.Equal(
            "- 45m 55%",
            document.RootElement.GetProperty("workouts")[0].GetProperty("description").GetString());
    }
}