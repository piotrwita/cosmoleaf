using Microsoft.AspNetCore.Http;

namespace Kitayec.Types.Exceptions;

internal sealed class AlreadyExistsException(string message) : BaseException(message)
{
    public override int StatusCode => StatusCodes.Status409Conflict;
}