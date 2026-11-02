using TrainingArchitect.Core.Models;
using TrainingArchitect.Services;

namespace TrainingArchitect.Tests;

public sealed class PromptBuilderTests
{
    [Theory]
    [InlineData("\"adaptation\"", "activity", "6")]
    [InlineData("\"transition\"", "planned", "null")]
    [InlineData("\"maximum\"", "planned_week", "8")]
    [InlineData("\"adaptation\"", "inferred", "null")]
    [InlineData("null", "none", "null")]
    public void BuildAssessPrompt_Strength_UsesActivityPhaseAndSource(string phase, string source, string rpe)
    {
        var athleteData = $$$"""
            {
              "current_date":"2026-11-02",
              "activities":[
                {"date":"2026-11-01","type":"Ride","ctl_delta":-0.2,"atl_delta":-1.4},
                {"date":"2026-11-02","type":"WeightTraining","tags":[],"duration_hours":0.75,
                 "strength_phase":{{{phase}}},"strength_phase_source":"{{{source}}}","rpe":{{{rpe}}}}
              ],
              "week_summary":{"strength":{
                "current_phase":"maintenance","phase_session_count":4,"phase_target_sessions":"4-6",
                "sessions_this_week":1,"days_since_last_strength":0
              }},
              "planned_workouts":{"current_week":{"planned_workouts":[
                {"date":"2026-11-03","activity_type":"Ride","name":"Hard ride"}
              ]}}
            }
            """;

        var prompt = PromptBuilder.BuildAssessPrompt(
            new AssessRequest(athleteData, "fitness", "en", AssessmentType.Activity));
        var strengthSection = prompt[..prompt.IndexOf("## Other activities", StringComparison.Ordinal)];

        Assert.Contains(athleteData, prompt);
        Assert.DoesNotContain("{{athlete_data}}", prompt);
        Assert.Contains("at most 80 words", strengthSection);
        Assert.Contains("instead of the five questions below", strengthSection);
        Assert.Contains("`strength_phase` and `strength_phase_source`", strengthSection);
        Assert.Contains("latest entry in `activities[]`, not the summary's `current_phase`", strengthSection);
        Assert.Contains("`phase_session_count` and", strengthSection);
        Assert.Contains("`phase_target_sessions`; do not increment the exported count", strengthSection);
        Assert.Contains("Source `activity`, `planned`, or `planned_week`", strengthSection);
        Assert.Contains("\"<Phase>, session n of <target>\"", strengthSection);
        Assert.Contains("\"<Phase>, session n of <target> (inferred, no tag)\"", strengthSection);
        Assert.Contains("Source `none`: \"phase unknown, not counted\"", strengthSection);
        Assert.Contains("Do not mention tagging otherwise; the data quality check handles the reminder", strengthSection);
        Assert.DoesNotContain("strength-<current_phase>", strengthSection);
        Assert.DoesNotContain("working assumption", strengthSection);
        Assert.Contains("only if `rpe` is present and non-null", strengthSection);
        Assert.Contains("Never ask for missing RPE", strengthSection);
        Assert.Contains("check `planned_workouts`", strengthSection);
        Assert.Contains("a hard ride or race tomorrow", strengthSection);
        Assert.Contains("twice per week; maintenance once per week", strengthSection);
        Assert.Contains("today/tomorrow and never reduces the weekly strength frequency", strengthSection);
        Assert.DoesNotContain("once on yellow readiness", strengthSection);
        Assert.DoesNotContain("none on red", strengthSection);
        Assert.Contains("at least two", strengthSection);
        Assert.Contains("Do not discuss fueling, pacing, decoupling, W'", strengthSection);
        Assert.Contains("or CTL/ATL/form for strength sessions", strengthSection);
        Assert.Contains("Stop here for WeightTraining", strengthSection);
    }

    [Theory]
    [InlineData("assess_activity")]
    [InlineData("assess_week")]
    [InlineData("assess_metrics")]
    [InlineData("plan_week")]
    [InlineData("plan_next_training")]
    public void StrengthPrompts_KeepWeeklyFrequencyIndependentOfDailyReadiness(string name)
    {
        var prompt = PromptLoader.Load(name).Replace("\r\n", "\n");
        var normalized = System.Text.RegularExpressions.Regex.Replace(prompt, @"\s+", " ");

        Assert.Contains("today/tomorrow", normalized);
        Assert.Contains("never reduces the weekly strength frequency", normalized);
        Assert.Contains("adaptation/transition/maximum twice per week", normalized);
        Assert.Contains("maintenance once per week", normalized);
        Assert.Contains("at least two days between", normalized);
        Assert.DoesNotMatch(@"(?i)(once|1) (per week |session )?(on|when).*yellow", normalized);
        Assert.DoesNotMatch(@"(?i)(none|0) (on|when).*red", normalized);
    }

    [Fact]
    public void BuildAssessPrompt_Ride_PreservesQuestionsAndExplainsLoadDeltaTrajectory()
    {
        const string athleteData = """
            {"activities":[{"date":"2026-11-02","type":"Ride","ctl_delta":-0.2,"atl_delta":-1.4}]}
            """;

        var prompt = PromptBuilder.BuildAssessPrompt(
            new AssessRequest(athleteData, "roadrace", "en", AssessmentType.Activity));
        var rideSection = prompt[prompt.IndexOf("## Other activities", StringComparison.Ordinal)..];

        Assert.Contains(athleteData, prompt);
        Assert.Contains("1. What was the training quality", rideSection);
        Assert.Contains("2. How was load management", rideSection);
        Assert.Contains("`dfa_a1_summary`", rideSection);
        Assert.Contains("3. What does the fueling tell us", rideSection);
        Assert.Contains("4. What is the CTL, ATL, and form trajectory since the previous exported activity?", rideSection);
        Assert.Contains("include rest-day decay", rideSection);
        Assert.Contains("never describe them as the effect of this session", rideSection);
        Assert.Contains("5. What do I take away", rideSection);
    }

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
    public void BuildPlanPrompt_NextTraining_IncludesDataAndOptionalStrengthContract()
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
        Assert.Contains("one main workout with an optional same-day strength", prompt);
        Assert.Contains("the last strength session was at least two days", prompt);
        Assert.Contains("yellow readiness rules out `strength-maximum`", prompt);
        Assert.Contains("red readiness rules out all strength", prompt);
        Assert.Contains("easier\n  alternative stays in prose only and is never a second upload entry", prompt.Replace("\r\n", "\n"));
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
