using BrandsAdvisory.Core.Interfaces;
using BrandsAdvisory.Core.Models;
using Microsoft.AspNetCore.Authorization;

namespace BrandsAdvisory.Endpoints;

/// <summary>
/// Minimal API endpoints for <see cref="AboutContent"/> — used by the WASM admin client.
/// </summary>
public static class AboutEndpoints
{
    public static IEndpointRouteBuilder MapAboutEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/about")
            .RequireAuthorization(p => p.RequireRole("SiteAdmin"));

        group.MapGet("/", async (IAboutRepository repo) =>
        {
            var about = await repo.GetOrCreateAsync();
            return Results.Ok(about);
        });

        group.MapPut("/", async (AboutContent about, IAboutRepository repo) =>
        {
            var saved = await repo.UpsertAsync(about);
            return Results.Ok(saved);
        });

        return app;
    }
}
