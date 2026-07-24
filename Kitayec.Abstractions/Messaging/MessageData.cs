namespace Kitayec.Abstractions.Messaging;

public record MessageData(Guid MessageId, byte[] Payload, string Type) : IMessage;