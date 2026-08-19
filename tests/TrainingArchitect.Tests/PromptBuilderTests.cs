using TrainingArchitect.Core.Models;
using TrainingArchitect.Services;

namespace TrainingArchitect.Tests;

public sealed class PromptBuilderTests
{
    [Fact]
    public void BuildAssessPrompt_Consistency_UsesConsistencyPrompt()
    {
        var request = new AssessRequest("{\"ftp\":250}", "road race", "en", AssessmentType.Consistency);

        var prompt = PromptBuilder.BuildAssessPrompt(request);

        Assert.Contains("Check the attached athlete data for completeness and internal consistency.", prompt);
        Assert.Contains("{\"ftp\":250}", prompt);
        Assert.DoesNotContain("Summarize the current metrics and wellness data.", prompt);
    }
}
