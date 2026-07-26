namespace TrainingArchitect.Core.Models;

public record PlanResponse(string Content, long? TotalTokens = null, string? ResponseId = null);
