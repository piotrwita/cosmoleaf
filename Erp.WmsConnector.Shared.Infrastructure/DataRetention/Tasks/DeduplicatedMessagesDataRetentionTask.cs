using Erp.WmsConnector.Shared.Abstractions.DataRetention;
using Erp.WmsConnector.Shared.Infrastructure.Database;
using Erp.WmsConnector.Shared.Infrastructure.DataRetention.Options;
using Microsoft.Extensions.Options;

namespace Erp.WmsConnector.Shared.Infrastructure.DataRetention.Tasks;

internal sealed class DeduplicatedMessagesDataRetentionTask(
    RetentionDbContext dbContext,
    IOptions<DeduplicatedMessagesDataRetentionOptions> retentionOptions) : IDataRetentionTask
{
    public string Name => "DeduplicatedMessagesDataRetentionTask";
    public int MinConfiguredRetentionDays => retentionOptions.Value.RetentionDays;

    public async Task<int> ExecuteAsync(CancellationToken cancellationToken)
    {
        DateTimeOffset cutoff = DateTimeOffset.UtcNow.AddDays(-retentionOptions.Value.RetentionDays);

        return await dbContext.Database.ExecuteSqlBatchedAsync(
            batchSize => $"DELETE TOP ({batchSize}) FROM [Deduplication].[DeduplicationEntries] WHERE [ProcessedAt] < {cutoff};",
            cancellationToken);
    }
}
