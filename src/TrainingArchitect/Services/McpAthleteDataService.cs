using System.Diagnostics;
using System.Text.Json;
using ModelContextProtocol;
using ModelContextProtocol.Client;
using ModelContextProtocol.Protocol;
using Microsoft.Extensions.Logging;
using TrainingArchitect.Core.Constants;
using TrainingArchitect.Endpoints;

namespace TrainingArchitect.Services;

/// <summary>
/// Resolves athlete data by calling an MCP tool over HTTP.
/// </summary>
public sealed class McpAthleteDataService(
    IConfiguration configuration,
    ILogger<McpAthleteDataService> logger) : IAthleteDataService
{
    public async Task<AthleteDataResponse> GetAsync(string athleteId, string apiKey, CancellationToken ct)
    {
        var stopwatch = Stopwatch.StartNew();

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

        logger.LogInformation(
            "Calling MCP tool {MethodName} at {Host} for athlete {AthleteIdSuffix}.",
            McpToolNames.PrepareWeekData,
            endpointUri.Host,
            GetAthleteIdSuffix(athleteId));

        await using var transport = new HttpClientTransport(new HttpClientTransportOptions
        {
            Endpoint = endpointUri,
            TransportMode = HttpTransportMode.AutoDetect,
            AdditionalHeaders = new Dictionary<string, string>
            {
                [IntervalsHeaders.AthleteId] = athleteId,
                [IntervalsHeaders.ApiKey] = apiKey
            }
        });

        await using var client = await McpClient.CreateAsync(transport, new McpClientOptions(), null, ct);

        var toolResult = await client.CallToolAsync(
            McpToolNames.PrepareWeekData,
            new Dictionary<string, object?>(),
            progress: null,
            options: new RequestOptions(),
            cancellationToken: ct);

        if (toolResult.IsError == true)
        {
            throw new McpToolExecutionException(
                $"MCP tool '{McpToolNames.PrepareWeekData}' returned an error: {ExtractTextContent(toolResult)}");
        }

        logger.LogInformation(
            "MCP tool {MethodName} finished in {ElapsedMs}ms for athlete {AthleteIdSuffix}.",
            McpToolNames.PrepareWeekData,
            stopwatch.ElapsedMilliseconds,
            GetAthleteIdSuffix(athleteId));

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

        var endpointUri = ResolveEndpointUri();
        var stopwatch = Stopwatch.StartNew();
        var toolArguments = BuildUploadArguments(weekPlanJson);

        logger.LogInformation(
            "Calling MCP tool {MethodName} at {Host} for athlete {AthleteIdSuffix}.",
            McpToolNames.UploadWeekPlan,
            endpointUri.Host,
            GetAthleteIdSuffix(athleteId));

        await using var transport = new HttpClientTransport(new HttpClientTransportOptions
        {
            Endpoint = endpointUri,
            TransportMode = HttpTransportMode.AutoDetect,
            AdditionalHeaders = new Dictionary<string, string>
            {
                [IntervalsHeaders.AthleteId] = athleteId,
                [IntervalsHeaders.ApiKey] = apiKey
            }
        });

        await using var client = await McpClient.CreateAsync(transport, new McpClientOptions(), null, ct);

        var toolResult = await client.CallToolAsync(
            McpToolNames.UploadWeekPlan,
            toolArguments,
            progress: null,
            options: new RequestOptions(),
            cancellationToken: ct);

        if (toolResult.IsError == true)
        {
            throw new McpToolExecutionException(
                $"MCP tool '{McpToolNames.UploadWeekPlan}' returned an error: {ExtractTextContent(toolResult)}");
        }

        logger.LogInformation(
            "MCP tool {MethodName} finished in {ElapsedMs}ms for athlete {AthleteIdSuffix}.",
            McpToolNames.UploadWeekPlan,
            stopwatch.ElapsedMilliseconds,
            GetAthleteIdSuffix(athleteId));
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
                if (existingPlanJson.ValueKind == JsonValueKind.String)
                {
                    return new Dictionary<string, object?>
                    {
                        ["plan_json"] = existingPlanJson.GetString()
                    };
                }

                return new Dictionary<string, object?>
                {
                    ["plan_json"] = existingPlanJson.GetRawText()
                };
            }

            return new Dictionary<string, object?>
            {
                ["plan_json"] = json.RootElement.GetRawText()
            };
        }
        catch (JsonException ex)
        {
            throw new McpToolExecutionException("Week plan JSON is invalid.", ex);
        }
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
