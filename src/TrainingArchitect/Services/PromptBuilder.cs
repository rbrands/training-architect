// TrainingArchitect.Server/Services/PromptBuilder.cs
namespace TrainingArchitect.Services;
using System.Globalization;
using System.Text.Json;
using TrainingArchitect.Core.Models;
using TrainingArchitect.Core.Services;
public static class PromptBuilder
{
    private const string StrengthPrescription = """
        Strength prescription (use this when no exact strength workout is returned by the library):
        - adaptation: 3-5 sets of 15-20 reps at 40-60% 1RM; rest 1-1.5 minutes.
        - transition: 3-5 sets of 8-12 reps at 70-80% 1RM; rest 2-3 minutes.
        - maximum: 3-4 sets of 3-6 reps at 85-95% 1RM; rest 3-4 minutes.
        - maintenance: 2 sets, first 12 reps at 60% 1RM and second 6 reps at 85% 1RM; rest 1-2 minutes.
        Each session uses 3-4 exercises: 1-2 primary lifts (squat, deadlift, step-up, lunge, or leg press), leg curl, and core. Do not prescribe power cleans unless the athlete asks. Keep duration 30-75 minutes, activity_type "WeightTraining", tss 0, no steps, no library_workout_id, and exactly the requested strength phase tag. Write the description in the response language with phase, session number of target, exercises, sets x reps @ % 1RM, rest, and a progression note.
        """;

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

    /// <summary>Builds the prompt for weekly planning or a next-session recommendation.</summary>
    public static string BuildPlanPrompt(PlanRequest request)
    {
        if (request.Mode == PlanningMode.NextTraining)
        {
            return PromptLoader.Load("plan_next_training")
                .Replace("{{recommendation_date}}", ResolveRecommendationDateText(request.WeekDataJson))
                .Replace("{{strength_phase}}", NormalizeStrengthPhase(request.StrengthPhase))
                .Replace("{{strength_phase_selection_context}}", GetStrengthPhaseSelectionContext(request))
                .Replace("{{strength_prescription}}", StrengthPrescription)
                .Replace("{{athlete_data}}", string.IsNullOrWhiteSpace(request.WeekDataJson)
                    ? "{}"
                    : request.WeekDataJson);
        }

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
            .Replace("{{strength_phase}}", NormalizeStrengthPhase(request.StrengthPhase))
            .Replace("{{strength_phase_selection_context}}", GetStrengthPhaseSelectionContext(request))
            .Replace("{{strength_prescription}}", StrengthPrescription)
            .Replace("{{athlete_data}}", string.IsNullOrWhiteSpace(request.WeekDataJson)
                ? "{}"
                : request.WeekDataJson);
    }

    private static string GetStrengthPhaseSelectionContext(PlanRequest request)
    {
        if (NormalizeStrengthPhase(request.StrengthPhase) == "none")
        {
            return string.Empty;
        }

        return request.StrengthPhaseIsManual
            ? $"The athlete explicitly requested strength training for this plan and selected the `{NormalizeStrengthPhase(request.StrengthPhase)}` phase. This explicit request overrides the default history-based strength opt-in rule. Treat strength as in use even when strength history is missing; do not omit an eligible strength session because there is no recent history."
            : $"The selected strength phase for this plan is `{NormalizeStrengthPhase(request.StrengthPhase)}`. Treat this non-none selection as strength being in use, even when strength history is missing; do not apply the default history-based opt-in rule as a reason to omit an eligible strength session.";
    }

    private static string NormalizeStrengthPhase(string? phase) => phase?.Trim().ToLowerInvariant() switch
    {
        "adaptation" => "adaptation",
        "transition" => "transition",
        "maximum" => "maximum",
        "maintenance" => "maintenance",
        "none" => "none",
        _ => "none"
    };

    private static string ResolveRecommendationDateText(string? weekDataJson)
    {
        var resolved = RecommendationDateResolver.Resolve(weekDataJson);
        if (resolved is { } date)
            return date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);

        try
        {
            using var document = JsonDocument.Parse(weekDataJson ?? "{}");
            if (document.RootElement.ValueKind == JsonValueKind.Object
                && document.RootElement.TryGetProperty("current_date", out var currentDate)
                && currentDate.ValueKind == JsonValueKind.String
                && !string.IsNullOrWhiteSpace(currentDate.GetString()))
                return currentDate.GetString()!;
        }
        catch (JsonException)
        {
        }

        return "current_date";
    }

    /// <summary>
    /// Builds a follow-up prompt that asks the agent to fix the reported problems in its previous plan.
    /// </summary>
    public static string BuildPlanCorrectionPrompt(
        IReadOnlyList<PlanValidationFinding> findings,
        string strengthPhase)
    {
        var template = PromptLoader.Load("plan_correction");
        
        var findingList = string.Join(
            Environment.NewLine,
            findings.Select(finding => $"- [{finding.Code}] {finding.Message}"));

        return template
            .Replace("{{strength_phase}}", NormalizeStrengthPhase(strengthPhase))
            .Replace("{{strength_prescription}}", StrengthPrescription)
            .Replace("{{findings}}", findingList);
    }
}