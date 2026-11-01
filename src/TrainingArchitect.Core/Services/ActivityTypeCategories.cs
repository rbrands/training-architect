namespace TrainingArchitect.Core.Services;

/// <summary>
/// Categorizes intervals.icu activity types for training-plan accounting.
/// </summary>
public static class ActivityTypeCategories
{
    /// <summary>
    /// Gets whether an activity type represents a ride, including an unspecified type.
    /// </summary>
    /// <param name="activityType">The intervals.icu activity type.</param>
    /// <returns><see langword="true"/> when the activity is a ride or has no type.</returns>
    public static bool IsRide(string? activityType) =>
        string.IsNullOrEmpty(activityType)
        || activityType is "Ride" or "VirtualRide" or "MountainBikeRide" or "GravelRide";

    /// <summary>
    /// Gets whether an activity type represents a strength session.
    /// </summary>
    /// <param name="activityType">The intervals.icu activity type.</param>
    /// <returns><see langword="true"/> when the activity is a strength session.</returns>
    public static bool IsStrength(string? activityType) => activityType == "WeightTraining";

    /// <summary>
    /// Gets whether an activity type represents a run.
    /// </summary>
    /// <param name="activityType">The intervals.icu activity type.</param>
    /// <returns><see langword="true"/> when the activity is a run.</returns>
    public static bool IsRun(string? activityType) =>
        activityType is "Run" or "TrailRun" or "VirtualRun";
}
