namespace TrainingArchitect.Core.Models;

/// <summary>
/// A single problem detected in a generated plan.
/// </summary>
/// <param name="Code">Stable identifier used for logging and tests.</param>
/// <param name="Message">Description handed to the coaching agent for correction.</param>
public record PlanValidationFinding(string Code, string Message);

/// <summary>
/// Outcome of validating a generated plan.
/// </summary>
public record PlanValidationResult(IReadOnlyList<PlanValidationFinding> Findings)
{
    /// <summary>
    /// Gets an optional status text describing what the validator checked, shown to the athlete.
    /// </summary>
    public string? Summary { get; init; }

    /// <summary>
    /// Gets a value indicating whether the plan passed validation without findings.
    /// </summary>
    public bool IsValid => Findings.Count == 0;

    /// <summary>
    /// Gets a result without any findings.
    /// </summary>
    public static PlanValidationResult Valid { get; } = new([]);

    /// <summary>
    /// Creates a result from a single finding.
    /// </summary>
    public static PlanValidationResult FromFinding(string code, string message) =>
        new([new PlanValidationFinding(code, message)]);
}
