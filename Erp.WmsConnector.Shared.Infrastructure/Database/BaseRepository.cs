using Erp.WmsConnector.Shared.Abstractions.Database;
using Erp.WmsConnector.Shared.Types.DataFiltering;
using Microsoft.EntityFrameworkCore;

namespace Erp.WmsConnector.Shared.Infrastructure.Database;

public abstract class BaseRepository<TEntity, TContext>(TContext context) : IRepository<TEntity>
    where TEntity : class
    where TContext : BaseDbContext<TContext>
{
    protected readonly TContext Context = context ?? throw new ArgumentNullException(nameof(context));
    protected readonly DbSet<TEntity> DbSet = context?.Set<TEntity>() ?? throw new ArgumentNullException(nameof(context));

    public virtual async Task<IReadOnlyList<TEntity>> GetAsync(
        Specification<TEntity> specification,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(specification);

        List<TEntity> entities = await GetNoTrackingQueryable()
            .Where(specification.ToExpression())
            .ToListAsync(cancellationToken);

        return entities;
    }

    public virtual async Task<IReadOnlyList<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        List<TEntity> entities = await GetNoTrackingQueryable()
            .ToListAsync(cancellationToken);

        return entities;
    }

    public virtual async Task<TEntity?> GetFirstOrDefaultAsync(
        Specification<TEntity> specification,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(specification);

        TEntity? entity = await GetNoTrackingQueryable()
            .Where(specification.ToExpression())
            .FirstOrDefaultAsync(cancellationToken);

        return entity;
    }

    public virtual async Task<int> CountAsync(
        Specification<TEntity> specification,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(specification);

        int count = await GetNoTrackingQueryable()
            .Where(specification.ToExpression())
            .CountAsync(cancellationToken);

        return count;
    }

    public virtual async Task<bool> AnyAsync(
        Specification<TEntity> specification,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(specification);

        bool exists = await GetNoTrackingQueryable()
            .Where(specification.ToExpression())
            .AnyAsync(cancellationToken);

        return exists;
    }

    protected virtual IQueryable<TEntity> GetNoTrackingQueryable()
        => DbSet.AsNoTracking();
}