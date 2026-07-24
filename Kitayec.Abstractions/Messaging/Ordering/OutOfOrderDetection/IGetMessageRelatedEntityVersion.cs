namespace Kitayec.Abstractions.Messaging.Ordering.OutOfOrderDetection;

public interface IGetMessageRelatedEntityVersion<TMessage> where TMessage : IVersionedMessage
{
    Task<int?> GetEntityVersionAsync(TMessage message, CancellationToken cancellationToken = default);
}