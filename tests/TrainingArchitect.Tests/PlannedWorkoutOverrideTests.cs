using System.Text.Json;
using System.Text.Json.Nodes;
using TrainingArchitect.Core.Services;

namespace TrainingArchitect.Tests;

public class PlannedWorkoutOverrideTests
{
    [Fact]
    public void Clear_EmptiesBothArraysAndPreservesOtherData()
    {
        const string dataset = """
            {
              "activities": [{"id": 1}],
              "week_summary": {"training_plan": [{"weekly_load_target": 400}]},
              "planned_workouts": {
                "current_week": {
                  "planned_workouts": [{"id": 2}, {"id": 3}],
                  "race_events": [{"priority": "RACE_A"}],
                  "week_starting": "2026-10-12"
                },
                "next_week": {
                  "planned_workouts": [{"id": 4}],
                  "race_events": [{"priority": "RACE_B"}],
                  "week_starting": "2026-10-19"
                },
                "other": {"keep": true}
              }
            }
            """;
        var expected = JsonNode.Parse(dataset)!;
        foreach (var week in new[] { "current_week", "next_week" })
            expected["planned_workouts"]![week]!["planned_workouts"] = new JsonArray();

        var result = PlannedWorkoutOverrideApplier.Clear(dataset);

        Assert.True(JsonNode.DeepEquals(expected, JsonNode.Parse(result)));
        Assert.Equal(result, PlannedWorkoutOverrideApplier.Clear(result));
        Assert.Equal(2, JsonNode.Parse(dataset)!["planned_workouts"]!["current_week"]!["planned_workouts"]!.AsArray().Count);
    }

    [Theory]
    [InlineData("{}")]
    [InlineData("""{"planned_workouts":{}}""")]
    [InlineData("""{"planned_workouts":{"current_week":{"race_events":[]}}}""")]
    [InlineData("""{"planned_workouts":{"current_week":{"planned_workouts":null},"next_week":null}}""")]
    public void Clear_CreatesMissingArrays(string dataset)
    {
        var result = JsonNode.Parse(PlannedWorkoutOverrideApplier.Clear(dataset))!;
        foreach (var week in new[] { "current_week", "next_week" })
            Assert.Empty(result["planned_workouts"]![week]!["planned_workouts"]!.AsArray());
    }

    [Theory]
    [InlineData("not JSON")]
    [InlineData("null")]
    [InlineData("[]")]
    [InlineData("""{"planned_workouts":[]}""")]
    [InlineData("""{"planned_workouts":{"current_week":[]}}""")]
    [InlineData("""{"planned_workouts":{"next_week":"invalid"}}""")]
    public void Clear_InvalidDatasetThrows(string dataset) =>
        Assert.ThrowsAny<JsonException>(() => PlannedWorkoutOverrideApplier.Clear(dataset));
}
