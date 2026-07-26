namespace TrainingArchitect.Client.Components;

public static class FeedbackTags
{
    public static readonly IReadOnlyDictionary<string, string[]> ByRequestType =
        new Dictionary<string, string[]>(StringComparer.Ordinal)
        {
            ["assess.metrics"] =
            [
                "Metric calculated incorrectly",
                "TSB recommendation not sound",
                "Wellness data ignored",
                "Too generic",
                "Other"
            ],
            ["assess.lastTraining"] =
            [
                "Session type/intensity misclassified",
                "Load recommendation off",
                "Recovery advice not appropriate",
                "Too generic",
                "Other"
            ],
            ["assess.week"] =
            [
                "Ramp rate assessment incorrect",
                "Training phase misjudged",
                "Too generic",
                "Other"
            ],
            ["plan"] =
            [
                "Volume not appropriate",
                "Intensity distribution off",
                "Session type doesn't fit goal",
                "Referenced event from wrong week",
                "Confused current week with next week",
                "Schedule constraints ignored",
                "Too generic",
                "Other"
            ]
        };
}