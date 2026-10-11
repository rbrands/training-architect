// TrainingArchitect.Core/Models/AssessRequest.cs
namespace TrainingArchitect.Core.Models;

public enum AssessmentType { Activity, Week, Metrics }

public record AssessRequest(
    string         WeekDataJson,
    string         DisciplineType,
    string         Language,
    AssessmentType AssessmentType
);

public record AssessResponse(string Content, long? TotalTokens = null, string? ResponseId = null);