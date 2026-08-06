using TrainingArchitect.Core.Interfaces;
using TrainingArchitect.Core.Models;

namespace TrainingArchitect.Endpoints;

/// <summary>
/// Minimal API endpoints for admin features consumed by the WASM admin client.
/// </summary>
public static class AdminEndpoints
{
    public static IEndpointRouteBuilder MapAdminEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/admin")
            .RequireAuthorization(p => p.RequireRole("SiteAdmin"));

        group.MapGet("/usage", async (IUsageCounterRepository repo) =>
            Results.Ok(await repo.GetAllAsync()));

        group.MapGet("/usage/{athleteId}", async (string athleteId, IUsageCounterRepository repo) =>
            Results.Ok(await repo.GetByAthleteIdAsync(athleteId)));

        group.MapPost("/usage/refresh", async (IUsageCounterRepository repo, CancellationToken ct) =>
        {
            await repo.RefreshGlobalCountersAsync(ct);
            return Results.Ok(new { refreshed = true });
        });

        group.MapGet("/athletes", async (IAthleteRepository repo) =>
            Results.Ok(await repo.GetAllAsync()));

        group.MapGet("/athletes/{athleteId}", async (string athleteId, IAthleteRepository repo) =>
        {
            var athleteConfig = await repo.GetByAthleteIdAsync(athleteId);
            return athleteConfig is null ? Results.NotFound() : Results.Ok(athleteConfig);
        });

        group.MapPut("/athletes", async (AthleteConfig athleteConfig, IAthleteRepository repo) =>
        {
            if (string.IsNullOrWhiteSpace(athleteConfig.AthleteId))
            {
                return Results.BadRequest("AthleteId is required.");
            }

            var saved = await repo.UpsertAsync(athleteConfig);
            return Results.Ok(saved);
        });

        group.MapDelete("/athletes/{athleteId}", async (string athleteId, IAthleteRepository repo) =>
        {
            await repo.DeleteAsync(athleteId);
            return Results.NoContent();
        });

        group.MapGet("/levels", async (ILevelRepository repo) =>
            Results.Ok(await repo.GetAllAsync()));

        group.MapGet("/levels/{id}", async (string id, ILevelRepository repo) =>
        {
            var levelConfig = await repo.GetByIdAsync(id);
            return levelConfig is null ? Results.NotFound() : Results.Ok(levelConfig);
        });

        group.MapPut("/levels", async (LevelConfig levelConfig, ILevelRepository repo) =>
        {
            if (string.IsNullOrWhiteSpace(levelConfig.Level))
            {
                return Results.BadRequest("Level is required.");
            }

            var saved = await repo.UpsertAsync(levelConfig);
            return Results.Ok(saved);
        });

        group.MapDelete("/levels/{id}", async (string id, ILevelRepository repo) =>
        {
            await repo.DeleteAsync(id);
            return Results.NoContent();
        });

        return app;
    }
}
