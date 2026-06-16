using BrandsAdvisory.Core.Interfaces;
using BrandsAdvisory.Core.Models;
using System.Linq.Expressions;
using System.Net.Http.Json;

namespace BrandsAdvisory.Client.Services;

/// <summary>
/// WebAssembly client implementation of <see cref="IAboutRepository"/>
/// that calls the <c>/api/about</c> minimal API endpoints on the host server.
/// </summary>
public class HttpAboutRepository(HttpClient http) : IAboutRepository
{
    public async Task<AboutContent> GetOrCreateAsync()
    {
        var result = await http.GetFromJsonAsync<AboutContent>("/api/about");
        return result ?? new AboutContent { Id = "about" };
    }

    public async Task<AboutContent> UpsertAsync(AboutContent document)
    {
        var response = await http.PutAsJsonAsync("/api/about", document);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<AboutContent>() ?? document;
    }

    public Task<AboutContent?> GetByIdAsync(string id) =>
        http.GetFromJsonAsync<AboutContent>($"/api/about");

    public async Task<IReadOnlyList<AboutContent>> GetAllAsync()
    {
        var item = await GetOrCreateAsync();
        return [item];
    }

    public Task DeleteAsync(string id) =>
        throw new NotSupportedException("Deleting the About document is not supported.");

    public Task<IReadOnlyList<AboutContent>> QueryAsync(Expression<Func<AboutContent, bool>> predicate) =>
        throw new NotSupportedException("QueryAsync is not supported in the WASM client.");
}
