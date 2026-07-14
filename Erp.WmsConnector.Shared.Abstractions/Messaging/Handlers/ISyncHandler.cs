using Erp.WmsConnector.Shared.Types.Messaging.Handlers;

namespace Erp.WmsConnector.Shared.Abstractions.Messaging.Handlers;

public interface ISyncHandler<in TMessage, TData>
    where TMessage : class
{
    SyncHandlerConfig Config { get; }
    Task<SyncResult<TData>> SyncAsync(TMessage message, CancellationToken cancellationToken = default);
}
