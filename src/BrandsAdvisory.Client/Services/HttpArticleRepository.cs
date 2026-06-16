using BrandsAdvisory.Core.Interfaces;
using BrandsAdvisory.Core.Models;
using System.Linq.Expressions;
using System.Net.Http.Json;

namespace BrandsAdvisory.Client.Services;

/// <summary>
/// WebAssembly client implementation of <see cref="IArticleRepository"/>
/// that calls the <c>/api/articles</c> minimal API endpoints on the host server.
/// </summary>
public class HttpArticleRepository(HttpClient http) : IArticleRepository
{
    public async Task<IReadOnlyList<Article>> GetAllAsync() =>
        await http.GetFromJsonAsync<List<Article>>("/api/articles") ?? [];

    public async Task<Article?> GetByIdAsync(string id) =>
        await http.GetFromJsonAsync<Article>($"/api/articles/{id}");

    public async Task<Article> UpsertAsync(Article document)
    {
        var response = await http.PutAsJsonAsync("/api/articles", document);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<Article>() ?? document;
    }

    public async Task DeleteAsync(string id)
    {
        var response = await http.DeleteAsync($"/api/articles/{id}");
        response.EnsureSuccessStatusCode();
    }

    // These methods are used only by server-side SSR pages which call
    // the Cosmos DB repository directly — not required in the WASM client.
    public Task<Article?> GetBySlugAsync(string slug) =>
        throw new NotSupportedException("Call GetBySlugAsync via the server-side repository.");

    public Task<IReadOnlyList<Article>> GetPublishedAsync() =>
        throw new NotSupportedException("Call GetPublishedAsync via the server-side repository.");

    public Task<IReadOnlyList<Article>> QueryAsync(Expression<Func<Article, bool>> predicate) =>
        throw new NotSupportedException("QueryAsync is not supported in the WASM client.");
}
