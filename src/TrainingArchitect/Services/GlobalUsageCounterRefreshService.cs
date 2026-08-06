using TrainingArchitect.Core.Interfaces;

namespace TrainingArchitect.Services;

/// <summary>
/// Periodically rebuilds global monthly/weekly usage counters from athlete-level usage documents.
/// </summary>
public sealed class GlobalUsageCounterRefreshService(
    IUsageCounterRepository usageCounterRepository,
    IConfiguration configuration,
    ILogger<GlobalUsageCounterRefreshService> logger) : BackgroundService
{
    private static readonly TimeSpan RefreshInterval = TimeSpan.FromMinutes(10);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!IsGlobalRefreshEnabledForCurrentSlot())
        {
            logger.LogInformation(
                "Global usage counter refresh service is disabled for non-production slot '{SlotName}'.",
                ResolveSlotName() ?? "unknown");
            return;
        }

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

    private bool IsGlobalRefreshEnabledForCurrentSlot()
    {
        var slotName = ResolveSlotName();
        return string.IsNullOrWhiteSpace(slotName)
            || string.Equals(slotName, "production", StringComparison.OrdinalIgnoreCase);
    }

    private string? ResolveSlotName()
    {
        return configuration["WEBSITE_SLOT_NAME"]
            ?? Environment.GetEnvironmentVariable("WEBSITE_SLOT_NAME");
    }
}
