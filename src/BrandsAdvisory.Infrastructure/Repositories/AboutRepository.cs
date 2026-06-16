using BrandsAdvisory.Core.Interfaces;
using BrandsAdvisory.Core.Models;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;

namespace BrandsAdvisory.Infrastructure.Repositories;

/// <summary>
/// Cosmos DB repository for the singleton <see cref="AboutContent"/> document.
/// </summary>
public class AboutRepository(CosmosClient client, IConfiguration configuration)
    : CosmosRepository<AboutContent>(client, configuration), IAboutRepository
{
    /// <inheritdoc/>
    public async Task<AboutContent> GetOrCreateAsync()
    {
        var all = await GetAllAsync();
        if (all.Count > 0)
            return all[0];

        var about = new AboutContent { Id = "about" };
        return await UpsertAsync(about);
    }
}
