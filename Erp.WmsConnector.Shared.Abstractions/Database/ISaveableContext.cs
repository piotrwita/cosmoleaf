namespace Erp.WmsConnector.Shared.Abstractions.Database;

public interface ISaveableContext
{
    bool HasChanges();
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}