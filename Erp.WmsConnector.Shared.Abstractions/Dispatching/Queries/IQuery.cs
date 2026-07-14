namespace Erp.WmsConnector.Shared.Abstractions.Dispatching.Queries;

[System.Diagnostics.CodeAnalysis.SuppressMessage(
    "Major Code Smell",
    "S2326:Unused type parameters should be removed",
    Justification = "This is a marker that indicates that a given class is a query that returns a TResult")]
public interface IQuery<TResult>
{ }