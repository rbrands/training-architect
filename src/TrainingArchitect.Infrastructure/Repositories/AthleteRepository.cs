using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using TrainingArchitect.Core.Interfaces;
using TrainingArchitect.Core.Models;

namespace TrainingArchitect.Infrastructure.Repositories;

/// <summary>
/// Cosmos DB repository for <see cref="AthleteConfig"/> documents.
/// </summary>
public class AthleteRepository(CosmosClient client, IConfiguration configuration)
    : CosmosRepository<AthleteConfig>(client, configuration), IAthleteRepository
{
    /// <inheritdoc/>
    public async Task<AthleteConfig?> GetByAthleteIdAsync(string athleteId)
        => await GetByIdAsync(athleteId);

    /// <inheritdoc/>
    public override async Task<AthleteConfig> UpsertAsync(AthleteConfig document)
    {
        document.Id = document.AthleteId;
        return await base.UpsertAsync(document);
    }
}