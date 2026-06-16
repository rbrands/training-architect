using BrandsAdvisory.Core.Interfaces;
using BrandsAdvisory.Core.Models;

namespace BrandsAdvisory.Endpoints;

/// <summary>
/// Minimal API endpoints for <see cref="Project"/> — used by the WASM admin client.
/// </summary>
public static class ProjectEndpoints
{
    public static IEndpointRouteBuilder MapProjectEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/projects")
            .RequireAuthorization(p => p.RequireRole("SiteAdmin"));

        group.MapGet("/", async (IProjectRepository repo) =>
            Results.Ok(await repo.GetAllAsync()));

        group.MapGet("/{id}", async (string id, IProjectRepository repo) =>
        {
            var project = await repo.GetByIdAsync(id);
            return project is null ? Results.NotFound() : Results.Ok(project);
        });

        group.MapPut("/", async (Project project, IProjectRepository repo) =>
        {
            var saved = await repo.UpsertAsync(project);
            return Results.Ok(saved);
        });

        group.MapDelete("/{id}", async (string id, IProjectRepository repo) =>
        {
            await repo.DeleteAsync(id);
            return Results.NoContent();
        });

        return app;
    }
}
