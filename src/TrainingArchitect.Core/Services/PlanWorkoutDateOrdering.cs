namespace TrainingArchitect.Core.Services;

/// <summary>
/// Applies date changes to generated plan workouts while preserving chronological order.
/// </summary>
public static class PlanWorkoutDateOrdering
{
    /// <summary>
    /// Sorts workouts chronologically, applying a caller-provided order within each date.
    /// </summary>
    /// <param name="workouts">The workout collection to sort.</param>
    /// <param name="getDate">Gets the date for a workout.</param>
    /// <param name="getDayOrder">Gets the order for entries sharing a date.</param>
    public static void Sort<T>(
        IList<T> workouts,
        Func<T, DateOnly> getDate,
        Func<T, int>? getDayOrder = null)
    {
        var orderedWorkouts = getDayOrder is null
            ? workouts.OrderBy(getDate).ToArray()
            : workouts.OrderBy(getDate).ThenBy(getDayOrder).ToArray();
        workouts.Clear();

        foreach (var workout in orderedWorkouts)
        {
            workouts.Add(workout);
        }
    }

    /// <summary>
    /// Moves a workout to a date, swaps an existing workout back to the original date, and sorts the list by date.
    /// </summary>
    /// <param name="workouts">The workout collection to update.</param>
    /// <param name="movedWorkout">The workout being moved.</param>
    /// <param name="targetDate">The destination date.</param>
    /// <param name="getDate">Gets the date for a workout.</param>
    /// <param name="setDate">Sets a workout's date.</param>
    /// <param name="getDayOrder">Gets the order for entries sharing a date.</param>
    /// <param name="isSwapCandidate">Determines whether an existing target-date workout should exchange dates.</param>
    public static void SwapAndSort<T>(
        IList<T> workouts,
        T movedWorkout,
        DateOnly targetDate,
        Func<T, DateOnly> getDate,
        Action<T, DateOnly> setDate,
        Func<T, int>? getDayOrder = null,
        Func<T, T, bool>? isSwapCandidate = null)
        where T : class
    {
        var originalDate = getDate(movedWorkout);
        var targetWorkout = workouts.FirstOrDefault(workout =>
            !ReferenceEquals(workout, movedWorkout)
            && getDate(workout) == targetDate
            && (isSwapCandidate is null || isSwapCandidate(movedWorkout, workout)));

        if (targetWorkout is not null)
        {
            setDate(targetWorkout, originalDate);
        }

        setDate(movedWorkout, targetDate);

        Sort(workouts, getDate, getDayOrder);
    }
}