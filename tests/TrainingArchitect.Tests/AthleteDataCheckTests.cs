using System.Reflection;
using System.Text.Json;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TrainingArchitect.Client.Components;
using TrainingArchitect.Core.Models;

namespace TrainingArchitect.Tests;

public sealed class AthleteDataCheckTests
{
    [Theory]
    [InlineData("null", "strength-adaptation")]
    [InlineData("\"maximum\"", "strength-maximum")]
    public async Task StrengthWarning_RendersLikeOtherWarnings_WithoutMissingFeedbackFindings(
        string phase, string suggestedTag)
    {
        // MCP report for one untagged WeightTraining activity with no RPE or carbs.
        var json = $$"""
            {
              "current_date":"2026-11-02","week_starting":"2026-11-02","lookback_days":7,
              "checks":[
                {
                  "area":"activity_feedback","status":"OK",
                  "explanation":"RPE is recorded for 0/0 and carbohydrate intake for 0/0 applicable activities (strength activities excluded: 1).",
                  "evidence":{"activity_count":1,"applicable_activities":0,"strength_activities_excluded":1,"rpe_recorded":0,"carbs_recorded":0}
                },
                {
                  "area":"strength_untagged","status":"Warning",
                  "explanation":"Completed WeightTraining activities in the current or previous ISO week have no canonical strength phase tag and no planned strength event in that week: 2026-11-02 Gym session (id strength-test). Suggested tag: {{suggestedTag}}. The strength phase cannot be tracked until these sessions are tagged.",
                  "evidence":{
                    "tagging_reminder_needed":true,"current_phase":{{phase}},"suggested_tag":"{{suggestedTag}}",
                    "untagged_activities":[{"date":"2026-11-02","name":"Gym session","id":"strength-test"}]
                  },
                  "context":["This session does not advance the phase counter."]
                }
              ],
              "overall":"Partial","non_ok_count":1,
              "suggested_fix":"Tag the listed WeightTraining activities with the suggested strength phase tag in intervals.icu."
            }
            """;
        var result = JsonSerializer.Deserialize<AthleteDataCheckResult>(json)!;
        var feedback = Assert.Single(result.Checks, check => check.Area == "activity_feedback");
        Assert.Equal("OK", feedback.Status);
        Assert.Equal(0, feedback.Evidence!.Value.GetProperty("applicable_activities").GetInt32());
        Assert.Equal(1, feedback.Evidence.Value.GetProperty("strength_activities_excluded").GetInt32());

        await using var services = new ServiceCollection().AddLogging().BuildServiceProvider();
        await using var renderer = new HtmlRenderer(services, services.GetRequiredService<ILoggerFactory>());
        var html = await renderer.Dispatcher.InvokeAsync(async () =>
        {
            var component = await renderer.RenderComponentAsync<ExpandedAthleteDataCheck>(
                ParameterView.FromDictionary(new Dictionary<string, object?> { ["Result"] = result }));
            return component.ToHtmlString();
        });

        Assert.Contains("1 of 2 checks need attention", html);
        Assert.Contains("Strength tagging", html);
        Assert.Matches("class=\"data-check-status is-yellow\"[^>]*>Warning</span>", html);
        Assert.Contains("Activity feedback tracking", html);
        Assert.Matches("class=\"data-check-status is-green\"[^>]*>OK</span>", html);
        Assert.Contains("strength activities excluded: 1", html);
        Assert.Contains(suggestedTag, html);
        Assert.Contains("Gym session", html);
        Assert.Contains("This session does not advance the phase counter.", html);
        Assert.DoesNotContain(">Missing<", html);
    }

    public sealed class ExpandedAthleteDataCheck : AthleteDataCheck
    {
        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            typeof(AthleteDataCheck).GetMethod("ToggleDetails", BindingFlags.Instance | BindingFlags.NonPublic)!
                .Invoke(this, null);
        }
    }
}
