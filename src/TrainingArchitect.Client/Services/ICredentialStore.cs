namespace TrainingArchitect.Client.Services;

public sealed record StoredCredential
{
    public string AthleteId { get; init; } = string.Empty;
    public string ApiKey { get; init; } = string.Empty;
    public CoachLanguage Language { get; init; } = CoachLanguage.English;
    public AthleteProfileGoal ProfileGoal { get; init; } = AthleteProfileGoal.RoadRace;
}

public enum CredentialDurability
{
    Session,     // sessionStorage – survives reload, gone when the tab is closed
    Persistent   // localStorage – survives browser restart
}

public sealed record LoadedCredential(StoredCredential Credential, CredentialDurability Durability);

public interface ICredentialStore
{
    Task<LoadedCredential?> LoadAsync(CancellationToken ct = default);
    Task SaveAsync(StoredCredential credential, CredentialDurability durability, CancellationToken ct = default);
    Task ClearAsync(CancellationToken ct = default);
}