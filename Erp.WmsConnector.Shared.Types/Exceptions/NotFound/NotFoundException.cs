using Microsoft.AspNetCore.Http;

namespace Erp.WmsConnector.Shared.Types.Exceptions.NotFound;

public class NotFoundException(string message) : BaseException(message)
{
    public override int StatusCode => StatusCodes.Status404NotFound;
}
