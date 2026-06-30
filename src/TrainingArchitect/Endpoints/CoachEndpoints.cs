using TrainingArchitect.Services;
using TrainingArchitect.Core.Constants;
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
            .RequireRateLimiting("coach");

        group.MapPost("/assess", AssessAsync);
        group.MapPost("/plan",   PlanAsync);
    }

    private static async Task<IResult> AssessAsync(
        HttpContext       httpContext,
        AssessRequest      request,
        ICoachingAgent     agent,
        CancellationToken  ct)
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

        var prompt = PromptBuilder.BuildAssessPrompt(request);
        var result = await agent.PromptAsync(
            prompt,
            request.DisciplineType,
            request.Language,
            ct,
            intervalsAthleteId: athleteIdHeader,
            intervalsApiKey: apiKeyHeader);

        return Results.Ok(new AssessResponse(result));
    }

    private static async Task<IResult> PlanAsync(
        HttpContext       httpContext,
        PlanRequest        request,
        ICoachingAgent     agent,
        CancellationToken  ct)
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

        var prompt = PromptBuilder.BuildPlanPrompt(request);
        var result = await agent.PromptAsync(
            prompt,
            request.DisciplineType,
            request.Language,
            ct,
            intervalsAthleteId: athleteIdHeader,
            intervalsApiKey: apiKeyHeader);

        return Results.Ok(result); // Validation + Upload kommt später
    }
}