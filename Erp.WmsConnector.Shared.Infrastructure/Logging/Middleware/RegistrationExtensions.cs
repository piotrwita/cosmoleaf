using Microsoft.AspNetCore.Builder;

namespace Erp.WmsConnector.Shared.Infrastructure.Logging.Middleware;

public static class RegistrationExtensions
{
    public static IApplicationBuilder UseCorrelationId(this IApplicationBuilder app)
    {
        app.UseMiddleware<CorrelationIdMiddleware>();

        return app;
    }
}
