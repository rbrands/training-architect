// TrainingArchitect.Core/Models/PlanRequest.cs
namespace TrainingArchitect.Core.Models;

public enum PlanningScope { CurrentWeek, NextWeek }

/// <summary>Identifies whether to plan a week or recommend the next training session.</summary>
public enum PlanningMode { Week, NextTraining }

public enum DayAvailability { Available, Limited, Unavailable, Race }

public record DayConstraint(DayOfWeek Day, DayAvailability Availability);

public record PlanConstraints(
    int?                         WeeklyTssTarget,
    IReadOnlyList<DayConstraint> DayConstraints
);

public record PlanRequest(
    string          WeekDataJson,
    string          DisciplineType,
    string          Language,
    PlanningScope   Scope,
    PlanConstraints Constraints,
    string?         SchedulingPreference = null,
    PlanningMode    Mode = PlanningMode.Week
);