using TrainingArchitect.Core.Models;
using TrainingArchitect.Services;

namespace TrainingArchitect.Tests;

public sealed class PromptBuilderTests
{
    [Fact]
    public void BuildAssessPrompt_Metrics_UsesMetricsPrompt()
    {
        var request = new AssessRequest("{\"ftp\":250}", "road race", "en", AssessmentType.Metrics);

        var prompt = PromptBuilder.BuildAssessPrompt(request);

        Assert.Contains("Summarize the current metrics and wellness data.", prompt);
        Assert.Contains("{\"ftp\":250}", prompt);
    }
}
