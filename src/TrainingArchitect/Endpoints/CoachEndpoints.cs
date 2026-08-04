using System.Globalization;
using TrainingArchitect.Services;
using TrainingArchitect.Core.Constants;
using TrainingArchitect.Core.Interfaces;
using TrainingArchitect.Core.Models;

namespace TrainingArchitect.Endpoints;

/// <summary>
/// Minimal API endpoints for coaching functionality. These endpoints send prompts to the coaching agent and 
/// return the generated responses.
/// </summary>
public static class CoachEndpoints
{
    public static void MapCoachEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/coach")
            .RequireRateLimiting("coach")
            .RequireCors("CoachApi");

        group.AddEndpointFilter(async (context, next) =>
        {
            var request = context.HttpContext.Request;

            if (!request.IsHttps)
            {
                return Results.Problem(
                    title: "HTTPS required.",
                    detail: "Coach API only accepts HTTPS requests.",
                    statusCode: StatusCodes.Status400BadRequest);
            }

            if (!request.HasJsonContentType())
            {
                return Results.Problem(
                    title: "Unsupported media type.",
                    detail: "Coach API only accepts application/json payloads.",
                    statusCode: StatusCodes.Status415UnsupportedMediaType);
            }

            // Keep payload size bounded to reduce abuse potential.
            if (request.ContentLength is > 128_000)
            {
                return Results.Problem(
                    title: "Payload too large.",
                    detail: "Coach API payload exceeds the 128 KB limit.",
                    statusCode: StatusCodes.Status413PayloadTooLarge);
            }

            if (!IsTrustedBrowserRequest(request))
            {
                return Results.Problem(
                    title: "Forbidden.",
                    detail: "Cross-site requests are not allowed for this endpoint.",
                    statusCode: StatusCodes.Status403Forbidden);
            }

            return await next(context);
        });

