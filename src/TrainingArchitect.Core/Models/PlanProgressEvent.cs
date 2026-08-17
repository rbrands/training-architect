namespace TrainingArchitect.Core.Models;

/// <summary>
/// Lifecycle stage of a multi-step plan creation run.
/// </summary>
public enum PlanProgressStage
{
    Started,
    Generating,
    Generated,
    Validating,
    Correcting,
    Completed,
    Failed
}

/// <summary>
/// Progress update streamed to the client while a plan is being created.
/// </summary>
/// <param name="Stage">The current orchestration stage.</param>
/// <param name="Message">Human readable status text for the UI.</param>
/// <param name="Round">Zero-based correction round the event belongs to.</param>
/// <param name="Result">The finished plan; only set on <see cref="PlanProgressStage.Completed"/>.</param>
/// <param name="Warnings">Validation findings that remained unresolved after the last correction round.</param>
public record PlanProgressEvent(
    PlanProgressStage Stage,
    string Message,
    int Round = 0,
    PlanResponse? Result = null,
    IReadOnlyList<string>? Warnings = null);
