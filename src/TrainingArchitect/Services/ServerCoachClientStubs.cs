using System.Text.Json;
using TrainingArchitect.Client.Services;

namespace TrainingArchitect.Services;

/// <summary>
/// Server-side placeholder for credential persistence while the Coach page is
/// bootstrapped as an InteractiveWebAssembly route.
/// </summary>
public sealed class ServerCredentialStore : ICredentialStore
{
    public Task<LoadedCredential?> LoadAsync(CancellationToken ct = default)
    {
        return Task.FromResult<LoadedCredential?>(null);
    }

    public Task SaveAsync(StoredCredential credential, CredentialDurability durability, CancellationToken ct = default)
    {
        return Task.CompletedTask;
    }

    public Task ClearAsync(CancellationToken ct = default)
    {
        return Task.CompletedTask;
    }
}

/// <summary>
/// Server-side placeholder session used only during initial endpoint rendering.
/// The real interactive behavior runs in the WebAssembly client.
/// </summary>
public sealed class ServerAthleteSession : IAthleteSession
{
    public CoachConnectionState State => CoachConnectionState.Disconnected;

    public bool IsRefreshing => false;

    public string? AthleteId => null;

    public string? Level => null;

    public string? LevelLabel => null;

    public CoachLanguage? Language => null;

    public AthleteProfileGoal? ProfileGoal => null;

    public DateTimeOffset? LastSynced => null;

    public string? ErrorMessage => null;

    public string? AthleteDataJson => null;

    public string? AthleteDataRaw => null;

    public JsonElement? AthleteDataParsed => null;

    public string? AthleteDataMethodName => null;

    public event Action? Changed;

    public Task ConnectAsync(string athleteId, string apiKey, CoachLanguage language, AthleteProfileGoal profileGoal, bool remember, CancellationToken ct = default)
    {
        return Task.CompletedTask;
    }

    public Task RefreshAsync(CancellationToken ct = default)
    {
        return Task.CompletedTask;
    }

    public Task DisconnectAsync(bool forget = true)
    {
        return Task.CompletedTask;
    }

    public bool TryGetActiveCredentials(out string athleteId, out string apiKey)
    {
        athleteId = string.Empty;
        apiKey = string.Empty;
        return false;
    }
}
