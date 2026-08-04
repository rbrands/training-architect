using TrainingArchitect.Core.Models;

namespace TrainingArchitect.Core.Interfaces;

/// <summary>
/// Repository interface for <see cref="LevelConfig"/> documents.
/// </summary>
public interface ILevelRepository : IRepository<LevelConfig>
{
    /// <summary>
    /// Returns a level configuration by level key.
    /// </summary>
    /// <param name="level">Level key, for example <c>basic</c>.</param>
    Task<LevelConfig?> GetByLevelAsync(string level);
}