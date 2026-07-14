using Erp.WmsConnector.Shared.Abstractions.FeatureManagement;
using Erp.WmsConnector.Shared.Infrastructure.DataRetention.Options;
using Erp.WmsConnector.Shared.Infrastructure.Logging;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.FeatureManagement;

namespace Erp.WmsConnector.Shared.Infrastructure.DataRetention;

[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage(Justification = "Background service infrastructure code that runs continuously in the background")]
internal sealed class DataRetentionService(
    DataRetentionProcessor processor,
    IOptions<DataRetentionSchedulerOptions> options,
    IFeatureManager featureManager,
    ILogger<DataRetentionService> logger) : BackgroundService
{
    private readonly DataRetentionSchedulerOptions _options = options.Value;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!await featureManager.IsEnabledAsync(FeatureFlags.Shared.DataRetention))
        {
            logger.LogInformationWithCode(DataRetentionLogCodes.DisabledFeature, "Data retention service is disabled by feature flag");
            return;
        }

        if (!_options.Enabled)
        {
            logger.LogInformationWithCode(DataRetentionLogCodes.DisabledConfig, "Data retention service is disabled by configuration");
            return;
        }

        if (_options.StartAtHourUtc is < 0 or > 23)
        {
            logger.LogWithCode(
                LogLevel.Warning,
                DataRetentionLogCodes.InvalidSettings,
                "Invalid data retention scheduler settings. StartAtHourUtc: {StartAtHourUtc}",
                _options.StartAtHourUtc);
            return;
        }

        try
        {
            int taskCount = processor.GetRegisteredTaskCount();
            logger.LogInformationWithCode(
                DataRetentionLogCodes.Started,
                "Data retention service started. RunOnStartup: {RunOnStartup} StartAtHourUtc: {StartAtHourUtc} IntervalHours: {IntervalHours} TaskCount: {TaskCount}",
                _options.RunOnStartup,
                _options.StartAtHourUtc,
                DataRetentionSchedulerOptions.IntervalHours,
                taskCount);

            if (_options.RunOnStartup)
            {
                await processor.RunSweepAsync(stoppingToken);
            }

            while (!stoppingToken.IsCancellationRequested)
            {
                DateTime nextRunUtc = DataRetentionScheduler.CalculateNextRunUtc(DateTime.UtcNow, _options.StartAtHourUtc);
                TimeSpan delay = nextRunUtc - DateTime.UtcNow;
                if (delay < TimeSpan.Zero)
                {
                    delay = TimeSpan.Zero;
                }

                logger.LogInformationWithCode(DataRetentionLogCodes.NextRun, "Next data retention sweep scheduled at {NextRunUtc}", nextRunUtc);
                await Task.Delay(delay, stoppingToken);
                await processor.RunSweepAsync(stoppingToken);
            }
        }
        catch (OperationCanceledException ex) when (stoppingToken.IsCancellationRequested)
        {
            logger.LogInformationWithCode(ex, DataRetentionLogCodes.Stopping, "Data retention service is stopping");
        }
        catch (Exception ex)
        {
            logger.LogErrorWithCode(ex, DataRetentionLogCodes.Failed, "Data retention service failed");
            throw new DataRetentionServiceException(ex);
        }
    }
}
