using System.Diagnostics.CodeAnalysis;

namespace Erp.WmsConnector.Shared.Types.Exceptions.AlreadyExists;

public static class AlreadyExistsThrowHelper
{
    [DoesNotReturn]
    public static void Throw(string message) => throw new AlreadyExistsException(message);

    public static void ThrowIfNotNull<T>([AllowNull] T? obj, string message)
        where T : struct
    {
        if (obj is not null)
        {
            Throw(message);
        }
    }
}