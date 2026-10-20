namespace TrainingArchitect.Core.Models;

/// <summary>Identifies the training phase used to estimate weekly load.</summary>
public enum TrainingPhase
{
    Base,
    Build,
    Peak,
    Transition
}

/// <summary>Represents the athlete's edited targets for an ISO week.</summary>
public sealed record WeekTargetOverride(
    DateOnly WeekStart,
    TrainingPhase Phase,
    bool IsRecovery,
    double? WeeklyHours,
    int? WeeklyTss,
    bool UpdateTargets = true);

/// <summary>Contains the original planning targets for a week.</summary>
public sealed record WeekTargetValues(
    TrainingPhase Phase,
    string? WeekType,
    double? WeeklyHours,
    int? WeeklyTss);