        group.MapPost("/assess", AssessAsync);
        group.MapPost("/plan", PlanAsync);
        group.MapPost("/plan/upload", UploadPlanAsync);
    }

    private static bool IsTrustedBrowserRequest(HttpRequest request)
    {
        var secFetchSite = request.Headers["Sec-Fetch-Site"].ToString();
        if (string.Equals(secFetchSite, "cross-site", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        var originHeader = request.Headers.Origin.ToString();
        if (!string.IsNullOrWhiteSpace(originHeader))
        {
            return IsSameOrigin(originHeader, request);
        }

        var refererHeader = request.Headers.Referer.ToString();
        if (!string.IsNullOrWhiteSpace(refererHeader))
        {
            return IsSameOrigin(refererHeader, request);
        }

        // Block requests without browser context headers.
        return false;
    }

    private static bool IsSameOrigin(string originOrReferer, HttpRequest request)
    {
        if (!Uri.TryCreate(originOrReferer, UriKind.Absolute, out var sourceUri))
        {
            return false;
        }

        var targetHost = request.Host.Host;
        var targetScheme = request.Scheme;
        var targetPort = request.Host.Port ?? (string.Equals(targetScheme, "https", StringComparison.OrdinalIgnoreCase) ? 443 : 80);

        var sourcePort = sourceUri.IsDefaultPort
            ? (string.Equals(sourceUri.Scheme, "https", StringComparison.OrdinalIgnoreCase) ? 443 : 80)
            : sourceUri.Port;

        return string.Equals(sourceUri.Scheme, targetScheme, StringComparison.OrdinalIgnoreCase)
            && string.Equals(sourceUri.Host, targetHost, StringComparison.OrdinalIgnoreCase)
            && sourcePort == targetPort;
    }

    private static async Task<IResult> AssessAsync(
        HttpContext       httpContext,
        AssessRequest      request,
        ICoachingAgent     agent,
        IAthleteRepository athleteRepository,
        ILevelRepository levelRepository,
        IUsageCounterRepository usageCounterRepository,
        ILoggerFactory loggerFactory,
        CancellationToken  ct)
    {
        var logger = loggerFactory.CreateLogger(nameof(CoachEndpoints));
        var athleteIdHeader = httpContext.Request.Headers[IntervalsHeaders.AthleteId].ToString();
        var apiKeyHeader = httpContext.Request.Headers[IntervalsHeaders.ApiKey].ToString();

        if (string.IsNullOrWhiteSpace(athleteIdHeader) || string.IsNullOrWhiteSpace(apiKeyHeader))
        {
            return Results.BadRequest(new
            {
                error = "Missing required headers: X-Intervals-Athlete-Id and X-Intervals-Api-Key."
            });
        }

        var athleteConfig = await EnsureAthleteConfigAsync(athleteRepository, levelRepository, athleteIdHeader, logger);

        var limitError = await CheckTokenLimitAsync(
            usageCounterRepository,
            levelRepository,
            athleteConfig,
            athleteIdHeader,
            logger);

        if (limitError is not null)
        {
            logger.LogWarning(
                "Rejected /api/coach/assess for athlete {AthleteId} due to token limit. Level: {Level}. WeeklyLimit: {WeeklyLimit}. MonthlyLimit: {MonthlyLimit}. Reason: {Reason}",
                athleteIdHeader,
                athleteConfig.Level,
                athleteConfig.Limits.WeeklyToken,
                athleteConfig.Limits.MonthlyToken,
                limitError);

            return Results.Json(
                new { error = limitError },
                statusCode: StatusCodes.Status429TooManyRequests);
        }

        var prompt = PromptBuilder.BuildAssessPrompt(request);
        var result = await agent.PromptAsync(
            prompt,
            request.DisciplineType,
            request.Language,
            ct,
            intervalsAthleteId: athleteIdHeader,
            intervalsApiKey: apiKeyHeader);

        var inputTokens = ToInt32NonNegative(result.InputTokens);
        var outputTokens = ToInt32NonNegative(result.OutputTokens);

        await RecordUsageBestEffortAsync(
            usageCounterRepository,
            logger,
            athleteIdHeader,
            MapAssessAction(request.AssessmentType),
            inputTokens,
            ToInt32NonNegative(result.CachedInputTokens),
            outputTokens);

        return Results.Ok(new AssessResponse(result.Content, result.TotalTokens, result.ResponseId));
    }

    private static async Task<IResult> PlanAsync(
        HttpContext       httpContext,
        PlanRequest        request,
        ICoachingAgent     agent,
        IAthleteRepository athleteRepository,
        ILevelRepository levelRepository,
        IUsageCounterRepository usageCounterRepository,
        ILoggerFactory loggerFactory,
        CancellationToken  ct)
    {
        var logger = loggerFactory.CreateLogger(nameof(CoachEndpoints));
        var athleteIdHeader = httpContext.Request.Headers[IntervalsHeaders.AthleteId].ToString();
        var apiKeyHeader = httpContext.Request.Headers[IntervalsHeaders.ApiKey].ToString();

        if (string.IsNullOrWhiteSpace(athleteIdHeader) || string.IsNullOrWhiteSpace(apiKeyHeader))
        {
            return Results.BadRequest(new
            {
                error = "Missing required headers: X-Intervals-Athlete-Id and X-Intervals-Api-Key."
            });
        }

        var athleteConfig = await EnsureAthleteConfigAsync(athleteRepository, levelRepository, athleteIdHeader, logger);

        var limitError = await CheckTokenLimitAsync(
            usageCounterRepository,
            levelRepository,
            athleteConfig,
            athleteIdHeader,
            logger);

        if (limitError is not null)
        {
            logger.LogWarning(
                "Rejected /api/coach/plan for athlete {AthleteId} due to token limit. Level: {Level}. WeeklyLimit: {WeeklyLimit}. MonthlyLimit: {MonthlyLimit}. Reason: {Reason}",
                athleteIdHeader,
                athleteConfig.Level,
                athleteConfig.Limits.WeeklyToken,
                athleteConfig.Limits.MonthlyToken,
                limitError);

            return Results.Json(
                new { error = limitError },
                statusCode: StatusCodes.Status429TooManyRequests);
        }

        var prompt = PromptBuilder.BuildPlanPrompt(request);
        var result = await agent.PromptAsync(
            prompt,
            request.DisciplineType,
            request.Language,
            ct,
            intervalsAthleteId: athleteIdHeader,
            intervalsApiKey: apiKeyHeader);

        var inputTokens = ToInt32NonNegative(result.InputTokens);
        var outputTokens = ToInt32NonNegative(result.OutputTokens);

        await RecordUsageBestEffortAsync(
            usageCounterRepository,
            logger,
            athleteIdHeader,
            "plan_create",
            inputTokens,
            ToInt32NonNegative(result.CachedInputTokens),
            outputTokens);

        return Results.Ok(new PlanResponse(result.Content, result.TotalTokens, result.ResponseId));
    }

    private static async Task<IResult> UploadPlanAsync(
        HttpContext httpContext,
        PlanUploadRequest request,
        IAthleteDataService athleteDataService,
        CancellationToken ct)
    {
        try
        {
            var athleteIdHeader = httpContext.Request.Headers[IntervalsHeaders.AthleteId].ToString();
            var apiKeyHeader = httpContext.Request.Headers[IntervalsHeaders.ApiKey].ToString();

            if (string.IsNullOrWhiteSpace(athleteIdHeader) || string.IsNullOrWhiteSpace(apiKeyHeader))
            {
                return Results.BadRequest(new
                {
                    error = "Missing required headers: X-Intervals-Athlete-Id and X-Intervals-Api-Key."
                });
            }

            if (string.IsNullOrWhiteSpace(request.WeekPlanJson))
            {
                return Results.BadRequest(new
                {
                    error = "Missing required payload field: weekPlanJson."
                });
            }

            await athleteDataService.UploadWeekPlanAsync(athleteIdHeader, apiKeyHeader, request.WeekPlanJson, ct);
            return Results.Ok(new { uploaded = true });
        }
        catch (McpToolExecutionException ex)
        {
            return Results.Problem(
                title: "MCP tool execution failed.",
                detail: ex.Message,
                statusCode: StatusCodes.Status502BadGateway);
        }
        catch (HttpRequestException ex)
        {
            return Results.Problem(
                title: "MCP server unreachable.",
                detail: ex.Message,
                statusCode: StatusCodes.Status502BadGateway);
        }
        catch (TimeoutException ex)
        {
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
            return Results.Problem(
                title: "MCP configuration error.",
                detail: ex.Message,
                statusCode: StatusCodes.Status500InternalServerError);
        }
    }

    private sealed record PlanUploadRequest(string WeekPlanJson);

    private static async Task<AthleteConfig> EnsureAthleteConfigAsync(
        IAthleteRepository athleteRepository,
        ILevelRepository levelRepository,
        string athleteId,
        ILogger logger)
    {
        var existingConfig = await athleteRepository.GetByAthleteIdAsync(athleteId);
        if (existingConfig is not null)
        {
            existingConfig.Limits ??= new AthleteLimits();
            return existingConfig;
        }

        var basicLevelConfig = await levelRepository.GetByLevelAsync("basic");
        var createdConfig = new AthleteConfig
        {
            AthleteId = athleteId,
            Level = basicLevelConfig?.Level ?? "basic",
            Limits = new AthleteLimits
            {
                WeeklyToken = basicLevelConfig?.Limits?.WeeklyToken ?? 0,
                MonthlyToken = basicLevelConfig?.Limits?.MonthlyToken ?? 0
            }
        };

        try
        {
            return await athleteRepository.UpsertAsync(createdConfig);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to create athlete config for {AthleteId}.", athleteId);
            return createdConfig;
        }
    }

    private static async Task<string?> CheckTokenLimitAsync(
        IUsageCounterRepository usageCounterRepository,
        ILevelRepository levelRepository,
        AthleteConfig athleteConfig,
        string athleteId,
        ILogger logger)
    {
        const string GlobalLevelId = "level_global";
        const string GlobalAthleteId = "__GLOBAL__";

        try
        {
            var usageCounters = await usageCounterRepository.GetByAthleteIdAsync(athleteId);
            var now = DateTime.UtcNow;
            var monthlyPeriodKey = $"{now:yyyy-MM}";
            var weeklyPeriodKey = $"{ISOWeek.GetYear(now)}-W{ISOWeek.GetWeekOfYear(now):00}";

            var monthlyCounter = usageCounters.FirstOrDefault(counter =>
                counter.UsageType == UsageCounter.MonthlyUsageType &&
                string.Equals(counter.PeriodKey, monthlyPeriodKey, StringComparison.Ordinal));
            var weeklyCounter = usageCounters.FirstOrDefault(counter =>
                counter.UsageType == UsageCounter.WeeklyUsageType &&
                string.Equals(counter.PeriodKey, weeklyPeriodKey, StringComparison.Ordinal));

            var currentMonthlyTokens = (monthlyCounter?.TotalInputTokens ?? 0) + (monthlyCounter?.TotalOutputTokens ?? 0);
            var currentWeeklyTokens = (weeklyCounter?.TotalInputTokens ?? 0) + (weeklyCounter?.TotalOutputTokens ?? 0);

            if (athleteConfig.Limits.MonthlyToken > 0 && currentMonthlyTokens > athleteConfig.Limits.MonthlyToken)
            {
                return "Your monthly token limit exceeded.";
            }

            if (athleteConfig.Limits.WeeklyToken > 0 && currentWeeklyTokens > athleteConfig.Limits.WeeklyToken)
            {
                return "Your weekly token limit exceeded.";
            }

            var globalLevelConfig = await levelRepository.GetByIdAsync(GlobalLevelId);
            if (globalLevelConfig is null)
            {
                return null;
            }

            var globalUsageCounters = await usageCounterRepository.GetByAthleteIdAsync(GlobalAthleteId);
            var globalMonthlyCounter = globalUsageCounters.FirstOrDefault(counter =>
                counter.UsageType == UsageCounter.MonthlyUsageType &&
                string.Equals(counter.PeriodKey, monthlyPeriodKey, StringComparison.Ordinal));
            var globalWeeklyCounter = globalUsageCounters.FirstOrDefault(counter =>
                counter.UsageType == UsageCounter.WeeklyUsageType &&
                string.Equals(counter.PeriodKey, weeklyPeriodKey, StringComparison.Ordinal));

            var currentGlobalMonthlyTokens = (globalMonthlyCounter?.TotalInputTokens ?? 0) + (globalMonthlyCounter?.TotalOutputTokens ?? 0);
            var currentGlobalWeeklyTokens = (globalWeeklyCounter?.TotalInputTokens ?? 0) + (globalWeeklyCounter?.TotalOutputTokens ?? 0);

            if (globalLevelConfig.Limits.MonthlyToken > 0 && currentGlobalMonthlyTokens > globalLevelConfig.Limits.MonthlyToken)
            {
                return "Global monthly token limit exceeded.";
            }

            if (globalLevelConfig.Limits.WeeklyToken > 0 && currentGlobalWeeklyTokens > globalLevelConfig.Limits.WeeklyToken)
            {
                return "Global weekly token limit exceeded.";
            }
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Token limit evaluation failed for athlete {AthleteId}.", athleteId);
        }

        return null;
    }

    private static string MapAssessAction(AssessmentType assessmentType)
    {
        return assessmentType switch
        {
            AssessmentType.Metrics => "assess_metrics",
            AssessmentType.Activity => "assess_last_training",
            AssessmentType.Week => "assess_week",
            _ => "assess_week"
        };
    }

    private static int ToInt32NonNegative(long? value)
    {
        if (!value.HasValue || value.Value <= 0)
        {
            return 0;
        }

        return value.Value > int.MaxValue ? int.MaxValue : (int)value.Value;
    }

    private static async Task RecordUsageBestEffortAsync(
        IUsageCounterRepository usageCounterRepository,
        ILogger logger,
        string athleteId,
        string action,
        int inputTokens,
        int cachedTokens,
        int outputTokens)
    {
        try
        {
            await usageCounterRepository.RecordUsageAsync(
                athleteId,
                action,
                inputTokens,
                cachedTokens,
                outputTokens);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Usage recording failed for coach action {Action}.", action);
        }
    }
}