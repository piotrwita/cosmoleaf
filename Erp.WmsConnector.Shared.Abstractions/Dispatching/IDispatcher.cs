using Erp.WmsConnector.Shared.Abstractions.Dispatching.Commands;
using Erp.WmsConnector.Shared.Abstractions.Dispatching.Queries;

namespace Erp.WmsConnector.Shared.Abstractions.Dispatching;

public interface IDispatcher
{
    Task DispatchAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default)
        where TCommand : class, ICommand;

    Task<TResult> DispatchAsync<TResult>(IQuery<TResult> query, CancellationToken cancellationToken = default);
}