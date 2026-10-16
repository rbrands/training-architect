using TrainingArchitect.Core.Models;
using TrainingArchitect.Services;

namespace TrainingArchitect.Tests;

public sealed class PromptBuilderTests
{
    [Fact]
    public void BuildAssessPrompt_Metrics_UsesMetricsPrompt()
    {
        var request = new AssessRequest("{\"ftp\":250}", "road race", "en", AssessmentType.Metrics);

        var prompt = PromptBuilder.BuildAssessPrompt(request);

        Assert.Contains("Summarize the current metrics and wellness data.", prompt);
        Assert.Contains("{\"ftp\":250}", prompt);
    }

    [Fact]
    public void BuildPlanPrompt_DefaultMode_PreservesWeeklyPlanning()
    {
        var request = new PlanRequest("{}", "fitness", "en", PlanningScope.NextWeek,
            new PlanConstraints(null, []), "VO2max on Tuesday");

        var prompt = PromptBuilder.BuildPlanPrompt(request);

        Assert.Contains("Plan target: next week", prompt);
        Assert.Contains("VO2max on Tuesday", prompt);
        Assert.DoesNotContain("exactly one workout", prompt);
    }

    [Fact]
    public void BuildPlanPrompt_NextTraining_IncludesDataAndSingleSessionContract()
    {
        const string athleteData = """{"current_date":"2026-10-18","activities":[{"start_date_local":"2026-10-18T09:00:00"}]}""";
        var request = new PlanRequest(athleteData, "fitness", "en", PlanningScope.NextWeek,
            new PlanConstraints(500, []), "VO2max on Tuesday", PlanningMode.NextTraining);

        var prompt = PromptBuilder.BuildPlanPrompt(request);

        Assert.Contains(athleteData, prompt);
        Assert.Contains("Use only the attached athlete data", prompt);
        Assert.Contains("Never call prepare_week_data", prompt);
        Assert.Contains("even if the attached data is incomplete or outdated", prompt);
        Assert.DoesNotContain("Fetch the current data from intervals.icu", prompt);
        Assert.Contains("exactly one workout", prompt);
        Assert.Contains("At most 100 words", prompt);
        Assert.Contains("BEGIN_UPLOAD_JSON / END_UPLOAD_JSON", prompt);
        Assert.Contains("prose-only rule", prompt);
        Assert.Contains("Upload only if validation succeeds", prompt);
        Assert.Contains("Do not upload automatically or ask for confirmation now", prompt);
        Assert.DoesNotContain("VO2max on Tuesday", prompt);
        Assert.DoesNotContain("Plan target: next week", prompt);
    }

    [Fact]
    public void BuildPlanPrompt_NextTraining_UsesAthleteDateAndTomorrowContext()
    {
        var request = new PlanRequest("{}", "fitness", "en", PlanningScope.CurrentWeek,
            new PlanConstraints(null, []), Mode: PlanningMode.NextTraining);

        var prompt = PromptBuilder.BuildPlanPrompt(request);

        Assert.Contains("Use the athlete data's `current_date` for today", prompt);
        Assert.Contains("add one\ncalendar day", prompt);
        Assert.Contains("tomorrow is in a new week", prompt);
        Assert.Contains("mark it as preliminary", prompt);
        Assert.Contains("Do not treat today's readiness as tomorrow's measured readiness", prompt);
        Assert.Contains("recommend conservatively instead of asking", prompt);
        Assert.Contains("training_allowed: false", prompt);
        Assert.Contains("max_training_time_hours", prompt);
    }
}
