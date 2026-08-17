using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http.Features;

namespace TrainingArchitect.Endpoints;

/// <summary>
/// Writes Server-Sent Events to a response body and keeps the connection alive with periodic heartbeats.
/// </summary>
public sealed class ServerSentEventWriter : IAsyncDisposable
{
    private static readonly TimeSpan HeartbeatInterval = TimeSpan.FromSeconds(20);
    private static readonly byte[] HeartbeatFrame = ":ping\n\n"u8.ToArray();

    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter() }
    };

    private readonly HttpResponse _response;
    private readonly SemaphoreSlim _writeLock = new(1, 1);
    private readonly CancellationTokenSource _heartbeatCts;
    private readonly Task _heartbeatTask;

    private ServerSentEventWriter(HttpResponse response, CancellationToken ct)
    {
        _response = response;
        _heartbeatCts = CancellationTokenSource.CreateLinkedTokenSource(ct);
        _heartbeatTask = RunHeartbeatAsync(_heartbeatCts.Token);
    }

    /// <summary>
    /// Switches the response into streaming mode and starts the heartbeat.
    /// </summary>
    /// <remarks>Must be called before anything else writes to the response body.</remarks>
    public static ServerSentEventWriter Start(HttpContext httpContext, CancellationToken ct)
    {
        var response = httpContext.Response;
        response.StatusCode = StatusCodes.Status200OK;
        response.ContentType = "text/event-stream";
        response.Headers.CacheControl = "no-cache";
        // Prevents buffering in reverse proxies such as Cloudflare and App Service ARR.
        response.Headers["X-Accel-Buffering"] = "no";

        httpContext.Features.Get<IHttpResponseBodyFeature>()?.DisableBuffering();

        return new ServerSentEventWriter(response, ct);
    }

    /// <summary>
    /// Serializes a payload as a single SSE data frame and flushes it.
    /// </summary>
    public async Task WriteEventAsync<T>(T payload, CancellationToken ct = default)
    {
        var frame = Encoding.UTF8.GetBytes($"data: {JsonSerializer.Serialize(payload, SerializerOptions)}\n\n");

        await _writeLock.WaitAsync(ct);
        try
        {
            await _response.Body.WriteAsync(frame, ct);
            await _response.Body.FlushAsync(ct);
        }
        finally
        {
            _writeLock.Release();
        }
    }

    private async Task RunHeartbeatAsync(CancellationToken ct)
    {
        using var timer = new PeriodicTimer(HeartbeatInterval);

        try
        {
            while (await timer.WaitForNextTickAsync(ct))
            {
                await _writeLock.WaitAsync(ct);
                try
                {
                    await _response.Body.WriteAsync(HeartbeatFrame, ct);
                    await _response.Body.FlushAsync(ct);
                }
                finally
                {
                    _writeLock.Release();
                }
            }
        }
        catch (OperationCanceledException)
        {
            // Expected when the run finishes or the client disconnects.
        }
        catch (Exception)
        {
            // A broken pipe must not fault the orchestration task.
        }
    }

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        await _heartbeatCts.CancelAsync();

        try
        {
            await _heartbeatTask;
        }
        catch (OperationCanceledException)
        {
            // Expected.
        }

        _heartbeatCts.Dispose();
        _writeLock.Dispose();
    }
}
