using Erp.WmsConnector.Shared.Types.Exceptions;

namespace Erp.WmsConnector.Shared.Abstractions.Exceptions;

public interface IErrorResponseFactory
{
    (ErrorResponse errorResponse, int statusCode) CreateErrorResponse(Exception exception);
}