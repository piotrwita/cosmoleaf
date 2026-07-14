using System.Linq.Expressions;

namespace Erp.WmsConnector.Shared.Types.DataFiltering;

/// <summary>
/// Expression visitor that replaces parameter expressions in expression trees
/// Used internally for combining specifications
/// </summary>
internal sealed class ParameterReplacer(ParameterExpression oldParameter, ParameterExpression newParameter) : ExpressionVisitor
{
    protected override Expression VisitParameter(ParameterExpression node)
        => node == oldParameter ? newParameter : base.VisitParameter(node);
}