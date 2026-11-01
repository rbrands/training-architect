using System.Text.Json;
using TrainingArchitect.Client.Models;
using TrainingArchitect.Core.Models;
using TrainingArchitect.Core.Services;

namespace TrainingArchitect.Tests;

public sealed class StrengthPlanUiTests
{
    [Theory]
    [InlineData("strength-adaptation", StrengthPhase.Adaptation)]
    [InlineData("strength-transition", StrengthPhase.Transition)]
    [InlineData("strength-maximum", StrengthPhase.Maximum)]
    [InlineData("strength-maintenance", StrengthPhase.Maintenance)]
    public void StrengthPhaseParser_ParsesCanonicalTag(string tag, StrengthPhase expectedPhase)
    {
        var parsed = StrengthPhaseParser.TryParse([tag], out var phase);

        Assert.True(parsed);
        Assert.Equal(expectedPhase, phase);
    }

    [Theory]
    [InlineData("strength")]
    [InlineData("strength-high")]
    [InlineData("strength-unknown")]
    [InlineData("Strength-adaptation")]
    public void StrengthPhaseParser_RejectsInvalidTag(string tag)
    {
        var parsed = StrengthPhaseParser.TryParse([tag], out _);

        Assert.False(parsed);
    }

    [Fact]
    public void StrengthPhaseParser_RejectsConflictingCanonicalTags()
    {
        var parsed = StrengthPhaseParser.TryParse(
            ["strength-adaptation", "strength-maximum"],
            out _);

        Assert.False(parsed);
    }

    [Fact]
    public void PlanWorkoutDateConflict_AllowsOneRideAndOneStrengthEntry()
    {
        var hasConflict = PlanWorkoutDateConflict.HasConflict(["Ride", "WeightTraining"]);

        Assert.False(hasConflict);
    }

    [Theory]
    [InlineData("Ride", "VirtualRide")]
    [InlineData("WeightTraining", "WeightTraining")]
    public void PlanWorkoutDateConflict_RejectsMultipleEntriesOfSameCategory(string first, string second)
    {
        var hasConflict = PlanWorkoutDateConflict.HasConflict([first, second]);

        Assert.True(hasConflict);
    }

    [Fact]
    public void PlanWorkoutDateConflict_SwapsOnlyEntriesOfTheSameCategory()
    {
        Assert.True(PlanWorkoutDateConflict.ShouldSwap("Ride", "VirtualRide"));
        Assert.True(PlanWorkoutDateConflict.ShouldSwap("WeightTraining", "WeightTraining"));
        Assert.False(PlanWorkoutDateConflict.ShouldSwap("Ride", "WeightTraining"));
        Assert.False(PlanWorkoutDateConflict.ShouldSwap("WeightTraining", "Ride"));
    }

    [Fact]
    public void PlanWorkoutViewModel_ParsesWeightTrainingAndHidesTss()
    {
        using var document = JsonDocument.Parse("""
            {
              "date":"2026-11-02",
              "activity_type":"WeightTraining",
              "name":"Strength session",
              "duration_minutes":45,
              "tss":0,
              "tags":["strength-transition"],
              "description":"Off-bike strength work."
            }
            """);

        var workout = PlanWorkoutViewModel.TryCreate(document.RootElement, 0);

        Assert.NotNull(workout);
        Assert.Equal("WeightTraining", workout.ActivityType);
        Assert.True(workout.IsStrength);
        Assert.Equal(StrengthPhase.Transition, workout.StrengthPhase);
        Assert.Null(workout.TargetTss);
        Assert.Equal(45, workout.DurationMinutes);
    }

    [Fact]
    public void PlanWorkoutDateOrdering_SortsRideBeforeStrengthWithinDate()
    {
        var date = new DateOnly(2026, 11, 2);
        var strength = new TestWorkout("Strength", date, 1);
        var ride = new TestWorkout("Ride", date, 0);
        IList<TestWorkout> workouts = [strength, ride];

        PlanWorkoutDateOrdering.Sort(workouts, workout => workout.Date, workout => workout.DayOrder);

        Assert.Equal(["Ride", "Strength"], workouts.Select(workout => workout.Name));
    }

    private sealed record TestWorkout(string Name, DateOnly Date, int DayOrder);
}
