using BrandsAdvisory.Core.Models;

namespace BrandsAdvisory.Core.Interfaces;

/// <summary>
/// Repository interface for the singleton <see cref="AboutContent"/> document.
/// </summary>
public interface IAboutRepository : IRepository<AboutContent>
{
    /// <summary>
    /// Returns the existing about document, or creates and persists a
    /// default one if none exists yet.
    /// </summary>
    Task<AboutContent> GetOrCreateAsync();
}
