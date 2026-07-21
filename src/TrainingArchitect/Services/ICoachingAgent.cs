namespace TrainingArchitect.Services;

public sealed record CoachingAgentResponse(string Content, long? TotalTokens);

/// <summary>
/// Sends prompts to the coaching agent and returns the generated response.
/// </summary>
public interface ICoachingAgent
{
    /// <summary>
    /// Sends a prompt to the coaching agent with discipline and language context.
    /// </summary>
    /// <param name="prompt">The athlete prompt.</param>
    /// <param name="discipline">The training discipline context.</param>
    /// <param name="language">The requested response language.</param>
    /// <param name="intervalsAthleteId">Optional intervals.icu athlete ID used in a later workflow step.</param>
    /// <param name="intervalsApiKey">Optional intervals.icu API key used in a later workflow step.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The coaching agent response payload including output text and optional token usage metadata.</returns>
    Task<CoachingAgentResponse> PromptAsync(
        string prompt,
        string discipline,
        string language,
        CancellationToken ct = default,
        string? intervalsAthleteId = null,
        string? intervalsApiKey = null);
}
