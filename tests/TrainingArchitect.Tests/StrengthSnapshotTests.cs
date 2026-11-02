using System.Net;
using System.Reflection;
using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using TrainingArchitect.Client.Components;
using TrainingArchitect.Client.Pages;
using TrainingArchitect.Client.Services;
using TrainingArchitect.Services;

namespace TrainingArchitect.Tests;

public sealed class StrengthSnapshotTests
{
    [Theory]
    [InlineData("", null, null)]
    [InlineData(",\"inferred_sessions_last_16_weeks\":null,\"phase_overdue\":null", null, null)]
    [InlineData(",\"inferred_sessions_last_16_weeks\":0,\"phase_overdue\":false", 0, false)]
    [InlineData(",\"inferred_sessions_last_16_weeks\":5,\"phase_overdue\":true", 5, true)]
    public async Task BuildStrengthSnapshot_ReadsNullableFields(
        string additionalFields, int? inferredSessions, bool? phaseOverdue)
    {
        var snapshot = await BuildSnapshotAsync("adaptation", 8, additionalFields);

        Assert.Equal(inferredSessions, snapshot.InferredSessionsLast16Weeks);
        Assert.Equal(phaseOverdue, snapshot.PhaseOverdue);
        Assert.Equal(8, snapshot.PhaseSessionCount);
        Assert.Equal(6, snapshot.PhaseTargetMax);
    }

    [Theory]
    [InlineData("adaptation", 9, 6, 6, true)]
    [InlineData("transition", 5, 3, 3, true)]
    [InlineData("maximum", 15, 12, 12, true)]
    [InlineData("adaptation", 6, 6, 6, true)]
    [InlineData("adaptation", 4, 6, 4, false)]
    [InlineData("adaptation", 9, 6, 6, false)]
    [InlineData("adaptation", 0, 6, 0, null)]
    [InlineData("adaptation", 9, 6, 6, null)]
    [InlineData("maintenance", 20, 0, 0, false)]
    public async Task StrengthCard_CapsSegmentsAndShowsExplicitOverdueFlag(
        string phase, int sessionCount, int expectedSegments, int expectedFilled, bool? overdue)
    {
        var additionalFields = overdue is null ? "" : $",\"phase_overdue\":{(overdue.Value ? "true" : "false")}";
        var snapshot = await BuildSnapshotAsync(phase, sessionCount, additionalFields);
        await using var services = new ServiceCollection().AddLogging().BuildServiceProvider();
        await using var renderer = new HtmlRenderer(services, services.GetRequiredService<ILoggerFactory>());

        var html = await renderer.Dispatcher.InvokeAsync(async () =>
        {
            var component = await renderer.RenderComponentAsync<MetricsSnapshot>(
                ParameterView.FromDictionary(new Dictionary<string, object?>
                {
                    ["Items"] = new MetricsSnapshot.MetricSnapshotItem[] { new("Test metric", "1") },
                    ["FitnessKpis"] = new MetricsSnapshot.FitnessKpiSnapshot(50, 55, -5, null, null, null),
                    ["Strength"] = snapshot
                }));
            return component.ToHtmlString();
        });

        Assert.Equal(expectedSegments, Regex.Matches(html, "class=\"coach-strength-progress-segment ").Count);
        Assert.Equal(expectedFilled, Regex.Matches(html, "class=\"coach-strength-progress-segment is-filled\"").Count);
        Assert.Equal(overdue == true, WebUtility.HtmlDecode(html).Contains("Phasenwechsel fällig", StringComparison.Ordinal));
        if (phase == "maintenance")
        {
            Assert.Contains("ongoing", html);
        }
        else
        {
            Assert.Contains($"{sessionCount} of", html);
        }
    }

    private static async Task<MetricsSnapshot.StrengthSnapshot> BuildSnapshotAsync(
        string phase, int sessionCount, string additionalFields)
    {
        var json = $$$"""
            {"week_summary":{"strength":{
              "current_phase":"{{{phase}}}","phase_session_count":{{{sessionCount}}},
              "sessions_this_week":1,"sessions_per_week_last_4":[2,2,2,2],
              "untagged_sessions_last_16_weeks":0{{{additionalFields}}}
            }
            }
            }
            """;
        var session = new AthleteSession(
            new StrengthDataClient(json), new ServerCredentialStore(), NullLogger<AthleteSession>.Instance);
        await session.ConnectAsync("__ATHLETE_ID__", "__API_KEY__", CoachLanguage.English, AthleteProfileGoal.Fitness, false);
        var coach = new Coach();
        typeof(Coach).GetProperty("Session", BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(coach, session);

        return Assert.IsType<MetricsSnapshot.StrengthSnapshot>(
            typeof(Coach).GetMethod("BuildStrengthSnapshot", BindingFlags.Instance | BindingFlags.NonPublic)!
                .Invoke(coach, null));
    }

    private sealed class StrengthDataClient(string json) : IAthleteDataClient
    {
        public Task<AthleteDataResult> FetchAsync(string athleteId, string apiKey, CancellationToken ct = default) =>
            Task.FromResult(new AthleteDataResult { DataParsed = JsonSerializer.Deserialize<JsonElement>(json), DataRaw = json });
    }
}
