using BrandsAdvisory.Core.Interfaces;
using BrandsAdvisory.Core.Models;

namespace BrandsAdvisory.Core.Services;

/// <summary>
/// Development stub for <see cref="IChatService"/>.
/// Returns canned responses; replace with the real AI orchestration pipeline.
/// TODO: Wire up to Microsoft.Extensions.AI / Semantic Kernel agent graph.
/// </summary>
public sealed class StubChatService : IChatService
{
    private readonly List<ChatMessage> _messages = [];

    public IReadOnlyList<ChatMessage> Messages => _messages;

    public Task<ChatMessage> SendAsync(string userMessage, CancellationToken cancellationToken = default)
    {
        _messages.Add(new ChatMessage
        {
            Role = MessageRole.User,
            Content = userMessage
        });

        // TODO: Replace with call to the AI orchestration agent.
        var reply = new ChatMessage
        {
            Role = MessageRole.Assistant,
            Content = "⚙️ [Stub] The AI coaching engine is not yet connected. " +
                      "Your message has been received and will be processed once " +
                      "the orchestration layer is wired up."
        };

        _messages.Add(reply);
        return Task.FromResult(reply);
    }

    public void ClearConversation() => _messages.Clear();
}
