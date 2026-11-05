using System.Text.Json;
using Microsoft.Extensions.Logging.Abstractions;
using TrainingArchitect.Client.Services;
using TrainingArchitect.Core.Models;
using TrainingArchitect.Core.Services;

namespace TrainingArchitect.Tests;

public sealed class StrengthPhaseSelectionTests
{
    [Fact]
    public void StoredCredential_OlderEntryDefaultsStrengthPhaseSelectionToAutomatic()
    {
        var credential = JsonSerializer.Deserialize<StoredCredential>(
            """{"AthleteId":"athlete","ApiKey":"key","Language":0,"ProfileGoal":3}""");

        Assert.NotNull(credential);
        Assert.Equal(StrengthPhaseSelection.Automatic, credential.StrengthPhaseSelection);
    }

    [Theory]
    [InlineData(StrengthPhaseSelection.Automatic)]
    [InlineData(StrengthPhaseSelection.Adaptation)]
    [InlineData(StrengthPhaseSelection.Transition)]
    [InlineData(StrengthPhaseSelection.Maximum)]
    [InlineData(StrengthPhaseSelection.Maintenance)]
    [InlineData(StrengthPhaseSelection.None)]
    public void StoredCredential_StrengthPhaseSelectionRoundTrips(StrengthPhaseSelection selection)
    {
        var credential = new StoredCredential { StrengthPhaseSelection = selection };

        var restored = JsonSerializer.Deserialize<StoredCredential>(JsonSerializer.Serialize(credential));

        Assert.NotNull(restored);
        Assert.Equal(selection, restored.StrengthPhaseSelection);
    }

    [Theory]
    [InlineData(PlanningScope.CurrentWeek, PlanningMode.Week, "adaptation")]
    [InlineData(PlanningScope.NextWeek, PlanningMode.Week, "maximum")]
    [InlineData(PlanningScope.NextWeek, PlanningMode.NextTraining, "adaptation")]
    public void AutomaticSelection_UsesPhaseForTileScope(
        PlanningScope scope,
        PlanningMode mode,
        string expected)
    {
        const string data = """
            {"week_summary":{"strength":{
              "phase_current_week":"adaptation",
              "phase_next_week":"maximum"
            }}}
            """;

        var phase = StrengthPhaseSelectionResolver.Resolve(
            StrengthPhaseSelection.Automatic,
            data,
            scope,
            mode);

        Assert.Equal(expected, phase);
    }

    [Theory]
    [InlineData("")]
    [InlineData("""{"week_summary":{"strength":{}}}""")]
    public void AutomaticSelection_UsesNoneWhenTargetPhaseIsAbsent(string data)
    {
        var phase = StrengthPhaseSelectionResolver.Resolve(
            StrengthPhaseSelection.Automatic,
            data,
            PlanningScope.NextWeek);

        Assert.Equal("none", phase);
        Assert.Null(StrengthPhaseSelectionResolver.ResolveAutomaticPhase(data, PlanningScope.NextWeek));
    }

    [Theory]
    [InlineData(false, CredentialDurability.Session)]
    [InlineData(true, CredentialDurability.Persistent)]
    public async Task SetStrengthPhaseSelectionAsync_PersistsWithCurrentDurability(
        bool remember,
        CredentialDurability expectedDurability)
    {
        var store = new RecordingCredentialStore();
        var session = new AthleteSession(
            new StaticAthleteDataClient(),
            store,
            NullLogger<AthleteSession>.Instance);

        await session.ConnectAsync(
            "athlete",
            "api-key",
            CoachLanguage.English,
            AthleteProfileGoal.Fitness,
            remember,
            strengthPhaseSelection: StrengthPhaseSelection.Maximum);

        await session.SetStrengthPhaseSelectionAsync(StrengthPhaseSelection.Transition);

        Assert.Equal(StrengthPhaseSelection.Transition, session.StrengthPhaseSelection);
        var saved = Assert.Single(store.Saves.Skip(1));
        Assert.Equal(expectedDurability, saved.Durability);
        Assert.Equal(StrengthPhaseSelection.Transition, saved.Credential.StrengthPhaseSelection);

        await session.DisconnectAsync();

        Assert.Null(session.StrengthPhaseSelection);
        Assert.True(store.WasCleared);
    }

    private sealed class StaticAthleteDataClient : IAthleteDataClient
    {
        public Task<AthleteDataResult> FetchAsync(
            string athleteId,
            string apiKey,
            CancellationToken ct = default) =>
            Task.FromResult(new AthleteDataResult
            {
                DataParsed = JsonSerializer.SerializeToElement(new { })
            });
    }

    private sealed class RecordingCredentialStore : ICredentialStore
    {
        public List<(StoredCredential Credential, CredentialDurability Durability)> Saves { get; } = [];
        public bool WasCleared { get; private set; }

        public Task<LoadedCredential?> LoadAsync(CancellationToken ct = default) =>
            Task.FromResult<LoadedCredential?>(null);

        public Task SaveAsync(
            StoredCredential credential,
            CredentialDurability durability,
            CancellationToken ct = default)
        {
            Saves.Add((credential, durability));
            return Task.CompletedTask;
        }

        public Task ClearAsync(CancellationToken ct = default)
        {
            WasCleared = true;
            return Task.CompletedTask;
        }
    }
}
