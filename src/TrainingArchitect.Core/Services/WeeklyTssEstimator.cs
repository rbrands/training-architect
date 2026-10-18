using TrainingArchitect.Core.Models;

namespace TrainingArchitect.Core.Services;

/// <summary>Estimates weekly TSS using rates derived from the intervals.icu Targets Generator.</summary>
public static class WeeklyTssEstimator
{
    /// <summary>Base phase TSS per hour, derived from the intervals.icu Targets Generator.</summary>
    public const double BaseTssPerHour = 48.4;
    /// <summary>Build phase TSS per hour, derived from the intervals.icu Targets Generator.</summary>
    public const double BuildTssPerHour = 52.5;
    /// <summary>Peak phase TSS per hour, derived from the intervals.icu Targets Generator.</summary>
    public const double PeakTssPerHour = 56.0;
    /// <summary>Transition phase TSS per hour, derived from the intervals.icu Targets Generator.</summary>
    public const double TransitionTssPerHour = 40.0;

    /// <summary>Gets the hourly TSS rate for the phase.</summary>
    public static double TssPerHour(TrainingPhase phase) => phase switch
    {
        TrainingPhase.Build => BuildTssPerHour,
        TrainingPhase.Peak => PeakTssPerHour,
        TrainingPhase.Transition => TransitionTssPerHour,
        _ => BaseTssPerHour
    };

    /// <summary>Estimates TSS from hours already adjusted for recovery, without an additional recovery reduction.</summary>
    public static int Estimate(TrainingPhase phase, double hours) =>
        (int)Math.Round(hours * TssPerHour(phase));
}