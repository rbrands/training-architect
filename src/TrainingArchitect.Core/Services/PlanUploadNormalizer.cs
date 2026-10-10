using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;

namespace TrainingArchitect.Core.Services;

/// <summary>Normalizes generated training plans before they are uploaded.</summary>
public static partial class PlanUploadNormalizer
{
    /// <summary>
    /// Keeps workout steps as structured JSON and removes any duplicated interval structure
    /// from the coaching description.
    /// </summary>
    /// <param name="planJson">The generated weekly plan JSON.</param>
    /// <returns>The normalized weekly plan JSON.</returns>
    public static string Normalize(string planJson)
    {
        var root = JsonNode.Parse(planJson) as JsonObject
            ?? throw new JsonException("Week plan JSON must be a JSON object.");

        if (root["workouts"] is not JsonArray workouts)
        {
            return root.ToJsonString();
        }

        foreach (var workout in workouts.OfType<JsonObject>())
        {
            if (workout["steps"] is not JsonArray { Count: > 0 } steps)
            {
                continue;
            }

            if (workout["library_workout_id"] is not null)
            {
                workout.Remove("steps");
                continue;
            }

            var coachingDescription = GetCoachingDescription(workout["description"]);
            workout["description"] = coachingDescription;
        }

        return root.ToJsonString();
    }

    private static string GetCoachingDescription(JsonNode? descriptionNode)
    {
        if (descriptionNode is not JsonValue descriptionValue ||
            !descriptionValue.TryGetValue<string>(out var description))
        {
            return string.Empty;
        }

        var lines = description.Replace("\r\n", "\n", StringComparison.Ordinal).Split('\n');
        var structureStart = Array.FindIndex(lines, line =>
            WorkoutStepLineRegex().IsMatch(line) || RepeatLineRegex().IsMatch(line));

        return string.Join('\n', structureStart < 0 ? lines : lines[..structureStart]).Trim();
    }

    [GeneratedRegex(@"^\s*-\s*.*\b\d+(?:[.,]\d+)?\s*(?:s|m|h)\b.*%", RegexOptions.IgnoreCase)]
    private static partial Regex WorkoutStepLineRegex();

    [GeneratedRegex(@"^\s*\d+\s*x\s*$", RegexOptions.IgnoreCase)]
    private static partial Regex RepeatLineRegex();
}