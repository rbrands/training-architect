// TrainingArchitect.Server/Services/PromptBuilder.cs
namespace TrainingArchitect.Services;
using TrainingArchitect.Core.Models;
public static class PromptBuilder
{
    public static string BuildAssessPrompt(AssessRequest request)
    {
        var template = request.AssessmentType switch
        {
            AssessmentType.Activity => PromptLoader.Load("assess_activity"),
            AssessmentType.Week => PromptLoader.Load("assess_week"),
            AssessmentType.Metrics => PromptLoader.Load("assess_metrics"),
            _ => throw new ArgumentOutOfRangeException()
        };

        return template.Replace("{{athlete_data}}", $"""
            <athlete_data>
            {request.WeekDataJson}
            </athlete_data>
            """);
    }

    public static string BuildPlanPrompt(PlanRequest request)
    {
        var template = request.Scope switch
        {
            PlanningScope.CurrentWeek => PromptLoader.Load("plan_current_week"),
            PlanningScope.NextWeek => PromptLoader.Load("plan_next_week"),
            _ => throw new ArgumentOutOfRangeException(nameof(request.Scope))
        };

        var tssLine = request.Constraints.WeeklyTssTarget.HasValue
            ? $"Weekly TSS target: {request.Constraints.WeeklyTssTarget.Value}"
            : "Weekly TSS target: use value from data";

        var dayLines = request.Constraints.DayConstraints.Count > 0
            ? string.Join("\n", request.Constraints.DayConstraints
                .Select(d => $"- {d.Day}: {d.Availability}"))
            : "- use availability from data";

        return template
            .Replace("{{weekly_tss_target}}", tssLine)
            .Replace("{{day_availability}}", dayLines)
            .Replace("{{athlete_data}}", $"""
                <athlete_data>
                {request.WeekDataJson}
                </athlete_data>
                """);
    }
}