using System.Globalization;
using System.Text.Json;
using System.Text.Json.Nodes;
using TrainingArchitect.Core.Models;

namespace TrainingArchitect.Core.Services;

/// <summary>Reads and applies weekly targets without changing the source athlete dataset.</summary>
public static class TrainingPlanOverrideApplier
{
    /// <summary>Parses a phase label case-insensitively, defaulting unknown labels to Base.</summary>
    public static TrainingPhase ParsePhase(string? label) =>
        Enum.TryParse<TrainingPhase>(label, true, out var phase)
        && Enum.IsDefined(phase)
        && Enum.GetNames<TrainingPhase>().Any(name => string.Equals(name, label?.Trim(), StringComparison.OrdinalIgnoreCase))
            ? phase : TrainingPhase.Base;

    /// <summary>Reads the dataset's current week start, or null when unavailable.</summary>
    public static DateOnly? ReadCurrentWeekStart(string? weekDataJson) =>
        ReadDate(ReadRoot(weekDataJson)?["week_starting"]);

    /// <summary>Reads the athlete dataset's date, or null when unavailable.</summary>
    public static DateOnly? ReadCurrentDate(string? weekDataJson) =>
        ReadDate(ReadRoot(weekDataJson)?["current_date"]);

    /// <summary>Reads existing targets for a week without estimating missing values.</summary>
    public static WeekTargetValues Read(string? weekDataJson, DateOnly weekStart)
    {
        var entry = FindEntry((ReadRoot(weekDataJson)?["week_summary"] as JsonObject)?["training_plan"] as JsonArray, weekStart);
        return new WeekTargetValues(
            ParsePhase(ReadString(entry?["phase"])),
            ReadString(entry?["week_type"]),
            ReadNumber<double>(entry?["weekly_time_target_hours"]),
            ReadNumber<int>(entry?["weekly_load_target"]));
    }

    /// <summary>Applies only supplied overrides, preserving other dataset properties. Invalid JSON is returned unchanged.</summary>
    public static string Apply(string weekDataJson, IReadOnlyList<WeekTargetOverride> overrides)
    {
        var root = ReadRoot(weekDataJson);
        if (root is null || overrides.Count == 0)
            return weekDataJson;

        if (root["week_summary"] is not null and not JsonObject)
            return weekDataJson;

        var summary = root["week_summary"] as JsonObject ?? new JsonObject();
        if (summary["training_plan"] is not null and not JsonArray)
            return weekDataJson;

        root["week_summary"] = summary;
        var plan = summary["training_plan"] as JsonArray ?? new JsonArray();
        summary["training_plan"] = plan;

        foreach (var target in overrides)
        {
            var entry = FindEntry(plan, target.WeekStart);
            if (entry is null)
            {
                entry = new JsonObject { ["week"] = target.WeekStart.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) };
                plan.Add(entry);
            }

            entry["phase"] = target.Phase.ToString();
            if (target.UpdateTargets)
            {
                if (target.WeeklyHours is { } hours)
                    entry["weekly_time_target_hours"] = hours;
                else
                    entry.Remove("weekly_time_target_hours");
                if (target.WeeklyTss is { } tss)
                    entry["weekly_load_target"] = tss;
                else
                    entry.Remove("weekly_load_target");
            }

            var previousType = ReadString(entry["week_type"]);
            if (previousType == "RACE")
                continue;

            var weekType = target.IsRecovery ? "RECOVERY" : "NORMAL";
            if (previousType != weekType)
                entry.Remove("week_note");
            entry["week_type"] = weekType;
        }

        return root.ToJsonString();
    }

    private static JsonObject? ReadRoot(string? json)
    {
        if (string.IsNullOrWhiteSpace(json))
            return null;
        try
        {
            return JsonNode.Parse(json) as JsonObject;
        }
        catch (JsonException)
        {
            return null;
        }
    }

    private static JsonObject? FindEntry(JsonArray? plan, DateOnly weekStart) =>
        plan?.OfType<JsonObject>().FirstOrDefault(entry => ReadDate(entry["week"]) == weekStart);

    private static DateOnly? ReadDate(JsonNode? node) =>
        DateOnly.TryParseExact(ReadString(node), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date)
            ? date : null;

    private static string? ReadString(JsonNode? node) =>
        node is JsonValue value && value.TryGetValue<string>(out var text) ? text : null;

    private static TNumber? ReadNumber<TNumber>(JsonNode? node) where TNumber : struct =>
        node is JsonValue value && value.TryGetValue<TNumber>(out var number) ? number : null;
}