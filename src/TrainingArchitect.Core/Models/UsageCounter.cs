namespace TrainingArchitect.Core.Models;

/// <summary>
/// Aggregated usage counters for a single athlete and period, stored in Cosmos DB.
/// </summary>
public sealed class UsageCounter : CosmosDocument
{
    public const string MonthlyUsageType = "monthly_usage";
    public const string WeeklyUsageType = "weekly_usage";
    public const int TimeToLiveSeconds = 365 * 24 * 60 * 60;

    /// <summary>
    /// Cosmos DB partition/type value, for example "monthly_usage" or "weekly_usage".
    /// </summary>
    public string UsageType { get; set; } = MonthlyUsageType;

    public override string Type => UsageType;

    public string AthleteId { get; set; } = string.Empty;

    public string PeriodKey { get; set; } = string.Empty;

    public Dictionary<string, int> Counts { get; set; } = new(StringComparer.Ordinal);

    public int TotalInputTokens { get; set; }

    public int TotalCachedTokens { get; set; }

    public int TotalOutputTokens { get; set; }
}