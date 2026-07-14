using Erp.WmsConnector.Shared.Abstractions.HttpContextAccessors;
using Erp.WmsConnector.Shared.Infrastructure.Logging;
using Microsoft.AspNetCore.Http;

namespace Erp.WmsConnector.Shared.Infrastructure.HttpContextAccessors;

public sealed class CorrelationIdAccessor(IHttpContextAccessor httpContextAccessor) : ICorrelationIdAccessor
{
    public string GetCorrelationId()
    {
        HttpContext? httpContext = httpContextAccessor.HttpContext;
        if (httpContext == null)
        {
            return ICorrelationIdAccessor.GenerateCorrelationId();
        }

        if (httpContext.Items.TryGetValue(Constants.CorrelationHttpContextKey, out var contextValue)
            && contextValue is string existingCorrelationId
            && !string.IsNullOrWhiteSpace(existingCorrelationId))
        {
            return existingCorrelationId;
        }

        if (httpContext.Request.Headers.TryGetValue(Constants.CorrelationHeaderName, out var headerValue))
        {
            string? headerCorrelationId = headerValue.FirstOrDefault();
            if (!string.IsNullOrWhiteSpace(headerCorrelationId))
            {
                // Store in HttpContext.Items for future access
                httpContext.Items[Constants.CorrelationHttpContextKey] = headerCorrelationId;
                return headerCorrelationId;
            }
        }

        if (!string.IsNullOrWhiteSpace(httpContext.TraceIdentifier))
        {
            httpContext.Items[Constants.CorrelationHttpContextKey] = httpContext.TraceIdentifier;
            return httpContext.TraceIdentifier;
        }

        string newCorrelationId = ICorrelationIdAccessor.GenerateCorrelationId();
        httpContext.Items[Constants.CorrelationHttpContextKey] = newCorrelationId;

        return newCorrelationId;
    }

    public void SetCorrelationId(string correlationId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(correlationId);

        HttpContext? httpContext = httpContextAccessor.HttpContext;
        if (httpContext != null)
        {
            httpContext.Items[Constants.CorrelationHttpContextKey] = correlationId;
        }
    }
}
