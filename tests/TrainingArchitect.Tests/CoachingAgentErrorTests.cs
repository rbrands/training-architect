using System.ClientModel.Primitives;
using System.Diagnostics;
using System.Net;
using System.Reflection;
using System.Text.Json;
using Azure;
using Azure.AI.Projects;
using Azure.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using TrainingArchitect.Endpoints;
using TrainingArchitect.Services;

namespace TrainingArchitect.Tests;

public sealed class CoachingAgentErrorTests
{
    private const string SensitiveText = "private-prompt-and-api-key";

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task PromptAsync_SdkFailure_LogsRedactedErrorDetails(bool azureException)
    {
        var logger = new RecordingLogger<FoundryCoachingAgent>();
        using var httpClient = new HttpClient(new ErrorHandler(
            """{"error":{"code":"server_error","type":"server_error","message":"Model routing failed: private-prompt-and-api-key","additionalInfo":{"request_id":"body-request"}}}""",
            500, "x-request-id", azureException));
        var agent = CreateAgent(httpClient, logger);
        using var activity = new Activity("coach-test").SetIdFormat(ActivityIdFormat.W3C).Start();

        var exception = await Assert.ThrowsAsync<CoachingAgentException>(() =>
            agent.PromptAsync(SensitiveText, "running", "en", intervalsApiKey: SensitiveText));

        Assert.Contains("HTTP 500", exception.Message);
        Assert.Equal(activity.TraceId.ToString(), exception.DiagnosticId);
        Assert.Null(exception.InnerException);
        var log = Assert.Single(logger.Messages);
        Assert.Contains("Status=500", log);
        Assert.Contains("Code=server_error", log);
        Assert.Contains(azureException ? "Azure.RequestFailedException" : "System.ClientModel.ClientResultException", log);
        Assert.Contains(azureException ? "body-request" : "header-request", log);
        Assert.Contains("test-agent@7", log);
        Assert.Contains(exception.DiagnosticId, log);
        Assert.Contains("Model routing failed: [REDACTED]", log);
        Assert.Contains("ExceptionDetails=", log);
        if (azureException)
        {
            Assert.Contains("Inner failure: [REDACTED]", log);
        }
        else
        {
            Assert.Contains("ResponseBody={", log);
        }
        Assert.DoesNotContain(SensitiveText, log);
        Assert.DoesNotContain(SensitiveText, exception.ToString());
    }

    [Fact]
    public async Task PromptAsync_ErrorDetails_RedactJsonEscapedSensitiveValues()
    {
        const string sensitiveValue = "private-\"quoted\"-value\nnext-line";
        var body = JsonSerializer.Serialize(new { error = new { message = $"Provider failure: {sensitiveValue}" } });
        var logger = new RecordingLogger<FoundryCoachingAgent>();
        using var httpClient = new HttpClient(new ErrorHandler(body, 500));

        await Assert.ThrowsAsync<CoachingAgentException>(() => CreateAgent(httpClient, logger)
            .PromptAsync("Assess metrics", "running", "en", intervalsApiKey: sensitiveValue));

        var log = Assert.Single(logger.Messages);
        Assert.Contains("Provider failure: [REDACTED]", log);
        Assert.DoesNotContain(sensitiveValue, log);
        Assert.DoesNotContain(JsonSerializer.Serialize(sensitiveValue)[1..^1], log);
    }

    [Theory]
    [InlineData("<html>private-prompt-and-api-key</html>", 502, "HTTP 502")]
    [InlineData("{broken", 500, "HTTP 500")]
    [InlineData("[]", 500, "HTTP 500")]
    [InlineData("null", 500, "could not complete")]
    [InlineData("{\"error\":{\"code\":{},\"message\":[]}}", 500, "HTTP 500")]
    [InlineData("{\"error\":{\"message\":\"Unsupported parameter: temperature\"}}", 400, "does not support the 'temperature'")]
    [InlineData("{\"message\":\"Unsupported parameter: temperature\"}", 400, "does not support the 'temperature'")]
    [InlineData("{}", 429, "rate limited")]
    [InlineData("{}", 401, "could not complete")]
    public async Task PromptAsync_ErrorShapes_ReturnSafeMessage(string body, int status, string expected)
    {
        var logger = new RecordingLogger<FoundryCoachingAgent>();
        using var httpClient = new HttpClient(new ErrorHandler(body, status));
        var exception = await Assert.ThrowsAsync<CoachingAgentException>(() =>
            CreateAgent(httpClient, logger).PromptAsync(SensitiveText, "running", "en"));

        Assert.Contains(expected, exception.Message);
        Assert.False(string.IsNullOrWhiteSpace(exception.DiagnosticId));
        Assert.DoesNotContain(SensitiveText, string.Join("\n", logger.Messages));
        Assert.DoesNotContain("<html>", exception.Message);
    }

