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

    /// <summary>
    /// Uploads a generated weekly training plan by calling the MCP upload tool.
    /// </summary>
    /// <param name="athleteId">Intervals athlete identifier.</param>
    /// <param name="apiKey">Intervals API key.</param>
    /// <param name="weekPlanJson">Weekly plan payload as JSON object text.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A task representing the asynchronous upload operation.</returns>
    Task UploadWeekPlanAsync(string athleteId, string apiKey, string weekPlanJson, CancellationToken ct);
}
