using Erp.WmsConnector.Shared.Types.DataFiltering;

namespace Erp.WmsConnector.Shared.Abstractions.Database;

/// <summary>
/// Generic repository interface with specification pattern support
/// </summary>
/// <typeparam name="TEntity">The entity type</typeparam>
public interface IRepository<TEntity>
    where TEntity : class
{
    Task<IReadOnlyList<TEntity>> GetAsync(Specification<TEntity> specification, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<TEntity?> GetFirstOrDefaultAsync(Specification<TEntity> specification, CancellationToken cancellationToken = default);
    Task<int> CountAsync(Specification<TEntity> specification, CancellationToken cancellationToken = default);
    Task<bool> AnyAsync(Specification<TEntity> specification, CancellationToken cancellationToken = default);
}