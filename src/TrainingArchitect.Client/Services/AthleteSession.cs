using Microsoft.Extensions.Logging;

namespace TrainingArchitect.Client.Services;

public enum CoachConnectionState
{
    Disconnected,
    Connecting,
    Connected,
    Error
}

public enum CoachLanguage
{
    English,
    German,
    Spanish,
    French,
    Turkish
}

public enum AthleteProfileGoal
{
    Climber,
    Criterium,
    Marathon,
    RoadRace
}

public interface IAthleteSession
{
    CoachConnectionState State { get; }
    bool IsConnected => State == CoachConnectionState.Connected;
    bool IsRefreshing { get; }          // Re-sync without tearing down the connected view

    string? AthleteId { get; }
    CoachLanguage? Language { get; }
    AthleteProfileGoal? ProfileGoal { get; }
    DateTimeOffset? LastSynced { get; }
    string? ErrorMessage { get; }

    // Raw intervals.icu JSON forwarded verbatim to /api/coach.
    string? AthleteDataJson { get; }

    event Action? Changed;

    Task ConnectAsync(
        string athleteId,
        string apiKey,
        CoachLanguage language,
        AthleteProfileGoal profileGoal,
        bool remember,
        CancellationToken ct = default);
    Task RefreshAsync(CancellationToken ct = default);
    Task DisconnectAsync(bool forget = true);   // Was void in the draft, async because localStorage must be cleared
}

// ---------------------------------------------------------------------------
// Seams required by AthleteSession, defined here only as contracts.
// Implementations follow separately (data client + credential store).
// ---------------------------------------------------------------------------

public interface IAthleteDataClient
{
    // POST /api/athlete-data -> raw intervals.icu JSON.
    Task<string> FetchAsync(string athleteId, string apiKey, CancellationToken ct = default);
}

// ---------------------------------------------------------------------------

public sealed class AthleteSession(
    IAthleteDataClient dataClient,
    ICredentialStore credentialStore,
    ILogger<AthleteSession> logger) : IAthleteSession
{
    // Kept only internally for refresh, never exposed externally.
    private string? _apiKey;

    public CoachConnectionState State { get; private set; } = CoachConnectionState.Disconnected;
    public bool IsRefreshing { get; private set; }
    public string? AthleteId { get; private set; }
    public CoachLanguage? Language { get; private set; }
    public AthleteProfileGoal? ProfileGoal { get; private set; }
    public DateTimeOffset? LastSynced { get; private set; }
    public string? ErrorMessage { get; private set; }
    public string? AthleteDataJson { get; private set; }

    public event Action? Changed;

    public async Task ConnectAsync(
        string athleteId,
        string apiKey,
        CoachLanguage language,
        AthleteProfileGoal profileGoal,
        bool remember,
        CancellationToken ct = default)
    {
        if (State == CoachConnectionState.Connecting)
            return; // a connect request is already in progress

        State = CoachConnectionState.Connecting;
        ErrorMessage = null;
        Notify();

        try
        {
            var json = await dataClient.FetchAsync(athleteId, apiKey, ct);

            AthleteId = athleteId;
            Language = language;
            ProfileGoal = profileGoal;
            _apiKey = apiKey;
            AthleteDataJson = json;
            LastSynced = DateTimeOffset.Now;
            State = CoachConnectionState.Connected;

            // Persist only after a successful connect so only validated
            // credentials are remembered.
            if (remember)
            {
                await credentialStore.SaveAsync(
                    new StoredCredential
                    {
                        AthleteId = athleteId,
                        ApiKey = apiKey,
                        Language = language,
                        ProfileGoal = profileGoal
                    },
                    CredentialDurability.Persistent,
                    ct);
            }
            else
            {
                await credentialStore.SaveAsync(
                    new StoredCredential
                    {
                        AthleteId = athleteId,
                        ApiKey = apiKey,
                        Language = language,
                        ProfileGoal = profileGoal
                    },
                    CredentialDurability.Session,
                    ct);
            }
        }
        catch (OperationCanceledException)
        {
            ResetToDisconnected(); // cancellation is not an error
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Connect for athlete {AthleteId} failed", athleteId);
            ClearData();
            _apiKey = null;
            ErrorMessage = "Could not connect. Please check your Athlete ID and API key.";
            State = CoachConnectionState.Error;
        }

        Notify();
    }

    public async Task RefreshAsync(CancellationToken ct = default)
    {
        // Refresh only makes sense while connected; credentials are held internally.
        if (State != CoachConnectionState.Connected || _apiKey is null || AthleteId is null)
            return;
        if (IsRefreshing)
            return;

        IsRefreshing = true;
        Notify();

        try
        {
            var json = await dataClient.FetchAsync(AthleteId, _apiKey, ct);
            AthleteDataJson = json;
            LastSynced = DateTimeOffset.Now;
            ErrorMessage = null;
        }
        catch (OperationCanceledException)
        {
            // Cancellation: the last successful sync remains valid.
        }
        catch (Exception ex)
        {
            // Intentionally do NOT discard existing data; only report the error.
            // State stays Connected and the last fetched data remains usable.
            logger.LogWarning(ex, "Refresh for athlete {AthleteId} failed", AthleteId);
            ErrorMessage = "Refresh failed. Showing the last synced data.";
        }
        finally
        {
            IsRefreshing = false;
            Notify();
        }
    }

    public async Task DisconnectAsync(bool forget = true)
    {
        ClearData();
        _apiKey = null;
        State = CoachConnectionState.Disconnected;
        IsRefreshing = false;
        ErrorMessage = null;

        if (forget)
            await credentialStore.ClearAsync();

        Notify();
    }

    private void ResetToDisconnected()
    {
        ClearData();
        _apiKey = null;
        State = CoachConnectionState.Disconnected;
        ErrorMessage = null;
    }

    private void ClearData()
    {
        AthleteId = null;
        Language = null;
        ProfileGoal = null;
        AthleteDataJson = null;
        LastSynced = null;
    }

    private void Notify() => Changed?.Invoke();
}