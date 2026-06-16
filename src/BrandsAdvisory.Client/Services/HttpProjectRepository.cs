using BrandsAdvisory.Core.Interfaces;
using BrandsAdvisory.Core.Models;
using System.Linq.Expressions;
using System.Net.Http.Json;

namespace BrandsAdvisory.Client.Services;

/// <summary>
/// WebAssembly client implementation of <see cref="IProjectRepository"/>
/// that calls the <c>/api/projects</c> minimal API endpoints on the host server.
/// </summary>
public class HttpProjectRepository(HttpClient http) : IProjectRepository
{
    public async Task<IReadOnlyList<Project>> GetAllAsync() =>
        await http.GetFromJsonAsync<List<Project>>("/api/projects") ?? [];

    public async Task<Project?> GetByIdAsync(string id) =>
        await http.GetFromJsonAsync<Project>($"/api/projects/{id}");

    public async Task<Project> UpsertAsync(Project document)
    {
        var response = await http.PutAsJsonAsync("/api/projects", document);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<Project>() ?? document;
    }

    public async Task DeleteAsync(string id)
    {
        var response = await http.DeleteAsync($"/api/projects/{id}");
        response.EnsureSuccessStatusCode();
    }

    public Task<IReadOnlyList<Project>> QueryAsync(Expression<Func<Project, bool>> predicate) =>
        throw new NotSupportedException("QueryAsync is not supported in the WASM client.");
}
