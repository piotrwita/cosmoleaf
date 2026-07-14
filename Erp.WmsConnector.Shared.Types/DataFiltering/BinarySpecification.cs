using System.Linq.Expressions;

namespace Erp.WmsConnector.Shared.Types.DataFiltering;

/// <summary>
/// Abstract base class for specifications that combine two specifications using a binary logical operator
/// </summary>
/// <typeparam name="T">The entity type to filter</typeparam>
internal abstract class BinarySpecification<T> : Specification<T>
{
    private readonly Specification<T> _left;
    private readonly Specification<T> _right;

    protected BinarySpecification(Specification<T> left, Specification<T> right)
    {
        _left = left ?? throw new ArgumentNullException(nameof(left));
        _right = right ?? throw new ArgumentNullException(nameof(right));
    }

    public override Expression<Func<T, bool>> ToExpression()
    {
        Expression<Func<T, bool>> leftExpression = _left.ToExpression();
        Expression<Func<T, bool>> rightExpression = _right.ToExpression();

        // Replace the parameter in the right expression with the parameter from the left expression
        ParameterExpression parameter = leftExpression.Parameters[0];
        Expression rightBody = new ParameterReplacer(rightExpression.Parameters[0], parameter)
            .Visit(rightExpression.Body);

        // Combine the expressions using the specific binary operator
        BinaryExpression combined = CreateBinaryExpression(leftExpression.Body, rightBody);

        return Expression.Lambda<Func<T, bool>>(combined, parameter);
    }

    /// <summary>
    /// Creates the specific binary expression for the logical operator (AND, OR, etc.)
    /// </summary>
    /// <param name="left">The left expression</param>
    /// <param name="right">The right expression</param>
    /// <returns>The combined binary expression</returns>
    protected abstract BinaryExpression CreateBinaryExpression(Expression left, Expression right);
}
