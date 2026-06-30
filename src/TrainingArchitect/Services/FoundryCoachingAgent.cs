using Azure;
using Azure.AI.Projects;
using Azure.AI.Extensions.OpenAI;
using Microsoft.Extensions.Logging;
using OpenAI.Responses;
using System.Text.Json;

namespace TrainingArchitect.Services;

#pragma warning disable OPENAI001
#pragma warning disable SCME0001

public sealed class FoundryCoachingAgent(
    AIProjectClient projectClient,
    IConfiguration configuration,
    ILogger<FoundryCoachingAgent> logger) : ICoachingAgent
{
    private readonly AIProjectClient _projectClient = projectClient;
    private readonly ILogger<FoundryCoachingAgent> _logger = logger;
    private readonly (string Name, string? Version) _configuredAgent = ParseConfiguredAgent(
        configuration["FoundryProjectAgentName"]
        ?? throw new InvalidOperationException("Configuration key 'FoundryProjectAgentName' is required."));

    public async Task<string> PromptAsync(
        string prompt,
        string discipline,
        string language,
        CancellationToken ct = default,
        string? intervalsAthleteId = null,
        string? intervalsApiKey = null)
    {
        var structuredInputs = new Dictionary<string, string>
        {
            ["discipline"] = discipline,
            ["response_language"] = language
        };

        if (!string.IsNullOrWhiteSpace(intervalsAthleteId))
        {
            structuredInputs["intervals_athlete_id"] = intervalsAthleteId;
        }

        if (!string.IsNullOrWhiteSpace(intervalsApiKey))
        {
            structuredInputs["intervals_api_key"] = intervalsApiKey;
        }

        var configuredReference = new AgentReference(_configuredAgent.Name, _configuredAgent.Version);
        var responseClient = _projectClient.ProjectOpenAIClient
            .GetProjectResponsesClientForAgent(configuredReference, defaultConversationId: null);

        var options = new CreateResponseOptions();
        options.InputItems.Add(ResponseItem.CreateUserMessageItem(prompt));
        options.Patch.Set("$.structured_inputs"u8, BinaryData.FromObjectAsJson(structuredInputs));

        try
        {
            var response = await responseClient.CreateResponseAsync(options, ct);
            return response.Value.GetOutputText();
        }
        catch (RequestFailedException ex)
        {
            var normalized = NormalizeErrorMessage(ex.Message);
            var requestId = ExtractRequestId(ex, normalized);

            _logger.LogError(
                ex,
                "Foundry agent call failed for agent {AgentName}@{AgentVersion}. RequestId={RequestId}; Code={Code}; Param={Param}; Type={Type}; Message={Message}",
                _configuredAgent.Name,
                _configuredAgent.Version ?? "latest",
                requestId ?? "unknown",
                normalized.Code ?? "unknown",
                normalized.Param ?? "unknown",
                normalized.Type ?? "unknown",
                normalized.Message ?? ex.Message);

            throw;
        }
    }

    private static (string Name, string? Version) ParseConfiguredAgent(string configuredValue)
    {
        var trimmedValue = configuredValue.Trim();
        if (string.IsNullOrWhiteSpace(trimmedValue))
        {
            throw new InvalidOperationException("Configuration key 'FoundryProjectAgentName' must not be empty.");
        }

        var separatorIndex = trimmedValue.IndexOf('@');
        if (separatorIndex <= 0)
        {
            return (trimmedValue, null);
        }

        var name = trimmedValue[..separatorIndex].Trim();
        var version = trimmedValue[(separatorIndex + 1)..].Trim();

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new InvalidOperationException("'FoundryProjectAgentName' must contain a valid agent name.");
        }

        if (string.IsNullOrWhiteSpace(version))
        {
            version = null;
        }

        return (name, version);
    }

    private static (string? Code, string? Param, string? Type, string? Message, string? RequestId) NormalizeErrorMessage(string rawMessage)
    {
        if (string.IsNullOrWhiteSpace(rawMessage))
        {
            return (null, null, null, null, null);
        }

        var jsonStart = rawMessage.IndexOf('{');
        if (jsonStart < 0)
        {
            return (null, null, null, rawMessage, null);
        }

        var json = rawMessage[jsonStart..];

        try
        {
            using var document = JsonDocument.Parse(json);
            var root = document.RootElement;

            var code = TryGetString(root, "code");
            var message = TryGetString(root, "message") ?? rawMessage;
            var param = TryGetString(root, "param");
            var type = TryGetString(root, "type");

            string? requestId = null;
            if (root.TryGetProperty("additionalInfo", out var additionalInfo)
                && additionalInfo.ValueKind == JsonValueKind.Object)
            {
                requestId = TryGetString(additionalInfo, "request_id");
            }

            return (code, param, type, message, requestId);
        }
        catch (JsonException)
        {
            return (null, null, null, rawMessage, null);
        }
    }

    private static string? TryGetString(JsonElement element, string propertyName)
    {
        return element.TryGetProperty(propertyName, out var value)
            && value.ValueKind == JsonValueKind.String
            ? value.GetString()
            : null;
    }

    private static string? ExtractRequestId(RequestFailedException exception, (string? Code, string? Param, string? Type, string? Message, string? RequestId) normalized)
    {
        if (!string.IsNullOrWhiteSpace(normalized.RequestId))
        {
            return normalized.RequestId;
        }

        var rawResponse = exception.GetRawResponse();
        if (rawResponse is not null
            && rawResponse.Headers.TryGetValue("x-ms-request-id", out var headerRequestId)
            && !string.IsNullOrWhiteSpace(headerRequestId))
        {
            return headerRequestId;
        }

        return null;
    }
}

#pragma warning restore OPENAI001
#pragma warning restore SCME0001