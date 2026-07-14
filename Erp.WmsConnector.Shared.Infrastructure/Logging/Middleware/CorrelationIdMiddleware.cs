using Erp.WmsConnector.Shared.Abstractions.HttpContextAccessors;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Erp.WmsConnector.Shared.Infrastructure.Logging.Middleware;

public sealed class CorrelationIdMiddleware(RequestDelegate next, ICorrelationIdAccessor correlationIdAccessor)
{
    public async Task InvokeAsync(HttpContext context, ILogger<CorrelationIdMiddleware> logger)
    {
        string correlationId = correlationIdAccessor.GetCorrelationId();

        if (!context.Response.Headers.ContainsKey(Constants.CorrelationHeaderName))
        {
            context.Response.Headers.Append(Constants.CorrelationHeaderName, correlationId);
        }

        logger.LogDebug("Request processing started with correlation ID: {CorrelationId}", correlationId);

        try
        {
            await next(context);
        }
        finally
        {
            logger.LogDebug("Request processing completed for correlation ID: {CorrelationId}", correlationId);
        }
    }
}