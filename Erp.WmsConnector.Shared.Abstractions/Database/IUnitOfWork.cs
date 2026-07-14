using Microsoft.EntityFrameworkCore;

namespace Erp.WmsConnector.Shared.Abstractions.Database;

public interface IUnitOfWork
{
    /// <summary>
    /// Executes the specified action within a database transaction scope for the given DbContext.
    /// The transaction is automatically committed if the action succeeds, or rolled back if an exception occurs.
    /// Changes are automatically saved after the action completes successfully.
    /// </summary>
    Task ExecuteAsync<TDbContext>(Func<ISaveableContext, Task> action) where TDbContext : DbContext, ISaveableContext;

    /// <summary>
    /// Executes the specified action within a distributed transaction scope using TransactionScope.
    /// The transaction is automatically completed if the action succeeds, or rolled back if an exception occurs.
    /// </summary>
    Task ExecuteAsync(Func<Task> action);
}
