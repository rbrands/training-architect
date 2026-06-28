using TrainingArchitect.Endpoints;

namespace TrainingArchitect.Services;

/// <summary>
/// Provides athlete data retrieval for intervals.icu requests.
/// </summary>
public interface IAthleteDataService
{
    /// <summary>
    /// Retrieves athlete data by calling the MCP tool for the provided athlete.
    /// </summary>
    /// <param name="athleteId">Intervals athlete identifier.</param>
    /// <param name="apiKey">Intervals API key.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Resolved athlete data response.</returns>
    Task<AthleteDataResponse> GetAsync(string athleteId, string apiKey, CancellationToken ct);
}
