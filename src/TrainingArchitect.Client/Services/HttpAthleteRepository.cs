using System.Linq.Expressions;
using System.Net.Http.Json;
using TrainingArchitect.Core.Interfaces;
using TrainingArchitect.Core.Models;

namespace TrainingArchitect.Client.Services;

/// <summary>
/// WebAssembly client implementation of <see cref="IAthleteRepository"/>
/// that calls the admin athlete endpoints on the host server.
/// </summary>
public class HttpAthleteRepository(HttpClient http) : IAthleteRepository
{
    public async Task<IReadOnlyList<AthleteConfig>> GetAllAsync() =>
        await http.GetFromJsonAsync<List<AthleteConfig>>("/api/admin/athletes") ?? [];

    public async Task<AthleteConfig?> GetByIdAsync(string id)
    {
        var encodedId = Uri.EscapeDataString(id);
        return await http.GetFromJsonAsync<AthleteConfig>($"/api/admin/athletes/{encodedId}");
    }

    public async Task<AthleteConfig> UpsertAsync(AthleteConfig document)
    {
        var response = await http.PutAsJsonAsync("/api/admin/athletes", document);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<AthleteConfig>()
            ?? throw new InvalidOperationException("Athlete config response was empty.");
    }

    public async Task DeleteAsync(string id)
    {
        var encodedId = Uri.EscapeDataString(id);
        var response = await http.DeleteAsync($"/api/admin/athletes/{encodedId}");
        response.EnsureSuccessStatusCode();
    }

    public Task<IReadOnlyList<AthleteConfig>> QueryAsync(Expression<Func<AthleteConfig, bool>> predicate) =>
        throw new NotSupportedException("QueryAsync is not supported in the WASM client.");

    public Task<AthleteConfig?> GetByAthleteIdAsync(string athleteId) => GetByIdAsync(athleteId);
}