using Erp.WmsConnector.Shared.Infrastructure.Messaging.RabbitMq.Exceptions;
using Humanizer;
using Kitayec.Abstractions.Messaging;
using Kitayec.Abstractions.Messaging.Topology;

namespace Erp.WmsConnector.Shared.Infrastructure.Messaging.RabbitMq;

public sealed class RabbitMqCustomMessageConventionProvider(ITopologyConventionProvider topologyConventionProvider)
    : IMessageConsumerConventionProvider, IMessagePublisherConventionProvider
{
    (string destination, string routingKey) IMessagePublisherConventionProvider.Get<TMessage>()
        => ((IMessagePublisherConventionProvider)this).Get(typeof(TMessage));

    (string destination, string routingKey) IMessagePublisherConventionProvider.Get(Type msgType)
    {
        string messageContext = ExtractMessageContext(msgType);
        string destination = topologyConventionProvider.ResolveExchangeName(messageContext);

        return (destination, string.Empty);
    }

    (string destination, string routingKey) IMessageConsumerConventionProvider.Get<TMessage>()
        => ((IMessageConsumerConventionProvider)this).Get(typeof(TMessage));

    (string destination, string routingKey) IMessageConsumerConventionProvider.Get(Type msgType)
    {
        _ = ExtractMessageContext(msgType);
        string messageTypeName = msgType.Name.Kebaberize();
        string destination = topologyConventionProvider.ResolveQueueName(messageTypeName);

        return (destination, string.Empty);
    }


    /// <summary>
    /// Extracts the message context from the message type name and validates it against the namespace.
    /// REQUIREMENT: The message type MUST start with the message context name AND the message context name MUST be present in the namespace.
    /// </summary>
    private static string ExtractMessageContext(Type messageType)
    {
        if (!typeof(IMessage).IsAssignableFrom(messageType))
        {
            throw new ArgumentException($"Type {messageType.Name} must implement {nameof(IMessage)}", nameof(messageType));
        }

        string messageTypeName = messageType.Name;
        string namespaceName = messageType.Namespace ?? string.Empty;

        string? messageContext = messageTypeName
            .Humanize()
            .Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .FirstOrDefault();

        if (string.IsNullOrWhiteSpace(messageContext) || !namespaceName.Contains(messageContext, StringComparison.OrdinalIgnoreCase))
        {
            throw new CannotExtractModuleNameFromMessageException(messageTypeName);
        }

        return messageContext.ToLowerInvariant();
    }
}