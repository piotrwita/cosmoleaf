using System.Diagnostics.CodeAnalysis;

namespace Erp.WmsConnector.Shared.Types.Exceptions.NotFound;

public static class NotFoundThrowHelper
{
    [DoesNotReturn]
    public static void Throw(string message) => throw new NotFoundException(message);

    public static void ThrowIfNull<T>([NotNull] T? obj, string message)
        where T : class
    {
        if (obj is null)
        {
            Throw(message);
        }
    }
}
