using TrainingArchitect.Core.Interfaces;
using TrainingArchitect.Core.Models;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;

namespace TrainingArchitect.Infrastructure.Repositories;

/// <summary>
/// Cosmos DB repository for <see cref="Article"/> documents.
/// </summary>
public class ArticleRepository(CosmosClient client, IConfiguration configuration)
    : CosmosRepository<Article>(client, configuration), IArticleRepository
{
    /// <inheritdoc/>
    public override async Task<Article> UpsertAsync(Article document)
    {
        // Map ExpiresAfterDays → Cosmos DB TTL (seconds).
        // null  → omit ttl field; container default (defaultTtl = -1) applies → item never expires.
        // n > 0 → item is deleted automatically after n days from the last write (_ts).
        document.TimeToLive = document.ExpiresAfterDays.HasValue
            ? document.ExpiresAfterDays.Value * 86_400
            : null;

        return await base.UpsertAsync(document);
    }

    /// <inheritdoc/>
    public override async Task<IReadOnlyList<Article>> GetAllAsync()
    {
        var query = new QueryDefinition(
            "SELECT * FROM c WHERE c.type = 'article' ORDER BY c.createdAt DESC");
        var options = new QueryRequestOptions { PartitionKey = new PartitionKey("article") };

        return await ExecuteQueryAsync(query, options);
    }

    /// <inheritdoc/>
    public async Task<Article?> GetBySlugAsync(string slug)
    {
        var query = new QueryDefinition(
            "SELECT * FROM c WHERE c.type = 'article' AND c.slug = @slug")
            .WithParameter("@slug", slug);
        var options = new QueryRequestOptions { PartitionKey = new PartitionKey("article") };

        var results = await ExecuteQueryAsync(query, options);
        return results.FirstOrDefault();
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<Article>> GetPublishedAsync()
    {
        var query = new QueryDefinition(
            "SELECT * FROM c WHERE c.type = 'article' AND c.isPublished = true " +
            "ORDER BY c.publishedDate DESC");
        var options = new QueryRequestOptions { PartitionKey = new PartitionKey("article") };

        return await ExecuteQueryAsync(query, options);
    }
}
