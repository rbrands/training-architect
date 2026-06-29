// TrainingArchitect.Core/Models/PlanRequest.cs
namespace TrainingArchitect.Core.Models;

public enum PlanningScope { CurrentWeek, NextWeek }

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
    PlanConstraints Constraints
);