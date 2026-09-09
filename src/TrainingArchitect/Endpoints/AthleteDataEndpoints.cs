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

        app.MapPost("/api/validate", async (
            HttpContext httpContext,
            IAthleteDataService athleteDataService,
            IAthleteRepository athleteRepository,
            ILoggerFactory loggerFactory,
            CancellationToken ct) =>
        {
            var logger = loggerFactory.CreateLogger("PlanApiEndpoints");

            if (!TryGetIntervalsCredentials(httpContext.Request, out var athleteIdHeader, out var apiKeyHeader))
            {
                logger.LogWarning("Rejected /api/validate request due to missing required credential headers.");
                return AthleteDataEndpointResults.CreateBadRequestResult(
                    "Missing intervals.icu credentials. Provide X-Intervals-Athlete-Id and X-Intervals-Api-Key headers.");
            }

            var existingAthleteConfig = await athleteRepository.GetByAthleteIdAsync(athleteIdHeader);
            if (AthleteDataEndpointResults.TryCreateLockedAthleteResult(existingAthleteConfig, out var lockedResult, out var lockMessage))
            {
                logger.LogWarning(
                    "Rejected /api/validate request for athlete {AthleteId} because the athlete config is locked. Message: {Message}",
                    athleteIdHeader,
                    lockMessage);

                return lockedResult;
            }

            var planJson = await ReadPlanJsonAsync(httpContext.Request, ct);
            if (string.IsNullOrWhiteSpace(planJson))
            {
                return AthleteDataEndpointResults.CreateBadRequestResult(
                    "Request body must contain the plan JSON to validate.");
            }

            try
            {
                var validation = await athleteDataService.ValidateWeekPlanAsync(
                    athleteIdHeader, apiKeyHeader, planJson, PlanApiMaxValidationErrors, ct);

                if (!string.IsNullOrWhiteSpace(validation.Error))
                {
                    return Results.Problem(
                        title: "Plan validation failed.",
                        detail: validation.Error,
                        statusCode: StatusCodes.Status502BadGateway);
                }

                return Results.Ok(validation);
            }
            catch (McpToolExecutionException ex)
            {
                logger.LogWarning(ex, "MCP tool execution failed in /api/validate.");
                return Results.Problem(
                    title: "MCP tool execution failed.",
                    detail: ex.Message,
                    statusCode: StatusCodes.Status502BadGateway);
            }
            catch (HttpRequestException ex)
            {
                logger.LogWarning(ex, "MCP server connection failed in /api/validate.");
                return Results.Problem(
                    title: "MCP server unreachable.",
                    detail: ex.Message,
                    statusCode: StatusCodes.Status502BadGateway);
            }
            catch (TimeoutException ex)
            {
                logger.LogWarning(ex, "MCP server timeout in /api/validate.");
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
                logger.LogError(ex, "Invalid MCP configuration for /api/validate.");
                return Results.Problem(
                    title: "MCP configuration error.",
                    detail: ex.Message,
                    statusCode: StatusCodes.Status500InternalServerError);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unhandled error in /api/validate.");
                return Results.Problem(
                    title: "Failed to validate plan.",
                    detail: ex.Message,
                    statusCode: StatusCodes.Status500InternalServerError);
            }
        })
        .WithName("ValidatePlan")
        .WithTags("Plan")
        .WithSummary("Validates the posted plan JSON against the upload schema for the athlete with Athlete ID provided in the X-Intervals-Athlete-Id header.")
        .WithDescription("Accepts the raw plan JSON as the request body (no wrapper object). Requires X-Intervals-Athlete-Id and X-Intervals-Api-Key headers.");

        app.MapPost("/api/upload", async (
            HttpContext httpContext,
            IAthleteDataService athleteDataService,
            IAthleteRepository athleteRepository,
            ILoggerFactory loggerFactory,
            CancellationToken ct) =>
        {
            var logger = loggerFactory.CreateLogger("PlanApiEndpoints");

            if (!TryGetIntervalsCredentials(httpContext.Request, out var athleteIdHeader, out var apiKeyHeader))
            {
                logger.LogWarning("Rejected /api/upload request due to missing required credential headers.");
                return AthleteDataEndpointResults.CreateBadRequestResult(
                    "Missing intervals.icu credentials. Provide X-Intervals-Athlete-Id and X-Intervals-Api-Key headers.");
            }

            var existingAthleteConfig = await athleteRepository.GetByAthleteIdAsync(athleteIdHeader);
            if (AthleteDataEndpointResults.TryCreateLockedAthleteResult(existingAthleteConfig, out var lockedResult, out var lockMessage))
            {
                logger.LogWarning(
                    "Rejected /api/upload request for athlete {AthleteId} because the athlete config is locked. Message: {Message}",
                    athleteIdHeader,
                    lockMessage);

                return lockedResult;
            }

            var planJson = await ReadPlanJsonAsync(httpContext.Request, ct);
            if (string.IsNullOrWhiteSpace(planJson))
            {
                return AthleteDataEndpointResults.CreateBadRequestResult(
                    "Request body must contain the plan JSON to upload.");
            }

            try
            {
                var validation = await athleteDataService.ValidateWeekPlanAsync(
                    athleteIdHeader, apiKeyHeader, planJson, PlanApiMaxValidationErrors, ct);

                if (!string.IsNullOrWhiteSpace(validation.Error))
                {
                    return Results.Problem(
                        title: "Plan validation failed.",
                        detail: validation.Error,
                        statusCode: StatusCodes.Status502BadGateway);
                }

                if (!validation.IsValid)
                {
                    logger.LogInformation(
                        "Rejected /api/upload request for athlete {AthleteId} because the plan failed validation.",
                        athleteIdHeader);

                    return Results.Json(
                        new { error = validation.Details ?? "The plan JSON did not pass validation." },
                        statusCode: StatusCodes.Status422UnprocessableEntity);
                }

                await athleteDataService.UploadWeekPlanAsync(athleteIdHeader, apiKeyHeader, planJson, ct);
                return Results.Ok(new { uploaded = true });
            }
            catch (McpToolExecutionException ex)
            {
                logger.LogWarning(ex, "MCP tool execution failed in /api/upload.");
                return Results.Problem(
                    title: "MCP tool execution failed.",
                    detail: ex.Message,
                    statusCode: StatusCodes.Status502BadGateway);
            }
            catch (HttpRequestException ex)
            {
                logger.LogWarning(ex, "MCP server connection failed in /api/upload.");
                return Results.Problem(
                    title: "MCP server unreachable.",
                    detail: ex.Message,
                    statusCode: StatusCodes.Status502BadGateway);
            }
            catch (TimeoutException ex)
            {
                logger.LogWarning(ex, "MCP server timeout in /api/upload.");
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
                logger.LogError(ex, "Invalid MCP configuration for /api/upload.");
                return Results.Problem(
                    title: "MCP configuration error.",
                    detail: ex.Message,
                    statusCode: StatusCodes.Status500InternalServerError);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unhandled error in /api/upload.");
                return Results.Problem(
                    title: "Failed to upload plan.",
                    detail: ex.Message,
                    statusCode: StatusCodes.Status500InternalServerError);
            }
        })
        .WithName("UploadPlan")
        .WithTags("Plan")
        .WithSummary("Validates and uploads the posted plan JSON for the athlete with Athlete ID provided in the X-Intervals-Athlete-Id header.")
        .WithDescription("Accepts the raw plan JSON as the request body (no wrapper object). Runs the same schema validation as /api/validate before uploading. Requires X-Intervals-Athlete-Id and X-Intervals-Api-Key headers.");

        return app;
    }

    private const int PlanApiMaxValidationErrors = 10;

    private static bool TryGetIntervalsCredentials(HttpRequest request, out string athleteId, out string apiKey)
    {
        athleteId = request.Headers[IntervalsHeaders.AthleteId].ToString();
        apiKey = request.Headers[IntervalsHeaders.ApiKey].ToString();
        return !string.IsNullOrWhiteSpace(athleteId) && !string.IsNullOrWhiteSpace(apiKey);
    }

    private static async Task<string> ReadPlanJsonAsync(HttpRequest request, CancellationToken ct)
    {
        using var reader = new StreamReader(request.Body);
        return await reader.ReadToEndAsync(ct);
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