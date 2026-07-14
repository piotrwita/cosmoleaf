using Erp.WmsConnector.Shared.Abstractions.DataRetention;
using Erp.WmsConnector.Shared.Infrastructure.Database;
using Erp.WmsConnector.Shared.Infrastructure.DataRetention.Options;
using Microsoft.Extensions.Options;

namespace Erp.WmsConnector.Shared.Infrastructure.DataRetention.Tasks;

internal sealed class AppLogsDataRetentionTask(
    RetentionDbContext dbContext,
    IOptions<AppLogsDataRetentionOptions> retentionOptions) : IDataRetentionTask
{
    public string Name => "AppLogsDataRetentionTask";
    public int MinConfiguredRetentionDays => Math.Min(retentionOptions.Value.InformationRetentionDays, retentionOptions.Value.OtherRetentionDays);

    public async Task<int> ExecuteAsync(CancellationToken cancellationToken)
    {
        DateTime informationCutoff = DateTime.UtcNow.AddDays(-retentionOptions.Value.InformationRetentionDays);
        DateTime otherCutoff = DateTime.UtcNow.AddDays(-retentionOptions.Value.OtherRetentionDays);

        return await dbContext.Database.ExecuteSqlBatchedAsync(
            batchSize => $"""
                 DELETE TOP ({batchSize}) FROM [dbo].[AppLogs]
                 WHERE ([Level] = {"Information"} AND [TimeStamp] < {informationCutoff})
                     OR ([Level] <> {"Information"} AND [TimeStamp] < {otherCutoff});
                 """,
            cancellationToken);
    }
}
