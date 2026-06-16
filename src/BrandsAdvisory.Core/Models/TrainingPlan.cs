namespace BrandsAdvisory.Core.Models;

/// <summary>Broad category of a planned workout.</summary>
public enum WorkoutType
{
    Endurance,
    Tempo,
    Threshold,
    VO2Max,
    Race,
    Rest
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
