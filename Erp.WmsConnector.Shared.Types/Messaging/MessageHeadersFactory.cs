namespace Erp.WmsConnector.Shared.Types.Messaging;

public class MessageHeadersFactory
{
    public static MessageHeadersFactory Get() => new();

    public const string ParentMessageId = "parent-message-id";
    public const string CorrelationId = "correlation-id";

    public static IDictionary<string, object> Create(string parentMessageId, string correlationId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(parentMessageId);
        ArgumentException.ThrowIfNullOrWhiteSpace(correlationId);

        return new Dictionary<string, object>
        {
            [ParentMessageId] = parentMessageId,
            [CorrelationId] = correlationId
        };
    }

    public static IDictionary<string, object> Create(Guid parentMessageId, Guid correlationId)
        => new Dictionary<string, object>
        {
            [ParentMessageId] = parentMessageId.ToString(),
            [CorrelationId] = correlationId.ToString()
        };
}