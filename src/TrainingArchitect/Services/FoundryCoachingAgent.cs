using Azure;
using Azure.AI.Projects;
using Azure.AI.Extensions.OpenAI;
using Microsoft.Extensions.Logging;
using OpenAI.Responses;
using System.Collections;
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
            var tokenUsage = ExtractTokenUsage(response.Value);
            LogTokenUsage(tokenUsage);
            var responseId = TryGetResponseId(response.Value);
            var content = ExtractAssistantOutputText(response.Value);

            if (LooksLikeHtmlDocument(content))
            {
                _logger.LogWarning(
                    "Foundry agent returned HTML-like content for {AgentName}@{AgentVersion}. ResponseId={ResponseId}",
                    _configuredAgent.Name,
                    _configuredAgent.Version ?? "latest",
                    responseId ?? "unknown");
            }

            return new CoachingAgentResponse(
                content,
                tokenUsage.TotalTokens,
                responseId,
                tokenUsage.InputTokens,
                tokenUsage.CachedInputTokens,
                tokenUsage.OutputTokens);
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

            if (TryBuildModelCompatibilityError(normalized.Message ?? ex.Message, out var compatibilityMessage))
            {
                throw new InvalidOperationException(compatibilityMessage, ex);
            }

            throw;
        }
        catch (Exception ex) when (TryBuildModelCompatibilityError(ex.Message, out var compatibilityMessage))
        {
            _logger.LogError(
                ex,
                "Foundry agent model compatibility issue for {AgentName}@{AgentVersion}: {Message}",
                _configuredAgent.Name,
                _configuredAgent.Version ?? "latest",
                compatibilityMessage);

            throw new InvalidOperationException(compatibilityMessage, ex);
        }
    }

    private void LogTokenUsage(TokenUsageSnapshot usage)
    {
        if (!usage.HasAnyValue)
        {
            _logger.LogDebug(
                "Foundry agent call completed for {AgentName}@{AgentVersion}, but token usage was not present in the response payload.",
                _configuredAgent.Name,
                _configuredAgent.Version ?? "latest");
            return;
        }

        _logger.LogInformation(
            "Foundry agent token usage for {AgentName}@{AgentVersion}: input={InputTokens}, cachedInput={CachedInputTokens}, output={OutputTokens}, total={TotalTokens}",
            _configuredAgent.Name,
            _configuredAgent.Version ?? "latest",
            usage.InputTokens,
            usage.CachedInputTokens,
            usage.OutputTokens,
            usage.TotalTokens);
    }

    private static TokenUsageSnapshot ExtractTokenUsage(object? responseValue)
    {
        if (responseValue is null)
        {
            return TokenUsageSnapshot.Empty;
        }

        var responseType = responseValue.GetType();
        var usageProperty = responseType.GetProperty("Usage");
        var usage = usageProperty?.GetValue(responseValue);

        if (usage is null)
        {
            return TokenUsageSnapshot.Empty;
        }

        var inputTokens = ReadLongProperty(usage, "InputTokenCount");
        var outputTokens = ReadLongProperty(usage, "OutputTokenCount");
        var totalTokens = ReadLongProperty(usage, "TotalTokenCount");

        long? cachedInputTokens = null;
        var inputDetails = usage.GetType().GetProperty("InputTokenDetails")?.GetValue(usage)
            ?? usage.GetType().GetProperty("InputTokensDetails")?.GetValue(usage);

        if (inputDetails is not null)
        {
            cachedInputTokens = ReadLongProperty(inputDetails, "CachedTokenCount", "CachedTokens", "Cached");
        }

        return new TokenUsageSnapshot(inputTokens, cachedInputTokens, outputTokens, totalTokens);
    }

    private static string? TryGetResponseId(object? responseValue)
    {
        if (responseValue is null)
        {
            return null;
        }

        var responseType = responseValue.GetType();
        var idProperty = responseType.GetProperty("Id") ?? responseType.GetProperty("ResponseId");
        var idValue = idProperty?.GetValue(responseValue)?.ToString();

        return string.IsNullOrWhiteSpace(idValue) ? null : idValue;
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

    private static string ExtractAssistantOutputText(object responseValue)
    {
        var assistantText = TryExtractAssistantMessageText(responseValue);
        if (!string.IsNullOrWhiteSpace(assistantText))
        {
            return assistantText;
        }

        // Fallback for SDKs that expose only aggregate helper methods.
        var getOutputTextMethod = responseValue.GetType().GetMethod("GetOutputText", Type.EmptyTypes);
        if (getOutputTextMethod is not null)
        {
            var outputText = getOutputTextMethod.Invoke(responseValue, null)?.ToString();
            if (!string.IsNullOrWhiteSpace(outputText))
            {
                return outputText;
            }
        }

        return string.Empty;
    }

    private static string? TryExtractAssistantMessageText(object responseValue)
    {
        var outputItemsProperty = responseValue.GetType().GetProperty("OutputItems")
            ?? responseValue.GetType().GetProperty("Output");

        if (outputItemsProperty?.GetValue(responseValue) is not IEnumerable outputItems)
        {
            return null;
        }

        var assistantMessages = new List<string>();

        foreach (var item in outputItems)
        {
            if (item is null)
            {
                continue;
            }

            var role = item.GetType().GetProperty("Role")?.GetValue(item)?.ToString();
            if (!string.Equals(role, "assistant", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            var contentProperty = item.GetType().GetProperty("Content");
            if (contentProperty?.GetValue(item) is not IEnumerable messageContents)
            {
                continue;
            }

            var textParts = new List<string>();

            foreach (var contentPart in messageContents)
            {
                if (contentPart is null)
                {
                    continue;
                }

                var directText = contentPart.GetType().GetProperty("Text")?.GetValue(contentPart)?.ToString();
                if (!string.IsNullOrWhiteSpace(directText))
                {
                    textParts.Add(directText);
                    continue;
                }

                // Some SDK variants nest text under a Value property.
                var textObject = contentPart.GetType().GetProperty("Text")?.GetValue(contentPart);
                var nestedText = textObject?.GetType().GetProperty("Value")?.GetValue(textObject)?.ToString();
                if (!string.IsNullOrWhiteSpace(nestedText))
                {
                    textParts.Add(nestedText);
                }
            }

            if (textParts.Count > 0)
            {
                assistantMessages.Add(string.Join("\n", textParts));
            }
        }

        if (assistantMessages.Count == 0)
        {
            return null;
        }

        // Prefer the last assistant message in case of intermediate tool loops.
        return assistantMessages[^1];
    }

    private static bool LooksLikeHtmlDocument(string content)
    {
        if (string.IsNullOrWhiteSpace(content))
        {
            return false;
        }

        var trimmed = content.TrimStart();
        return trimmed.StartsWith("<!DOCTYPE html", StringComparison.OrdinalIgnoreCase)
            || trimmed.StartsWith("<html", StringComparison.OrdinalIgnoreCase)
            || trimmed.Contains("<script", StringComparison.OrdinalIgnoreCase)
            || trimmed.Contains("</body>", StringComparison.OrdinalIgnoreCase);
    }

    private static bool TryBuildModelCompatibilityError(string? rawMessage, out string message)
    {
        message = string.Empty;

        if (string.IsNullOrWhiteSpace(rawMessage))
        {
            return false;
        }

        var hasUnsupportedParameter = rawMessage.Contains("Unsupported parameter", StringComparison.OrdinalIgnoreCase);
        var hasTemperature = rawMessage.Contains("temperature", StringComparison.OrdinalIgnoreCase);

        if (!hasUnsupportedParameter || !hasTemperature)
        {
            return false;
        }

        message = "The configured Foundry model does not support the 'temperature' parameter. Update the agent/model settings to remove temperature or use a model that supports it.";
        return true;
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

    private readonly record struct TokenUsageSnapshot(
        long? InputTokens,
        long? CachedInputTokens,
        long? OutputTokens,
        long? TotalTokens)
    {
        public static TokenUsageSnapshot Empty => new(null, null, null, null);

        public bool HasAnyValue =>
            InputTokens.HasValue || CachedInputTokens.HasValue || OutputTokens.HasValue || TotalTokens.HasValue;
    }
}

#pragma warning restore OPENAI001
#pragma warning restore SCME0001