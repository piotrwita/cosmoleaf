using Erp.WmsConnector.Shared.Abstractions.Exceptions;
using Erp.WmsConnector.Shared.Abstractions.HttpContextAccessors;
using Erp.WmsConnector.Shared.Infrastructure.HttpContextAccessors;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Erp.WmsConnector.Shared.Infrastructure.ErrorHandling;

public static class RegistrationExtensions
{
    public static IServiceCollection AddGlobalErrorHandling(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        services.TryAddSingleton<ICorrelationIdAccessor, CorrelationIdAccessor>();
        services.AddSingleton<IErrorResponseFactory, ErrorResponseFactory>();

        return services;
    }

    public static IApplicationBuilder UseGlobalErrorHandling(this IApplicationBuilder app)
    {
        app.UseMiddleware<GlobalErrorMiddleware>();

        return app;
    }
}
