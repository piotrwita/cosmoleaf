namespace Kitayec.Abstractions.Messaging.Ordering.OutOfOrderDetection;

public interface IVersionedMessage : IMessage
{
    int Version { get; }
    string ToHumanReadableString();
}