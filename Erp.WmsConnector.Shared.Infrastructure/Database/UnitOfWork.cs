using System.Transactions;
using Erp.WmsConnector.Shared.Abstractions.Database;
using Erp.WmsConnector.Shared.Infrastructure.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;

namespace Erp.WmsConnector.Shared.Infrastructure.Database;

internal sealed class UnitOfWork(IServiceProvider provider) : IUnitOfWork
{
    /// <inheritdoc />
    public async Task ExecuteAsync<TDbContext>(Func<ISaveableContext, Task> action)
        where TDbContext : DbContext, ISaveableContext
    {
        ArgumentNullException.ThrowIfNull(action);

        var context = provider.GetRequiredService<TDbContext>();
        await using IDbContextTransaction? transaction = await context.TryBeginDbContextTransaction();
        try
        {
            await action(context);

            if (context.HasChanges())
            {
                _ = await context.SaveChangesAsync();
            }

            if (transaction is not null)
            {
                await transaction.CommitWithTimeoutAsync();
            }
        }
        catch
        {
            if (transaction is not null)
            {
                await transaction.RollbackWithTimeoutAsync();
            }

            throw;
        }
    }

    /// <inheritdoc />
    public async Task ExecuteAsync(Func<Task> action)
    {
        ArgumentNullException.ThrowIfNull(action);

        using TransactionScope? transaction = TransactionHelper.TryBeginTransactionScope();

        await action();

        transaction?.Complete();
    }
}