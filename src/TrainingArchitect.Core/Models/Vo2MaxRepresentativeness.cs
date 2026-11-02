namespace TrainingArchitect.Core.Models;

/// <summary>
/// Describes whether recent five-minute power represents a maximal effort.
/// </summary>
public enum Vo2MaxEstimateRepresentativeness
{
    /// <summary>The estimate is based on a representative effort.</summary>
    Representative,

    /// <summary>Recent data probably lacks a maximal effort.</summary>
    LikelyUnderstated,

    /// <summary>The estimate is not based on a representative effort.</summary>
    NotRepresentative
}

/// <summary>
/// Carries optional VO2max estimate quality information from vo2max_classification.
/// A null status means the source did not assess representativeness.
/// </summary>
/// <param name="EstimateRepresentative">The source's assessment of the estimate.</param>
/// <param name="P5minPctFtp">Best five-minute power as a percentage of FTP.</param>
/// <param name="PeriodDays">The power profile's observation period in days.</param>
public sealed record Vo2MaxRepresentativeness(
    Vo2MaxEstimateRepresentativeness? EstimateRepresentative,
    double? P5minPctFtp,
    int? PeriodDays);
