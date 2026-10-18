using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.Extensions.Logging.Abstractions;
using TrainingArchitect.Client.Components;
using TrainingArchitect.Client.Pages;
using TrainingArchitect.Client.Services;
using TrainingArchitect.Core.Models;
using TrainingArchitect.Core.Services;
using TrainingArchitect.Services;

namespace TrainingArchitect.Tests;

public class WeeklyTargetOverrideTests
{
    private static readonly DateOnly CurrentWeek = new(2026, 10, 12);
    private const string Dataset = """
        {
          "week_starting": "2026-10-12",
          "unknown": { "keep": true },
          "week_summary": {
            "training_plan": [
              { "week": "2026-10-12", "phase": "Base", "weekly_load_target": 320,
                "weekly_time_target_hours": 8.5, "week_type": "NORMAL", "week_note": "Base week",
                "day_constraints": [{ "day": "Monday", "hours": 1 }], "extra": 42 },
              { "week": "2026-10-19", "phase": "Peak", "weekly_load_target": 400, "week_type": "RACE", "week_note": "Race week" }
            ]
          }
        }
        """;

    [Theory]
    [InlineData(TrainingPhase.Base, 10, 484)]
    [InlineData(TrainingPhase.Base, 8.5, 411)]
    [InlineData(TrainingPhase.Build, 10, 525)]
    [InlineData(TrainingPhase.Peak, 10, 560)]
    [InlineData(TrainingPhase.Transition, 10, 400)]
    [InlineData(TrainingPhase.Base, 0, 0)]
    public void Estimate_UsesPhaseAndHours(TrainingPhase phase, double hours, int expected) =>
        Assert.Equal(expected, WeeklyTssEstimator.Estimate(phase, hours));

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Estimate_RecoveryDoesNotChangeRate(bool recovery)
    {
        var target = new WeekTargetOverride(CurrentWeek, TrainingPhase.Base, recovery, 10, null);
        Assert.Equal(484, WeeklyTssEstimator.Estimate(target.Phase, target.WeeklyHours!.Value));
    }

    [Theory]
    [InlineData("bUiLd", TrainingPhase.Build)]
    [InlineData("PEAK", TrainingPhase.Peak)]
    [InlineData("transition", TrainingPhase.Transition)]
    [InlineData(null, TrainingPhase.Base)]
    [InlineData("unknown", TrainingPhase.Base)]
    [InlineData("1", TrainingPhase.Base)]
    public void ParsePhase_UsesKnownLabelsOnly(string? label, TrainingPhase expected) =>
        Assert.Equal(expected, TrainingPlanOverrideApplier.ParsePhase(label));

    [Fact]
    public void Read_PreservesExistingTargetsWithoutEstimation()
    {
        var values = TrainingPlanOverrideApplier.Read(Dataset, CurrentWeek);
        Assert.Equal(new WeekTargetValues(TrainingPhase.Base, "NORMAL", 8.5, 320), values);
        Assert.Equal(new WeekTargetValues(TrainingPhase.Base, null, null, null),
            TrainingPlanOverrideApplier.Read("{}", CurrentWeek));
        Assert.Equal(CurrentWeek, TrainingPlanOverrideApplier.ReadCurrentWeekStart(Dataset));
    }

    [Fact]
    public void Apply_UpdatesExistingEntryAndPreservesUnrelatedData()
    {
        var result = JsonNode.Parse(TrainingPlanOverrideApplier.Apply(Dataset,
            [new(CurrentWeek, TrainingPhase.Build, false, 10, 525)]))!;
        var original = JsonNode.Parse(Dataset)!;
        var entry = result["week_summary"]!["training_plan"]![0]!;
        Assert.Equal("Build", entry["phase"]!.GetValue<string>());
        Assert.Equal(10, entry["weekly_time_target_hours"]!.GetValue<double>());
        Assert.Equal(525, entry["weekly_load_target"]!.GetValue<int>());
        Assert.Equal("Base week", entry["week_note"]!.GetValue<string>());
        Assert.True(JsonNode.DeepEquals(original["unknown"], result["unknown"]));
        Assert.True(JsonNode.DeepEquals(original["week_summary"]!["training_plan"]![0]!["day_constraints"], entry["day_constraints"]));
        Assert.Equal(42, entry["extra"]!.GetValue<int>());
        Assert.True(JsonNode.DeepEquals(original["week_summary"]!["training_plan"]![1], result["week_summary"]!["training_plan"]![1]));
    }

