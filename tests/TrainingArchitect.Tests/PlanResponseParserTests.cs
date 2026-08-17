using TrainingArchitect.Core.Services;

namespace TrainingArchitect.Tests;

public class PlanResponseParserTests
{
    [Fact]
    public void Split_WhenMarkersWrapFencedJson_ReturnsPlanTextAndJson()
    {
        const string response = """
            Here is your week.

            BEGIN_UPLOAD_JSON
            ```json
            { "workouts": [ { "date": "2026-08-17" } ] }
            ```
            END_UPLOAD_JSON
            """;

        var (planText, uploadJson) = PlanResponseParser.Split(response);

        Assert.Equal("Here is your week.", planText);
        Assert.Equal("""{ "workouts": [ { "date": "2026-08-17" } ] }""", uploadJson);
    }

    [Fact]
    public void Split_WhenMarkersWrapBareJson_ReturnsNormalizedJson()
    {
        const string response = """
            Plan text.

            BEGIN_UPLOAD_JSON
            {"workouts":[]}
            END_UPLOAD_JSON
            """;

        var (planText, uploadJson) = PlanResponseParser.Split(response);

        Assert.Equal("Plan text.", planText);
        Assert.Equal("""{"workouts":[]}""", uploadJson);
    }

    [Fact]
    public void Split_WhenJsonIsSurroundedByProse_FallsBackToBraceExtraction()
    {
        const string response = """
            Plan text.

            BEGIN_UPLOAD_JSON
            Use this payload: {"workouts":[]} and upload it.
            END_UPLOAD_JSON
            """;

        var (_, uploadJson) = PlanResponseParser.Split(response);

        Assert.Equal("""{"workouts":[]}""", uploadJson);
    }

    [Fact]
    public void Split_WhenMarkersAreMissing_ReturnsWholeTextAsPlan()
    {
        const string response = "Just a plan without any upload block.";

        var (planText, uploadJson) = PlanResponseParser.Split(response);

        Assert.Equal(response, planText);
        Assert.Equal(string.Empty, uploadJson);
    }

    [Fact]
    public void Split_WhenResponseIsEmpty_ReturnsEmptyParts()
    {
        var (planText, uploadJson) = PlanResponseParser.Split("   ");

        Assert.Equal(string.Empty, planText);
        Assert.Equal(string.Empty, uploadJson);
    }
}
