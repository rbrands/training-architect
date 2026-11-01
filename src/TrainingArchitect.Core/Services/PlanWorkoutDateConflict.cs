namespace TrainingArchitect.Core.Services;

/// <summary>
/// Applies the permitted same-day ride and strength workout rules.
/// </summary>
public static class PlanWorkoutDateConflict
{
    /// <summary>
    /// Determines whether the activity types on a date exceed one ride and one strength session.
    /// </summary>
    /// <param name="activityTypes">Activity types assigned to the date.</param>
    /// <returns><see langword="true"/> when the date contains multiple rides or strength sessions.</returns>
    public static bool HasConflict(IEnumerable<string?> activityTypes)
    {
        ArgumentNullException.ThrowIfNull(activityTypes);
        var rideCount = 0;
        var strengthCount = 0;
        foreach (var activityType in activityTypes)
        {
            if (ActivityTypeCategories.IsRide(activityType))
            {
                rideCount++;
            }
            else if (ActivityTypeCategories.IsStrength(activityType))
            {
                strengthCount++;
            }
        }

        return rideCount > 1 || strengthCount > 1;
    }

    /// <summary>
    /// Determines whether moving a workout to a date should exchange dates with an existing workout.
    /// </summary>
    /// <param name="movingActivityType">Activity type of the workout being moved.</param>
    /// <param name="targetActivityType">Activity type of a workout already on the target date.</param>
    /// <returns><see langword="true"/> when both workouts belong to the same ride or strength category.</returns>
    public static bool ShouldSwap(string? movingActivityType, string? targetActivityType) =>
        ActivityTypeCategories.IsRide(movingActivityType)
            ? ActivityTypeCategories.IsRide(targetActivityType)
            : ActivityTypeCategories.IsStrength(movingActivityType)
                && ActivityTypeCategories.IsStrength(targetActivityType);
}
