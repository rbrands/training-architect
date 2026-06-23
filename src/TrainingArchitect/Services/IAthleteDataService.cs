using TrainingArchitect.Endpoints;

namespace TrainingArchitect.Services;

/// <summary>
/// Provides athlete data retrieval for intervals.icu requests.
/// </summary>
public interface IAthleteDataService
{
    /// <summary>
    /// Retrieves athlete data for the requested scope.
    /// </summary>
    /// <param name="request">Athlete credentials and requested data scope.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Resolved athlete data response.</returns>
    Task<AthleteDataResponse> GetAsync(AthleteDataRequest request, CancellationToken ct);
}
