using Erp.WmsConnector.Shared.Abstractions.HttpContextAccessors;
using Erp.WmsConnector.Shared.Infrastructure.Logging;
using Kitayec.Abstractions.Messaging;
using Serilog.Context;

namespace Erp.WmsConnector.Shared.Infrastructure.Messaging.Decorators;

public sealed class CorrelationIdMessageHandlerDecorator<TMessage>(
    IMessageHandler<TMessage> inner,
    IMessagePropertiesAccessor messagePropertiesAccessor) : IMessageHandler<TMessage>
    where TMessage : class, IMessage
{
    public async Task HandleAsync(TMessage message, CancellationToken cancellationToken = default)
    {
        string correlationId = messagePropertiesAccessor.Get()?.MessageId
            ?? ICorrelationIdAccessor.GenerateCorrelationId();

        using (LogContext.PushProperty(Constants.CorrelationPropertyName, correlationId))
        {
            await inner.HandleAsync(message, cancellationToken);
        }
    }
}