    [Theory]
    [InlineData("x-request-id")]
    [InlineData("x-ms-request-id")]
    [InlineData("apim-request-id")]
    [InlineData(null)]
    public async Task PromptAsync_ExtractsRequestIdFromHeadersOrBody(string? header)
    {
        var logger = new RecordingLogger<FoundryCoachingAgent>();
        using var httpClient = new HttpClient(new ErrorHandler(
            """{"error":{"code":"server_error","message":"failed","additionalInfo":{"request_id":"body-request"}}}""", 500, header));

        await Assert.ThrowsAsync<CoachingAgentException>(() =>
            CreateAgent(httpClient, logger).PromptAsync("test", "running", "en"));

        var log = Assert.Single(logger.Messages);
        Assert.True(log.Contains($"RequestId={(header is null ? "body-request" : "header-request")}"), log);
    }

    [Fact]
    public void RemoveCitationMarkers_StripsLeakedFileCitations()
    {
        var method = typeof(FoundryCoachingAgent).GetMethod("RemoveCitationMarkers", BindingFlags.NonPublic | BindingFlags.Static)!;
        var text = "Verpflegung absichern. \uE200filecite\uE202turn0file0\uE202turn0file5\uE201\nNext line \uE202";

        var result = Assert.IsType<string>(method.Invoke(null, [text]));

        Assert.Equal("Verpflegung absichern.\nNext line ", result);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void AssessFailure_ProblemDetailsReachFormatterWithDiagnosticId(bool knownFailure)
    {
        var logger = new RecordingLogger<FoundryCoachingAgent>();
        var exception = knownFailure
            ? (Exception)new CoachingAgentException("Upstream HTTP 500. Please try again later.", "test-diagnostic")
            : new InvalidOperationException(SensitiveText);
        var method = typeof(CoachEndpoints).GetMethod("CreateAgentFailureResult", BindingFlags.NonPublic | BindingFlags.Static)!;
        var result = Assert.IsType<ProblemHttpResult>(method.Invoke(null, [new DefaultHttpContext(), exception, logger]));

        Assert.Equal(502, result.StatusCode);
        var diagnosticId = Assert.IsType<string>(result.ProblemDetails.Extensions["diagnosticId"]);
        Assert.False(string.IsNullOrWhiteSpace(diagnosticId));
        var json = JsonSerializer.Serialize(result.ProblemDetails, new JsonSerializerOptions(JsonSerializerDefaults.Web));
        var displayed = FormatError(json, 502);
        Assert.Contains(result.ProblemDetails.Detail!, displayed);
        Assert.Contains($"Diagnostic ID: {diagnosticId}", displayed);
        Assert.Contains(diagnosticId, Assert.Single(logger.Messages));
        Assert.DoesNotContain(SensitiveText, displayed);
        Assert.DoesNotContain(SensitiveText, string.Join("\n", logger.Messages));
    }

    [Theory]
    [InlineData("{\"error\":\"Monthly token limit reached.\"}", "Monthly token limit reached.")]
    [InlineData("{\"title\":\"Service unavailable\"}", "Service unavailable")]
    [InlineData("{\"detail\":\"Try later\",\"diagnosticId\":\"abc123\"}", "Try later Diagnostic ID: abc123.")]
    [InlineData("<html>private-prompt-and-api-key</html>", "Assess request failed (HTTP 502). Please try again.")]
    [InlineData("private-prompt-and-api-key", "Assess request failed (HTTP 502). Please try again.")]
    [InlineData("{broken", "Assess request failed (HTTP 502). Please try again.")]
    [InlineData("[]", "Assess request failed (HTTP 502). Please try again.")]
    public void FormatRequestError_HandlesProblemDetailsAndSafeFallback(string body, string expected)
    {
        Assert.Equal(expected, FormatError(body, 502));
    }

    [Theory]
    [InlineData("code")]
    [InlineData("type")]
    [InlineData("param")]
    [InlineData("request_id")]
    public async Task PromptAsync_DiagnosticFields_DoNotLogEchoedCredentials(string property)
    {
        var body = JsonSerializer.Serialize(new { error = new Dictionary<string, string> { [property] = SensitiveText } });
        var logger = new RecordingLogger<FoundryCoachingAgent>();
        using var httpClient = new HttpClient(new ErrorHandler(body, 500));

        await Assert.ThrowsAsync<CoachingAgentException>(() => CreateAgent(httpClient, logger)
            .PromptAsync("Assess metrics", "running", "en", intervalsApiKey: SensitiveText));

        Assert.DoesNotContain(SensitiveText, Assert.Single(logger.Messages));
    }

    [Fact]
    public async Task PromptAsync_UpstreamTimeout_IsLoggedAsAgentFailure()
    {
        var logger = new RecordingLogger<FoundryCoachingAgent>();
        using var httpClient = new HttpClient(new ErrorHandler("{}", 500, timeout: true));

        await Assert.ThrowsAsync<CoachingAgentException>(() => CreateAgent(httpClient, logger)
            .PromptAsync("Assess metrics", "running", "en"));

        Assert.Contains("Upstream request timed out.", Assert.Single(logger.Messages));
    }

    [Fact]
    public async Task PromptAsync_Cancellation_IsNotReportedAsAgentFailure()
    {
        var logger = new RecordingLogger<FoundryCoachingAgent>();
        using var httpClient = new HttpClient(new ErrorHandler("{}", 500));

        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => CreateAgent(httpClient, logger)
            .PromptAsync("Assess metrics", "running", "en", new CancellationToken(true)));

        Assert.Empty(logger.Messages);
    }

