using Microsoft.ApplicationInsights;

namespace TrainingArchitect.Endpoints;

/// <summary>
/// Minimal API endpoint for collecting user feedback about generated AI responses.
/// Emits an OpenTelemetry event that can be correlated by response ID.
/// </summary>
public static class FeedbackEndpoints
{
    private static readonly HashSet<string> AllowedRequestTypes = new(StringComparer.Ordinal)
    {
        "assess.metrics",
        "assess.lastTraining",
        "assess.week",
        "plan"
    };

    private static readonly HashSet<string> AllowedRatings = new(StringComparer.Ordinal)
    {
        "positive",
        "negative"
    };

    public static IEndpointRouteBuilder MapFeedbackEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/feedback");

        group.MapPost("/", (
            HttpContext httpContext,
            FeedbackRequest request,
            TelemetryClient telemetryClient,
            ILoggerFactory loggerFactory) =>        {
            var logger = loggerFactory.CreateLogger("FeedbackEndpoints");

            if (!httpContext.Request.IsHttps)
            {
                logger.LogWarning("Rejected /api/feedback request because HTTPS is required.");
                return Results.Problem(
                    title: "HTTPS required.",
                    detail: "Feedback API only accepts HTTPS requests.",
                    statusCode: StatusCodes.Status400BadRequest);
            }

            if (!httpContext.Request.HasJsonContentType())
            {
                logger.LogWarning("Rejected /api/feedback request because content type is not JSON.");
                return Results.Problem(
                    title: "Unsupported media type.",
                    detail: "Feedback API only accepts application/json payloads.",
                    statusCode: StatusCodes.Status415UnsupportedMediaType);
            }

            if (!IsTrustedBrowserRequest(httpContext.Request))
            {
                logger.LogWarning("Rejected /api/feedback request due to untrusted browser context headers.");
                return Results.Problem(
                    title: "Forbidden.",
                    detail: "Cross-site requests are not allowed for this endpoint.",
                    statusCode: StatusCodes.Status403Forbidden);
            }

            var responseId = request.ResponseId?.Trim();
            var requestType = request.RequestType?.Trim();
            var rating = request.Rating?.Trim();
            var tags = request.Tags?.Trim();

            if (string.IsNullOrWhiteSpace(responseId))
            {
                logger.LogWarning("Rejected /api/feedback request because ResponseId is missing.");
                return Results.BadRequest(new { error = "ResponseId is required." });
            }

            if (string.IsNullOrWhiteSpace(requestType) || !AllowedRequestTypes.Contains(requestType))
            {
                logger.LogWarning("Rejected /api/feedback request because RequestType is invalid: {RequestType}", requestType);
                return Results.BadRequest(new { error = "RequestType is invalid." });
            }

            if (string.IsNullOrWhiteSpace(rating) || !AllowedRatings.Contains(rating))
            {
                logger.LogWarning("Rejected /api/feedback request because Rating is invalid: {Rating}", rating);
                return Results.BadRequest(new { error = "Rating is invalid." });
            }

            if (tags?.Length > 512)
            {
                tags = tags[..512];
            }
            var scoreValue = rating == "positive" ? "1.0" : "0.0";
            var scoreLabel = rating == "positive" ? "pass" : "fail";

            var internalProperties = System.Text.Json.JsonSerializer.Serialize(new Dictionary<string, object>
            {
                ["gen_ai.evaluation.type"] = "boolean",
                ["gen_ai.evaluation.min_value"] = 0.0,
                ["gen_ai.evaluation.max_value"] = 1.0,
                ["gen_ai.evaluation.threshold"] = 1.0,
                ["gen_ai.evaluation.desirable_direction"] = "increase"
            });


            telemetryClient.TrackEvent(
                "gen_ai.evaluation.result",
                new Dictionary<string, string>(StringComparer.Ordinal)
                {
                    ["gen_ai.evaluation.name"] = requestType,               // z.B. "assess.week" — dient als Gruppierungslabel in KQL
                    ["gen_ai.evaluation.score.value"] = scoreValue,
                    ["gen_ai.evaluation.score.label"] = scoreLabel,
                    ["gen_ai.response.id"] = responseId,
                    ["response_id"] = responseId,
                    ["microsoft.gen_ai.human_evaluation.source"] = "end_user",
                    ["internal_properties"] = internalProperties,
                    ["gen_ai.feedback.tags"] = tags ?? string.Empty          // eigenes Zusatzfeld, außerhalb des offiziellen Schemas, unproblematisch
                });

            logger.LogInformation(
                "Recorded user feedback for ResponseId={ResponseId}, RequestType={RequestType}, Rating={Rating}",
                responseId,
                requestType,
                rating);

            return Results.Accepted();
        });

        return app;
    }

    private static bool IsTrustedBrowserRequest(HttpRequest request)
    {
        var secFetchSite = request.Headers["Sec-Fetch-Site"].ToString();

        // Same-origin browser fetches are trusted even when Origin/Referer is stripped.
        if (string.Equals(secFetchSite, "same-origin", StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        if (string.Equals(secFetchSite, "cross-site", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        var originHeader = request.Headers.Origin.ToString();
        if (!string.IsNullOrWhiteSpace(originHeader))
        {
            return IsSameOrigin(originHeader, request);
        }

        var refererHeader = request.Headers.Referer.ToString();
        if (!string.IsNullOrWhiteSpace(refererHeader))
        {
            return IsSameOrigin(refererHeader, request);
        }

        return false;
    }

    private static bool IsSameOrigin(string originOrReferer, HttpRequest request)
    {
        if (!Uri.TryCreate(originOrReferer, UriKind.Absolute, out var sourceUri))
        {
            return false;
        }

        var targetHost = request.Host.Host;
        var targetScheme = request.Scheme;
        var targetPort = request.Host.Port
            ?? (string.Equals(targetScheme, "https", StringComparison.OrdinalIgnoreCase) ? 443 : 80);

        var sourcePort = sourceUri.IsDefaultPort
            ? (string.Equals(sourceUri.Scheme, "https", StringComparison.OrdinalIgnoreCase) ? 443 : 80)
            : sourceUri.Port;

        return string.Equals(sourceUri.Scheme, targetScheme, StringComparison.OrdinalIgnoreCase)
            && string.Equals(sourceUri.Host, targetHost, StringComparison.OrdinalIgnoreCase)
            && sourcePort == targetPort;
    }

    /// <summary>
    /// Feedback payload sent from the client widget.
    /// </summary>
    /// <param name="ResponseId">Response ID from the AI responses API.</param>
    /// <param name="RequestType">Feedback context key (assess.* or plan).</param>
    /// <param name="Rating">Rating value: positive or negative.</param>
    /// <param name="Tags">Optional comma-separated tags chosen by the user.</param>
    public sealed record FeedbackRequest(string ResponseId, string RequestType, string Rating, string? Tags);
}