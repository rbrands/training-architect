using System.Globalization;
using System.Text.Json;

namespace TrainingArchitect.Core.Services;

/// <summary>Determines the next training recommendation date from the athlete dataset.</summary>
public static class RecommendationDateResolver
{
    /// <summary>
    /// Returns current_date, or the following day when any activity has that date.
    /// Returns null for missing or invalid current_date values or invalid JSON.
    /// </summary>
    public static DateOnly? Resolve(string? weekDataJson)
    {
        if (string.IsNullOrWhiteSpace(weekDataJson))
            return null;

        try
        {
            using var document = JsonDocument.Parse(weekDataJson);
            var root = document.RootElement;
            if (root.ValueKind != JsonValueKind.Object
                || !root.TryGetProperty("current_date", out var dateProperty)
                || dateProperty.ValueKind != JsonValueKind.String
                || !DateOnly.TryParseExact(dateProperty.GetString(), "yyyy-MM-dd", CultureInfo.InvariantCulture,
                    DateTimeStyles.None, out var currentDate))
                return null;

            if (root.TryGetProperty("activities", out var activities) && activities.ValueKind == JsonValueKind.Array
                && activities.EnumerateArray().Any(activity => activity.ValueKind == JsonValueKind.Object
                    && activity.TryGetProperty("date", out var activityDate)
                    && activityDate.ValueKind == JsonValueKind.String
                    && activityDate.GetString() == dateProperty.GetString()))
                return currentDate == DateOnly.MaxValue ? null : currentDate.AddDays(1);

            return currentDate;
        }
        catch (JsonException)
        {
            return null;
        }
    }
}