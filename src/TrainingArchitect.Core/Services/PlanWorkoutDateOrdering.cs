namespace TrainingArchitect.Core.Services;

/// <summary>
/// Applies date changes to generated plan workouts while preserving chronological order.
/// </summary>
public static class PlanWorkoutDateOrdering
{
    /// <summary>
    /// Moves a workout to a date, swaps an existing workout back to the original date, and sorts the list by date.
    /// </summary>
    public static void SwapAndSort<T>(
        IList<T> workouts,
        T movedWorkout,
        DateOnly targetDate,
        Func<T, DateOnly> getDate,
        Action<T, DateOnly> setDate)
        where T : class
    {
        var originalDate = getDate(movedWorkout);
        var targetWorkout = workouts.FirstOrDefault(workout =>
            !ReferenceEquals(workout, movedWorkout) && getDate(workout) == targetDate);

        if (targetWorkout is not null)
        {
            setDate(targetWorkout, originalDate);
        }

        setDate(movedWorkout, targetDate);

        var orderedWorkouts = workouts.OrderBy(getDate).ToArray();
        workouts.Clear();

        foreach (var workout in orderedWorkouts)
        {
            workouts.Add(workout);
        }
    }
}