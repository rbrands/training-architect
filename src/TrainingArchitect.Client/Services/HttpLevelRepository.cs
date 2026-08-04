using System.Linq.Expressions;
using System.Net.Http.Json;
using TrainingArchitect.Core.Interfaces;
using TrainingArchitect.Core.Models;

namespace TrainingArchitect.Client.Services;

/// <summary>
/// WebAssembly client implementation of <see cref="ILevelRepository"/>
/// that calls the admin level endpoints on the host server.
/// </summary>
public class HttpLevelRepository(HttpClient http) : ILevelRepository
{
    public async Task<IReadOnlyList<LevelConfig>> GetAllAsync() =>
        await http.GetFromJsonAsync<List<LevelConfig>>("/api/admin/levels") ?? [];

    public async Task<LevelConfig?> GetByIdAsync(string id)
    {
        var encodedId = Uri.EscapeDataString(id);
        return await http.GetFromJsonAsync<LevelConfig>($"/api/admin/levels/{encodedId}");
    }

    public async Task<LevelConfig> UpsertAsync(LevelConfig document)
    {
        var response = await http.PutAsJsonAsync("/api/admin/levels", document);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<LevelConfig>()
            ?? throw new InvalidOperationException("Level config response was empty.");
    }

    public async Task DeleteAsync(string id)
    {
        var encodedId = Uri.EscapeDataString(id);
        var response = await http.DeleteAsync($"/api/admin/levels/{encodedId}");
        response.EnsureSuccessStatusCode();
    }

    public Task<IReadOnlyList<LevelConfig>> QueryAsync(Expression<Func<LevelConfig, bool>> predicate) =>
        throw new NotSupportedException("QueryAsync is not supported in the WASM client.");

    public async Task<LevelConfig?> GetByLevelAsync(string level)
    {
        var all = await GetAllAsync();
        return all.FirstOrDefault(l => string.Equals(l.Level, level, StringComparison.OrdinalIgnoreCase));
    }
}