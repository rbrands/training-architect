using TrainingArchitect.Core.Interfaces;
using TrainingArchitect.Core.Models;
using System.Linq.Expressions;
using System.Net.Http.Json;

namespace TrainingArchitect.Client.Services;

/// <summary>
/// WebAssembly client implementation of <see cref="IUsageCounterRepository"/>
/// that calls the admin usage endpoints on the host server.
/// </summary>
public class HttpUsageCounterRepository(HttpClient http) : IUsageCounterRepository
{
    public async Task<IReadOnlyList<UsageCounter>> GetAllAsync() =>
        await http.GetFromJsonAsync<List<UsageCounter>>("/api/admin/usage") ?? [];

    public Task<UsageCounter?> GetByIdAsync(string id) =>
        throw new NotSupportedException("GetByIdAsync is not supported in the WASM client.");

    public Task<UsageCounter> UpsertAsync(UsageCounter document) =>
        throw new NotSupportedException("UpsertAsync is not supported in the WASM client.");

    public Task DeleteAsync(string id) =>
        throw new NotSupportedException("DeleteAsync is not supported in the WASM client.");

    public Task<IReadOnlyList<UsageCounter>> QueryAsync(Expression<Func<UsageCounter, bool>> predicate) =>
        throw new NotSupportedException("QueryAsync is not supported in the WASM client.");

    public Task RecordUsageAsync(
        string athleteId,
        string action,
        int inputTokens,
        int cachedTokens,
        int outputTokens) =>
        throw new NotSupportedException("RecordUsageAsync is not supported in the WASM client.");

    public async Task<IReadOnlyList<UsageCounter>> GetByAthleteIdAsync(string athleteId)
    {
        var encodedAthleteId = Uri.EscapeDataString(athleteId);
        return await http.GetFromJsonAsync<List<UsageCounter>>($"/api/admin/usage/{encodedAthleteId}") ?? [];
    }

    public Task<(UsageCounter? Monthly, UsageCounter? Weekly)> GetByAthleteAndPeriodsAsync(
        string athleteId,
        string monthlyPeriodKey,
        string weeklyPeriodKey) =>
        throw new NotSupportedException("GetByAthleteAndPeriodsAsync is not supported in the WASM client.");

    public async Task RefreshGlobalCountersAsync(CancellationToken ct = default)
    {
        var response = await http.PostAsync("/api/admin/usage/refresh", content: null, ct);
        response.EnsureSuccessStatusCode();
    }
}
