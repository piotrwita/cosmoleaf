using Erp.WmsConnector.Shared.Abstractions.Messaging.Handlers;
using Erp.WmsConnector.Shared.Types.Messaging.Handlers;
using Kitayec.Abstractions.Messaging;
using Microsoft.Extensions.Logging;
using Microsoft.FeatureManagement;

namespace Erp.WmsConnector.Shared.Infrastructure.Messaging.Handlers;

internal class SyncHandlerOrchestrator<TMessage, TData>(
    ISyncHandler<TMessage, TData> syncHandler,
    INotificationPublisher<TMessage, TData> notificationPublisher,
    ISyncHandlerAlertPublisher<TMessage> alertPublisher,
    IFeatureManager featureManager,
    ILogger<SyncHandlerOrchestrator<TMessage, TData>> logger) : IMessageHandler<TMessage>
    where TMessage : class, IMessage
{
    public async Task HandleAsync(TMessage message, CancellationToken cancellationToken = default)
    {
        if (!await featureManager.IsEnabledAsync(syncHandler.Config.SyncFeatureFlag))
        {
            logger.LogDebug(
                "Sync feature flag {FeatureFlag} is disabled, skipping message {MessageType}",
                syncHandler.Config.SyncFeatureFlag,
                typeof(TMessage).Name);
            return;
        }

        SyncResult<TData> result = await syncHandler.SyncAsync(message, cancellationToken);

        if (result.Alert is not null)
        {
            await alertPublisher.PublishAsync(message, result.Alert, cancellationToken);
        }

        if (result.Data is not null)
        {
            if (!await featureManager.IsEnabledAsync(syncHandler.Config.NotificationFeatureFlag))
            {
                logger.LogDebug(
                    "Notification feature flag {FeatureFlag} is disabled",
                    syncHandler.Config.NotificationFeatureFlag);
                return;
            }

            await notificationPublisher.PublishAsync(message, result.Data!, cancellationToken);
        }
    }
}
