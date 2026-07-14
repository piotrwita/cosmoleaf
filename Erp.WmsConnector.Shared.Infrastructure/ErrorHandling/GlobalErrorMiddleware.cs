using System.Text.Json;
using Erp.WmsConnector.Shared.Abstractions.Exceptions;
using Erp.WmsConnector.Shared.Infrastructure.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Erp.WmsConnector.Shared.Infrastructure.ErrorHandling;

public sealed class GlobalErrorMiddleware(
    RequestDelegate next, 
    IErrorResponseFactory errorResponseFactory,
    ILoggerFactory loggerFactory)
{
    private readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(
        HttpContext context,
        Exception exception)
    {
        var logger = loggerFactory.CreateLogger<GlobalErrorMiddleware>();

        var (errorResponse, statusCode) = errorResponseFactory.CreateErrorResponse(exception);

        logger.LogErrorWithCode(exception, errorResponse.ErrorCode, exception.Message);

        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";

        await context.Response.WriteAsJsonAsync(errorResponse, JsonOptions);
    }
}