namespace TrainingArchitect.Core.Models;

/// <summary>A single point in the athlete's fitness history (one day).</summary>
public class FitnessPoint
{
    public DateOnly Date { get; set; }

    /// <summary>Chronic Training Load — 42-day exponentially weighted average of daily TSS.</summary>
    public double Ctl { get; set; }

    /// <summary>Acute Training Load — 7-day exponentially weighted average of daily TSS.</summary>
    public double Atl { get; set; }

    /// <summary>Training Stress Balance — CTL minus ATL. Positive = fresh, negative = fatigued.</summary>
    public double Tsb => Ctl - Atl;
}

/// <summary>
/// Current fitness snapshot for an athlete, sourced from intervals.icu.
/// TODO: Replace sample data with real intervals.icu API response.
/// </summary>
public class AthleteSnapshot
{
    public string AthleteId { get; set; } = string.Empty;

    public double CtlToday { get; set; }
    public double AtlToday { get; set; }
    public double TsbToday => CtlToday - AtlToday;

    /// <summary>Chronological fitness history (oldest first).</summary>
    public List<FitnessPoint> FitnessHistory { get; set; } = [];
}
