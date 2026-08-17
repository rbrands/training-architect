using TrainingArchitect.Core.Models;

namespace TrainingArchitect.Services;

/// <summary>
/// Validates a generated weekly plan before it is handed back to the athlete.
/// </summary>
public interface IPlanValidator
{
    /// <summary>
    /// Checks the upload JSON of a generated plan.
    /// </summary>
    /// <param name="uploadJson">The extracted upload JSON block.</param>
    /// <param name="request">The originating plan request.</param>
    /// <param name="intervalsAthleteId">Optional intervals.icu athlete ID.</param>
    /// <param name="intervalsApiKey">Optional intervals.icu API key.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The findings that require a correction round, or an empty result when the plan is acceptable.</returns>
    Task<PlanValidationResult> ValidateAsync(
        string uploadJson,
        PlanRequest request,
        string? intervalsAthleteId,
        string? intervalsApiKey,
        CancellationToken ct = default);
}
