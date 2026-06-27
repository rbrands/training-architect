using System.Net.Http.Json;
using System.Text.Json;
using TrainingArchitect.Core.Constants;

namespace TrainingArchitect.Client.Services;

internal sealed class HttpAthleteDataClient(HttpClient httpClient) : IAthleteDataClient
{
    public async Task<AthleteDataResult> FetchAsync(string athleteId, string apiKey, CancellationToken ct = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, "/api/athlete-data");

        request.Headers.TryAddWithoutValidation(IntervalsHeaders.AthleteId, athleteId);
        request.Headers.TryAddWithoutValidation(IntervalsHeaders.ApiKey, apiKey);

        using var response = await httpClient.SendAsync(request, ct);
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync(ct);
            throw new InvalidOperationException(
                $"Athlete data request failed ({(int)response.StatusCode}): {error}");
        }

        var envelope = await response.Content.ReadFromJsonAsync<AthleteDataEnvelope>(cancellationToken: ct);
        if (envelope is null)
        {
            return CreateEmptyResult();
        }

        var parsed = envelope.DataParsed.ValueKind is JsonValueKind.Undefined or JsonValueKind.Null
            ? JsonSerializer.Deserialize<JsonElement>("{}")
            : envelope.DataParsed.Clone();

        var raw = string.IsNullOrWhiteSpace(envelope.DataRaw)
            ? parsed.GetRawText()
            : envelope.DataRaw;

        return new AthleteDataResult
        {
            MethodName = envelope.MethodName ?? string.Empty,
            DataRaw = raw,
            DataParsed = parsed
        };
    }

    private static AthleteDataResult CreateEmptyResult()
    {
        var empty = JsonSerializer.Deserialize<JsonElement>("{}");
        return new AthleteDataResult
        {
            MethodName = string.Empty,
            DataRaw = empty.GetRawText(),
            DataParsed = empty
        };
    }

    private sealed class AthleteDataEnvelope
    {
        public string? MethodName { get; init; }
        public string? DataRaw { get; init; }
        public JsonElement DataParsed { get; init; }
    }
}
