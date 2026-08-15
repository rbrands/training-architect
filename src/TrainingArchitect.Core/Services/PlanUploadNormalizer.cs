using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;

namespace TrainingArchitect.Core.Services;

/// <summary>Normalizes generated training plans before they are uploaded.</summary>
public static partial class PlanUploadNormalizer
{
    /// <summary>
    /// Converts explicit steps to the description syntax required by the MCP upload tool, then removes
    /// the separate steps representation to prevent duplicate duration and load.
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
            var isLibraryWorkout = workout["library_workout_id"] is not null;

            if (workout["steps"] is not JsonArray { Count: > 0 } steps)
            {
                if (isLibraryWorkout)
                {
                    RemoveLibraryOverrides(workout);
                }

                continue;
            }

            // Materialized library data must be uploaded exactly once instead of being resolved again by ID.
            workout.Remove("library_workout_id");
            var coachingDescription = GetCoachingDescription(workout["description"]);
            var intervalsStructure = BuildIntervalsStructure(steps);
            workout["description"] = string.IsNullOrWhiteSpace(coachingDescription)
                ? intervalsStructure
                : $"{coachingDescription}\n{intervalsStructure}";
            workout.Remove("steps");
        }

        return root.ToJsonString();
    }

    private static void RemoveLibraryOverrides(JsonObject workout)
    {
        workout.Remove("name");
        workout.Remove("duration_minutes");
        workout.Remove("description");
        workout.Remove("tags");
        workout.Remove("tss");
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

    private static string BuildIntervalsStructure(JsonArray steps)
    {
        var lines = steps
            .OfType<JsonObject>()
            .Select(BuildIntervalsStep)
            .Where(line => line is not null);

        return string.Join('\n', lines!);
    }

    private static string? BuildIntervalsStep(JsonObject step)
    {
        if (step["duration_seconds"] is not JsonValue durationValue ||
            !durationValue.TryGetValue<int>(out var durationSeconds) ||
            durationSeconds <= 0 ||
            step["power_pct_ftp"] is not JsonValue powerValue ||
            !powerValue.TryGetValue<double>(out var powerPercent))
        {
            return null;
        }

        var duration = durationSeconds % 3600 == 0
            ? $"{durationSeconds / 3600}h"
            : durationSeconds % 60 == 0
                ? $"{durationSeconds / 60}m"
                : $"{durationSeconds}s";

        return $"- {duration} {powerPercent:0.##}%";
    }

    [GeneratedRegex(@"^\s*-\s*.*\b\d+(?:[.,]\d+)?\s*(?:s|m|h)\b.*%", RegexOptions.IgnoreCase)]
    private static partial Regex WorkoutStepLineRegex();

    [GeneratedRegex(@"^\s*\d+\s*x\s*$", RegexOptions.IgnoreCase)]
    private static partial Regex RepeatLineRegex();
}