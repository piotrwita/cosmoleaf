using Erp.WmsConnector.Shared.Types.Messaging.Handlers;
using Kitayec.Abstractions.Messaging;

namespace Erp.WmsConnector.Shared.Abstractions.Messaging.Handlers;

public interface ISyncHandlerAlertPublisher<in TMessage>
    where TMessage : class, IMessage
{
    Task PublishAsync(TMessage message, AlertInfo alert, CancellationToken cancellationToken);
}
