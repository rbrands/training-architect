using System.Net;
using System.Reflection;
using System.Text.Json;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.JSInterop;
using Syncfusion.Blazor;
using TrainingArchitect.Client.Components;
using TrainingArchitect.Client.Pages;
using TrainingArchitect.Client.Services;
using TrainingArchitect.Core.Models;
using TrainingArchitect.Services;

namespace TrainingArchitect.Tests;

public sealed class Vo2MaxRepresentativenessTests
{
    [Theory]
    [InlineData("false", Vo2MaxEstimateRepresentativeness.NotRepresentative, "critical", "VO2max estimate not representative")]
    [InlineData("\"likely_understated\"", Vo2MaxEstimateRepresentativeness.LikelyUnderstated, "warning", "VO2max estimate may be understated")]
    [InlineData("true", Vo2MaxEstimateRepresentativeness.Representative, null, null)]
    [InlineData("null", null, null, null)]
    [InlineData(null, null, null, null)]
    public async Task Snapshot_ParsesAndRendersEstimateQuality(
        string? assessment, Vo2MaxEstimateRepresentativeness? expectedStatus, string? severity, string? title)
    {
        var fields = assessment is null
            ? ""
            : $"""
              ,"estimate_representative":{assessment},"p5min_pct_ftp":99.25,"power_profile_period_days":42
              """;
        var (items, quality) = await BuildSnapshotAsync(fields);

        Assert.NotNull(quality);
        Assert.Equal(expectedStatus, quality.EstimateRepresentative);
        Assert.Equal(assessment is null ? null : (double?)99.25, quality.P5minPctFtp);
        Assert.Equal(assessment is null ? null : (int?)42, quality.PeriodDays);

        var html = await RenderAsync(items, quality);
        Assert.Contains("263 W", html);
        Assert.Contains("265 W", html);

        if (severity is null)
        {
            Assert.DoesNotContain("coach-ftp-delta-notice", html);
            Assert.DoesNotContain("coach-ftp-delta-value--", html);
            Assert.DoesNotContain("coach-ftp-category-gauge-slot--muted", html);
            Assert.DoesNotContain(" (estimate)", html);
            return;
        }

        Assert.Matches($"""<dt[^>]*>VO2Max Power</dt><dd class="coach-ftp-delta-value--{severity}"[^>]*>263 W</dd>""", html);
        Assert.Contains($"coach-ftp-delta-notice--{severity}", html);
        Assert.Matches($"<strong[^>]*>{title}</strong>", html);
        Assert.Contains("role=\"status\"", html);
        Assert.Contains("coach-ftp-category-gauge-slot--muted", html);
        Assert.Contains("aria-label=\"VO2max category: Average, level 3 of 5 (estimate)\"", html);
        Assert.Contains("aria-label=\"FTP category: Recreational, level 2 of 6\"", html);

        var expectedMessage = severity == "critical"
            ? "Your best 5-minute power in the last 42 days is 99.25 % of your FTP, so it does not reflect a maximal effort. VO2max and the category are derived from it and are likely understated. Do a 5-minute test or a hard VO2max session to update them."
            : "Your best 5-minute power is only 99.25 % of your FTP; recent data probably lacks a maximal 5-minute effort. VO2max and the category may be understated.";
        Assert.Contains(expectedMessage, html);
    }

    [Theory]
    [InlineData("")]
    [InlineData(",\"power_profile_period_days\":null")]
    public async Task Snapshot_OmitsUnknownPeriod(string periodField)
    {
        var (items, quality) = await BuildSnapshotAsync(
            ",\"estimate_representative\":false,\"p5min_pct_ftp\":99.25" + periodField);

        Assert.NotNull(quality);
        Assert.Null(quality.PeriodDays);
        var html = await RenderAsync(items, quality);

        Assert.Contains("Your best 5-minute power is 99.25 % of your FTP, so it does not reflect a maximal effort.", html);
        Assert.DoesNotContain("in the last", html);
    }

    [Theory]
    [InlineData("false")]
    [InlineData("\"likely_understated\"")]
    public async Task Snapshot_HandlesUnknownPercentage(string assessment)
    {
        var (items, quality) = await BuildSnapshotAsync(
            $",\"estimate_representative\":{assessment},\"p5min_pct_ftp\":null");

        Assert.NotNull(quality);
        Assert.Null(quality.P5minPctFtp);
        var html = await RenderAsync(items, quality);

        Assert.Contains("coach-ftp-delta-notice", html);
        Assert.DoesNotContain("% of your FTP", html);
        Assert.DoesNotContain("in the last", html);
    }

