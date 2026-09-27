namespace TrainingArchitect.Services;

/// <summary>Represents an agent failure with a safe message and a server diagnostic reference.</summary>
public sealed class CoachingAgentException(string message, string diagnosticId) : Exception(message)
{
    /// <summary>Gets the reference used to correlate the failure with server diagnostics.</summary>
    public string DiagnosticId { get; } = diagnosticId;
}