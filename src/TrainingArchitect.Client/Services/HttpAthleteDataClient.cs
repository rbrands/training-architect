using System.Text.Json;
using TrainingArchitect.Core.Constants;

namespace TrainingArchitect.Client.Services;

internal sealed class HttpAthleteDataClient(HttpClient httpClient) : IAthleteDataClient
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public async Task<AthleteDataResult> FetchAsync(string athleteId, string apiKey, CancellationToken ct = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, "/api/athlete-data");

        request.Headers.TryAddWithoutValidation(IntervalsHeaders.AthleteId, athleteId);
        request.Headers.TryAddWithoutValidation(IntervalsHeaders.ApiKey, apiKey);

        using var response = await httpClient.SendAsync(request, ct);
        var responseBody = await response.Content.ReadAsStringAsync(ct);

        if (TryExtractErrorMessage(responseBody, out var extractedError))
        {
            throw new InvalidOperationException(extractedError);
        }

        if (!response.IsSuccessStatusCode)
        {
            var message = ExtractErrorMessage(responseBody, response.ReasonPhrase, response.StatusCode.ToString());
            throw new InvalidOperationException(
                message);
        }

        var envelope = JsonSerializer.Deserialize<AthleteDataEnvelope>(responseBody, JsonOptions);
        if (envelope is null)
        {
            return CreateEmptyResult();
        }

        if (TryBuildConciseErrorMessage(envelope, out var conciseError))
        {
            throw new InvalidOperationException(conciseError);
        }

        if (IsEmptyEnvelope(envelope))
        {
            throw new InvalidOperationException(ExtractErrorMessage(responseBody, response.ReasonPhrase, response.StatusCode.ToString()));
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
            DataParsed = parsed,
            Level = envelope.Level ?? string.Empty,
            LevelLabel = envelope.LevelLabel ?? string.Empty
        };
    }

    private static AthleteDataResult CreateEmptyResult()
    {
        var empty = JsonSerializer.Deserialize<JsonElement>("{}");
        return new AthleteDataResult
        {
            MethodName = string.Empty,
            DataRaw = empty.GetRawText(),
            DataParsed = empty,
            Level = string.Empty,
            LevelLabel = string.Empty
        };
    }

    private static bool IsEmptyEnvelope(AthleteDataEnvelope envelope)
    {
        var noMethodName = string.IsNullOrWhiteSpace(envelope.MethodName);
        var noRawData = string.IsNullOrWhiteSpace(envelope.DataRaw);
        var noDataObject = envelope.DataParsed.ValueKind is JsonValueKind.Undefined or JsonValueKind.Null;

        return noMethodName && noRawData && noDataObject;
    }

    private static string ExtractErrorMessage(string responseBody, string? reasonPhrase, string fallbackStatus)
    {
        if (!string.IsNullOrWhiteSpace(responseBody))
        {
            return responseBody.Trim();
        }

        if (!string.IsNullOrWhiteSpace(reasonPhrase))
        {
            return reasonPhrase;
        }

        return $"Athlete data request failed ({fallbackStatus}).";
    }

    private static bool TryBuildConciseErrorMessage(AthleteDataEnvelope envelope, out string message)
    {
        var errorText = GetStringProperty(envelope.DataParsed, "error");

        if (string.IsNullOrWhiteSpace(errorText))
        {
            message = string.Empty;
            return false;
        }

        var hintText = GetHintText(envelope.DataParsed);

        message = string.IsNullOrWhiteSpace(hintText)
            ? errorText.Trim()
            : $"{errorText.Trim()}\n{hintText.Trim()}";
        return true;
    }

    private static string? GetHintText(JsonElement value)
    {
        if (value.ValueKind != JsonValueKind.Object)
        {
            return null;
        }

        if (!value.TryGetProperty("log", out var logProperty) || logProperty.ValueKind != JsonValueKind.String)
        {
            return GetStringProperty(value, "hint");
        }

        var logText = logProperty.GetString();
        if (string.IsNullOrWhiteSpace(logText))
        {
            return GetStringProperty(value, "hint");
        }

        var hintIndex = logText.IndexOf("Hint:", StringComparison.OrdinalIgnoreCase);
        if (hintIndex < 0)
        {
            return GetStringProperty(value, "hint");
        }

        var hintText = logText[(hintIndex + "Hint:".Length)..].Trim();
        return string.IsNullOrWhiteSpace(hintText) ? GetStringProperty(value, "hint") : hintText;
    }

    private static string? GetHintText(object? value)
    {
        if (value is null)
        {
            return null;
        }

        var json = JsonSerializer.SerializeToElement(value);
        return GetHintText(json);
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

    private static string? GetStringProperty(object? value, string propertyName)
    {
        if (value is null)
        {
            return null;
        }

        var json = JsonSerializer.SerializeToElement(value);
        return GetStringProperty(json, propertyName);
    }

    private static bool TryExtractErrorMessage(string responseBody, out string message)
    {
        try
        {
            using var document = JsonDocument.Parse(responseBody);
            var root = document.RootElement;

            if (root.ValueKind != JsonValueKind.Object)
            {
                message = string.Empty;
                return false;
            }

            var messageParts = new List<string>();

            string[] propertyNames = ["error", "detail", "title", "message", "log", "hint"];

            foreach (var propertyName in propertyNames)
            {
                if (root.TryGetProperty(propertyName, out var value) &&
                    value.ValueKind == JsonValueKind.String)
                {
                    var text = value.GetString();
                    if (!string.IsNullOrWhiteSpace(text) && !messageParts.Contains(text))
                    {
                        messageParts.Add(text.Trim());
                    }
                }
            }

            if (messageParts.Count == 0)
            {
                message = string.Empty;
                return false;
            }

            message = string.Join("\n", messageParts);
            return true;
        }
        catch (JsonException)
        {
            message = string.Empty;
            return false;
        }
    }

    private sealed class AthleteDataEnvelope
    {
        public string? MethodName { get; init; }
        public string? DataRaw { get; init; }
        public JsonElement DataParsed { get; init; }
        public string? Level { get; init; }
        public string? LevelLabel { get; init; }
    }
}
