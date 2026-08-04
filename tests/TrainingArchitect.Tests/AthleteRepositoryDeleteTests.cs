using TrainingArchitect.Core.Models;

namespace TrainingArchitect.Tests;

public class AthleteRepositoryDeleteTests
{
    [Fact]
    public void GetDeleteId_ShouldPreferStoredDocumentId_WhenAvailable()
    {
        var athlete = new AthleteConfig
        {
            Id = "legacy-document-id",
            AthleteId = "athlete-123"
        };

        Assert.Equal("legacy-document-id", athlete.GetDeleteId());
    }

    [Fact]
    public void GetDeleteId_ShouldFallbackToAthleteId_WhenDocumentIdIsMissing()
    {
        var athlete = new AthleteConfig
        {
            AthleteId = "athlete-456"
        };

        Assert.Equal("athlete-456", athlete.GetDeleteId());
    }
}
