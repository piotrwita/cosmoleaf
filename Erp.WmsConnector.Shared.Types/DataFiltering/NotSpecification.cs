using System.Linq.Expressions;

namespace Erp.WmsConnector.Shared.Types.DataFiltering;

/// <summary>
/// Specification that negates another specification using logical NOT
/// </summary>
/// <typeparam name="T">The entity type to filter</typeparam>
internal sealed class NotSpecification<T>(Specification<T> specification) : Specification<T>
{
    private readonly Specification<T> _specification = specification ?? throw new ArgumentNullException(nameof(specification));

    public override Expression<Func<T, bool>> ToExpression()
    {
        Expression<Func<T, bool>> expression = _specification.ToExpression();

        UnaryExpression notExpression = Expression.Not(expression.Body);

        return Expression.Lambda<Func<T, bool>>(notExpression, expression.Parameters[0]);
    }
}