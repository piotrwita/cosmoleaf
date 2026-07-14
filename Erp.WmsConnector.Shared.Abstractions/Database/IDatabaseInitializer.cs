namespace Erp.WmsConnector.Shared.Abstractions.Database;

public interface IDatabaseInitializer
{
    Task InitializeAsync(IMigratableDatabase dbContext, CancellationToken cancellationToken = default);
}