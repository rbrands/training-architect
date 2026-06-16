using BrandsAdvisory.Core.Interfaces;
using BrandsAdvisory.Core.Models;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;

namespace BrandsAdvisory.Infrastructure.Repositories;

/// <summary>
/// Cosmos DB repository for <see cref="Project"/> documents.
/// </summary>
public class ProjectRepository(CosmosClient client, IConfiguration configuration)
    : CosmosRepository<Project>(client, configuration), IProjectRepository
{
    /// <inheritdoc/>
    public override async Task<IReadOnlyList<Project>> GetAllAsync()
    {
        var query = new QueryDefinition(
            "SELECT * FROM c WHERE c.type = 'project' ORDER BY c.projectStart DESC");
        var options = new QueryRequestOptions { PartitionKey = new PartitionKey("project") };

        return await ExecuteQueryAsync(query, options);
    }
}
