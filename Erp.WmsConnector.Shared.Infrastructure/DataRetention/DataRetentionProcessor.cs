using Erp.WmsConnector.Shared.Abstractions.DataRetention;
using Erp.WmsConnector.Shared.Infrastructure.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Erp.WmsConnector.Shared.Infrastructure.DataRetention;

internal sealed class DataRetentionProcessor(
    IServiceScopeFactory scopeFactory,
    ILogger<DataRetentionProcessor> logger)
{
    public int GetRegisteredTaskCount()
    {
        using IServiceScope scope = scopeFactory.CreateScope();
        return scope.ServiceProvider.GetServices<IDataRetentionTask>().Count();
    }

    public async Task RunSweepAsync(CancellationToken cancellationToken)
    {
        using IServiceScope scope = scopeFactory.CreateScope();
        IDataRetentionTask[] tasks = scope.ServiceProvider.GetServices<IDataRetentionTask>().ToArray();
        logger.LogInformationWithCode(DataRetentionLogCodes.SweepStarted, "Running data retention sweep. TaskCount: {TaskCount}", tasks.Length);

        foreach (IDataRetentionTask task in tasks)
        {
            if (task.MinConfiguredRetentionDays < IDataRetentionTask.MinimumRetentionDays)
            {
                logger.LogWithCode(
                    LogLevel.Warning,
                    DataRetentionLogCodes.TaskSkipped,
                    "Data retention task {TaskName} skipped. RetentionDays: {RetentionDays} below minimum {MinimumRetentionDays}",
                    task.Name,
                    task.MinConfiguredRetentionDays,
                    IDataRetentionTask.MinimumRetentionDays);
                continue;
            }

            try
            {
                int deletedRows = await task.ExecuteAsync(cancellationToken);
                logger.LogInformationWithCode(DataRetentionLogCodes.TaskCompleted, "Data retention task {TaskName} completed. DeletedRows: {DeletedRows}", task.Name, deletedRows);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception ex)
            {
                logger.LogErrorWithCode(ex, DataRetentionLogCodes.TaskFailed, "Data retention task {TaskName} failed", task.Name);
            }
        }
    }
}
