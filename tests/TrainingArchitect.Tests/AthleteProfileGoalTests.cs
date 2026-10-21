using System.Reflection;
using System.Text.Json;
using TrainingArchitect.Client.Pages;
using TrainingArchitect.Client.Services;
using TrainingArchitect.Core.Models;
using TrainingArchitect.Services;

namespace TrainingArchitect.Tests;

public class AthleteProfileGoalTests
{
    [Theory]
    [InlineData(AthleteProfileGoal.Climber, "climber")]
    [InlineData(AthleteProfileGoal.Criterium, "criterium")]
    [InlineData(AthleteProfileGoal.Marathon, "marathon")]
    [InlineData(AthleteProfileGoal.RoadRace, "roadrace")]
    [InlineData(AthleteProfileGoal.Fitness, "fitness")]
    [InlineData(AthleteProfileGoal.Cross, "cross")]
    [InlineData(AthleteProfileGoal.ZwiftRacing, "zwift")]
    public void CoachRequests_UseSelectedGoalAsDisciplineType(AthleteProfileGoal goal, string expected)
    {
        var coach = new Coach();
        typeof(Coach).GetProperty("Session", BindingFlags.Instance | BindingFlags.NonPublic)!
            .SetValue(coach, new ServerAthleteSession());
        typeof(Coach).GetField("_profileGoal", BindingFlags.Instance | BindingFlags.NonPublic)!
            .SetValue(coach, goal);

        var plan = (PlanRequest)typeof(Coach)
            .GetMethod("BuildPlanRequest", BindingFlags.Instance | BindingFlags.NonPublic)!
            .Invoke(coach, null)!;
        var buildAssessment = typeof(Coach)
            .GetMethod("BuildAssessRequest", BindingFlags.Instance | BindingFlags.NonPublic)!;
        var cardType = Enum.Parse(buildAssessment.GetParameters()[0].ParameterType, "MetricsWellness");
        var assessment = (AssessRequest)buildAssessment.Invoke(coach, [cardType])!;

        Assert.Equal(expected, plan.DisciplineType);
        Assert.Equal(expected, assessment.DisciplineType);
    }

    [Theory]
    [InlineData(0, AthleteProfileGoal.Climber)]
    [InlineData(1, AthleteProfileGoal.Criterium)]
    [InlineData(2, AthleteProfileGoal.Marathon)]
    [InlineData(3, AthleteProfileGoal.RoadRace)]
    [InlineData(4, AthleteProfileGoal.Fitness)]
    [InlineData(5, AthleteProfileGoal.Cross)]
    public void StoredCredential_ExistingGoalValuesRemainCompatible(int storedValue, AthleteProfileGoal expected)
    {
        var credential = JsonSerializer.Deserialize<StoredCredential>($"{{\"ProfileGoal\":{storedValue}}}");

        Assert.NotNull(credential);
        Assert.Equal(expected, credential.ProfileGoal);
    }

    [Fact]
    public void StoredCredential_FitnessRoundTrips()
    {
        var credential = new StoredCredential { ProfileGoal = AthleteProfileGoal.Fitness };

        var restored = JsonSerializer.Deserialize<StoredCredential>(JsonSerializer.Serialize(credential));

        Assert.NotNull(restored);
        Assert.Equal(AthleteProfileGoal.Fitness, restored.ProfileGoal);
    }

    [Fact]
    public void StoredCredential_CrossRoundTrips()
    {
        var credential = new StoredCredential { ProfileGoal = AthleteProfileGoal.Cross };

        var restored = JsonSerializer.Deserialize<StoredCredential>(JsonSerializer.Serialize(credential));

        Assert.NotNull(restored);
        Assert.Equal(AthleteProfileGoal.Cross, restored.ProfileGoal);
    }

    [Fact]
    public void StoredCredential_ZwiftRacingRoundTrips()
    {
        var credential = new StoredCredential { ProfileGoal = AthleteProfileGoal.ZwiftRacing };

        var restored = JsonSerializer.Deserialize<StoredCredential>(JsonSerializer.Serialize(credential));

        Assert.NotNull(restored);
        Assert.Equal(AthleteProfileGoal.ZwiftRacing, restored.ProfileGoal);
    }
}
