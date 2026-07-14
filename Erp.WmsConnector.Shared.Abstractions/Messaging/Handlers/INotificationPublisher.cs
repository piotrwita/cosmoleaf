namespace Erp.WmsConnector.Shared.Abstractions.Messaging.Handlers;

public interface INotificationPublisher<in TMessage, in TData>
    where TMessage : class
{
    Task PublishAsync(TMessage originalMessage, TData data, CancellationToken cancellationToken = default);
}
