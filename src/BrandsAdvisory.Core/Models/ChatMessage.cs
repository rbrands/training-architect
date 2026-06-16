namespace BrandsAdvisory.Core.Models;

/// <summary>Identifies the originator of a chat message.</summary>
public enum MessageRole
{
    User,
    Assistant
}

/// <summary>A single message in the coaching chat conversation.</summary>
public class ChatMessage
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public required MessageRole Role { get; init; }
    public required string Content { get; init; }
    public DateTimeOffset Timestamp { get; init; } = DateTimeOffset.UtcNow;
}
