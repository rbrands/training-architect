namespace TrainingArchitect.Services;

/// <summary>
/// Represents an error response returned by an MCP tool call.
/// </summary>
public sealed class McpToolExecutionException : Exception
{
    public McpToolExecutionException(string message)
        : base(message)
    {
    }

    public McpToolExecutionException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
