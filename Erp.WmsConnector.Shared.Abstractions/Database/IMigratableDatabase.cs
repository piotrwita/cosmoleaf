namespace Erp.WmsConnector.Shared.Abstractions.Database;

public interface IMigratableDatabase
{
    Task MigrateAsync(CancellationToken cancellationToken = default);
}