namespace TrainingArchitect.Client.Services;

// TEMPORARY - Placeholder until the real HTTP client for /api/athlete-data is implemented.
internal sealed class StubAthleteDataClient : IAthleteDataClient
{
    public Task<string> FetchAsync(string athleteId, string apiKey, CancellationToken ct = default)
        => Task.FromResult("""{ "_stub": true }""");
}

