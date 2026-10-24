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
        Assert.Contains("The only tool you may call", prompt);
        Assert.Contains("even if the attached data is incomplete or\noutdated", prompt.Replace("\r\n", "\n"));
        Assert.DoesNotContain("Fetch the current data from intervals.icu", prompt);
        Assert.Contains("exactly one workout", prompt);
        Assert.Contains("At most 100 words", prompt);
        Assert.Contains("BEGIN_UPLOAD_JSON / END_UPLOAD_JSON", prompt);
        Assert.Contains("prose-only rule", prompt);
        Assert.Contains("Validation and upload are performed by", prompt);
        Assert.Contains("Do not describe or simulate an upload", prompt);
        Assert.DoesNotContain("VO2max on Tuesday", prompt);
        Assert.DoesNotContain("Plan target: next week", prompt);
    }

    [Fact]
    public void BuildPlanPrompt_NextTraining_UsesApplicationDateAndPreliminaryContext()
    {
        var request = new PlanRequest("{}", "fitness", "en", PlanningScope.CurrentWeek,
            new PlanConstraints(null, []), Mode: PlanningMode.NextTraining);

        var prompt = PromptBuilder.BuildPlanPrompt(request);

        Assert.Contains("The recommendation date is determined by the application", prompt);
        Assert.Contains("Do not derive it yourself", prompt);
        Assert.Contains("for the recommendation date current_date", prompt);
        Assert.Contains("tomorrow is in a new week", prompt);
        Assert.Contains("mark the recommendation as preliminary", prompt);
        Assert.Contains("ARE the measured readiness for the", prompt);
        Assert.Contains("today's readiness is then a proxy", prompt);
        Assert.Contains("means \"no hard training\", not \"no training\"", prompt);
        Assert.Contains("it is NOT missing key data", prompt);
        Assert.Contains("instead of asking", prompt);
        Assert.Contains("training_allowed: false", prompt);
        Assert.Contains("max_training_time_hours", prompt);
    }

    [Theory]
    [InlineData("{\"current_date\":\"2026-10-18\",\"activities\":[{\"date\":\"2026-10-18\"}]}", "2026-10-19")]
    [InlineData("{\"current_date\":\"2026-10-18\"}", "2026-10-18")]
    [InlineData("{\"current_date\":\"unknown\"}", "unknown")]
    [InlineData("{}", "current_date")]
    [InlineData("not JSON", "current_date")]
    public void BuildPlanPrompt_NextTraining_ReplacesRecommendationDate(string json, string expected)
    {
        var request = new PlanRequest(json, "fitness", "en", PlanningScope.CurrentWeek,
            new PlanConstraints(null, []), Mode: PlanningMode.NextTraining);

        var prompt = PromptBuilder.BuildPlanPrompt(request);

        Assert.Contains($"for the recommendation date {expected}", prompt);
        Assert.Contains($"Evaluate the plan, week target, and constraints of {expected}", prompt);
        Assert.Contains(json, prompt);
        Assert.DoesNotContain("{{recommendation_date}}", prompt);
    }
}