    [Fact]
    public async Task Snapshot_WithoutQualityParameter_PreservesDisplay()
    {
        var (items, _) = await BuildSnapshotAsync("");
        var html = await RenderAsync(items, null);

        Assert.DoesNotContain("coach-ftp-delta-notice", html);
        Assert.DoesNotContain("coach-ftp-category-gauge-slot--muted", html);
        Assert.DoesNotContain(" (estimate)", html);
    }

    private static async Task<string> RenderAsync(
        IReadOnlyList<MetricsSnapshot.MetricSnapshotItem> items, Vo2MaxRepresentativeness? quality)
    {
        var collection = new ServiceCollection().AddLogging();
        collection.AddSyncfusionBlazor();
        collection.AddSingleton<IJSRuntime, PrerenderJsRuntime>();
        collection.AddSingleton<NavigationManager, TestNavigationManager>();
        await using var services = collection.BuildServiceProvider();
        await using var renderer = new HtmlRenderer(services, services.GetRequiredService<ILoggerFactory>());

        return await renderer.Dispatcher.InvokeAsync(async () =>
        {
            var component = await renderer.RenderComponentAsync<MetricsSnapshot>(
                ParameterView.FromDictionary(new Dictionary<string, object?>
                {
                    [nameof(MetricsSnapshot.Items)] = items,
                    [nameof(MetricsSnapshot.Vo2MaxRepresentativeness)] = quality
                }));
            return WebUtility.HtmlDecode(component.ToHtmlString());
        });
    }

    private static async Task<(IReadOnlyList<MetricsSnapshot.MetricSnapshotItem> Items, Vo2MaxRepresentativeness? Quality)>
        BuildSnapshotAsync(string fields)
    {
        var json = $$$"""
            {"metrics":{
              "ftp":265,
              "ftp_classification":{"category":"recreational"},
              "power_profile":{"p5min":{"watts":263}},
              "vo2max_classification":{"category":"average","ml_per_kg_min":42{{{fields}}}
              }
            }}
            """;
        var session = new AthleteSession(
            new MetricsDataClient(json), new ServerCredentialStore(), NullLogger<AthleteSession>.Instance);
        await session.ConnectAsync("__ATHLETE_ID__", "__API_KEY__", CoachLanguage.English, AthleteProfileGoal.Fitness, false);
        var coach = new Coach();
        typeof(Coach).GetProperty("Session", BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(coach, session);

        var items = Assert.IsAssignableFrom<IReadOnlyList<MetricsSnapshot.MetricSnapshotItem>>(
            typeof(Coach).GetMethod("BuildMetricsSnapshot", BindingFlags.Instance | BindingFlags.NonPublic)!
                .Invoke(coach, null));
        var quality = (Vo2MaxRepresentativeness?)typeof(Coach)
            .GetMethod("BuildVo2MaxRepresentativeness", BindingFlags.Instance | BindingFlags.NonPublic)!
            .Invoke(coach, null);
        return (items, quality);
    }

    private sealed class MetricsDataClient(string json) : IAthleteDataClient
    {
        public Task<AthleteDataResult> FetchAsync(string athleteId, string apiKey, CancellationToken ct = default) =>
            Task.FromResult(new AthleteDataResult { DataParsed = JsonSerializer.Deserialize<JsonElement>(json), DataRaw = json });
    }

    private sealed class PrerenderJsRuntime : IJSRuntime
    {
        public ValueTask<TValue> InvokeAsync<TValue>(string identifier, object?[]? args) =>
            throw new InvalidOperationException("JavaScript must not be invoked during static rendering.");

        public ValueTask<TValue> InvokeAsync<TValue>(string identifier, CancellationToken cancellationToken, object?[]? args) =>
            InvokeAsync<TValue>(identifier, args);
    }

    private sealed class TestNavigationManager : NavigationManager
    {
        public TestNavigationManager() => Initialize("https://localhost/", "https://localhost/coach");

        protected override void NavigateToCore(string uri, bool forceLoad) =>
            throw new InvalidOperationException("Navigation must not occur during snapshot rendering.");
    }
}
