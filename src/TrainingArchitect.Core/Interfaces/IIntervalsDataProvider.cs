using TrainingArchitect.Core.Models;

namespace TrainingArchitect.Core.Interfaces;

/// <summary>
/// Retrieves the athlete's current fitness data from the intervals.icu API.
/// </summary>
public interface IIntervalsDataProvider
{
    /// <summary>
    /// Returns the athlete's fitness snapshot (CTL / ATL / TSB history).
    /// TODO: Replace stub with a real intervals.icu API call authenticated
    ///       with the athlete's stored API key (retrieved server-side).
    /// </summary>
    /// <param name="athleteId">The intervals.icu athlete identifier.</param>
    Task<AthleteSnapshot> GetSnapshotAsync(
        string athleteId,
        CancellationToken cancellationToken = default);
}
