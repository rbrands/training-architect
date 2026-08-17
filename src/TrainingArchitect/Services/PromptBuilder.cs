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
        var template = PromptLoader.Load("plan_week");

        var planningScopeInstruction = request.Scope switch
        {
            PlanningScope.CurrentWeek => "Plan target: current week (remaining days in this week).",
            PlanningScope.NextWeek => "Plan target: next week (full next calendar week).",
            _ => throw new ArgumentOutOfRangeException(nameof(request.Scope))
        };

        var schedulingPreference = string.IsNullOrWhiteSpace(request.SchedulingPreference)
            ? string.Empty
            : request.SchedulingPreference.Trim();

        return template
            .Replace("{{planning_scope_instruction}}", planningScopeInstruction)
            .Replace("{{scheduling_preference}}", schedulingPreference)
            .Replace("{{athlete_data}}", string.IsNullOrWhiteSpace(request.WeekDataJson)
                ? "{}"
                : request.WeekDataJson);
    }

    /// <summary>
    /// Builds a follow-up prompt that asks the agent to fix the reported problems in its previous plan.
    /// </summary>
    public static string BuildPlanCorrectionPrompt(IReadOnlyList<PlanValidationFinding> findings)
    {
        var findingList = string.Join(
            Environment.NewLine,
            findings.Select(finding => $"- [{finding.Code}] {finding.Message}"));

        return $"""
            The plan you just produced did not pass validation. Fix the following problems:

            {findingList}

            Return the complete corrected plan again in the exact same format as before, including the
            readable plan text and the upload JSON between BEGIN_UPLOAD_JSON and END_UPLOAD_JSON.
            Do not comment on the corrections, only return the corrected plan.
            """;
    }
}