using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;


namespace TrainingArchitect.Client.Services;

// Persists athlete ID + API key. Durability selects the storage:
//   Persistent -> localStorage (survives browser restart)
//   Session    -> sessionStorage (survives reload, cleared when the tab is closed)
public sealed class CredentialStore(
    IJSRuntime js,
    ILogger<CredentialStore> logger) : ICredentialStore
{
    private const string Key = "ta.athlete-credential";
    private const string LocalApi = "localStorage";
    private const string SessionApi = "sessionStorage";

    public async Task<LoadedCredential?> LoadAsync(CancellationToken ct = default)
    {
        // localStorage has priority (explicitly "remembered"), then sessionStorage.
        var persistent = await ReadAsync(LocalApi, ct);
        if (persistent is not null)
            return new LoadedCredential(persistent, CredentialDurability.Persistent);

        var session = await ReadAsync(SessionApi, ct);
        return session is null
            ? null
            : new LoadedCredential(session, CredentialDurability.Session);
    }

    public async Task SaveAsync(StoredCredential credential, CredentialDurability durability, CancellationToken ct = default)
    {
        var (target, other) = durability == CredentialDurability.Persistent
            ? (LocalApi, SessionApi)
            : (SessionApi, LocalApi);

        var json = JsonSerializer.Serialize(credential);
        await js.InvokeVoidAsync($"{target}.setItem", ct, Key, json);
        // Keep exactly one copy: clear the opposite store if the selection changed.
        await js.InvokeVoidAsync($"{other}.removeItem", ct, Key);
    }

    public async Task ClearAsync(CancellationToken ct = default)
    {
        await js.InvokeVoidAsync($"{LocalApi}.removeItem", ct, Key);
        await js.InvokeVoidAsync($"{SessionApi}.removeItem", ct, Key);
    }

    private async Task<StoredCredential?> ReadAsync(string store, CancellationToken ct)
    {
        try
        {
            var json = await js.InvokeAsync<string?>($"{store}.getItem", ct, Key);
            return string.IsNullOrEmpty(json)
                ? null
                : JsonSerializer.Deserialize<StoredCredential>(json);
        }
        catch (Exception ex)
        {
            // Corrupted entry: do not crash, clear it, and treat as "nothing stored".
            logger.LogWarning(ex, "Stored credential in {Store} could not be read; clearing it.", store);
            try { await js.InvokeVoidAsync($"{store}.removeItem", ct, Key); }
            catch { /* best effort */ }
            return null;
        }
    }
}