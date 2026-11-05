using System.Text.Json;
using TrainingArchitect.Core.Models;

namespace TrainingArchitect.Core.Services;

/// <summary>Resolves an athlete's strength phase preference for a planning scope.</summary>
public static class StrengthPhaseSelectionResolver
{
    /// <summary>Gets the data-suggested phase for the requested planning scope, if present.</summary>
    public static string? ResolveAutomaticPhase(
        string? weekDataJson,
        PlanningScope scope,
        PlanningMode mode = PlanningMode.Week)
    {
        if (string.IsNullOrWhiteSpace(weekDataJson))
        {
            return null;
        }

        try
        {
            using var document = JsonDocument.Parse(weekDataJson);
            if (document.RootElement.ValueKind != JsonValueKind.Object
                || !document.RootElement.TryGetProperty("week_summary", out var weekSummary)
                || weekSummary.ValueKind != JsonValueKind.Object
                || !weekSummary.TryGetProperty("strength", out var strength)
                || strength.ValueKind != JsonValueKind.Object)
            {
                return null;
            }

            var propertyName = mode == PlanningMode.NextTraining || scope == PlanningScope.CurrentWeek
                ? "phase_current_week"
                : "phase_next_week";

            if (!strength.TryGetProperty(propertyName, out var phase)
                || phase.ValueKind != JsonValueKind.String)
            {
                return null;
            }

            return NormalizePhase(phase.GetString());
        }
        catch (JsonException)
        {
            return null;
        }
    }

    /// <summary>Resolves a selection to its canonical lowercase prompt phase.</summary>
    public static string Resolve(
        StrengthPhaseSelection selection,
        string? weekDataJson,
        PlanningScope scope,
        PlanningMode mode = PlanningMode.Week) =>
        selection switch
        {
            StrengthPhaseSelection.Automatic => ResolveAutomaticPhase(weekDataJson, scope, mode) ?? "none",
            StrengthPhaseSelection.Adaptation => "adaptation",
            StrengthPhaseSelection.Transition => "transition",
            StrengthPhaseSelection.Maximum => "maximum",
            StrengthPhaseSelection.Maintenance => "maintenance",
            StrengthPhaseSelection.None => "none",
            _ => throw new ArgumentOutOfRangeException(nameof(selection), selection, null)
        };

    private static string? NormalizePhase(string? phase) => phase?.Trim().ToLowerInvariant() switch
    {
        "adaptation" => "adaptation",
        "transition" => "transition",
        "maximum" => "maximum",
        "maintenance" => "maintenance",
        _ => null
    };
}
