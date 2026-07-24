namespace Kitayec.Types;

/// <summary>
/// Standard message header names used across the messaging infrastructure.
/// </summary>
public static class MessageHeaders
{
    /// <summary>
    /// Optional header for message ID when AMQP basic message_id is not set.
    /// If this header is not present, the library will generate a GUID as fallback.
    /// Used for deduplication when AMQP message_id is unavailable.
    /// </summary>
    public const string MessageId = "x-message-id";
}