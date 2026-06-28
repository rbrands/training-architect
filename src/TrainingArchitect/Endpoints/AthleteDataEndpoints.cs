using TrainingArchitect.Services;
using TrainingArchitect.Core.Constants;
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

                    return Results.BadRequest(new
                    {
                        error = "Missing intervals.icu credentials. Provide X-Intervals-Athlete-Id and X-Intervals-Api-Key headers."
                    });
                }

                var response = await athleteDataService.GetAsync(athleteIdHeader, apiKeyHeader, ct);
                return Results.Ok(response);
            }
            catch (McpToolExecutionException ex)
            {
                logger.LogWarning(ex, "MCP tool execution failed in /api/athlete-data.");

                return Results.Problem(
                    title: "MCP tool execution failed.",
                    detail: "The MCP server returned an error. Check server logs for details.",
                    statusCode: StatusCodes.Status502BadGateway);
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
    public WeekDataDto? DataDeserialized { get; init; }
}