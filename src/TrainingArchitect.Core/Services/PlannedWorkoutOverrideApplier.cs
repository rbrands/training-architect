using System.Text.Json;
using System.Text.Json.Nodes;

namespace TrainingArchitect.Core.Services;

/// <summary>Overrides planned workouts in a request without changing the source athlete dataset.</summary>
public static class PlannedWorkoutOverrideApplier
{
    /// <summary>Empties both weeks' planned workout arrays, preserving race events and other data.</summary>
    /// <exception cref="JsonException">The dataset or a weekly workout block is not a JSON object.</exception>
    public static string Clear(string weekDataJson)
    {
        var root = JsonNode.Parse(weekDataJson) as JsonObject
            ?? throw new JsonException("The athlete dataset must be a JSON object.");
        var plannedWorkouts = root["planned_workouts"] as JsonObject
            ?? (root["planned_workouts"] is null
                ? new JsonObject()
                : throw new JsonException("planned_workouts must be a JSON object."));
        root["planned_workouts"] = plannedWorkouts;

        foreach (var weekName in new[] { "current_week", "next_week" })
        {
            var week = plannedWorkouts[weekName] as JsonObject
                ?? (plannedWorkouts[weekName] is null
                    ? new JsonObject()
                    : throw new JsonException($"planned_workouts.{weekName} must be a JSON object."));
            plannedWorkouts[weekName] = week;
            week["planned_workouts"] = new JsonArray();
        }

        return root.ToJsonString();
    }
}
