using System.Linq.Expressions;

namespace Erp.WmsConnector.Shared.Types.DataFiltering;

/// <summary>
/// Specification that combines two specifications using logical OR
/// </summary>
/// <typeparam name="T">The entity type to filter</typeparam>
internal sealed class OrSpecification<T>(Specification<T> left, Specification<T> right) : BinarySpecification<T>(left, right)
{
    protected override BinaryExpression CreateBinaryExpression(Expression left, Expression right)
        => Expression.OrElse(left, right);
}