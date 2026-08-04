using TrainingArchitect.Services;
using TrainingArchitect.Core.Constants;
using TrainingArchitect.Core.Interfaces;
using TrainingArchitect.Core.Models;
using System.Text.Json;

namespace TrainingArchitect.Endpoints;

/// <summary>
/// Minimal API endpoint for retrieving athlete data from intervals.icu.
/// Credentials are forwarded to the application service and not persisted server-side.
/// </summary>
public static class AthleteDataEndpoints
{
    public static IEndpointRouteBuilder MapAthleteDataEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/athlete-data");

        group.MapGet("/", async (
            HttpContext httpContext,
            IAthleteDataService athleteDataService,
            IAthleteRepository athleteRepository,
            ILevelRepository levelRepository,
            ILoggerFactory loggerFactory,
            CancellationToken ct) =>
        {
            var logger = loggerFactory.CreateLogger("AthleteDataEndpoints");

            try
            {
                var athleteIdHeader = httpContext.Request.Headers[IntervalsHeaders.AthleteId].ToString();
                var apiKeyHeader = httpContext.Request.Headers[IntervalsHeaders.ApiKey].ToString();

                if (string.IsNullOrWhiteSpace(athleteIdHeader) ||
                    string.IsNullOrWhiteSpace(apiKeyHeader))
                {
                    logger.LogWarning(
                        "Rejected /api/athlete-data request due to missing required credential headers.");

                    return AthleteDataEndpointResults.CreateBadRequestResult(
                        "Missing intervals.icu credentials. Provide X-Intervals-Athlete-Id and X-Intervals-Api-Key headers.");
                }

                var existingAthleteConfig = await athleteRepository.GetByAthleteIdAsync(athleteIdHeader);
                if (AthleteDataEndpointResults.TryCreateLockedAthleteResult(existingAthleteConfig, out var lockedResult, out var lockMessage))
                {
                    logger.LogWarning(
                        "Rejected /api/athlete-data request for athlete {AthleteId} because the athlete config is locked. Message: {Message}",
                        athleteIdHeader,
                        lockMessage);

                    return lockedResult;
                }

                var athleteLevel = existingAthleteConfig?.Level?.Trim() ?? string.Empty;
                var athleteLevelLabel = string.Empty;

                if (!string.IsNullOrWhiteSpace(athleteLevel))
                {
                    var levelConfig = await levelRepository.GetByLevelAsync(athleteLevel);
                    athleteLevelLabel = levelConfig?.Label?.Trim() ?? string.Empty;
                }

                var response = await athleteDataService.GetAsync(athleteIdHeader, apiKeyHeader, ct);
                return Results.Ok(response with
                {
                    Level = athleteLevel,
                    LevelLabel = athleteLevelLabel
                });
            }
            catch (McpToolExecutionException ex)
            {
                logger.LogWarning(ex, "MCP tool execution failed in /api/athlete-data.");

                return Results.Problem(
                    title: "MCP tool execution failed.",
                    detail: "The MCP server returned an error. Check server logs for details.",
                    statusCode: StatusCodes.Status502BadGateway);
            }

            catch (HttpRequestException ex)
            {
                logger.LogWarning(ex, "MCP server connection failed in /api/athlete-data.");

                return Results.Problem(
                    title: "MCP server unreachable.",
                    detail: ex.Message,
                    statusCode: StatusCodes.Status502BadGateway);
            }
            catch (TimeoutException ex)
            {
                logger.LogWarning(ex, "MCP server timeout in /api/athlete-data.");

                return Results.Problem(
                    title: "MCP server timeout.",
                    detail: ex.Message,
                    statusCode: StatusCodes.Status504GatewayTimeout);
            }
            catch (OperationCanceledException)
            {
                return Results.Problem(
                    title: "Request was canceled.",
                    statusCode: StatusCodes.Status408RequestTimeout);
            }
            catch (InvalidOperationException ex)
            {
                logger.LogError(ex, "Invalid MCP configuration for /api/athlete-data.");

                return Results.Problem(
                    title: "MCP configuration error.",
                    detail: ex.Message,
                    statusCode: StatusCodes.Status500InternalServerError);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unhandled error in /api/athlete-data.");

                return Results.Problem(
                    title: "Failed to retrieve athlete data.",
                    detail: ex.Message,
                    statusCode: StatusCodes.Status500InternalServerError);
            }
        });

        var myDatasetGroup = app.MapGroup("/api/dataset");

