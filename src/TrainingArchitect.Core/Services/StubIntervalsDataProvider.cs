using TrainingArchitect.Core.Interfaces;
using TrainingArchitect.Core.Models;

namespace TrainingArchitect.Core.Services;

/// <summary>
/// Development stub for <see cref="IIntervalsDataProvider"/>.
/// Returns 28 days of synthetic CTL/ATL/TSB data.
/// TODO: Replace with a real intervals.icu REST API client that uses the
///       athlete's stored API key (fetched server-side from Key Vault / DB).
/// </summary>
public sealed class StubIntervalsDataProvider : IIntervalsDataProvider
{
    public Task<AthleteSnapshot> GetSnapshotAsync(
        string athleteId,
        CancellationToken cancellationToken = default)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var history = BuildSampleHistory(today, days: 28);

        var latest = history.Last();
        var snapshot = new AthleteSnapshot
        {
            AthleteId = athleteId,
            CtlToday = latest.Ctl,
            AtlToday = latest.Atl,
            FitnessHistory = history
        };

        return Task.FromResult(snapshot);
    }

    private static List<FitnessPoint> BuildSampleHistory(DateOnly endDate, int days)
    {
        // Simulate a realistic build-then-taper arc over the requested window.
        var points = new List<FitnessPoint>(days);
        double ctl = 55.0;
        double atl = 48.0;

        for (int daysAgo = days - 1; daysAgo >= 0; daysAgo--)
        {
            var date = endDate.AddDays(-daysAgo);

            // Rough simulation: build in weeks 1–3, taper in week 4
            double dailyTss = daysAgo > 7 ? 80 + (Math.Sin(daysAgo * 0.6) * 20) : 45;

            // TSS → ATL (7-day decay), CTL (42-day decay)
            atl = atl + (dailyTss - atl) * (1.0 / 7.0);
            ctl = ctl + (dailyTss - ctl) * (1.0 / 42.0);

            points.Add(new FitnessPoint
            {
                Date = date,
                Ctl = Math.Round(ctl, 1),
                Atl = Math.Round(atl, 1)
            });
        }

        return points;
    }
}