    private static string FormatError(string body, int status) =>
        (string)typeof(TrainingArchitect.Client.Pages.Coach)
            .GetMethod("FormatRequestError", BindingFlags.NonPublic | BindingFlags.Static)!
            .Invoke(null, ["Assess", body, status])!;

    private static FoundryCoachingAgent CreateAgent(HttpClient httpClient, ILogger<FoundryCoachingAgent> logger)
    {
        var options = new AIProjectClientOptions
        {
            Transport = new HttpClientPipelineTransport(httpClient),
            RetryPolicy = new ClientRetryPolicy(0)
        };
        var client = new AIProjectClient(new Uri("https://unit-test.invalid/api/projects/test"), new TestCredential(), options);
        var configuration = new ConfigurationBuilder().AddInMemoryCollection(
            new Dictionary<string, string?> { ["FoundryProjectAgentName"] = "test-agent@7" }).Build();
        return new FoundryCoachingAgent(client, configuration, logger);
    }

    private sealed class TestCredential : TokenCredential
    {
        public override AccessToken GetToken(TokenRequestContext requestContext, CancellationToken cancellationToken) =>
            new("__TEST_TOKEN__", DateTimeOffset.UtcNow.AddHours(1));

        public override ValueTask<AccessToken> GetTokenAsync(TokenRequestContext requestContext, CancellationToken cancellationToken) =>
            ValueTask.FromResult(GetToken(requestContext, cancellationToken));
    }

    private sealed class ErrorHandler(string body, int status, string? header = null, bool azureException = false, bool timeout = false) : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (timeout)
            {
                throw new TaskCanceledException("Upstream request timed out.");
            }
            if (azureException)
            {
                throw new RequestFailedException(status, body, "server_error",
                    new InvalidOperationException($"Inner failure: {SensitiveText}"));
            }
            var response = new HttpResponseMessage((HttpStatusCode)status)
            {
                Content = new StringContent(body, System.Text.Encoding.UTF8, "application/json")
            };
            if (header is not null)
            {
                response.Headers.Add(header, "header-request");
            }
            return Task.FromResult(response);
        }
    }

    private sealed class RecordingLogger<TCategory> : ILogger<TCategory>
    {
        public List<string> Messages { get; } = [];
        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;
        public bool IsEnabled(LogLevel logLevel) => true;
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter) =>
            Messages.Add(formatter(state, exception) + exception);
    }
}