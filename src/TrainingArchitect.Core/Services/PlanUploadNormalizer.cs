using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using TrainingArchitect.Core.Models;

namespace TrainingArchitect.Core.Services;

/// <summary>Normalizes generated training plans before they are uploaded.</summary>
public static partial class PlanUploadNormalizer
{
    /// <summary>
    /// Keeps workout steps as structured JSON and removes any duplicated interval structure
    /// from the coaching description. Derives generated non-strength workout durations and TSS from
    /// their steps, updates existing TSS mentions, and sets strength workout TSS to zero.
    /// </summary>
    /// <param name="planJson">The generated weekly plan JSON.</param>
    /// <param name="onDurationChanged">Optional callback for logging the old and normalized duration in minutes.</param>
    /// <param name="request">Optional planning request used to preserve fixed workout anchors.</param>
    /// <returns>The normalized weekly plan JSON.</returns>
    public static string Normalize(
        string planJson,
        Action<double?, double>? onDurationChanged = null,
        PlanRequest? request = null)
    {
        var root = JsonNode.Parse(planJson) as JsonObject
            ?? throw new JsonException("Week plan JSON must be a JSON object.");

        if (root["workouts"] is not JsonArray workouts)
        {
            return root.ToJsonString();
        }

        IReadOnlyList<JsonObject> anchors = request is null
            ? []
            : WeeklyRideBudget.ReadAnchors(
                JsonNode.Parse(string.IsNullOrWhiteSpace(request.WeekDataJson) ? "{}" : request.WeekDataJson),
                request.Scope);

        foreach (var workout in workouts.OfType<JsonObject>())
        {
            if (request is not null && WeeklyRideBudget.ReadDate(workout) is { } date
                && anchors.Any(anchor => WeeklyRideBudget.ReadDate(anchor) == date))
            {
                continue;
            }

            var isStrength = workout["activity_type"] is JsonValue activityType
                && activityType.TryGetValue<string>(out var type)
                && string.Equals(type, "WeightTraining", StringComparison.OrdinalIgnoreCase);
            if (isStrength)
            {
                workout["tss"] = 0;
            }

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

            if (isStrength)
            {
                continue;
            }

            if (!TryGetStepTotals(steps, out var totalSeconds, out var totalTss))
            {
                continue;
            }

            var normalizedMinutes = Math.Round(totalSeconds / 60);
            double? previousMinutes = workout["duration_minutes"] is JsonValue previousDuration
                && previousDuration.TryGetValue<double>(out var minutes) ? minutes : null;
            if (previousMinutes != normalizedMinutes)
            {
                workout["duration_minutes"] = normalizedMinutes;
                onDurationChanged?.Invoke(previousMinutes, normalizedMinutes);
            }

            if (totalTss is { } tss)
            {
                var normalizedTss = Math.Round(tss);
                workout["tss"] = normalizedTss;
                workout["description"] = TssMentionRegex().Replace(coachingDescription,
                    $"TSS: {normalizedTss.ToString(System.Globalization.CultureInfo.InvariantCulture)}");
            }
        }

        return root.ToJsonString();
    }

    internal static bool TryGetStepTotals(JsonArray steps, out double totalSeconds, out double? totalTss)
    {
        totalSeconds = 0;
        var tss = 0d;
        var validPower = true;
        totalTss = null;
        foreach (var step in steps)
        {
            if (step is not JsonObject stepObject
                || stepObject["duration_seconds"] is not JsonValue duration
                || !duration.TryGetValue<double>(out var seconds)
                || !double.IsFinite(seconds)
                || seconds <= 0)
                return false;

            totalSeconds += seconds;
            if (stepObject["power_pct_ftp"] is JsonValue power
                && power.TryGetValue<double>(out var powerPctFtp)
                && double.IsFinite(powerPctFtp)
                && powerPctFtp >= 0)
                tss += seconds / 3600 * Math.Pow(powerPctFtp / 100, 2) * 100;
            else
                validPower = false;
        }
        if (validPower && double.IsFinite(tss))
            totalTss = tss;
        return double.IsFinite(totalSeconds);
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

    [GeneratedRegex(@"TSS:\s*\d+(?:[.,]\d+)?")]
    private static partial Regex TssMentionRegex();
}