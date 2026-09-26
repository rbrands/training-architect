using Azure;
using Azure.AI.Projects;
using Azure.AI.Extensions.OpenAI;
using Microsoft.Extensions.Logging;
using OpenAI.Responses;
using System.Collections;
using System.ClientModel;
using System.Diagnostics;
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
        string? intervalsApiKey = null,
        string? previousResponseId = null)
    {
        ct.ThrowIfCancellationRequested();
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

        if (!string.IsNullOrWhiteSpace(previousResponseId))
        {
            options.Patch.Set("$.previous_response_id"u8, BinaryData.FromObjectAsJson(previousResponseId));
        }

        try
        {
            var response = await responseClient.CreateResponseAsync(options, ct);
            var tokenUsage = ExtractTokenUsage(response.Value);
            LogTokenUsage(tokenUsage);
            var responseId = TryGetResponseId(response.Value);
            var content = ExtractAssistantOutputText(response.Value);

            if (LooksLikeHtmlDocument(content))
            {
                _logger.LogError(
                    "Foundry agent returned HTML-like content for {AgentName}@{AgentVersion}. ResponseId={ResponseId}",
                    _configuredAgent.Name,
                    _configuredAgent.Version ?? "latest",
                    responseId ?? "unknown");

                throw new HttpRequestException("The Foundry agent returned an invalid HTML response.");
            }

            return new CoachingAgentResponse(
                content,
                tokenUsage.TotalTokens,
                responseId,
                tokenUsage.InputTokens,
                tokenUsage.CachedInputTokens,
                tokenUsage.OutputTokens);
        }
        catch (Exception) when (ct.IsCancellationRequested)
        {
            throw new OperationCanceledException(ct);
        }
        catch (Exception ex)
        {
            var status = ex switch
            {
                ClientResultException clientError => clientError.Status,
                RequestFailedException azureError => azureError.Status,
                _ => 0
            };
            var responseBody = ex switch
            {
                ClientResultException clientError => clientError.GetRawResponse()?.Content?.ToString(),
                RequestFailedException azureError => azureError.GetRawResponse()?.Content?.ToString(),
                _ => null
            };
            var normalized = NormalizeErrorMessage(responseBody ?? ex.Message);
            var requestId = ExtractRequestId(ex, normalized);
            var diagnosticId = Activity.Current?.TraceId.ToString() ?? Guid.NewGuid().ToString("N");
            string?[] sensitiveValues = [prompt, intervalsAthleteId, intervalsApiKey];

            _logger.LogError(
                "Foundry agent call failed for agent {AgentName}@{AgentVersion}. DiagnosticId={DiagnosticId}; Status={Status}; RequestId={RequestId}; Code={Code}; Param={Param}; Type={Type}; ExceptionType={ExceptionType}; ExceptionDetails={ExceptionDetails}; ResponseBody={ResponseBody}",
                _configuredAgent.Name,
                _configuredAgent.Version ?? "latest",
                diagnosticId,
                status,
                SafeDiagnosticValue(requestId, sensitiveValues),
                SafeDiagnosticValue(normalized.Code ?? (ex as RequestFailedException)?.ErrorCode, sensitiveValues),
                SafeDiagnosticValue(normalized.Param, sensitiveValues),
                SafeDiagnosticValue(normalized.Type, sensitiveValues),
                ex.GetType().FullName,
                RedactDiagnosticText(ex.ToString(), sensitiveValues),
                RedactDiagnosticText(responseBody, sensitiveValues));

            var message = TryBuildModelCompatibilityError(normalized.Message ?? ex.Message, out var compatibilityMessage)
                ? compatibilityMessage
                : status >= 500
                    ? $"The coaching service returned an upstream error (HTTP {status}). Please try again later."
                    : status == 429
                        ? "The coaching service is currently rate limited. Please try again later."
                        : "The coaching service could not complete the request. Please contact support with the diagnostic ID.";
            throw new CoachingAgentException(message, diagnosticId);
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
            if (root.ValueKind != JsonValueKind.Object)
            {
                return (null, null, null, null, null);
            }
            if (root.TryGetProperty("error", out var error) && error.ValueKind == JsonValueKind.Object)
            {
                root = error;
            }

            var code = TryGetString(root, "code");
            var message = TryGetString(root, "message") ?? rawMessage;
            var param = TryGetString(root, "param");
            var type = TryGetString(root, "type");

            string? requestId = TryGetString(root, "request_id");
            if (root.TryGetProperty("additionalInfo", out var additionalInfo)
                && additionalInfo.ValueKind == JsonValueKind.Object)
            {
                requestId ??= TryGetString(additionalInfo, "request_id");
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

    private static string RedactDiagnosticText(string? value, string?[] sensitiveValues)
    {
        if (string.IsNullOrEmpty(value))
        {
            return "unavailable";
        }

        var redactions = sensitiveValues
            .Where(sensitive => !string.IsNullOrEmpty(sensitive))
            .SelectMany(sensitive => new[] { sensitive!, JsonSerializer.Serialize(sensitive)[1..^1] })
            .Distinct(StringComparer.Ordinal)
            .OrderByDescending(sensitive => sensitive.Length);

        foreach (var sensitive in redactions)
        {
            value = value.Replace(sensitive, "[REDACTED]", StringComparison.Ordinal);
        }

        return value;
    }

    private static string SafeDiagnosticValue(string? value, string?[] sensitiveValues)
    {
        return !string.IsNullOrWhiteSpace(value) && value.Length <= 128
            && value.All(character => char.IsAsciiLetterOrDigit(character) || character is '_' or '-' or '.')
            && !sensitiveValues.Any(sensitive => !string.IsNullOrWhiteSpace(sensitive) && value.Contains(sensitive, StringComparison.Ordinal))
                ? value
                : "unknown";
    }

    private static string? ExtractRequestId(Exception exception, (string? Code, string? Param, string? Type, string? Message, string? RequestId) normalized)
    {
        foreach (var headerName in new[] { "x-request-id", "x-ms-request-id", "apim-request-id" })
        {
            if (exception is ClientResultException clientException
                && clientException.GetRawResponse() is { } clientResponse
                && clientResponse.Headers.TryGetValue(headerName, out var clientRequestId)
                && !string.IsNullOrWhiteSpace(clientRequestId))
            {
                return clientRequestId;
            }
            if (exception is RequestFailedException azureException
                && azureException.GetRawResponse() is { } azureResponse
                && azureResponse.Headers.TryGetValue(headerName, out var azureRequestId)
                && !string.IsNullOrWhiteSpace(azureRequestId))
            {
                return azureRequestId;
            }
        }

        return normalized.RequestId;
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