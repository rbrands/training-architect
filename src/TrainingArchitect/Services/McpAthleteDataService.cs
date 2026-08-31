using System.Diagnostics;
using System.Net.Sockets;
using System.Text.Json;
using ModelContextProtocol;
using ModelContextProtocol.Client;
using ModelContextProtocol.Protocol;
using Microsoft.Extensions.Logging;
using TrainingArchitect.Core.Constants;
using TrainingArchitect.Core.Models;
using TrainingArchitect.Core.Services;
using TrainingArchitect.Endpoints;

namespace TrainingArchitect.Services;

/// <summary>
/// Resolves athlete data by calling an MCP tool over HTTP.
/// </summary>
public sealed class McpAthleteDataService(
    IConfiguration configuration,
    IHttpClientFactory httpClientFactory,
    ILogger<McpAthleteDataService> logger) : IAthleteDataService
{
    /// <summary>Name of the pooled <see cref="HttpClient"/> used for MCP transport requests.</summary>
    public const string HttpClientName = "mcp-athlete-data";

    private const int DefaultTimeoutSeconds = 300;
    private const int DefaultConnectionTimeoutSeconds = 30;
    private const int DefaultMaxRetryAttempts = 2;

    public async Task<AthleteDataResponse> GetAsync(string athleteId, string apiKey, CancellationToken ct)
    {
        var toolResult = await CallToolAsync(
            McpToolNames.PrepareWeekData,
            athleteId,
            apiKey,
            new Dictionary<string, object?>(),
            allowRetry: true,
            ct);

        var extractedData = ExtractDataJson(toolResult);
        var normalizedData = NormalizeResultPayload(extractedData);

        return new AthleteDataResponse
        {
            AthleteId = athleteId,
            MethodName = McpToolNames.PrepareWeekData,
            DataRaw = extractedData.GetRawText(),
            DataParsed = normalizedData
        };
    }

    public async Task UploadWeekPlanAsync(string athleteId, string apiKey, string weekPlanJson, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(weekPlanJson))
        {
            throw new ArgumentException("Week plan JSON must not be empty.", nameof(weekPlanJson));
        }

        var toolArguments = BuildUploadArguments(weekPlanJson);

        // Uploading is not idempotent, so a failed attempt must never be retried automatically.
        await CallToolAsync(
            McpToolNames.UploadWeekPlan,
            athleteId,
            apiKey,
            toolArguments,
            allowRetry: false,
            ct);
    }

    public async Task<PlanTssCheckResult> CheckPlanTssAsync(
        string athleteId,
        string apiKey,
        string planJson,
        double loadTarget,
        double? tolerancePct,
        CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(planJson))
        {
            throw new ArgumentException("Plan JSON must not be empty.", nameof(planJson));
        }

        var toolArguments = new Dictionary<string, object?>
        {
            ["plan_json"] = planJson,
            ["load_target"] = loadTarget
        };

        if (tolerancePct.HasValue)
        {
            toolArguments["tolerance_pct"] = tolerancePct.Value;
        }

        var toolResult = await CallToolAsync(
            McpToolNames.CheckPlanTss,
            athleteId,
            apiKey,
            toolArguments,
            allowRetry: true,
            ct);

        var payload = NormalizeResultPayload(ExtractDataJson(toolResult));

        return payload.Deserialize<PlanTssCheckResult>()
            ?? throw new McpToolExecutionException(
                $"MCP tool '{McpToolNames.CheckPlanTss}' returned an unexpected payload.");
    }

    private static Dictionary<string, object?> BuildUploadArguments(string weekPlanJson)
    {
        try
        {
            using var json = JsonDocument.Parse(weekPlanJson);
            if (json.RootElement.ValueKind != JsonValueKind.Object)
            {
                throw new McpToolExecutionException("Week plan JSON must be a JSON object.");
            }

            // The MCP tool contract requires a top-level argument named "plan_json".
            if (json.RootElement.TryGetProperty("plan_json", out var existingPlanJson))
            {
                var planJson = existingPlanJson.ValueKind == JsonValueKind.String
                    ? existingPlanJson.GetString()
                        ?? throw new McpToolExecutionException("Week plan JSON must not be null.")
                    : existingPlanJson.GetRawText();

                return new Dictionary<string, object?>
                {
                    ["plan_json"] = PlanUploadNormalizer.Normalize(planJson)
                };
            }

            return new Dictionary<string, object?>
            {
                ["plan_json"] = PlanUploadNormalizer.Normalize(json.RootElement.GetRawText())
            };
        }
        catch (JsonException ex)
        {
            throw new McpToolExecutionException("Week plan JSON is invalid.", ex);
        }
    }

    /// <summary>
    /// Executes a single MCP tool call with an explicit per-call timeout and bounded retries
    /// for transient transport failures.
    /// </summary>
    private async Task<CallToolResult> CallToolAsync(
        string toolName,
        string athleteId,
        string apiKey,
        IReadOnlyDictionary<string, object?> arguments,
        bool allowRetry,
        CancellationToken ct)
    {
        var endpointUri = ResolveEndpointUri();
        var callTimeout = TimeSpan.FromSeconds(GetPositiveInt("TimeoutSeconds", DefaultTimeoutSeconds));
        var connectionTimeout = TimeSpan.FromSeconds(
            GetPositiveInt("ConnectionTimeoutSeconds", DefaultConnectionTimeoutSeconds));
        var maxAttempts = allowRetry
            ? GetPositiveInt("MaxRetryAttempts", DefaultMaxRetryAttempts) + 1
            : 1;

        for (var attempt = 1; ; attempt++)
        {
            ct.ThrowIfCancellationRequested();

            var stopwatch = Stopwatch.StartNew();
            using var callCts = CancellationTokenSource.CreateLinkedTokenSource(ct);
            callCts.CancelAfter(callTimeout);

            logger.LogInformation(
                "Calling MCP tool {MethodName} at {Host} for athlete {AthleteIdSuffix} (attempt {Attempt}/{MaxAttempts}).",
                toolName,
                endpointUri.Host,
                GetAthleteIdSuffix(athleteId),
                attempt,
                maxAttempts);

            try
            {
                await using var transport = new HttpClientTransport(
                    new HttpClientTransportOptions
                    {
                        Endpoint = endpointUri,
                        TransportMode = HttpTransportMode.AutoDetect,
                        ConnectionTimeout = connectionTimeout,
                        AdditionalHeaders = new Dictionary<string, string>
                        {
                            [IntervalsHeaders.AthleteId] = athleteId,
                            [IntervalsHeaders.ApiKey] = apiKey
                        }
                    },
                    httpClientFactory.CreateClient(HttpClientName),
                    loggerFactory: null,
                    ownsHttpClient: true);

                await using var client = await McpClient.CreateAsync(
                    transport,
                    new McpClientOptions(),
                    null,
                    callCts.Token);

                var toolResult = await client.CallToolAsync(
                    toolName,
                    arguments,
                    progress: null,
                    options: new RequestOptions(),
                    cancellationToken: callCts.Token);

                if (toolResult.IsError == true)
                {
                    throw new McpToolExecutionException(
                        $"MCP tool '{toolName}' returned an error: {ExtractTextContent(toolResult)}");
                }

                logger.LogInformation(
                    "MCP tool {MethodName} finished in {ElapsedMs}ms for athlete {AthleteIdSuffix}.",
                    toolName,
                    stopwatch.ElapsedMilliseconds,
                    GetAthleteIdSuffix(athleteId));

                return toolResult;
            }
            catch (Exception ex) when (IsTransientTransportFailure(ex)
                                       && !ct.IsCancellationRequested
                                       && attempt < maxAttempts)
            {
                logger.LogWarning(
                    ex,
                    "MCP tool {MethodName} failed after {ElapsedMs}ms on attempt {Attempt}/{MaxAttempts}. Retrying.",
                    toolName,
                    stopwatch.ElapsedMilliseconds,
                    attempt,
                    maxAttempts);

                await Task.Delay(TimeSpan.FromMilliseconds(500 * attempt), ct);
            }
            catch (OperationCanceledException ex) when (!ct.IsCancellationRequested)
            {
                // The linked token fired, so this is our own timeout and not a caller-initiated abort.
                throw new TimeoutException(
                    $"MCP tool '{toolName}' did not complete within {callTimeout.TotalSeconds:0} seconds.",
                    ex);
            }
        }
    }

    private static bool IsTransientTransportFailure(Exception exception) => exception switch
    {
        HttpRequestException => true,
        SocketException => true,
        IOException => true,
        OperationCanceledException => true,
        _ => exception.InnerException is not null && IsTransientTransportFailure(exception.InnerException)
    };

    // Falls back to the default when the value is still an unreplaced __PLACEHOLDER__.
    private int GetPositiveInt(string key, int defaultValue)
    {
        var raw = configuration[$"Mcp:AthleteData:{key}"];
        return int.TryParse(raw, out var value) && value > 0 ? value : defaultValue;
    }

    private Uri ResolveEndpointUri()
    {
        var endpointUrl = configuration["Mcp:AthleteData:Endpoint"];
        if (string.IsNullOrWhiteSpace(endpointUrl))
        {
            throw new InvalidOperationException(
                "MCP endpoint is not configured. Set Mcp:AthleteData:Endpoint.");
        }

        if (!Uri.TryCreate(endpointUrl, UriKind.Absolute, out var endpointUri)
            || (endpointUri.Scheme != Uri.UriSchemeHttp && endpointUri.Scheme != Uri.UriSchemeHttps))
        {
            throw new InvalidOperationException(
                "MCP endpoint is invalid. Mcp:AthleteData:Endpoint must be an absolute HTTP/HTTPS URL.");
        }

        return endpointUri;
    }

    private static JsonElement ExtractDataJson(CallToolResult result)
    {
        if (result.StructuredContent is JsonElement structuredJson)
        {
            return structuredJson.Clone();
        }

        var text = ExtractTextContent(result);
        if (string.IsNullOrWhiteSpace(text))
        {
            return JsonSerializer.Deserialize<JsonElement>("{}");
        }

        try
        {
            using var document = JsonDocument.Parse(text);
            return document.RootElement.Clone();
        }
        catch (JsonException ex)
        {
            throw new McpToolExecutionException(
                "MCP tool returned non-JSON content. Expected JSON output for athlete data.",
                ex);
        }
    }

    private static string ExtractTextContent(CallToolResult result)
    {
        var textBlocks = result.Content
            .OfType<TextContentBlock>()
            .Select(block => block.Text)
            .Where(text => !string.IsNullOrWhiteSpace(text))
            .ToArray();

        return textBlocks.Length == 0 ? string.Empty : string.Join("\n", textBlocks);
    }

    private static JsonElement NormalizeResultPayload(JsonElement payload)
    {
        if (payload.ValueKind != JsonValueKind.Object)
        {
            return payload;
        }

        if (!payload.TryGetProperty("result", out var resultProperty))
        {
            return payload;
        }

        if (resultProperty.ValueKind != JsonValueKind.String)
        {
            return payload;
        }

        var resultText = resultProperty.GetString();
        if (string.IsNullOrWhiteSpace(resultText))
        {
            return payload;
        }

        try
        {
            using var parsedResult = JsonDocument.Parse(resultText);
            return parsedResult.RootElement.Clone();
        }
        catch (JsonException)
        {
            // Keep original payload when result is plain text instead of JSON.
            return payload;
        }
    }

    private static string GetAthleteIdSuffix(string athleteId)
    {
        if (string.IsNullOrWhiteSpace(athleteId))
        {
            return "unknown";
        }

        return athleteId.Length <= 4 ? athleteId : athleteId[^4..];
    }
}
