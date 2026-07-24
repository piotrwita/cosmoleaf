namespace Kitayec.Abstractions.Messaging;

public interface IMessageConsumer
{
    Task<IMessageConsumer> ConsumeMessageAsync<TMessage>(
        Func<TMessage, Task>? handle = default,
        string? queue = default,
        string[]? acceptedMessageTypes = default,
        CancellationToken cancellationToken = default) where TMessage : class, IMessage;

    Task<IMessageConsumer> ConsumeMessageAsync(
        Func<MessageData, Task> handleRawPayload,
        string queue,
        string[]? acceptedMessageTypes = default,
        CancellationToken cancellationToken = default);

    Task GetMessageAsync<TMessage>(
        Func<TMessage, Task> handle,
        string? queue = default,
        CancellationToken cancellationToken = default) where TMessage : class, IMessage;
}