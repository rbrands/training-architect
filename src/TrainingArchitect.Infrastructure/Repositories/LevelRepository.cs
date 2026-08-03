using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using TrainingArchitect.Core.Interfaces;
using TrainingArchitect.Core.Models;

namespace TrainingArchitect.Infrastructure.Repositories;

/// <summary>
/// Cosmos DB repository for <see cref="LevelConfig"/> documents.
/// </summary>
public class LevelRepository(CosmosClient client, IConfiguration configuration)
    : CosmosRepository<LevelConfig>(client, configuration), ILevelRepository
{
    /// <inheritdoc/>
    public async Task<LevelConfig?> GetByLevelAsync(string level)
    {
        var normalizedLevel = NormalizeLevel(level);
        return await GetByIdAsync(BuildId(normalizedLevel));
    }

    /// <inheritdoc/>
    public override async Task<LevelConfig> UpsertAsync(LevelConfig document)
    {
        document.Level = NormalizeLevel(document.Level);
        document.Label = document.Label.Trim();
        document.Id = BuildId(document.Level);
        return await base.UpsertAsync(document);
    }

    private static string NormalizeLevel(string level) => level.Trim().ToLowerInvariant();

    private static string BuildId(string level) => $"level_{level}";
}