using System.Globalization;
using System.Linq.Expressions;
using TrainingArchitect.Core.Interfaces;
using TrainingArchitect.Core.Models;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using Microsoft.Extensions.Configuration;

namespace TrainingArchitect.Infrastructure.Repositories;

/// <summary>
/// Cosmos DB repository for <see cref="UsageCounter"/> documents.
/// Supports both monthly and weekly usage partitions.
/// </summary>
public class UsageCounterRepository(CosmosClient client, IConfiguration configuration)
    : IUsageCounterRepository
{
    private static readonly string[] UsageTypes =
    [
        UsageCounter.MonthlyUsageType,
        UsageCounter.WeeklyUsageType
    ];

    private readonly Container _container = client.GetContainer(
        configuration["CosmosDb:DatabaseId"]!,
        configuration["CosmosDb:ContainerName"]!);

    /// <inheritdoc/>
    public async Task RecordUsageAsync(
        string athleteId,
        string action,
        int inputTokens,
        int cachedTokens,
        int outputTokens)
    {
        var now = DateTime.UtcNow;
        var isoWeek = ISOWeek.GetWeekOfYear(now);
        var isoYear = ISOWeek.GetYear(now);

        var monthlyPeriodKey = $"{now:yyyy-MM}";
        var monthlyId = $"{athleteId}_month_{monthlyPeriodKey}";

        var weeklyPeriodKey = $"{isoYear}-W{isoWeek:00}";
        var weeklyId = $"{athleteId}_week_{weeklyPeriodKey}";

        await UpsertPatchAsync(
            monthlyId,
            athleteId,
            UsageCounter.MonthlyUsageType,
            monthlyPeriodKey,
            action,
            inputTokens,
            cachedTokens,
            outputTokens);

        await UpsertPatchAsync(
            weeklyId,
            athleteId,
            UsageCounter.WeeklyUsageType,
            weeklyPeriodKey,
            action,
            inputTokens,
            cachedTokens,
            outputTokens);
    }

    /// <inheritdoc/>
    public async Task<UsageCounter?> GetByIdAsync(string id)
    {
        foreach (var usageType in UsageTypes)
        {
            try
            {
                var response = await _container.ReadItemAsync<UsageCounter>(
                    id,
                    new PartitionKey(usageType));
                return response.Resource;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                // Try the next supported partition type.
            }
        }

        return null;
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<UsageCounter>> GetAllAsync()
    {
        var query = new QueryDefinition(
            "SELECT * FROM c WHERE c.type IN (@monthlyType, @weeklyType) ORDER BY c.periodKey DESC")
            .WithParameter("@monthlyType", UsageCounter.MonthlyUsageType)
            .WithParameter("@weeklyType", UsageCounter.WeeklyUsageType);

        return await ExecuteQueryAsync(query, options: null);
    }

    /// <inheritdoc/>
    public async Task<UsageCounter> UpsertAsync(UsageCounter document)
    {
        document.UpdatedAt = DateTime.UtcNow;
        document.TimeToLive = UsageCounter.TimeToLiveSeconds;
        var response = await _container.UpsertItemAsync(document, new PartitionKey(document.Type));
        return response.Resource;
    }

    /// <inheritdoc/>
    public async Task DeleteAsync(string id)
    {
        foreach (var usageType in UsageTypes)
        {
            try
            {
                await _container.DeleteItemAsync<UsageCounter>(id, new PartitionKey(usageType));
                return;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                // Continue until one partition matches.
            }
        }
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<UsageCounter>> QueryAsync(Expression<Func<UsageCounter, bool>> predicate)
    {
        var iterator = _container
            .GetItemLinqQueryable<UsageCounter>()
            .Where(d => d.Type == UsageCounter.MonthlyUsageType || d.Type == UsageCounter.WeeklyUsageType)
            .Where(predicate)
            .ToFeedIterator();

        var results = new List<UsageCounter>();
        while (iterator.HasMoreResults)
        {
            var page = await iterator.ReadNextAsync();
            results.AddRange(page);
        }

        return results;
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<UsageCounter>> GetByAthleteIdAsync(string athleteId)
    {
        var query = new QueryDefinition(
            "SELECT * FROM c WHERE c.athleteId = @athleteId AND c.type IN (@monthlyType, @weeklyType) " +
            "ORDER BY c.periodKey DESC")
            .WithParameter("@athleteId", athleteId)
            .WithParameter("@monthlyType", UsageCounter.MonthlyUsageType)
            .WithParameter("@weeklyType", UsageCounter.WeeklyUsageType);

        return await ExecuteQueryAsync(query, options: null);
    }

    private async Task UpsertPatchAsync(
        string id,
        string athleteId,
        string type,
        string periodKey,
        string action,
        int inputTokens,
        int cachedTokens,
        int outputTokens)
    {
        try
        {
            await PatchCountersAsync(id, type, action, inputTokens, cachedTokens, outputTokens);
        }
        catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            try
            {
                var seed = new UsageCounter
                {
                    Id = id,
                    AthleteId = athleteId,
                    UsageType = type,
                    PeriodKey = periodKey,
                    Counts = new Dictionary<string, int>(StringComparer.Ordinal),
                    TimeToLive = UsageCounter.TimeToLiveSeconds
                };

                await _container.CreateItemAsync(seed, new PartitionKey(type));
            }
            catch (CosmosException createEx) when (createEx.StatusCode == System.Net.HttpStatusCode.Conflict)
            {
                // Another request created the item first.
            }

            await PatchCountersAsync(id, type, action, inputTokens, cachedTokens, outputTokens);
        }
    }

    private async Task PatchCountersAsync(
        string id,
        string type,
        string action,
        int inputTokens,
        int cachedTokens,
        int outputTokens)
    {
        var escapedAction = EscapeJsonPointerSegment(action);
        var incrementOps = new List<PatchOperation>
        {
            PatchOperation.Increment($"/counts/{escapedAction}", 1),
            PatchOperation.Increment("/totalInputTokens", inputTokens),
            PatchOperation.Increment("/totalCachedTokens", cachedTokens),
            PatchOperation.Increment("/totalOutputTokens", outputTokens),
            PatchOperation.Set("/ttl", UsageCounter.TimeToLiveSeconds),
            PatchOperation.Set("/updatedAt", DateTime.UtcNow)
        };

        try
        {
            await _container.PatchItemAsync<UsageCounter>(
                id,
                new PartitionKey(type),
                incrementOps);
        }
        catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.BadRequest)
        {
            // Missing action key is common on first usage for a given action.
            var initActionOps = new List<PatchOperation>
            {
                PatchOperation.Set($"/counts/{escapedAction}", 1),
                PatchOperation.Increment("/totalInputTokens", inputTokens),
                PatchOperation.Increment("/totalCachedTokens", cachedTokens),
                PatchOperation.Increment("/totalOutputTokens", outputTokens),
                PatchOperation.Set("/ttl", UsageCounter.TimeToLiveSeconds),
                PatchOperation.Set("/updatedAt", DateTime.UtcNow)
            };

            await _container.PatchItemAsync<UsageCounter>(
                id,
                new PartitionKey(type),
                initActionOps);
        }
    }

    private static string EscapeJsonPointerSegment(string value)
    {
        return value.Replace("~", "~0", StringComparison.Ordinal)
            .Replace("/", "~1", StringComparison.Ordinal);
    }

    private async Task<IReadOnlyList<UsageCounter>> ExecuteQueryAsync(
        QueryDefinition query,
        QueryRequestOptions? options)
    {
        var iterator = _container.GetItemQueryIterator<UsageCounter>(query, requestOptions: options);
        var results = new List<UsageCounter>();

        while (iterator.HasMoreResults)
        {
            var page = await iterator.ReadNextAsync();
            results.AddRange(page);
        }

        return results;
    }
}
