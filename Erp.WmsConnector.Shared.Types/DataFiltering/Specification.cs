using System.Linq.Expressions;

namespace Erp.WmsConnector.Shared.Types.DataFiltering;

/// <summary>
/// Base class for implementing the Specification Pattern for repository filtering
/// </summary>
/// <typeparam name="T">The entity type to filter</typeparam>
public abstract class Specification<T>
{
    public abstract Expression<Func<T, bool>> ToExpression();

    public bool IsSatisfiedBy(T entity)
    {
        Func<T, bool> predicate = ToExpression().Compile();
        return predicate(entity);
    }

    public Specification<T> And(Specification<T> other) => new AndSpecification<T>(this, other);

    public Specification<T> Or(Specification<T> other) => new OrSpecification<T>(this, other);

    public Specification<T> Not() => new NotSpecification<T>(this);

    public static implicit operator Expression<Func<T, bool>>(Specification<T> specification) => specification.ToExpression();

    public static Specification<T> operator &(Specification<T> left, Specification<T> right) => left.And(right);

    public static Specification<T> operator |(Specification<T> left, Specification<T> right) => left.Or(right);

    public static Specification<T> operator !(Specification<T> specification) => specification.Not();
}