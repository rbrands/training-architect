using System.Text.Json;
using System.Text.RegularExpressions;

namespace TrainingArchitect.Core.Services;

/// <summary>
/// Splits a coaching agent plan response into the readable plan text and the upload JSON payload.
/// </summary>
public static partial class PlanResponseParser
{
    private const string BeginMarker = "BEGIN_UPLOAD_JSON";
    private const string EndMarker = "END_UPLOAD_JSON";

    /// <summary>
    /// Separates the readable plan text from the marked upload JSON block.
    /// </summary>
    /// <param name="responseText">The raw agent response.</param>
    /// <returns>The plan text and the upload JSON; either can be empty when the response does not contain it.</returns>
    public static (string PlanText, string UploadJson) Split(string? responseText)
    {
        if (string.IsNullOrWhiteSpace(responseText))
        {
            return (string.Empty, string.Empty);
        }

        var beginIndex = responseText.IndexOf(BeginMarker, StringComparison.Ordinal);
        if (beginIndex < 0)
        {
            return (responseText.Trim(), string.Empty);
        }

        var endIndex = responseText.IndexOf(EndMarker, beginIndex + BeginMarker.Length, StringComparison.Ordinal);
        if (endIndex < 0)
        {
            return (responseText.Trim(), string.Empty);
        }

        var planText = responseText[..beginIndex].Trim();
        var markedSection = responseText[(beginIndex + BeginMarker.Length)..endIndex].Trim();

        return (planText, ExtractJson(markedSection));
    }

    /// <summary>
    /// Extracts the JSON object from a marked section that may be wrapped in a fenced code block.
    /// </summary>
    public static string ExtractJson(string? markedSection)
    {
        if (string.IsNullOrWhiteSpace(markedSection))
        {
            return string.Empty;
        }

        var fencedMatch = FencedJsonRegex().Match(markedSection);
        if (fencedMatch.Success)
        {
            return fencedMatch.Groups["json"].Value.Trim();
        }

        try
        {
            using var document = JsonDocument.Parse(markedSection);
            return document.RootElement.GetRawText();
        }
        catch (JsonException)
        {
            // Fall through to brace-based extraction for mixed text responses.
        }

        var startBrace = markedSection.IndexOf('{');
        var endBrace = markedSection.LastIndexOf('}');

        return startBrace >= 0 && endBrace > startBrace
            ? markedSection[startBrace..(endBrace + 1)].Trim()
            : markedSection.Trim();
    }

    [GeneratedRegex(@"```json\s*(?<json>\{[\s\S]*\})\s*```", RegexOptions.IgnoreCase)]
    private static partial Regex FencedJsonRegex();
}
