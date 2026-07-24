namespace Kitayec.Types;

public sealed record MessageProperties(string MessageId, IDictionary<string, object> Headers, string MessageType, bool Redelivered);