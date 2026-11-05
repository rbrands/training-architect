using System.Text.Json;
using TrainingArchitect.Core.Services;

namespace TrainingArchitect.Tests;

public class PlanUploadNormalizerTests
{
    [Theory]
    [InlineData(600, 50, 3000, 80, 58)]
    [InlineData(1800, 50, 1800, 50, 25)]
    [InlineData(1800, 0, 1800, 100, 50)]
    [InlineData(1800, 120, 1800, 50, 84)]
    public void Normalize_GeneratedRide_RecomputesMismatchedTssFromSteps(
        int firstSeconds, double firstPower, int secondSeconds, double secondPower, int expectedTss)
    {
        var planJson = $$"""
            {"workouts":[{"activity_type":"Ride","library_workout_id":null,"tss":999,"duration_minutes":70,
            "steps":[{"duration_seconds":{{firstSeconds}},"power_pct_ftp":{{firstPower}}},
                     {"duration_seconds":{{secondSeconds}},"power_pct_ftp":{{secondPower}}}]}]}
            """;

        var normalized = PlanUploadNormalizer.Normalize(planJson);
        using var document = JsonDocument.Parse(normalized);
        var workout = document.RootElement.GetProperty("workouts")[0];

        Assert.Equal(expectedTss, workout.GetProperty("tss").GetDouble());
        Assert.Equal(Math.Round((firstSeconds + secondSeconds) / 60d),
            workout.GetProperty("duration_minutes").GetDouble());
        Assert.Equal(normalized, PlanUploadNormalizer.Normalize(normalized));
    }

    [Theory]
    [InlineData("Easy ride. TSS: 999. Fueling: 30 g/h.", "Easy ride. TSS: 25. Fueling: 30 g/h.")]
    [InlineData("TSS:\t999; TSS:123", "TSS: 25; TSS: 25")]
    [InlineData("Easy ride. Fueling: 30 g/h.", "Easy ride. Fueling: 30 g/h.")]
    public void Normalize_GeneratedRide_UpdatesOnlyExistingTssMentions(string description, string expected)
    {
        var planJson = JsonSerializer.Serialize(new
        {
            workouts = new[]
            {
                new { activity_type = "Ride", tss = 999, description,
                    steps = new[] { new { duration_seconds = 3600, power_pct_ftp = 50 } } }
            }
        });

        using var document = JsonDocument.Parse(PlanUploadNormalizer.Normalize(planJson));

        Assert.Equal(expected, document.RootElement.GetProperty("workouts")[0].GetProperty("description").GetString());
    }

    [Theory]
    [InlineData("WeightTraining", "")]
    [InlineData("weighttraining", """, "steps":[{"duration_seconds":3600,"power_pct_ftp":100}]""")]
    [InlineData("WeightTraining", """, "library_workout_id":7""")]
    public void Normalize_StrengthWorkout_ForcesZeroTssAndPreservesDuration(string activityType, string properties)
    {
        var planJson = $$"""
            {"workouts":[{"activity_type":"{{activityType}}","tss":99,"duration_minutes":45{{properties}}}]}
            """;

        using var document = JsonDocument.Parse(PlanUploadNormalizer.Normalize(planJson));
        var workout = document.RootElement.GetProperty("workouts")[0];

        Assert.Equal(0, workout.GetProperty("tss").GetInt32());
        Assert.Equal(45, workout.GetProperty("duration_minutes").GetInt32());
    }

    [Theory]
    [InlineData("""[{"duration_seconds":3600}]""")]
    [InlineData("""[{"duration_seconds":3600,"power_pct_ftp":"50"}]""")]
    [InlineData("""[{"duration_seconds":3600,"power_pct_ftp":-50}]""")]
    [InlineData("""[{"duration_seconds":-60,"power_pct_ftp":50}]""")]
    [InlineData("""[{"duration_seconds":3600,"power_pct_ftp":50},{}]""")]
    public void Normalize_InvalidSteps_PreservesStatedTssForValidation(string steps)
    {
        using var document = JsonDocument.Parse(PlanUploadNormalizer.Normalize(
            $$"""{"workouts":[{"tss":99,"description":"Ride. TSS: 99.","steps":{{steps}}}]}"""));
        var workout = document.RootElement.GetProperty("workouts")[0];

        Assert.Equal(99, workout.GetProperty("tss").GetInt32());
        Assert.Equal("Ride. TSS: 99.", workout.GetProperty("description").GetString());
    }

