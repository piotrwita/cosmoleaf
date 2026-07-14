using Erp.WmsConnector.Shared.Abstractions.Exceptions;
using Erp.WmsConnector.Shared.Abstractions.HttpContextAccessors;
using Erp.WmsConnector.Shared.Types.Exceptions;
using Microsoft.AspNetCore.Http;

namespace Erp.WmsConnector.Shared.Infrastructure.ErrorHandling;

internal class ErrorResponseFactory(ICorrelationIdAccessor correlationIdAccessor) : IErrorResponseFactory
{
    private const string DefaultErrorMessage = "Something went wrong! Please check the logs for details.";

    public (ErrorResponse errorResponse, int statusCode) CreateErrorResponse(Exception exception)
    {
        string correlationId = correlationIdAccessor.GetCorrelationId();
        return exception switch
        {
            BaseException baseEx => (
                new ErrorResponse(baseEx.ErrorCode, baseEx.Message, correlationId),
                baseEx.StatusCode
            ),
            _ => (
                new ErrorResponse(BaseException.GetErrorCodeFromType(exception.GetType()), DefaultErrorMessage, correlationId),
                StatusCodes.Status500InternalServerError
            )
        };
    }
}
