using TrainingArchitect.Core.Models;

namespace TrainingArchitect.Core.Interfaces;

/// <summary>
/// Repository interface for <see cref="AthleteConfig"/> documents.
/// </summary>
public interface IAthleteRepository : IRepository<AthleteConfig>
{
    /// <summary>
    /// Returns the athlete configuration for the given athlete identifier.
    /// </summary>
    /// <param name="athleteId">Athlete identifier.</param>
    Task<AthleteConfig?> GetByAthleteIdAsync(string athleteId);
}