using System.Text.Json;
using TrainingArchitect.Core.Models;
using TrainingArchitect.Core.Services;

namespace TrainingArchitect.Services;

/// <summary>
/// Validates the structure of the upload JSON and verifies the weekly TSS via the
/// <c>check_plan_tss</c> MCP tool. The TSS check is a backstop: technical failures never
/// block plan generation.
/// </summary>
public sealed class TssPlanValidator(
    IAthleteDataService athleteDataService,
    ILogger<TssPlanValidator> logger) : IPlanValidator
{
    /// <inheritdoc />
    public async Task<PlanValidationResult> ValidateAsync(
        string uploadJson,
        PlanRequest request,
        string? intervalsAthleteId,
        string? intervalsApiKey,
        CancellationToken ct = default)
    {
        var structuralResult = ValidateStructure(uploadJson);
        if (!structuralResult.IsValid)
        {
            return structuralResult;
        }

        var loadTarget = request.Constraints?.WeeklyTssTarget is > 0
            ? request.Constraints.WeeklyTssTarget.Value
            : WeeklyLoadTargetResolver.Resolve(request.WeekDataJson, request.Scope);

        if (loadTarget is null or <= 0)
        {
            logger.LogWarning("Skipping TSS validation because no weekly load target could be resolved.");
            return WithSummary(PlanValidationResult.Valid, "TSS check skipped: no weekly load target in the dataset.");
        }

        if (string.IsNullOrWhiteSpace(intervalsAthleteId) || string.IsNullOrWhiteSpace(intervalsApiKey))
        {
            logger.LogWarning("Skipping TSS validation because intervals.icu credentials are missing.");
            return WithSummary(PlanValidationResult.Valid, "TSS check skipped: intervals.icu credentials are missing.");
        }

        PlanTssCheckResult checkResult;

        try
        {
            checkResult = await athleteDataService.CheckPlanTssAsync(
                intervalsAthleteId,
                intervalsApiKey,
                uploadJson,
                loadTarget.Value,
                tolerancePct:  null,
                ct);
        }
        catch (OperationCanceledException) when (ct.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "TSS validation could not be performed; accepting the plan without a TSS check.");
            return WithSummary(PlanValidationResult.Valid, "TSS check unavailable: the plan was accepted without verification.");
        }

        var summary = BuildSummary(checkResult);

        if (checkResult.Valid)
        {
            return WithSummary(PlanValidationResult.Valid, summary);
        }

        var issues = checkResult.Issues.Where(issue => !string.IsNullOrWhiteSpace(issue)).ToArray();
        var findings = issues.Length == 0
            ? [new PlanValidationFinding("plan.tss_deviation", BuildFallbackMessage(checkResult))]
            : issues.Select(issue => new PlanValidationFinding("plan.tss_deviation", issue)).ToArray();

        logger.LogInformation(
            "TSS validation reported {FindingCount} finding(s) for the generated plan.",
            findings.Length);

        return new PlanValidationResult(findings) { Summary = summary };
    }

    private static PlanValidationResult WithSummary(PlanValidationResult result, string summary) =>
        result with { Summary = summary };

    private static string BuildSummary(PlanTssCheckResult checkResult)
    {
        if (checkResult.Week is not { } week)
        {
            return checkResult.Valid ? "TSS check passed." : "TSS check failed.";
        }

        var outcome = checkResult.Valid ? "TSS check passed" : "TSS check failed";
        return $"{outcome}: {week.TotalTss:0} TSS vs. target {week.LoadTarget:0} ({week.DeviationPct:+0.#;-0.#;0}%).";
    }

    private static PlanValidationResult ValidateStructure(string uploadJson)
    {
        if (string.IsNullOrWhiteSpace(uploadJson))
        {
            return PlanValidationResult.FromFinding(
                "plan.upload_json_missing",
                "The response did not contain an upload JSON block between BEGIN_UPLOAD_JSON and END_UPLOAD_JSON.");
        }

        try
        {
            using var document = JsonDocument.Parse(uploadJson);

            if (document.RootElement.ValueKind != JsonValueKind.Object)
            {
                return PlanValidationResult.FromFinding(
                    "plan.upload_json_not_an_object",
                    "The upload JSON must be a single JSON object.");
            }

            if (!document.RootElement.TryGetProperty("workouts", out var workouts)
                || workouts.ValueKind != JsonValueKind.Array
                || workouts.GetArrayLength() == 0)
            {
                return PlanValidationResult.FromFinding(
                    "plan.workouts_missing",
                    "The upload JSON must contain a non-empty \"workouts\" array.");
            }
        }
        catch (JsonException ex)
        {
            return PlanValidationResult.FromFinding(
                "plan.upload_json_invalid",
                $"The upload JSON could not be parsed: {ex.Message}");
        }

        return PlanValidationResult.Valid;
    }

    private static string BuildFallbackMessage(PlanTssCheckResult checkResult) =>
        checkResult.Week is { } week
            ? $"The weekly TSS of {week.TotalTss:0.#} deviates by {week.DeviationPct:0.#}% from the target of {week.LoadTarget:0.#}."
            : "The weekly TSS check failed without further details.";
}
