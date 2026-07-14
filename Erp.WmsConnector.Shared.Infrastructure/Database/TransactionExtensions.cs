using Erp.WmsConnector.Shared.Infrastructure.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Erp.WmsConnector.Shared.Infrastructure.Database;

public static class TransactionExtensions
{
    internal const int CommitTimeoutSeconds = 30;
    private const int RollbackTimeoutSeconds = 10;

    public static async Task CommitWithTimeoutAsync(this IDbContextTransaction transaction)
    {
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(CommitTimeoutSeconds));
        await transaction.CommitAsync(cts.Token);
    }

    public static async Task RollbackWithTimeoutAsync(this IDbContextTransaction transaction)
    {
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(RollbackTimeoutSeconds));
        await transaction.RollbackAsync(cts.Token);
    }

    public static async Task<IDbContextTransaction?> TryBeginDbContextTransaction(this DbContext context)
    {
        if (IsInTransaction(context))
        {
            return null;
        }
        else
        {
            return await context.Database.BeginTransactionAsync();
        }
    }

    public static bool IsInTransaction(this DbContext context)
        => TransactionHelper.IsInExternalTransaction() || context.Database.CurrentTransaction != null;
}