    [Theory]
    [InlineData("{}")]
    [InlineData("{\"week_summary\":{}}")]
    [InlineData("{\"week_summary\":{\"training_plan\":[]}}")]
    public void Apply_CreatesMissingSummaryPlanAndEntry(string json)
    {
        var result = JsonNode.Parse(TrainingPlanOverrideApplier.Apply(json,
            [new(CurrentWeek, TrainingPhase.Base, true, 8.5, 411)]))!;
        var entry = Assert.Single(result["week_summary"]!["training_plan"]!.AsArray())!;
        Assert.Equal("2026-10-12", entry["week"]!.GetValue<string>());
        Assert.Equal("RECOVERY", entry["week_type"]!.GetValue<string>());
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Apply_PreservesRaceClassificationAndNote(bool recovery)
    {
        var result = JsonNode.Parse(TrainingPlanOverrideApplier.Apply(Dataset,
            [new(CurrentWeek.AddDays(7), TrainingPhase.Build, recovery, 10, 525)]))!;
        var entry = result["week_summary"]!["training_plan"]![1]!;
        Assert.Equal("RACE", entry["week_type"]!.GetValue<string>());
        Assert.Equal("Race week", entry["week_note"]!.GetValue<string>());
        Assert.Equal(525, entry["weekly_load_target"]!.GetValue<int>());
    }

    [Theory]
    [InlineData("NORMAL", true, "RECOVERY")]
    [InlineData("RECOVERY", false, "NORMAL")]
    public void Apply_RemovesNoteWhenClassificationChanges(string originalType, bool recovery, string expectedType)
    {
        var source = JsonNode.Parse(Dataset)!;
        source["week_summary"]!["training_plan"]![0]!["week_type"] = originalType;
        var result = JsonNode.Parse(TrainingPlanOverrideApplier.Apply(source.ToJsonString(),
            [new(CurrentWeek, TrainingPhase.Base, recovery, null, null)]))!;
        var entry = result["week_summary"]!["training_plan"]![0]!.AsObject();
        Assert.Equal(expectedType, entry["week_type"]!.GetValue<string>());
        Assert.False(entry.ContainsKey("week_note"));
        Assert.False(entry.ContainsKey("weekly_load_target"));
        Assert.False(entry.ContainsKey("weekly_time_target_hours"));
    }

    [Theory]
    [InlineData("")]
    [InlineData("not JSON")]
    [InlineData("{")]
    [InlineData("null")]
    [InlineData("[]")]
    public void Apply_InvalidOrEmptyDatasetIsUnchanged(string json) =>
        Assert.Equal(json, TrainingPlanOverrideApplier.Apply(json,
            [new(CurrentWeek, TrainingPhase.Base, false, 10, 484)]));

    [Fact]
    public void Apply_NoEditsReturnsOriginalText() =>
        Assert.Equal(Dataset, TrainingPlanOverrideApplier.Apply(Dataset, []));

    [Fact]
    public void Apply_PhaseOnlyPreservesTargetsAndDayConstraints()
    {
        var original = JsonNode.Parse(Dataset)!["week_summary"]!["training_plan"]![0]!;
        var result = JsonNode.Parse(TrainingPlanOverrideApplier.Apply(Dataset,
            [new(CurrentWeek, TrainingPhase.Transition, true, null, null, UpdateTargets: false)]))!["week_summary"]!["training_plan"]![0]!;

        Assert.Equal("Transition", result["phase"]!.GetValue<string>());
        Assert.Equal("RECOVERY", result["week_type"]!.GetValue<string>());
        foreach (var property in new[] { "weekly_time_target_hours", "weekly_load_target", "day_constraints" })
            Assert.True(JsonNode.DeepEquals(original[property], result[property]));
    }

    [Theory]
    [InlineData(PlanningScope.CurrentWeek, 525)]
    [InlineData(PlanningScope.NextWeek, 411)]
    public void Resolver_UsesOverriddenTargetsForBothScopes(PlanningScope scope, double expected)
    {
        var result = TrainingPlanOverrideApplier.Apply(Dataset,
            [new(CurrentWeek, TrainingPhase.Build, false, 10, 525),
             new(CurrentWeek.AddDays(7), TrainingPhase.Base, false, 8.5, 411)]);
        Assert.Equal(expected, WeeklyLoadTargetResolver.Resolve(result, scope));
    }

    [Theory]
    [InlineData(PlanningScope.CurrentWeek, 8.5)]
    [InlineData(PlanningScope.NextWeek, null)]
    public void TimeTargetResolver_UsesTargetWeek(PlanningScope scope, double? expected) =>
        Assert.Equal(expected, WeeklyTimeTargetResolver.Resolve(Dataset, scope));

    [Fact]
    public async Task Editor_OverridesBelongToPageStateAndAreNotStoredInSession()
    {
        var (session, _) = await CreateSessionAsync();
        var editor = CreateEditor(session);
        var target = GetTargets(editor)[0];
        Assert.Equal(320, target.WeeklyTss);
        Assert.Equal(8.5, target.WeeklyHours);
        Assert.Empty(GetEditorOverrides(editor));

        Edit(editor, target, target with { WeeklyTss = 600 });
        var navigatedEditor = CreateEditor(session);
        Assert.Equal(600, GetTargets(editor)[0].WeeklyTss);
        Assert.Equal(320, GetTargets(navigatedEditor)[0].WeeklyTss);
        Assert.Single(GetEditorOverrides(editor));
        Assert.Empty(GetEditorOverrides(navigatedEditor));
        Assert.Equal(Dataset, session.AthleteDataJson);
    }

    [Theory]
    [InlineData(PlanningScope.CurrentWeek, 0, 320)]
    [InlineData(PlanningScope.NextWeek, 7, 400)]
    public async Task Editor_LoadsOnlyWeekInPlanningFocus(PlanningScope scope, int offsetDays, int expectedTss)
    {
        var (session, _) = await CreateSessionAsync();
        var editor = CreateEditor(session, scope);
        var target = Assert.Single(GetTargets(editor));

        Assert.Equal(CurrentWeek.AddDays(offsetDays), target.WeekStart);
        Assert.Equal(expectedTss, target.WeeklyTss);
        Assert.Empty(GetEditorOverrides(editor));
    }

    [Fact]
    public async Task Editor_FocusChangeReloadsVisibleWeekAndPreservesEdits()
    {
        var (session, _) = await CreateSessionAsync();
        var editor = CreateEditor(session);
        var target = Assert.Single(GetTargets(editor));
        Edit(editor, target, target with { WeeklyTss = 600 });

        typeof(WeekTargetsEditor).GetProperty(nameof(WeekTargetsEditor.Scope))!.SetValue(editor, PlanningScope.NextWeek);
        typeof(WeekTargetsEditor).GetMethod("OnParametersSet", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(editor, null);
        Assert.Equal(CurrentWeek.AddDays(7), Assert.Single(GetTargets(editor)).WeekStart);

        typeof(WeekTargetsEditor).GetProperty(nameof(WeekTargetsEditor.Scope))!.SetValue(editor, PlanningScope.CurrentWeek);
        typeof(WeekTargetsEditor).GetMethod("OnParametersSet", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(editor, null);
        Assert.Equal(600, Assert.Single(GetTargets(editor)).WeeklyTss);
        Assert.Single(GetEditorOverrides(editor));
    }

    [Fact]
    public async Task Editor_RecalculatesOnlyForChangedPhaseOrNonemptyHours()
    {
        var (session, _) = await CreateSessionAsync();
        var editor = CreateEditor(session);
        var target = GetTargets(editor)[0];
        Edit(editor, target, target, true);
        Assert.Empty(GetEditorOverrides(editor));

        Edit(editor, target, target with { WeeklyTss = 600 });
        target = GetTargets(editor)[0];
        Edit(editor, target, target with { IsRecovery = true });
        target = GetTargets(editor)[0];
        Assert.Equal(600, target.WeeklyTss);

        Edit(editor, target, target with { Phase = TrainingPhase.Build }, true);
        target = GetTargets(editor)[0];
        Assert.Equal(446, target.WeeklyTss);

        Edit(editor, target, target with { WeeklyHours = 10 }, true);
        target = GetTargets(editor)[0];
        Assert.Equal(525, target.WeeklyTss);

        Edit(editor, target, target with { WeeklyHours = null }, true);
        target = GetTargets(editor)[0];
        Edit(editor, target, target with { Phase = TrainingPhase.Peak }, true);
        target = GetTargets(editor)[0];
        Assert.Equal(525, target.WeeklyTss);
        Edit(editor, target, target with { WeeklyTss = null });
        Assert.Null(GetTargets(editor)[0].WeeklyTss);
    }

    [Fact]
    public async Task Editor_DisabledIgnoresChanges()
    {
        var (session, _) = await CreateSessionAsync();
        var editor = CreateEditor(session);
        typeof(WeekTargetsEditor).GetProperty(nameof(WeekTargetsEditor.Disabled))!.SetValue(editor, true);
        var target = GetTargets(editor)[0];
        Edit(editor, target, target with { WeeklyHours = 10 }, true);
        Assert.Empty(GetEditorOverrides(editor));
    }

    [Fact]
    public async Task Page_RefreshRetainsRelevantWeeksAndPrunesExpiredOverrides()
    {
        var (session, client) = await CreateSessionAsync();
        var coach = CreateCoach(session);
        var overrides = GetPageOverrides(coach);
        overrides[CurrentWeek] = new(CurrentWeek, TrainingPhase.Build, false, 10, 525);
        var nextWeek = CurrentWeek.AddDays(7);
        overrides[nextWeek] = new(nextWeek, TrainingPhase.Peak, false, 10, 560);
        await session.RefreshAsync();
        SynchronizeOverrides(coach);
        Assert.Equal(2, overrides.Count);

        var refreshed = JsonNode.Parse(Dataset)!;
        refreshed["week_starting"] = "2026-10-19";
        client.Json = refreshed.ToJsonString();
        await session.RefreshAsync();
        SynchronizeOverrides(coach);
        Assert.Equal(nextWeek, Assert.Single(overrides).Key);

        client.Json = "{}";
        await session.RefreshAsync();
        SynchronizeOverrides(coach);
        Assert.Empty(overrides);
    }

    [Fact]
    public async Task Page_ChangingAthleteAndDisconnectingClearOverrides()
    {
        var (session, _) = await CreateSessionAsync();
        var coach = CreateCoach(session);
        var overrides = GetPageOverrides(coach);
        overrides[CurrentWeek] = new(CurrentWeek, TrainingPhase.Build, false, 10, 525);
        await session.ConnectAsync("__OTHER_ATHLETE_ID__", "__API_KEY__", CoachLanguage.English, AthleteProfileGoal.Fitness, false);
        SynchronizeOverrides(coach);
        Assert.Empty(overrides);

        overrides[CurrentWeek] = new(CurrentWeek, TrainingPhase.Build, false, 10, 525);
        await session.DisconnectAsync();
        SynchronizeOverrides(coach);
        Assert.Empty(overrides);
    }

    [Fact]
    public async Task CoachRequests_UseSameOverridesWithoutMutatingRawDataOrConstraints()
    {
        var (session, _) = await CreateSessionAsync();
        var coach = CreateCoach(session);
        var overrides = GetPageOverrides(coach);
        overrides[CurrentWeek] = new(CurrentWeek, TrainingPhase.Build, true, 10, 525);
        overrides[CurrentWeek.AddDays(7)] = new(CurrentWeek.AddDays(7), TrainingPhase.Peak, false, 10, 560);
        var plan = (PlanRequest)typeof(Coach).GetMethod("BuildPlanRequest", BindingFlags.Instance | BindingFlags.NonPublic)!
            .Invoke(coach, null)!;
        var buildAssess = typeof(Coach).GetMethod("BuildAssessRequest", BindingFlags.Instance | BindingFlags.NonPublic)!;
        var cardType = Enum.Parse(buildAssess.GetParameters()[0].ParameterType, "Week");
        var assessment = (AssessRequest)buildAssess.Invoke(coach, [cardType])!;

        Assert.Equal(plan.WeekDataJson, assessment.WeekDataJson);
        Assert.Equal(525, WeeklyLoadTargetResolver.Resolve(plan.WeekDataJson, PlanningScope.CurrentWeek));
        Assert.Equal(560, WeeklyLoadTargetResolver.Resolve(plan.WeekDataJson, PlanningScope.NextWeek));
        Assert.Null(plan.Constraints.WeeklyTssTarget);
        Assert.Equal(Dataset, session.AthleteDataJson);
        Assert.Equal(Dataset, session.AthleteDataRaw);

        var displayedJson = (string)typeof(Coach).GetProperty("DataParsedPrettyJson", BindingFlags.Instance | BindingFlags.NonPublic)!
            .GetValue(coach)!;
        var overview = (JsonElement)typeof(Coach).GetMethod("GetOverviewRoot", BindingFlags.Instance | BindingFlags.NonPublic)!
            .Invoke(coach, null)!;
        var rawJson = (string)typeof(Coach).GetProperty("DataRawPrettyJson", BindingFlags.Instance | BindingFlags.NonPublic)!
            .GetValue(coach)!;

        Assert.True(JsonNode.DeepEquals(JsonNode.Parse(plan.WeekDataJson), JsonNode.Parse(displayedJson)));
        Assert.True(JsonNode.DeepEquals(JsonNode.Parse(plan.WeekDataJson), JsonNode.Parse(overview.GetRawText())));
        Assert.True(JsonNode.DeepEquals(JsonNode.Parse(Dataset), JsonNode.Parse(rawJson)));
    }

    [Theory]
    [InlineData(PlanningMode.Week, true)]
    [InlineData(PlanningMode.Week, false)]
    [InlineData(PlanningMode.NextTraining, true)]
    [InlineData(PlanningMode.NextTraining, false)]
    public async Task Page_TargetChangesInvalidateAssessmentsAndUploadablePlans(PlanningMode mode, bool updateTargets)
    {
        var (session, _) = await CreateSessionAsync();
        var coach = CreateCoach(session);
        var coachType = typeof(Coach);
        const BindingFlags privateInstance = BindingFlags.Instance | BindingFlags.NonPublic;
        coachType.GetField("_planMode", privateInstance)!.SetValue(coach, mode);
        coachType.GetField("_planReadableText", privateInstance)!.SetValue(coach, "Previous plan");
        coachType.GetField("_planUploadJson", privateInstance)!.SetValue(coach, "{}");
        coachType.GetField("_planResponseId", privateInstance)!.SetValue(coach, "previous-plan");
        coachType.GetField("_planUploaded", privateInstance)!.SetValue(coach, true);
        coachType.GetField("_isPlanExpanded", privateInstance)!.SetValue(coach, true);

        var assessments = (System.Collections.IDictionary)coachType.GetField("_assessCardStates", privateInstance)!.GetValue(coach)!;
        foreach (var assessment in assessments.Values.Cast<object>())
        {
            assessment.GetType().GetProperty("ResultText")!.SetValue(assessment, "Previous assessment");
            assessment.GetType().GetProperty("ResponseId")!.SetValue(assessment, "previous-assessment");
        }

        Assert.True((bool)coachType.GetProperty("HasPlanResult", privateInstance)!.GetValue(coach)!);
        var target = new WeekTargetOverride(CurrentWeek, TrainingPhase.Build, true, 10, 525, updateTargets);
        GetPageOverrides(coach)[CurrentWeek] = target;
        coachType.GetMethod("OnWeekTargetChanged", privateInstance)!.Invoke(coach, [target]);

        Assert.False((bool)coachType.GetProperty("HasPlanResult", privateInstance)!.GetValue(coach)!);
        Assert.False((bool)coachType.GetProperty("CanUploadPlan", privateInstance)!.GetValue(coach)!);
        Assert.Null(coachType.GetField("_planUploadJson", privateInstance)!.GetValue(coach));
        Assert.Null(coachType.GetField("_planResponseId", privateInstance)!.GetValue(coach));
        Assert.False((bool)coachType.GetField("_planUploaded", privateInstance)!.GetValue(coach)!);
        Assert.False((bool)coachType.GetField("_isPlanExpanded", privateInstance)!.GetValue(coach)!);
        foreach (var assessment in assessments.Values.Cast<object>())
        {
            Assert.Null(assessment.GetType().GetProperty("ResultText")!.GetValue(assessment));
            Assert.Null(assessment.GetType().GetProperty("ResponseId")!.GetValue(assessment));
        }

        Assert.Equal(target, GetPageOverrides(coach)[CurrentWeek]);
        var request = (PlanRequest)coachType.GetMethod("BuildPlanRequest", privateInstance)!.Invoke(coach, null)!;
        var values = TrainingPlanOverrideApplier.Read(request.WeekDataJson, CurrentWeek);
        Assert.Equal(TrainingPhase.Build, values.Phase);
        Assert.Equal(updateTargets ? 525 : 320, values.WeeklyTss);
        Assert.Equal("RECOVERY", values.WeekType);
    }

    private static WeekTargetsEditor CreateEditor(IAthleteSession session, PlanningScope scope = PlanningScope.CurrentWeek)
    {
        var editor = new WeekTargetsEditor();
        typeof(WeekTargetsEditor).GetProperty("Session", BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(editor, session);
        typeof(WeekTargetsEditor).GetProperty(nameof(WeekTargetsEditor.Scope))!.SetValue(editor, scope);
        typeof(WeekTargetsEditor).GetMethod("OnParametersSet", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(editor, null);
        return editor;
    }

    [Theory]
    [InlineData(null, null, false)]
    [InlineData(0.0, 0, false)]
    [InlineData(-1.0, -1, false)]
    [InlineData(null, 0, false)]
    [InlineData(0.0, null, false)]
    [InlineData(8.5, null, true)]
    [InlineData(null, 400, true)]
    [InlineData(0.0, 400, true)]
    [InlineData(8.5, 0, true)]
    [InlineData(8.5, 400, true)]
    public async Task Page_RequiresPositiveWeeklyHoursOrTss(double? hours, int? tss, bool expected)
    {
        var (session, _) = await CreateSessionAsync();
        var coach = CreateCoach(session);
        var week = (DateOnly)typeof(Coach).GetProperty("PlannedWeekStart", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(coach)!;
        GetPageOverrides(coach)[week] = new(week, TrainingPhase.Base, false, hours, tss);

        var valid = (bool)typeof(Coach).GetProperty("HasRequiredWeeklyTarget", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(coach)!;
        Assert.Equal(expected, valid);
        if (!expected)
        {
            await (Task)typeof(Coach).GetMethod("HandlePlanButtonAsync", BindingFlags.Instance | BindingFlags.NonPublic)!
                .Invoke(coach, [PlanningMode.Week])!;
            Assert.False((bool)typeof(Coach).GetField("_isPlanRequestInFlight", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(coach)!);
        }
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task Page_PrefilledWeeklyTargetsAreValidWithoutAnyEdits(bool includeHours)
    {
        var (session, client) = await CreateSessionAsync();
        var source = JsonNode.Parse(Dataset)!;
        if (includeHours)
            source["week_summary"]!["training_plan"]![1]!["weekly_time_target_hours"] = 8.0;
        client.Json = source.ToJsonString();
        await session.RefreshAsync();
        var coach = CreateCoach(session);

        Assert.True((bool)typeof(Coach).GetProperty("HasRequiredWeeklyTarget", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(coach)!);
        Assert.Empty(GetPageOverrides(coach));
    }

    [Fact]
    public async Task Editor_PhaseOnlyUsesCurrentWeekWithoutRecalculatingTargets()
    {
        var (session, _) = await CreateSessionAsync();
        var editor = CreateEditor(session, PlanningScope.NextWeek);
        typeof(WeekTargetsEditor).GetProperty(nameof(WeekTargetsEditor.PhaseOnly))!.SetValue(editor, true);
        typeof(WeekTargetsEditor).GetMethod("OnParametersSet", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(editor, null);
        var target = Assert.Single(GetTargets(editor));
        Assert.Equal(CurrentWeek, target.WeekStart);
        Assert.False(target.UpdateTargets);

        Edit(editor, target, target with { Phase = TrainingPhase.Build }, true);
        var changed = Assert.Single(GetEditorOverrides(editor)).Value;
        Assert.False(changed.UpdateTargets);
        Assert.Equal(320, changed.WeeklyTss);
        Assert.Equal(8.5, changed.WeeklyHours);
        var json = TrainingPlanOverrideApplier.Apply(Dataset, [changed]);
        Assert.Equal(320, WeeklyLoadTargetResolver.Resolve(json, PlanningScope.CurrentWeek));
    }

    [Fact]
    public async Task Editor_PhaseOnlyPreservesPreviousWeeklyTargetEdits()
    {
        var (session, _) = await CreateSessionAsync();
        var editor = CreateEditor(session);
        var overrides = GetEditorOverrides(editor);
        overrides[CurrentWeek] = new(CurrentWeek, TrainingPhase.Base, false, 10, 600);
        typeof(WeekTargetsEditor).GetProperty(nameof(WeekTargetsEditor.PhaseOnly))!.SetValue(editor, true);
        typeof(WeekTargetsEditor).GetMethod("OnParametersSet", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(editor, null);
        var target = Assert.Single(GetTargets(editor));

        Edit(editor, target, target with { Phase = TrainingPhase.Transition }, true);
        Assert.True(overrides[CurrentWeek].UpdateTargets);
        Assert.Equal(600, overrides[CurrentWeek].WeeklyTss);
        Assert.Equal(10, overrides[CurrentWeek].WeeklyHours);
        Assert.Equal(TrainingPhase.Transition, overrides[CurrentWeek].Phase);
    }

    [Theory]
    [InlineData("2026-10-17", true, "2026-10-18", 0)]
    [InlineData("2026-10-18", false, "2026-10-18", 0)]
    [InlineData("2026-10-18", true, "2026-10-19", 1)]
    public async Task Page_NextTrainingEditsOnlyRecommendationWeek(string today, bool completed, string expectedDate, int entryIndex)
    {
        var (session, client) = await CreateSessionAsync();
        var source = JsonNode.Parse(Dataset)!;
        source["current_date"] = today;
        source["activities"] = completed ? new JsonArray(new JsonObject { ["date"] = today }) : new JsonArray();
        source["week_summary"]!["training_plan"]![1]!["day_constraints"] = new JsonArray(new JsonObject { ["day"] = "Tuesday" });
        source["week_summary"]!["training_plan"]![1]!["weekly_time_target_hours"] = 7.0;
        client.Json = source.ToJsonString();
        await session.RefreshAsync();
        var coach = CreateCoach(session);
        var editor = CreateEditor(session);
        typeof(WeekTargetsEditor).GetProperty(nameof(WeekTargetsEditor.PhaseOnly))!.SetValue(editor, true);
        typeof(WeekTargetsEditor).GetProperty(nameof(WeekTargetsEditor.Overrides))!.SetValue(editor, GetPageOverrides(coach));
        typeof(WeekTargetsEditor).GetMethod("OnParametersSet", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(editor, null);
        var target = Assert.Single(GetTargets(editor));
        Assert.Equal(CurrentWeek.AddDays(entryIndex * 7), target.WeekStart);
        Assert.Equal(entryIndex == 0 ? TrainingPhase.Base : TrainingPhase.Peak, target.Phase);
        Assert.Equal(entryIndex == 0 ? "NORMAL" : "RACE", TrainingPlanOverrideApplier.Read(session.AthleteDataJson, target.WeekStart).WeekType);
        Edit(editor, target, target with { Phase = TrainingPhase.Transition, IsRecovery = true });
        typeof(Coach).GetField("_planMode", BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(coach, PlanningMode.NextTraining);

        var request = (PlanRequest)typeof(Coach).GetMethod("BuildPlanRequest", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(coach, null)!;
        var plan = JsonNode.Parse(request.WeekDataJson)!["week_summary"]!["training_plan"]!.AsArray();
        Assert.Equal(2, plan.Count);
        Assert.Equal("Transition", plan[entryIndex]!["phase"]!.GetValue<string>());
        var originalPlan = source["week_summary"]!["training_plan"]!;
        Assert.True(JsonNode.DeepEquals(originalPlan[1 - entryIndex], plan[1 - entryIndex]));
        foreach (var property in new[] { "weekly_load_target", "weekly_time_target_hours", "day_constraints" })
            Assert.True(JsonNode.DeepEquals(originalPlan[entryIndex]![property], plan[entryIndex]![property]));
        Assert.Equal(expectedDate, ((DateOnly)typeof(Coach).GetProperty("NextTrainingRecommendationDate", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(coach)!).ToString("yyyy-MM-dd"));
        var prompt = PromptBuilder.BuildPlanPrompt(request);
        Assert.Contains($"for the recommendation date {expectedDate}", prompt);
        Assert.DoesNotContain("{{recommendation_date}}", prompt);
        Assert.Empty(GetPageOverrides(CreateCoach(session)));
    }

    private static List<WeekTargetOverride> GetTargets(WeekTargetsEditor editor) =>
        (List<WeekTargetOverride>)typeof(WeekTargetsEditor).GetField("_targets", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(editor)!;

    private static Dictionary<DateOnly, WeekTargetOverride> GetEditorOverrides(WeekTargetsEditor editor) =>
        (Dictionary<DateOnly, WeekTargetOverride>)typeof(WeekTargetsEditor).GetProperty(nameof(WeekTargetsEditor.Overrides))!.GetValue(editor)!;

    private static Coach CreateCoach(IAthleteSession session)
    {
        var coach = new Coach();
        typeof(Coach).GetProperty("Session", BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(coach, session);
        SynchronizeOverrides(coach);
        return coach;
    }

    private static Dictionary<DateOnly, WeekTargetOverride> GetPageOverrides(Coach coach) =>
        (Dictionary<DateOnly, WeekTargetOverride>)typeof(Coach).GetField("_weekTargetOverrides", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(coach)!;

    private static void SynchronizeOverrides(Coach coach) =>
        typeof(Coach).GetMethod("SynchronizeWeekTargetOverrides", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(coach, null);

    private static void Edit(WeekTargetsEditor editor, WeekTargetOverride previous, WeekTargetOverride updated, bool estimate = false) =>
        typeof(WeekTargetsEditor).GetMethod("Change", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(editor, [previous, updated, estimate]);

    private static async Task<(AthleteSession Session, MutableAthleteDataClient Client)> CreateSessionAsync()
    {
        var client = new MutableAthleteDataClient();
        var session = new AthleteSession(client, new ServerCredentialStore(), NullLogger<AthleteSession>.Instance);
        await session.ConnectAsync("__ATHLETE_ID__", "__API_KEY__", CoachLanguage.English, AthleteProfileGoal.Fitness, false);
        return (session, client);
    }

    private sealed class MutableAthleteDataClient : IAthleteDataClient
    {
        public string Json { get; set; } = Dataset;

        public Task<AthleteDataResult> FetchAsync(string athleteId, string apiKey, CancellationToken ct = default) =>
            Task.FromResult(new AthleteDataResult { DataParsed = JsonSerializer.Deserialize<JsonElement>(Json), DataRaw = Json });
    }
}