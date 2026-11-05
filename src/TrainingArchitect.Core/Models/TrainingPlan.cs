namespace TrainingArchitect.Core.Models;

/// <summary>Broad category of a planned workout.</summary>
public enum WorkoutType
{
    Endurance,
    Tempo,
    Threshold,
    VO2Max,
    Race,
    Strength,
    Rest
}

/// <summary>Training phase for a planned strength session.</summary>
public enum StrengthPhase
{
    Adaptation,
    Transition,
    Maximum,
    Maintenance
}

/// <summary>Parses canonical strength phase tags.</summary>
public static class StrengthPhaseParser
{
    /// <summary>
    /// Finds the strength phase represented by a canonical <c>strength-&lt;phase&gt;</c> tag.
    /// </summary>
    /// <param name="tags">Workout tags to inspect.</param>
    /// <param name="phase">The parsed phase, or the default value when no unique phase is found.</param>
    /// <returns><see langword="true"/> when exactly one canonical strength phase is present.</returns>
    public static bool TryParse(IEnumerable<string> tags, out StrengthPhase phase)
    {
        ArgumentNullException.ThrowIfNull(tags);
        StrengthPhase? parsedPhase = null;
        foreach (var tag in tags)
        {
            var candidate = tag switch
            {
                "strength-adaptation" => StrengthPhase.Adaptation,
                "strength-transition" => StrengthPhase.Transition,
                "strength-maximum" => StrengthPhase.Maximum,
                "strength-maintenance" => StrengthPhase.Maintenance,
                _ => (StrengthPhase?)null
            };

            if (candidate is null)
            {
                continue;
            }

            if (parsedPhase is not null && parsedPhase != candidate)
            {
                phase = default;
                return false;
            }

            parsedPhase = candidate;
        }

        phase = parsedPhase ?? default;
        return parsedPhase is not null;
    }
}

/// <summary>A single workout within a training plan week.</summary>
public class PlannedWorkout
{
    /// <summary>Day of the week, e.g. "Monday".</summary>
    public string Day { get; set; } = string.Empty;

    /// <summary>Short display title, e.g. "Z2 Long Ride".</summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>Coaching notes and execution guidance for the athlete.</summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>Gets or sets the strength training phase when this is a strength workout.</summary>
    public StrengthPhase? StrengthPhase { get; set; }

    public WorkoutType Type { get; set; }

    public int DurationMinutes { get; set; }

    /// <summary>Training Stress Score estimate for this session.</summary>
    public double Tss { get; set; }

    /// <summary>Intensity Factor (normalised power / FTP).</summary>
    public double IntensityFactor { get; set; }

    /// <summary>Primary training zone, e.g. "Zone 2", "Zone 4".</summary>
    public string Zone { get; set; } = string.Empty;
}

/// <summary>A structured weekly training plan proposed by the coaching AI.</summary>
public class TrainingPlan
{
    public Guid Id { get; init; } = Guid.NewGuid();

    /// <summary>Human-readable title, e.g. "Build Week 3 — FTP Focus".</summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>Summary rationale for this plan block.</summary>
    public string Description { get; set; } = string.Empty;

    public DateOnly WeekStarting { get; set; }

    /// <summary>Ordered list of workouts for the week (Mon → Sun).</summary>
    public List<PlannedWorkout> Workouts { get; set; } = [];

    /// <summary>Total TSS across all sessions in the week.</summary>
    public double WeeklyTss => Workouts.Sum(w => w.Tss);
}
