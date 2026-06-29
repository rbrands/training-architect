using TrainingArchitect.Services;
using TrainingArchitect.Core.Constants;
using TrainingArchitect.Core.Models;
using System.Text.Json;

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
        AssessRequest      request,
        ICoachingAgent     agent,
        CancellationToken  ct)
    {
        var prompt = PromptBuilder.BuildAssessPrompt(request);
        var result = await agent.PromptAsync(prompt, request.DisciplineType, request.Language, ct);
        return Results.Ok(new AssessResponse(result));
    }

    private static async Task<IResult> PlanAsync(
        PlanRequest        request,
        ICoachingAgent     agent,
        CancellationToken  ct)
    {
        var prompt = PromptBuilder.BuildPlanPrompt(request);
        var result = await agent.PromptAsync(prompt, request.DisciplineType, request.Language, ct);
        return Results.Ok(result); // Validation + Upload kommt später
    }
}