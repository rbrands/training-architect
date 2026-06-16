using BrandsAdvisory.Core.Models;

namespace BrandsAdvisory.Core.Interfaces;

/// <summary>
/// Repository interface for <see cref="Article"/> documents.
/// </summary>
public interface IArticleRepository : IRepository<Article>
{
    /// <summary>
    /// Returns a published article matching the given slug, or 
    /// <c>null</c> if not found.
    /// </summary>
    /// <param name="slug">The URL-friendly article identifier.</param>
    Task<Article?> GetBySlugAsync(string slug);

    /// <summary>
    /// Returns all published articles ordered by published date descending.
    /// </summary>
    Task<IReadOnlyList<Article>> GetPublishedAsync();
}
