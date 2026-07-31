using TrainingArchitect.Core.Interfaces;

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

        return app;
    }
}
