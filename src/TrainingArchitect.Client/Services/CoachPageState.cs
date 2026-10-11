namespace TrainingArchitect.Client.Services;

/// <summary>
/// Keeps transient Coach page UI state across in-app navigation.
/// Lifetime is scoped to the current WASM session.
/// </summary>
public sealed class CoachPageState
{
    public string PlanSchedulingPreference { get; set; } = string.Empty;
    public string? ExpandedAssessCard { get; set; }

    public string? MetricsResultText { get; set; }
    public string? LastTrainingResultText { get; set; }
    public string? WeekResultText { get; set; }
    public string? MetricsResponseId { get; set; }
    public string? LastTrainingResponseId { get; set; }
    public string? WeekResponseId { get; set; }

    public long? MetricsTotalTokens { get; set; }
    public long? LastTrainingTotalTokens { get; set; }
    public long? WeekTotalTokens { get; set; }

    public string? MetricsErrorMessage { get; set; }
    public string? LastTrainingErrorMessage { get; set; }
    public string? WeekErrorMessage { get; set; }

    public bool IsPlanExpanded { get; set; }
    public string? PlanReadableText { get; set; }
    public string? PlanUploadJson { get; set; }
    public string? PlanErrorMessage { get; set; }
    public long? PlanTotalTokens { get; set; }
    public string? PlanResponseId { get; set; }
    public int SelectedPlanTabIndex { get; set; }
    public string? OpenOverviewSectionTitle { get; set; }
}
