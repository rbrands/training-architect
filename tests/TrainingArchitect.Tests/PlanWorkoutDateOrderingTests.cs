using TrainingArchitect.Core.Services;

namespace TrainingArchitect.Tests;

public class PlanWorkoutDateOrderingTests
{
    [Fact]
    public void SwapAndSort_WhenTargetDateIsOccupied_SwapsWorkoutsAndOrdersByDate()
    {
        var monday = new TestWorkout("Monday", new DateOnly(2026, 8, 17));
        var tuesday = new TestWorkout("Tuesday", new DateOnly(2026, 8, 18));
        var thursday = new TestWorkout("Thursday", new DateOnly(2026, 8, 20));
        IList<TestWorkout> workouts = [thursday, monday, tuesday];

        PlanWorkoutDateOrdering.SwapAndSort(
            workouts,
            tuesday,
            thursday.Date,
            workout => workout.Date,
            (workout, date) => workout.Date = date);

        Assert.Equal(new[] { "Monday", "Thursday", "Tuesday" }, workouts.Select(workout => workout.Name));
        Assert.Equal(new DateOnly(2026, 8, 18), thursday.Date);
        Assert.Equal(new DateOnly(2026, 8, 20), tuesday.Date);
    }

    [Fact]
    public void SwapAndSort_WhenTargetDateIsEmpty_MovesWorkoutAndOrdersByDate()
    {
        var monday = new TestWorkout("Monday", new DateOnly(2026, 8, 17));
        var tuesday = new TestWorkout("Tuesday", new DateOnly(2026, 8, 18));
        var thursday = new TestWorkout("Thursday", new DateOnly(2026, 8, 20));
        IList<TestWorkout> workouts = [thursday, tuesday, monday];

        PlanWorkoutDateOrdering.SwapAndSort(
            workouts,
            tuesday,
            new DateOnly(2026, 8, 19),
            workout => workout.Date,
            (workout, date) => workout.Date = date);

        Assert.Equal(new[] { "Monday", "Tuesday", "Thursday" }, workouts.Select(workout => workout.Name));
        Assert.Equal(new DateOnly(2026, 8, 19), tuesday.Date);
    }

    private sealed record TestWorkout(string Name, DateOnly InitialDate)
    {
        public DateOnly Date { get; set; } = InitialDate;
    }
}