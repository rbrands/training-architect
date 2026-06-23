using TrainingArchitect.Services;

namespace TrainingArchitect.Endpoints;

/// <summary>
/// Minimal API endpoint for retrieving athlete data from intervals.icu.
/// Credentials are forwarded to the application service and not persisted server-side.
/// </summary>
public static class AthleteDataEndpoints
{
    public static IEndpointRouteBuilder MapAthleteDataEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/athlete-data");

        group.MapPost("/", async (
            AthleteDataRequest request,
            IAthleteDataService athleteDataService,
            CancellationToken ct) =>
        {
            try
            {
                var response = await athleteDataService.GetAsync(request, ct);
                return Results.Ok(response);
            }
            catch (Exception ex)
            {
                return Results.Problem(
                    title: "Failed to retrieve athlete data.",
                    detail: ex.Message);
            }
        });

        return app;
    }
}

/// <summary>Request payload for athlete data retrieval.</summary>
public record AthleteDataRequest
{
    public string AthleteId { get; init; } = string.Empty;
    public string ApiKey { get; init; } = string.Empty;
    public string DataScope { get; init; } = string.Empty;
}

/// <summary>Response payload returned for athlete data retrieval.</summary>
public record AthleteDataResponse
{
    public string AthleteId { get; init; } = string.Empty;
    public string DataScope { get; init; } = string.Empty;
    public Dictionary<string, object?> Data { get; init; } = new();
}