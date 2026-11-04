using System.Globalization;
using System.Text.Json;
using TrainingArchitect.Core.Models;
using TrainingArchitect.Core.Services;

namespace TrainingArchitect.Client.Models;

/// <summary>
/// Editable representation of a workout entry in the Coach plan editor.
/// </summary>
public sealed class PlanWorkoutViewModel
{
    /// <summary>Gets the stable editor row identifier.</summary>
    public required string Id { get; init; }

    /// <summary>Gets the library workout identifier, when present.</summary>
    public string? LibraryWorkoutId { get; init; }

    /// <summary>Gets whether the workout came from the workout library.</summary>
    public bool IsLibraryWorkout => !string.IsNullOrWhiteSpace(LibraryWorkoutId);

    /// <summary>Gets or sets the scheduled date.</summary>
    public required DateOnly Date { get; set; }

    /// <summary>Gets the workout title.</summary>
    public required string Name { get; init; }

    /// <summary>Gets the workout activity type.</summary>
    public string? ActivityType { get; init; }

    /// <summary>Gets whether the workout is a strength session.</summary>
    public bool IsStrength => ActivityTypeCategories.IsStrength(ActivityType);

    /// <summary>Gets the strength phase parsed from canonical workout tags.</summary>
    public StrengthPhase? StrengthPhase { get; init; }

    /// <summary>Gets the planned duration in minutes.</summary>
    public int? DurationMinutes { get; init; }

    /// <summary>Gets the displayed target TSS, omitted for strength entries.</summary>
    public double? TargetTss { get; init; }

    /// <summary>Gets the workout description.</summary>
    public string? Description { get; init; }

    /// <summary>Gets the workout tags.</summary>
    public IReadOnlyList<string> Tags { get; init; } = [];

    /// <summary>Gets tag chips, excluding redundant strength tags when the phase label is displayed.</summary>
    public IEnumerable<string> DisplayTags => IsStrength && StrengthPhase is not null
        ? Tags.Where(tag => !tag.StartsWith("strength-", StringComparison.Ordinal))
        : Tags;

    /// <summary>Gets the source JSON preserved when the editor serializes changes.</summary>
    public required JsonElement OriginalWorkout { get; init; }

    /// <summary>
    /// Creates an editor view model from a workout JSON object.
    /// </summary>
    /// <param name="workout">The source workout JSON object.</param>
    /// <param name="index">The workout's original array index.</param>
    /// <returns>The view model, or <see langword="null"/> when the workout has no valid date.</returns>
    public static PlanWorkoutViewModel? TryCreate(JsonElement workout, int index)
    {
        var dateText = ReadString(workout, "date");
        if (string.IsNullOrWhiteSpace(dateText)
            || !DateOnly.TryParse(dateText, CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
        {
            return null;
        }

        var tags = workout.TryGetProperty("tags", out var tagsElement) && tagsElement.ValueKind == JsonValueKind.Array
            ? tagsElement
                .EnumerateArray()
                .Where(tag => tag.ValueKind == JsonValueKind.String)
                .Select(tag => tag.GetString())
                .Where(tag => !string.IsNullOrWhiteSpace(tag))
                .Select(tag => tag!)
                .ToArray()
            : [];
        StrengthPhase? strengthPhase = StrengthPhaseParser.TryParse(tags, out var parsedPhase)
            ? parsedPhase
            : null;
        var activityType = ReadString(workout, "activity_type");

        return new PlanWorkoutViewModel
        {
            Id = $"workout-{index}",
            LibraryWorkoutId = ReadString(workout, "library_workout_id") ?? ReadString(workout, "id"),
            Date = date,
            Name = ReadString(workout, "name") ?? $"Workout {index + 1}",
            ActivityType = activityType,
            StrengthPhase = strengthPhase,
            DurationMinutes = ReadInt(workout, "duration_minutes"),
            TargetTss = ActivityTypeCategories.IsStrength(activityType) ? null : ReadDouble(workout, "tss"),
            Description = ReadString(workout, "description"),
            Tags = tags,
            OriginalWorkout = workout.Clone()
        };
    }

    private static string? ReadString(JsonElement element, string propertyName) =>
        element.TryGetProperty(propertyName, out var property) && property.ValueKind == JsonValueKind.String
            ? property.GetString()
            : null;

    private static int? ReadInt(JsonElement element, string propertyName) =>
        element.TryGetProperty(propertyName, out var property)
        && property.ValueKind == JsonValueKind.Number
        && property.TryGetInt32(out var value)
            ? value
            : null;

    private static double? ReadDouble(JsonElement element, string propertyName) =>
        element.TryGetProperty(propertyName, out var property)
        && property.ValueKind == JsonValueKind.Number
        && property.TryGetDouble(out var value)
        && double.IsFinite(value)
            ? value
            : null;
}
