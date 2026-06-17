using TrainingArchitect.Core.Models;

namespace TrainingArchitect.Core.Services;

/// <summary>
/// Generates a representative sample <see cref="TrainingPlan"/> for development
/// and UI scaffolding.  Replace with AI-generated plans once the orchestration
/// layer is connected.
/// TODO: Remove or gate behind a feature flag when real plan generation is active.
/// </summary>
public static class SampleTrainingPlan
{
    /// <summary>Returns a sample week-long training plan starting from today.</summary>
    public static TrainingPlan Create()
    {
        var monday = GetNextMonday();
        return new TrainingPlan
        {
            Title = "Build Week 3 — FTP Focus (sample)",
            Description = "A structured mid-build week targeting threshold adaptation. " +
                          "Replace this with an AI-generated plan once the orchestration layer is wired up.",
            WeekStarting = monday,
            Workouts =
            [
                new PlannedWorkout
                {
                    Day = "Monday",
                    Title = "Rest / Mobility",
                    Description = "Complete rest or 20 min easy mobility work. Let the previous week's load absorb.",
                    Type = WorkoutType.Rest,
                    DurationMinutes = 20,
                    Tss = 0,
                    IntensityFactor = 0.0,
                    Zone = "—"
                },
                new PlannedWorkout
                {
                    Day = "Tuesday",
                    Title = "Sweet Spot Intervals",
                    Description = "3 × 12 min @ 88–93 % FTP with 5 min recovery between. " +
                                  "Focus on smooth pedalling and controlled breathing.",
                    Type = WorkoutType.Threshold,
                    DurationMinutes = 75,
                    Tss = 85,
                    IntensityFactor = 0.87,
                    Zone = "Zone 4"
                },
                new PlannedWorkout
                {
                    Day = "Wednesday",
                    Title = "Z2 Endurance Spin",
                    Description = "90 min steady aerobic ride at 60–75 % FTP. Conversational pace. " +
                                  "Focus on fat oxidation and active recovery.",
                    Type = WorkoutType.Endurance,
                    DurationMinutes = 90,
                    Tss = 65,
                    IntensityFactor = 0.68,
                    Zone = "Zone 2"
                },
                new PlannedWorkout
                {
                    Day = "Thursday",
                    Title = "VO₂max Efforts",
                    Description = "6 × 3 min @ 110–115 % FTP, 3 min easy between. " +
                                  "Push hard but aim to complete all reps.",
                    Type = WorkoutType.VO2Max,
                    DurationMinutes = 60,
                    Tss = 90,
                    IntensityFactor = 0.94,
                    Zone = "Zone 5"
                },
                new PlannedWorkout
                {
                    Day = "Friday",
                    Title = "Easy Spin / Off",
                    Description = "30–45 min easy legs or full rest based on how you feel. " +
                                  "Keep HR below 130 bpm.",
                    Type = WorkoutType.Endurance,
                    DurationMinutes = 40,
                    Tss = 25,
                    IntensityFactor = 0.55,
                    Zone = "Zone 1"
                },
                new PlannedWorkout
                {
                    Day = "Saturday",
                    Title = "Tempo Long Ride",
                    Description = "2.5 h with 2 × 20 min tempo blocks @ 83–88 % FTP in the middle hour. " +
                                  "Eat and drink well throughout.",
                    Type = WorkoutType.Tempo,
                    DurationMinutes = 150,
                    Tss = 130,
                    IntensityFactor = 0.79,
                    Zone = "Zone 3–4"
                },
                new PlannedWorkout
                {
                    Day = "Sunday",
                    Title = "Long Aerobic Ride",
                    Description = "3 h easy group ride or solo long ride at Z2. " +
                                  "No pressure — just time in the saddle.",
                    Type = WorkoutType.Endurance,
                    DurationMinutes = 180,
                    Tss = 100,
                    IntensityFactor = 0.65,
                    Zone = "Zone 2"
                }
            ]
        };
    }

    private static DateOnly GetNextMonday()
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        int daysUntilMonday = ((int)DayOfWeek.Monday - (int)today.DayOfWeek + 7) % 7;
        return daysUntilMonday == 0 ? today : today.AddDays(daysUntilMonday);
    }
}
