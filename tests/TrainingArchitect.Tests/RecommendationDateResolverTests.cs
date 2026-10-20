using TrainingArchitect.Core.Services;

namespace TrainingArchitect.Tests;

public class RecommendationDateResolverTests
{
    [Theory]
    [InlineData("{\"current_date\":\"2026-10-17\"}", "2026-10-17")]
    [InlineData("{\"current_date\":\"2026-10-17\",\"activities\":[]}", "2026-10-17")]
    [InlineData("{\"current_date\":\"2026-10-17\",\"activities\":[{\"date\":\"2026-10-16\"}]}", "2026-10-17")]
    [InlineData("{\"current_date\":\"2026-10-17\",\"activities\":[{\"date\":\"2026-10-17\",\"type\":\"Walk\"}]}", "2026-10-18")]
    [InlineData("{\"current_date\":\"2026-10-18\",\"activities\":[{\"date\":\"2026-10-18\"}]}", "2026-10-19")]
    [InlineData("{\"current_date\":\"2026-12-31\",\"activities\":[{\"date\":\"2026-12-31\"}]}", "2027-01-01")]
    [InlineData("{\"current_date\":\"2026-10-18\",\"activities\":[null,1,{}, {\"date\":7}]}", "2026-10-18")]
    [InlineData("{\"current_date\":\"2026-10-18\",\"activities\":[{\"start_date_local\":\"2026-10-18T09:00:00\"}]}", "2026-10-18")]
    public void Resolve_UsesCurrentDateAndAnyMatchingActivity(string json, string expected) =>
        Assert.Equal(DateOnly.ParseExact(expected, "yyyy-MM-dd"), RecommendationDateResolver.Resolve(json));

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("not JSON")]
    [InlineData("{}")]
    [InlineData("[]")]
    [InlineData("{\"current_date\":\"invalid\"}")]
    [InlineData("{\"current_date\":\"2026-02-30\"}")]
    [InlineData("{\"current_date\":123}")]
    public void Resolve_MissingOrInvalidDateReturnsNull(string? json) =>
        Assert.Null(RecommendationDateResolver.Resolve(json));
}