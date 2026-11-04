using System.Reflection;
using TrainingArchitect.Client.Pages;

namespace TrainingArchitect.Tests;

public class CoachPlanResultTests
{
    [Theory]
    [InlineData(null, false)]
    [InlineData("_planReadableText", true)]
    [InlineData("_planUploadJson", true)]
    [InlineData("_planErrorMessage", true)]
    public void HasPlanResult_RequestJsonAloneDoesNotExposeResult(string? resultField, bool expected)
    {
        var page = new Coach();
        const BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;
        var requestField = typeof(Coach).GetField("_planRequestJson", flags);
        Assert.NotNull(requestField);
        requestField.SetValue(page, """{"weekDataJson":"{}"}""");

        if (resultField is not null)
        {
            var field = typeof(Coach).GetField(resultField, flags);
            Assert.NotNull(field);
            field.SetValue(page, resultField == "_planUploadJson" ? """{"workouts":[]}""" : "Plan outcome");
        }

        var hasPlanResult = typeof(Coach).GetProperty("HasPlanResult", flags);
        Assert.NotNull(hasPlanResult);
        Assert.Equal(expected, hasPlanResult.GetValue(page));
    }
}
