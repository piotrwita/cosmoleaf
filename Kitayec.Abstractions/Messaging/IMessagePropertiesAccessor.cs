using Kitayec.Types;

namespace Kitayec.Abstractions.Messaging;

public interface IMessagePropertiesAccessor
{
    MessageProperties? Get();

    MessageProperties InitializeIfEmpty();

    public void Set(MessageProperties messageProperties);

    public void Clear();
}