    [Theory]
    [InlineData(4680, 70, 78)]
    [InlineData(4679, 70, 78)]
    [InlineData(4621, 70, 77)]
    [InlineData(4680, 78, 78)]
    public void Normalize_GeneratedWorkout_DerivesDurationFromStepsAndReportsOnlyChanges(
        int totalSeconds, int initialMinutes, int expectedMinutes)
    {
        var planJson = $$"""
            {"workouts":[{"activity_type":"Ride","duration_minutes":{{initialMinutes}},
            "steps":[{"duration_seconds":600},{"duration_seconds":{{totalSeconds - 600}}}]}]}
            """;
        var changes = new List<(double? Previous, double Normalized)>();

        var normalized = PlanUploadNormalizer.Normalize(planJson,
            (previous, duration) => changes.Add((previous, duration)));
        using var document = JsonDocument.Parse(normalized);

        Assert.Equal(expectedMinutes,
            document.RootElement.GetProperty("workouts")[0].GetProperty("duration_minutes").GetDouble());
        if (initialMinutes == expectedMinutes)
            Assert.Empty(changes);
        else
            Assert.Equal(((double?)initialMinutes, (double)expectedMinutes), Assert.Single(changes));
        changes.Clear();
        Assert.Equal(normalized, PlanUploadNormalizer.Normalize(normalized,
            (previous, duration) => changes.Add((previous, duration))));
        Assert.Empty(changes);
    }

    [Theory]
    [InlineData("\"activity_type\":\"WeightTraining\",")]
    [InlineData("\"library_workout_id\":7,")]
    public void Normalize_StrengthOrLibraryWorkout_PreservesDuration(string properties)
    {
        var planJson = $$"""
            {"workouts":[{ {{properties}} "duration_minutes":70,
            "steps":[{"duration_seconds":4680}]}]}
            """;
        var changes = new List<double>();

        var normalized = PlanUploadNormalizer.Normalize(planJson, (_, duration) => changes.Add(duration));
        using var document = JsonDocument.Parse(normalized);

        Assert.Equal(70, document.RootElement.GetProperty("workouts")[0].GetProperty("duration_minutes").GetInt32());
        Assert.Empty(changes);
    }

    [Theory]
    [InlineData("[]")]
    [InlineData("""[{"duration_seconds":600},{}]""")]
    [InlineData("""[{"duration_seconds":600},{"duration_seconds":-60}]""")]
    [InlineData("""[{"duration_seconds":"4680"}]""")]
    public void Normalize_MissingOrInvalidStepDurations_PreservesDurationForValidation(string steps)
    {
        var normalized = PlanUploadNormalizer.Normalize(
            $$"""{"workouts":[{"duration_minutes":70,"steps":{{steps}}}]}""");
        using var document = JsonDocument.Parse(normalized);

        Assert.Equal(70, document.RootElement.GetProperty("workouts")[0].GetProperty("duration_minutes").GetInt32());
    }

    [Fact]
    public void Normalize_WhenWorkoutContainsExplicitSteps_PreservesStepsAsStructuredJson()
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

        Assert.Equal(3, workout.GetProperty("steps").GetArrayLength());
        Assert.Equal(90, workout.GetProperty("duration_minutes").GetInt32());
        Assert.Equal(
          "Aerobic ride. TSS: 58. Fueling: 30-50 g carbs per hour.",
          workout.GetProperty("description").GetString());
    }

    [Fact]
    public void Normalize_WhenDescriptionAlreadyContainsStructure_RemovesDuplicateStructureButPreservesSteps()
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

        var workout = document.RootElement.GetProperty("workouts")[0];
        Assert.Equal("Recovery ride.", workout.GetProperty("description").GetString());
        Assert.Equal(
            """[{"duration_seconds":2700,"power_pct_ftp":50}]""",
            workout.GetProperty("steps").GetRawText());
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
    public void Normalize_WhenLibraryWorkoutContainsSteps_PreservesIdAndUsesStoredWorkout()
    {
        const string planJson = """
            {
              "workouts": [{
                "date": "2026-08-18",
                "library_workout_id": 54,
                "tss": 42,
                "description": "Library VO2 workout. TSS: 42.",
                "steps": [{ "duration_seconds": 30, "power_pct_ftp": 120 }]
              }]
            }
            """;

        var normalized = PlanUploadNormalizer.Normalize(planJson);
        using var document = JsonDocument.Parse(normalized);
        var workout = document.RootElement.GetProperty("workouts")[0];

        Assert.Equal(54, workout.GetProperty("library_workout_id").GetInt32());
        Assert.Equal(42, workout.GetProperty("tss").GetInt32());
        Assert.False(workout.TryGetProperty("steps", out _));
        Assert.Equal("Library VO2 workout. TSS: 42.", workout.GetProperty("description").GetString());
    }
}