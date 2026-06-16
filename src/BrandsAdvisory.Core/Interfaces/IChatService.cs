using BrandsAdvisory.Core.Models;

namespace BrandsAdvisory.Core.Interfaces;

/// <summary>
/// Manages the coaching chat conversation between the athlete and the AI coach.
/// </summary>
public interface IChatService
{
    /// <summary>Current ordered conversation history (oldest first).</summary>
    IReadOnlyList<ChatMessage> Messages { get; }

    /// <summary>
    /// Appends the athlete's message and returns the assistant reply.
    /// TODO: Implement by calling the .NET AI orchestration pipeline
    ///       (e.g. Microsoft.Extensions.AI / Semantic Kernel agent).
    /// </summary>
    Task<ChatMessage> SendAsync(string userMessage, CancellationToken cancellationToken = default);

    /// <summary>Clears the current conversation history.</summary>
    void ClearConversation();
}
