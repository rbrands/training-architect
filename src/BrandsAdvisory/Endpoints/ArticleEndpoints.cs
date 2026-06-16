using BrandsAdvisory.Core.Interfaces;
using BrandsAdvisory.Core.Models;

namespace BrandsAdvisory.Endpoints;

/// <summary>
/// Minimal API endpoints for <see cref="Article"/> — used by the WASM admin client.
/// </summary>
public static class ArticleEndpoints
{
    public static IEndpointRouteBuilder MapArticleEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/articles")
            .RequireAuthorization(p => p.RequireRole("SiteAdmin"));

        group.MapGet("/", async (IArticleRepository repo) =>
            Results.Ok(await repo.GetAllAsync()));

        group.MapGet("/{id}", async (string id, IArticleRepository repo) =>
        {
            var article = await repo.GetByIdAsync(id);
            return article is null ? Results.NotFound() : Results.Ok(article);
        });

        group.MapPut("/", async (Article article, IArticleRepository repo) =>
        {
            var saved = await repo.UpsertAsync(article);
            return Results.Ok(saved);
        });

        group.MapDelete("/{id}", async (string id, IArticleRepository repo) =>
        {
            await repo.DeleteAsync(id);
            return Results.NoContent();
        });

        return app;
    }
}
