using Kitayec.Types;

namespace Kitayec.Abstractions.Messaging.Outbox;

public interface IMessageOutbox
{
    Task<IReadOnlyList<OutboxMessage>> GetUnsentAsync(int batchSize = default, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<OutboxMessage>> GetUnsentByMessageTypeAsync(
        int batchSize = default,
        string? messageTypePrefix = default,
        CancellationToken cancellationToken = default)
        => GetUnsentAsync(batchSize, cancellationToken);

    Task AddAsync<TMessage>(TMessage message, string messageId, string? destination = default, string? routingKey = default,
        IDictionary<string, object>? headers = default, CancellationToken cancellationToken = default) where TMessage : IMessage;

    Task MarkAsProcessedAsync(OutboxMessage outboxMessage, CancellationToken cancellationToken = default);
}