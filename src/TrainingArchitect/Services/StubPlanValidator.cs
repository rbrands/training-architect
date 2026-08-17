using System.Text.Json;
using TrainingArchitect.Core.Models;

namespace TrainingArchitect.Services;

/// <summary>
/// Placeholder validator that only checks structural integrity of the upload JSON.
/// The TSS verification will be added here once the corresponding MCP method is available.
/// </summary>
public sealed class StubPlanValidator : IPlanValidator
{
    /// <inheritdoc />
    public Task<PlanValidationResult> ValidateAsync(
        string uploadJson,
        PlanRequest request,
        string? intervalsAthleteId,
        string? intervalsApiKey,
        CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(uploadJson))
        {
            return Task.FromResult(PlanValidationResult.FromFinding(
                "plan.upload_json_missing",
                "The response did not contain an upload JSON block between BEGIN_UPLOAD_JSON and END_UPLOAD_JSON."));
        }

        try
        {
            using var document = JsonDocument.Parse(uploadJson);

            if (document.RootElement.ValueKind != JsonValueKind.Object)
            {
                return Task.FromResult(PlanValidationResult.FromFinding(
                    "plan.upload_json_not_an_object",
                    "The upload JSON must be a single JSON object."));
            }

            if (!document.RootElement.TryGetProperty("workouts", out var workouts)
                || workouts.ValueKind != JsonValueKind.Array
                || workouts.GetArrayLength() == 0)
            {
                return Task.FromResult(PlanValidationResult.FromFinding(
                    "plan.workouts_missing",
                    "The upload JSON must contain a non-empty \"workouts\" array."));
            }
        }
        catch (JsonException ex)
        {
            return Task.FromResult(PlanValidationResult.FromFinding(
                "plan.upload_json_invalid",
                $"The upload JSON could not be parsed: {ex.Message}"));
        }

        return Task.FromResult(PlanValidationResult.Valid);
    }
}
