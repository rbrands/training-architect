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

    public async Task<CoachingAgentResponse> PromptAsync(
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
            var totalTokens = LogTokenUsage(response.Value);
            return new CoachingAgentResponse(response.Value.GetOutputText(), totalTokens);
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

    private long? LogTokenUsage(object? responseValue)
    {
        if (!TryGetTokenUsage(responseValue, out var inputTokens, out var outputTokens, out var totalTokens))
        {
            _logger.LogDebug(
                "Foundry agent call completed for {AgentName}@{AgentVersion}, but token usage was not present in the response payload.",
                _configuredAgent.Name,
                _configuredAgent.Version ?? "latest");
            return null;
        }

        _logger.LogInformation(
            "Foundry agent token usage for {AgentName}@{AgentVersion}: input={InputTokens}, output={OutputTokens}, total={TotalTokens}",
            _configuredAgent.Name,
            _configuredAgent.Version ?? "latest",
            inputTokens,
            outputTokens,
            totalTokens);

            return totalTokens;
    }

    private static bool TryGetTokenUsage(object? responseValue, out long? inputTokens, out long? outputTokens, out long? totalTokens)
    {
        inputTokens = null;
        outputTokens = null;
        totalTokens = null;

        if (responseValue is null)
        {
            return false;
        }

        var responseType = responseValue.GetType();
        var usageProperty = responseType.GetProperty("Usage");
        var usage = usageProperty?.GetValue(responseValue);

        if (usage is null)
        {
            return false;
        }

        // Confirmed runtime shape:
        // OpenAI.Responses.ResponseTokenUsage with
        // InputTokenCount / OutputTokenCount / TotalTokenCount.
        inputTokens = ReadLongProperty(usage, "InputTokenCount");
        outputTokens = ReadLongProperty(usage, "OutputTokenCount");
        totalTokens = ReadLongProperty(usage, "TotalTokenCount");

        return inputTokens.HasValue || outputTokens.HasValue || totalTokens.HasValue;
    }

    private static long? ReadLongProperty(object source, params string[] propertyNames)
    {
        var sourceType = source.GetType();

        foreach (var propertyName in propertyNames)
        {
            var property = sourceType.GetProperty(propertyName);
            if (property is null)
            {
                continue;
            }

            var value = property.GetValue(source);
            if (value is null)
            {
                continue;
            }

            if (value is int intValue)
            {
                return intValue;
            }

            if (value is long longValue)
            {
                return longValue;
            }

            if (long.TryParse(value.ToString(), out var parsed))
            {
                return parsed;
            }
        }

        return null;
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