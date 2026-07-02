using Microsoft.Extensions.Logging;
using System.Text.Json;
using TrainingArchitect.Core.Models;

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
    string? AthleteDataRaw { get; }
    JsonElement? AthleteDataParsed { get; }
    WeekDataDto? AthleteDataDeserialized { get; }
    string? AthleteDataMethodName { get; }

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

    /// <summary>
    /// Tries to resolve the currently active credentials for authenticated coach API calls.
    /// </summary>
    /// <param name="athleteId">Resolved athlete id.</param>
    /// <param name="apiKey">Resolved API key.</param>
    /// <returns>True when valid credentials are available in the current connected session.</returns>
    bool TryGetActiveCredentials(out string athleteId, out string apiKey);
}

// ---------------------------------------------------------------------------
// Seams required by AthleteSession, defined here only as contracts.
// Implementations follow separately (data client + credential store).
// ---------------------------------------------------------------------------

public interface IAthleteDataClient
{
    // GET /api/athlete-data -> parsed + raw payload.
    Task<AthleteDataResult> FetchAsync(string athleteId, string apiKey, CancellationToken ct = default);
}

public sealed class AthleteDataResult
{
    public string MethodName { get; init; } = string.Empty;
    public string DataRaw { get; init; } = string.Empty;
    public JsonElement DataParsed { get; init; }
    public WeekDataDto? DataDeserialized { get; init; }
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
    public string? AthleteDataRaw { get; private set; }
    public JsonElement? AthleteDataParsed { get; private set; }
    public WeekDataDto? AthleteDataDeserialized { get; private set; }
    public string? AthleteDataMethodName { get; private set; }

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
            var result = await dataClient.FetchAsync(athleteId, apiKey, ct);

            AthleteId = athleteId;
            Language = language;
            ProfileGoal = profileGoal;
            _apiKey = apiKey;
            AthleteDataMethodName = result.MethodName;
            AthleteDataRaw = result.DataRaw;
            AthleteDataParsed = result.DataParsed;
            AthleteDataDeserialized = result.DataDeserialized;
            AthleteDataJson = result.DataParsed.GetRawText();
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
            ErrorMessage = FormatConnectErrorMessage(ex.Message);
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
            var result = await dataClient.FetchAsync(AthleteId, _apiKey, ct);
            AthleteDataMethodName = result.MethodName;
            AthleteDataRaw = result.DataRaw;
            AthleteDataParsed = result.DataParsed;
            AthleteDataDeserialized = result.DataDeserialized;
            AthleteDataJson = result.DataParsed.GetRawText();
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

    public bool TryGetActiveCredentials(out string athleteId, out string apiKey)
    {
        if (State == CoachConnectionState.Connected
            && !string.IsNullOrWhiteSpace(AthleteId)
            && !string.IsNullOrWhiteSpace(_apiKey))
        {
            athleteId = AthleteId;
            apiKey = _apiKey;
            return true;
        }

        athleteId = string.Empty;
        apiKey = string.Empty;
        return false;
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
        AthleteDataRaw = null;
        AthleteDataParsed = null;
        AthleteDataDeserialized = null;
        AthleteDataMethodName = null;
        LastSynced = null;
    }

    private static string FormatConnectErrorMessage(string message)
    {
        if (string.IsNullOrWhiteSpace(message))
        {
            return "Could not connect. Please check your Athlete ID and API key.";
        }

        if (TryParseConnectError(message, out var conciseMessage))
        {
            return conciseMessage;
        }

        return message.Trim();
    }

    private static bool TryParseConnectError(string message, out string conciseMessage)
    {
        conciseMessage = string.Empty;

        try
        {
            using var document = JsonDocument.Parse(message);
            var root = document.RootElement;

            if (root.ValueKind != JsonValueKind.Object)
            {
                return false;
            }

            var errorText = GetStringProperty(root, "error")
                ?? GetStringProperty(root, "detail")
                ?? GetStringProperty(root, "title")
                ?? GetStringProperty(root, "message");

            if (string.IsNullOrWhiteSpace(errorText))
            {
                return false;
            }

            var hintText = GetHintFromJson(root);
            conciseMessage = string.IsNullOrWhiteSpace(hintText)
                ? errorText.Trim()
                : $"{errorText.Trim()}\n{hintText.Trim()}";
            return true;
        }
        catch (JsonException)
        {
            return false;
        }
    }

    private static string? GetHintFromJson(JsonElement value)
    {
        if (value.ValueKind != JsonValueKind.Object)
        {
            return null;
        }

        if (value.TryGetProperty("hint", out var hintProperty) &&
            hintProperty.ValueKind == JsonValueKind.String)
        {
            var hintText = hintProperty.GetString();
            if (!string.IsNullOrWhiteSpace(hintText))
            {
                return hintText.Trim();
            }
        }

        if (!value.TryGetProperty("log", out var logProperty) ||
            logProperty.ValueKind != JsonValueKind.String)
        {
            return null;
        }

        var logText = logProperty.GetString();
        if (string.IsNullOrWhiteSpace(logText))
        {
            return null;
        }

        var hintIndex = logText.IndexOf("Hint:", StringComparison.OrdinalIgnoreCase);
        if (hintIndex < 0)
        {
            return null;
        }

        var hintTextFromLog = logText[(hintIndex + "Hint:".Length)..].Trim();
        return string.IsNullOrWhiteSpace(hintTextFromLog) ? null : hintTextFromLog;
    }

    private static string? GetStringProperty(JsonElement value, string propertyName)
    {
        if (value.ValueKind != JsonValueKind.Object)
        {
            return null;
        }

        if (!value.TryGetProperty(propertyName, out var property) || property.ValueKind != JsonValueKind.String)
        {
            return null;
        }

        var text = property.GetString();
        return string.IsNullOrWhiteSpace(text) ? null : text;
    }

    private void Notify() => Changed?.Invoke();
}