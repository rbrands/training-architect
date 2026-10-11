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
            bool? includeDataCheck,
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
                response = response with
                {
                    Level = athleteLevel,
                    LevelLabel = athleteLevelLabel
                };

                if (includeDataCheck == true)
                {
                    response = await AddDataCheckAsync(
                        response, athleteDataService, athleteIdHeader, apiKeyHeader, logger, ct);
                }

                return Results.Ok(response);
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

        myDatasetGroup.MapGet("/check", CheckCurrentDatasetAsync)
            .WithName("CheckCurrentDataset")
            .WithTags("Dataset")
            .WithSummary("Fetches the current curated dataset and runs the deterministic data quality check for the athlete with Athlete ID provided in the X-Intervals-Athlete-Id header.")
            .WithDescription("Returns the data quality report (checks per area, overall rating, suggested fix, and report text). No training recommendations are made. Requires X-Intervals-Athlete-Id and X-Intervals-Api-Key headers.");

        myDatasetGroup.MapPost("/check", CheckPostedDatasetAsync)
            .WithName("CheckPostedDataset")
            .WithTags("Dataset")
            .WithSummary("Runs the deterministic data quality check on the posted curated dataset.")
            .WithDescription("Accepts the raw dataset JSON as returned by GET /api/dataset as the request body (no wrapper object). Returns the data quality report. Requires X-Intervals-Athlete-Id and X-Intervals-Api-Key headers.");

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

            if (httpContext.Request.ContentLength is > 128_000)
            {
                return Results.Problem(
                    title: "Payload too large.",
                    detail: "Plan JSON payload exceeds the 128 KB limit.",
                    statusCode: StatusCodes.Status413PayloadTooLarge);
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

            if (httpContext.Request.ContentLength is > 128_000)
            {
                return Results.Problem(
                    title: "Payload too large.",
                    detail: "Plan JSON payload exceeds the 128 KB limit.",
                    statusCode: StatusCodes.Status413PayloadTooLarge);
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
    private const int DatasetCheckMaxPayloadBytes = 512_000;

    private static async Task<IResult> CheckCurrentDatasetAsync(
        HttpContext httpContext,
        IAthleteDataService athleteDataService,
        IAthleteRepository athleteRepository,
        ILoggerFactory loggerFactory,
        CancellationToken ct)
    {
        var logger = loggerFactory.CreateLogger("MyDatasetEndpoints");

        var rejection = await ValidateDatasetCheckRequestAsync(httpContext.Request, athleteRepository, logger);
        if (rejection is not null)
        {
            return rejection;
        }

        TryGetIntervalsCredentials(httpContext.Request, out var athleteId, out var apiKey);

        try
        {
            var dataset = await athleteDataService.GetAsync(athleteId, apiKey, ct);
            var result = await athleteDataService.CheckAthleteDataAsync(
                athleteId, apiKey, dataset.DataParsed.GetRawText(), ct);

            return Results.Ok(result);
        }
        catch (Exception ex)
        {
            return CreateDatasetCheckFailureResult(ex, ct, logger);
        }
    }

    private static async Task<IResult> CheckPostedDatasetAsync(
        HttpContext httpContext,
        IAthleteDataService athleteDataService,
        IAthleteRepository athleteRepository,
        ILoggerFactory loggerFactory,
        CancellationToken ct)
    {
        var logger = loggerFactory.CreateLogger("MyDatasetEndpoints");

        var rejection = await ValidateDatasetCheckRequestAsync(httpContext.Request, athleteRepository, logger);
        if (rejection is not null)
        {
            return rejection;
        }

        if (httpContext.Request.ContentLength is > DatasetCheckMaxPayloadBytes)
        {
            return Results.Problem(
                title: "Payload too large.",
                detail: "Dataset JSON payload exceeds the 512 KB limit.",
                statusCode: StatusCodes.Status413PayloadTooLarge);
        }

        var datasetJson = await ReadPlanJsonAsync(httpContext.Request, ct);
        if (!IsJsonObject(datasetJson))
        {
            return AthleteDataEndpointResults.CreateBadRequestResult(
                "Request body must contain the dataset JSON object to check.");
        }

        TryGetIntervalsCredentials(httpContext.Request, out var athleteId, out var apiKey);

        try
        {
            var result = await athleteDataService.CheckAthleteDataAsync(athleteId, apiKey, datasetJson, ct);
            return Results.Ok(result);
        }
        catch (Exception ex)
        {
            return CreateDatasetCheckFailureResult(ex, ct, logger);
        }
    }

    private static async Task<IResult?> ValidateDatasetCheckRequestAsync(
        HttpRequest request,
        IAthleteRepository athleteRepository,
        ILogger logger)
    {
        if (!TryGetIntervalsCredentials(request, out var athleteId, out _))
        {
            logger.LogWarning("Rejected /api/dataset/check request due to missing required credential headers.");
            return AthleteDataEndpointResults.CreateBadRequestResult(
                "Missing intervals.icu credentials. Provide X-Intervals-Athlete-Id and X-Intervals-Api-Key headers.");
        }

        var athleteConfig = await athleteRepository.GetByAthleteIdAsync(athleteId);
        if (AthleteDataEndpointResults.TryCreateLockedAthleteResult(athleteConfig, out var lockedResult, out var lockMessage))
        {
            logger.LogWarning(
                "Rejected /api/dataset/check request for athlete {AthleteId} because the athlete config is locked. Message: {Message}",
                athleteId,
                lockMessage);

            return lockedResult;
        }

        return null;
    }

    private static bool IsJsonObject(string json)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            return false;
        }

        try
        {
            using var document = JsonDocument.Parse(json);
            return document.RootElement.ValueKind == JsonValueKind.Object;
        }
        catch (JsonException)
        {
            return false;
        }
    }

    private static IResult CreateDatasetCheckFailureResult(Exception exception, CancellationToken ct, ILogger logger)
    {
        switch (exception)
        {
            case McpToolExecutionException:
                logger.LogWarning(exception, "MCP tool execution failed in /api/dataset/check.");
                return Results.Problem(
                    title: "Data quality check failed.",
                    detail: "The MCP server could not check the dataset. Make sure the body is a curated dataset as returned by GET /api/dataset.",
                    statusCode: StatusCodes.Status502BadGateway);
            case HttpRequestException:
                logger.LogWarning(exception, "MCP server connection failed in /api/dataset/check.");
                return Results.Problem(
                    title: "MCP server unreachable.",
                    statusCode: StatusCodes.Status502BadGateway);
            case TimeoutException:
                logger.LogWarning(exception, "MCP server timeout in /api/dataset/check.");
                return Results.Problem(
                    title: "MCP server timeout.",
                    statusCode: StatusCodes.Status504GatewayTimeout);
            case OperationCanceledException when ct.IsCancellationRequested:
                return Results.Problem(
                    title: "Request was canceled.",
                    statusCode: StatusCodes.Status408RequestTimeout);
            case InvalidOperationException:
                logger.LogError(exception, "Invalid MCP configuration for /api/dataset/check.");
                return Results.Problem(
                    title: "MCP configuration error.",
                    statusCode: StatusCodes.Status500InternalServerError);
            default:
                logger.LogError(exception, "Unhandled error in /api/dataset/check.");
                return Results.Problem(
                    title: "Failed to check dataset.",
                    statusCode: StatusCodes.Status500InternalServerError);
        }
    }

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

    // The data check is best-effort: a failure must not prevent the athlete data from being returned.
    private static async Task<AthleteDataResponse> AddDataCheckAsync(
        AthleteDataResponse response,
        IAthleteDataService athleteDataService,
        string athleteId,
        string apiKey,
        ILogger logger,
        CancellationToken ct)
    {
        if (response.DataParsed.ValueKind != JsonValueKind.Object)
        {
            return response with { DataCheckError = "Data quality check is not available for this dataset." };
        }

        try
        {
            var dataCheck = await athleteDataService.CheckAthleteDataAsync(
                athleteId, apiKey, response.DataParsed.GetRawText(), ct);

            return response with { DataCheck = dataCheck };
        }
        catch (Exception ex) when (!ct.IsCancellationRequested)
        {
            logger.LogWarning(ex, "Data quality check failed in /api/athlete-data.");
            return response with { DataCheckError = "Data quality check is currently unavailable." };
        }
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

    /// <summary>Gets the data quality report when requested via <c>includeDataCheck=true</c>.</summary>
    public AthleteDataCheckResult? DataCheck { get; init; }

    /// <summary>Gets the reason why the requested data quality check is missing.</summary>
    public string? DataCheckError { get; init; }
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