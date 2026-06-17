namespace TrainingArchitect.Core.Models;

/// <summary>The athlete's verdict on a proposed training plan.</summary>
public enum PlanDecisionType
{
    Accepted,
    AdjustmentRequested,
    Rejected
}

/// <summary>
/// Carries the athlete's decision and any accompanying notes back to the
/// orchestration layer.
/// </summary>
public record PlanDecision(PlanDecisionType Type, string Notes);
