namespace TrainingArchitect.Services;

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
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The coaching agent response text.</returns>
    Task<string> PromptAsync(
        string prompt,
        string discipline,
        string language,
        CancellationToken ct = default);
}
