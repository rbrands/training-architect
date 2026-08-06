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

        await Task.WhenAll(
            UpsertPatchAsync(
                monthlyId,
                athleteId,
                UsageCounter.MonthlyUsageType,
                monthlyPeriodKey,
                action,
                inputTokens,
                cachedTokens,
                outputTokens),
            UpsertPatchAsync(
                weeklyId,
                athleteId,
                UsageCounter.WeeklyUsageType,
                weeklyPeriodKey,
                action,
                inputTokens,
                cachedTokens,
                outputTokens));
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
        document.TimeToLive = UsageCounter.GetTimeToLiveSeconds(document.Type);
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

    /// <inheritdoc/>
    public async Task<(UsageCounter? Monthly, UsageCounter? Weekly)> GetByAthleteAndPeriodsAsync(
        string athleteId,
        string monthlyPeriodKey,
        string weeklyPeriodKey)
    {
        var monthlyId = $"{athleteId}_month_{monthlyPeriodKey}";
        var weeklyId = $"{athleteId}_week_{weeklyPeriodKey}";

        var monthlyTask = TryReadByIdAndTypeAsync(monthlyId, UsageCounter.MonthlyUsageType);
        var weeklyTask = TryReadByIdAndTypeAsync(weeklyId, UsageCounter.WeeklyUsageType);

        await Task.WhenAll(monthlyTask, weeklyTask);
        return (await monthlyTask, await weeklyTask);
    }

    /// <inheritdoc/>
    public async Task RefreshGlobalCountersAsync(CancellationToken ct = default)
    {
        const string globalAthleteId = "__GLOBAL__";

        var now = DateTime.UtcNow;
        var monthlyPeriodKey = $"{now:yyyy-MM}";
        var weeklyPeriodKey = $"{ISOWeek.GetYear(now)}-W{ISOWeek.GetWeekOfYear(now):00}";

        var monthlyAthleteCounters = await GetCountersForPeriodAsync(
            UsageCounter.MonthlyUsageType,
            monthlyPeriodKey,
            globalAthleteId,
            ct);

        var weeklyAthleteCounters = await GetCountersForPeriodAsync(
            UsageCounter.WeeklyUsageType,
            weeklyPeriodKey,
            globalAthleteId,
            ct);

        var globalMonthly = BuildGlobalCounter(
            id: $"global_month_{monthlyPeriodKey}",
            usageType: UsageCounter.MonthlyUsageType,
            periodKey: monthlyPeriodKey,
            athleteCounters: monthlyAthleteCounters,
            globalAthleteId: globalAthleteId);

        var globalWeekly = BuildGlobalCounter(
            id: $"global_week_{weeklyPeriodKey}",
            usageType: UsageCounter.WeeklyUsageType,
            periodKey: weeklyPeriodKey,
            athleteCounters: weeklyAthleteCounters,
            globalAthleteId: globalAthleteId);

        await Task.WhenAll(
            UpsertAsync(globalMonthly),
            UpsertAsync(globalWeekly));
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
                    TimeToLive = UsageCounter.GetTimeToLiveSeconds(type)
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
            PatchOperation.Set("/ttl", UsageCounter.GetTimeToLiveSeconds(type)),
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
                PatchOperation.Set("/ttl", UsageCounter.GetTimeToLiveSeconds(type)),
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

    private async Task<IReadOnlyList<UsageCounter>> GetCountersForPeriodAsync(
        string usageType,
        string periodKey,
        string globalAthleteId,
        CancellationToken ct)
    {
        var query = new QueryDefinition(
            "SELECT * FROM c WHERE c.type = @type AND c.periodKey = @periodKey AND c.athleteId != @globalAthleteId")
            .WithParameter("@type", usageType)
            .WithParameter("@periodKey", periodKey)
            .WithParameter("@globalAthleteId", globalAthleteId);

        var options = new QueryRequestOptions { PartitionKey = new PartitionKey(usageType) };
        var iterator = _container.GetItemQueryIterator<UsageCounter>(query, requestOptions: options);
        var results = new List<UsageCounter>();

        while (iterator.HasMoreResults)
        {
            var page = await iterator.ReadNextAsync(ct);
            results.AddRange(page);
        }

        return results;
    }

    private static UsageCounter BuildGlobalCounter(
        string id,
        string usageType,
        string periodKey,
        IReadOnlyList<UsageCounter> athleteCounters,
        string globalAthleteId)
    {
        var counts = new Dictionary<string, int>(StringComparer.Ordinal);
        var totalInputTokens = 0;
        var totalCachedTokens = 0;
        var totalOutputTokens = 0;

        foreach (var counter in athleteCounters)
        {
            checked
            {
                totalInputTokens += counter.TotalInputTokens;
                totalCachedTokens += counter.TotalCachedTokens;
                totalOutputTokens += counter.TotalOutputTokens;
            }

            foreach (var (actionKey, actionCount) in counter.Counts)
            {
                counts[actionKey] = counts.TryGetValue(actionKey, out var existing)
                    ? existing + actionCount
                    : actionCount;
            }
        }

        return new UsageCounter
        {
            Id = id,
            AthleteId = globalAthleteId,
            UsageType = usageType,
            PeriodKey = periodKey,
            Counts = counts,
            TotalInputTokens = totalInputTokens,
            TotalCachedTokens = totalCachedTokens,
            TotalOutputTokens = totalOutputTokens,
            TimeToLive = UsageCounter.GetTimeToLiveSeconds(usageType),
            UpdatedAt = DateTime.UtcNow
        };
    }

    private async Task<UsageCounter?> TryReadByIdAndTypeAsync(string id, string usageType)
    {
        try
        {
            var response = await _container.ReadItemAsync<UsageCounter>(id, new PartitionKey(usageType));
            return response.Resource;
        }
        catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }
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
