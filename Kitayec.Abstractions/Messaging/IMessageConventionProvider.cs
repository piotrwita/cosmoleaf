namespace Kitayec.Abstractions.Messaging;

public interface IMessagePublisherConventionProvider
{
    (string destination, string routingKey) Get<TMessage>() where TMessage : class, IMessage;
    (string destination, string routingKey) Get(Type msgType);
}

public interface IMessageConsumerConventionProvider
{
    (string destination, string routingKey) Get<TMessage>() where TMessage : class, IMessage;
    (string destination, string routingKey) Get(Type msgType);
}