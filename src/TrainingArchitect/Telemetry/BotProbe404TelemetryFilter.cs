using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;

namespace TrainingArchitect.Telemetry;

/// <summary>
/// Filters noisy 404 request telemetry caused by automated internet probes.
/// This keeps Application Insights failure alerts focused on real user-impacting issues.
/// </summary>
internal sealed class BotProbe404TelemetryFilter : ITelemetryProcessor
{
    private static readonly string[] SuspiciousPathFragments =
    [
        "/.env",
        "/wp-",
        "/wordpress",
        "/phpmyadmin",
        "/cgi-bin",
        "/server-status",
        "/.git",
        "/boaform",
        "/actuator",
        "/jenkins",
        "/webhook-test/",
        "/var/task/",
        "/src/config/",
        "/s3.key"
    ];

    private readonly ITelemetryProcessor _next;

    public BotProbe404TelemetryFilter(ITelemetryProcessor next)
    {
        _next = next;
    }

    public void Process(ITelemetry item)
    {
        if (item is RequestTelemetry request && ShouldSuppress(request))
        {
            return;
        }

        _next.Process(item);
    }

    private static bool ShouldSuppress(RequestTelemetry request)
    {
        if (!string.Equals(request.ResponseCode, "404", StringComparison.Ordinal))
        {
            return false;
        }

        var operationName = request.Name ?? string.Empty;
        if (operationName.Contains("/not-found", StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        var path = request.Url?.AbsolutePath ?? string.Empty;
        if (string.IsNullOrWhiteSpace(path))
        {
            return false;
        }

        return SuspiciousPathFragments.Any(fragment =>
            path.Contains(fragment, StringComparison.OrdinalIgnoreCase));
    }
}
