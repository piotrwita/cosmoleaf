using Microsoft.AspNetCore.Http;

namespace Erp.WmsConnector.Shared.Types.Exceptions.AlreadyExists;

public class AlreadyExistsException(string message) : BaseException(message)
{
    public override int StatusCode => StatusCodes.Status409Conflict;
}