namespace TrainingArchitect.Client.Services;

using System.Text.Json;

// TEMPORARY - Placeholder until the real HTTP client for /api/athlete-data is implemented.
internal sealed class StubAthleteDataClient : IAthleteDataClient
{
    public Task<AthleteDataResult> FetchAsync(string athleteId, string apiKey, CancellationToken ct = default)
    {
        var parsed = JsonSerializer.Deserialize<JsonElement>("""{ "_stub": true }""");
        return Task.FromResult(new AthleteDataResult
        {
            MethodName = "stub",
            DataRaw = parsed.GetRawText(),
            DataParsed = parsed
        });
    }
}

