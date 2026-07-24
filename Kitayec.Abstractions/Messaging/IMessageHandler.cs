namespace Kitayec.Abstractions.Messaging;

public interface IMessageHandler<in TMessage> where TMessage : class, IMessage
{
    Task HandleAsync(TMessage message, CancellationToken cancellationToken = default);
}