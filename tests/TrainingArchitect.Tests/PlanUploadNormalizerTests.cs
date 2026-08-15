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

    [Fact]
    public void Normalize_WhenLibraryWorkoutHasNoSteps_PreservesCompleteWorkout()
    {
        const string planJson = """
            {
              "workouts": [{
                "date": "2026-08-22",
                "library_workout_id": 7,
                "name": "Generated name",
                "duration_minutes": 150,
                "description": "Generated description. TSS: 80.",
                "tags": ["aerobic-threshold-high"]
              }]
            }
            """;

        var normalized = PlanUploadNormalizer.Normalize(planJson);
        using var document = JsonDocument.Parse(normalized);
        var workout = document.RootElement.GetProperty("workouts")[0];

        Assert.Equal("2026-08-22", workout.GetProperty("date").GetString());
        Assert.Equal(7, workout.GetProperty("library_workout_id").GetInt32());
        Assert.Equal("Generated name", workout.GetProperty("name").GetString());
        Assert.Equal(150, workout.GetProperty("duration_minutes").GetInt32());
        Assert.Equal("Generated description. TSS: 80.", workout.GetProperty("description").GetString());
        Assert.Equal("aerobic-threshold-high", workout.GetProperty("tags")[0].GetString());
    }

    [Fact]
    public void Normalize_WhenLibraryWorkoutIsMaterialized_UploadsItsDataWithoutResolvingIdAgain()
    {
        const string planJson = """
            {
              "workouts": [{
                "date": "2026-08-18",
                "library_workout_id": 54,
                "description": "Library VO2 workout. TSS: 42.",
                "steps": [{ "duration_seconds": 30, "power_pct_ftp": 120 }]
              }]
            }
            """;

        var normalized = PlanUploadNormalizer.Normalize(planJson);
        using var document = JsonDocument.Parse(normalized);
        var workout = document.RootElement.GetProperty("workouts")[0];

        Assert.False(workout.TryGetProperty("library_workout_id", out _));
        Assert.False(workout.TryGetProperty("steps", out _));
        Assert.Equal("Library VO2 workout. TSS: 42.\n- 30s 120%", workout.GetProperty("description").GetString());
    }
}