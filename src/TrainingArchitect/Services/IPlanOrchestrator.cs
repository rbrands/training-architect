using TrainingArchitect.Core.Models;

namespace TrainingArchitect.Services;

/// <summary>
/// Runs the multi-step plan creation workflow and reports progress while it executes.
/// </summary>
public interface IPlanOrchestrator
{
    /// <summary>
    /// Generates a weekly plan, validates it, and applies correction rounds when needed.
    /// </summary>
    /// <param name="request">The plan request.</param>
    /// <param name="intervalsAthleteId">The intervals.icu athlete ID.</param>
    /// <param name="intervalsApiKey">The intervals.icu API key.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A stream of progress events; the final event is either Completed or Failed.</returns>
    IAsyncEnumerable<PlanProgressEvent> RunAsync(
        PlanRequest request,
        string intervalsAthleteId,
        string intervalsApiKey,
        CancellationToken ct = default);
}
