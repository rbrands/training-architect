using BrandsAdvisory.Core.Interfaces;
using BrandsAdvisory.Core.Models;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using Microsoft.Extensions.Configuration;
using System.Linq.Expressions;

namespace BrandsAdvisory.Infrastructure.Repositories;

/// <summary>
/// Generic Cosmos DB repository implementation.
/// </summary>
/// <typeparam name="T">The document type, must inherit <see cref="CosmosDocument"/>.</typeparam>
public class CosmosRepository<T>(CosmosClient client, IConfiguration configuration)
    : IRepository<T> where T : CosmosDocument
{
    /// <summary>The Cosmos DB container this repository targets.</summary>
    protected readonly Container Container = client.GetContainer(
        configuration["CosmosDb:DatabaseId"]!,
        configuration["CosmosDb:ContainerName"]!);

    private static readonly string TypeKey = Activator.CreateInstance<T>().Type;

    /// <inheritdoc/>
    public async Task<T?> GetByIdAsync(string id)
    {
        try
        {
            var response = await Container.ReadItemAsync<T>(id, new PartitionKey(TypeKey));
            return response.Resource;
        }
        catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }
    }

    /// <inheritdoc/>
    public virtual async Task<IReadOnlyList<T>> GetAllAsync()
    {
        var query = new QueryDefinition("SELECT * FROM c WHERE c.type = @type")
            .WithParameter("@type", TypeKey);
        var options = new QueryRequestOptions { PartitionKey = new PartitionKey(TypeKey) };

        return await ExecuteQueryAsync(query, options);
    }

    /// <inheritdoc/>
    public virtual async Task<T> UpsertAsync(T document)
    {
        document.UpdatedAt = DateTime.UtcNow;
        var response = await Container.UpsertItemAsync(document, new PartitionKey(document.Type));
        return response.Resource;
    }

    /// <inheritdoc/>
    public async Task DeleteAsync(string id)
    {
        await Container.DeleteItemAsync<T>(id, new PartitionKey(TypeKey));
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<T>> QueryAsync(Expression<Func<T, bool>> predicate)
    {
        var options = new QueryRequestOptions { PartitionKey = new PartitionKey(TypeKey) };
        var iterator = Container
            .GetItemLinqQueryable<T>(requestOptions: options)
            .Where(d => d.Type == TypeKey)
            .Where(predicate)
            .ToFeedIterator();

        var results = new List<T>();
        while (iterator.HasMoreResults)
        {
            var page = await iterator.ReadNextAsync();
            results.AddRange(page);
        }
        return results;
    }

    /// <summary>
    /// Executes a parameterised query and returns all matching documents.
    /// </summary>
    /// <param name="query">The query definition.</param>
    /// <param name="options">Optional per-request options, e.g. partition key.</param>
    protected async Task<IReadOnlyList<T>> ExecuteQueryAsync(
        QueryDefinition query, QueryRequestOptions options)
    {
        var iterator = Container.GetItemQueryIterator<T>(query, requestOptions: options);
        var results = new List<T>();

        while (iterator.HasMoreResults)
        {
            var page = await iterator.ReadNextAsync();
            results.AddRange(page);
        }

        return results;
    }
}
