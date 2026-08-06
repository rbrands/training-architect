using TrainingArchitect.Core.Interfaces;

namespace TrainingArchitect.Services;

/// <summary>
/// Periodically rebuilds global monthly/weekly usage counters from athlete-level usage documents.
/// </summary>
public sealed class GlobalUsageCounterRefreshService(
    IUsageCounterRepository usageCounterRepository,
    ILogger<GlobalUsageCounterRefreshService> logger) : BackgroundService
{
    private static readonly TimeSpan RefreshInterval = TimeSpan.FromMinutes(5);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Run one refresh at startup so global counters are available quickly.
        await RefreshSafeAsync(stoppingToken);

        using var timer = new PeriodicTimer(RefreshInterval);
        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            await RefreshSafeAsync(stoppingToken);
        }
    }

    private async Task RefreshSafeAsync(CancellationToken ct)
    {
        try
        {
            await usageCounterRepository.RefreshGlobalCountersAsync(ct);
        }
        catch (OperationCanceledException) when (ct.IsCancellationRequested)
        {
            // Graceful shutdown.
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Global usage counter refresh failed.");
        }
    }
}