        myDatasetGroup.MapGet("", async (
            HttpContext httpContext,
            IAthleteDataService athleteDataService,
            IAthleteRepository athleteRepository,
            ILoggerFactory loggerFactory,
            CancellationToken ct) =>
        {
            var logger = loggerFactory.CreateLogger("MyDatasetEndpoints");

            try
            {
                var athleteIdHeader = httpContext.Request.Headers[IntervalsHeaders.AthleteId].ToString();
                var apiKeyHeader = httpContext.Request.Headers[IntervalsHeaders.ApiKey].ToString();

                if (string.IsNullOrWhiteSpace(athleteIdHeader) ||
                    string.IsNullOrWhiteSpace(apiKeyHeader))
                {
                    logger.LogWarning(
                        "Rejected /api/dataset request due to missing required credential headers.");

                    return AthleteDataEndpointResults.CreateBadRequestResult(
                        "Missing intervals.icu credentials. Provide X-Intervals-Athlete-Id and X-Intervals-Api-Key headers.");
                }

                var existingAthleteConfig = await athleteRepository.GetByAthleteIdAsync(athleteIdHeader);
                if (AthleteDataEndpointResults.TryCreateLockedAthleteResult(existingAthleteConfig, out var lockedResult, out var lockMessage))
                {
                    logger.LogWarning(
                        "Rejected /api/dataset request for athlete {AthleteId} because the athlete config is locked. Message: {Message}",
                        athleteIdHeader,
                        lockMessage);

                    return lockedResult;
                }

                var response = await athleteDataService.GetAsync(athleteIdHeader, apiKeyHeader, ct);
                return Results.Json(response.DataParsed);
            }
            catch (McpToolExecutionException ex)
            {
                logger.LogWarning(ex, "MCP tool execution failed in /api/dataset.");
                return Results.Problem(
                    title: "MCP tool execution failed.",
                    detail: "The MCP server returned an error. Check server logs for details.",
                    statusCode: StatusCodes.Status502BadGateway);
            }
            catch (HttpRequestException ex)
            {
                logger.LogWarning(ex, "MCP server connection failed in /api/dataset.");
                return Results.Problem(
                    title: "MCP server unreachable.",
                    detail: ex.Message,
                    statusCode: StatusCodes.Status502BadGateway);
            }
            catch (TimeoutException ex)
            {
                logger.LogWarning(ex, "MCP server timeout in /api/dataset.");
                return Results.Problem(
                    title: "MCP server timeout.",
                    detail: ex.Message,
                    statusCode: StatusCodes.Status504GatewayTimeout);
            }
            catch (OperationCanceledException)
            {
                return Results.Problem(
                    title: "Request was canceled.",
                    statusCode: StatusCodes.Status408RequestTimeout);
            }
            catch (InvalidOperationException ex)
            {
                logger.LogError(ex, "Invalid MCP configuration for /api/dataset.");
                return Results.Problem(
                    title: "MCP configuration error.",
                    detail: ex.Message,
                    statusCode: StatusCodes.Status500InternalServerError);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unhandled error in /api/dataset.");
                return Results.Problem(
                    title: "Failed to retrieve dataset.",
                    detail: ex.Message,
                    statusCode: StatusCodes.Status500InternalServerError);
            }
        })
        .WithName("GetDataset")
        .WithTags("Dataset")
        .WithSummary("Returns the \"curated\" dataset from intervals.icu for the athlete with Athlete ID provided in the X-Intervals-Athlete-Id header.")
        .WithDescription("Calls the intervals.icu athlete data service and returns the parsed JSON payload only. Requires X-Intervals-Athlete-Id and X-Intervals-Api-Key headers.");

        return app;
    }
}

/// <summary>Response payload returned for athlete data retrieval.</summary>
public record AthleteDataResponse
{
    public string AthleteId { get; init; } = string.Empty;
    public string MethodName { get; init; } = string.Empty;
    public string DataRaw { get; init; } = string.Empty;
    public JsonElement DataParsed { get; init; }
    public string Level { get; init; } = string.Empty;
    public string LevelLabel { get; init; } = string.Empty;
}

internal static class AthleteDataEndpointResults
{
    public static IResult CreateBadRequestResult(string message) =>
        Results.Json(new { error = message }, statusCode: StatusCodes.Status400BadRequest);

    public static bool TryCreateLockedAthleteResult(
        AthleteConfig? athleteConfig,
        out IResult result,
        out string lockMessage)
    {
        if (athleteConfig is null || !athleteConfig.Locked)
        {
            result = Results.Empty;
            lockMessage = string.Empty;
            return false;
        }

        lockMessage = string.IsNullOrWhiteSpace(athleteConfig.Message)
            ? "Your athlete account is locked. Please contact support to re-enable access."
            : athleteConfig.Message.Trim();

        result = Results.Json(
            new { error = lockMessage },
            statusCode: StatusCodes.Status423Locked);

        return true;
    }
}