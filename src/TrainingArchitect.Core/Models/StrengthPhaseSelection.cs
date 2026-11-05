namespace TrainingArchitect.Core.Models;

/// <summary>Strength phase preference used when generating a training plan.</summary>
public enum StrengthPhaseSelection
{
    Automatic,
    Adaptation,
    Transition,
    Maximum,
    Maintenance,
    None
}
