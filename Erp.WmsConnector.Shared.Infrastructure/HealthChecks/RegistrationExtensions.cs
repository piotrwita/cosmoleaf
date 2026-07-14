using Erp.WmsConnector.Shared.Abstractions.HealthChecks;
using Erp.WmsConnector.Shared.Infrastructure.HealthChecks.Options;
using Erp.WmsConnector.Shared.Infrastructure.Options;
using Erp.WmsConnector.Shared.Infrastructure.Serialization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Erp.WmsConnector.Shared.Infrastructure.HealthChecks;

public static class RegistrationExtensions
{
    public static IServiceCollection AddHealthChecks(this IServiceCollection services, IConfiguration configuration, Action<IHealthChecksRegisterer> register)
    {
        var registerer = new HealthChecksRegisterer(services, configuration);
        services.ConfigureOptions<HealthChecksOptions>(configuration);
        register(registerer);

        return services;
    }

    public static WebApplication UseHealthChecks(this WebApplication app)
    {
        var healthCheckService = app.Services.GetService<HealthCheckService>();

        // Only register health check endpoints if the service is available
        if (healthCheckService is not null)
        {
            app.UseHealthChecks("/health", new HealthCheckOptions
            {
                Predicate = static registration => !registration.Tags.Contains("ready")
            });

            app.UseHealthChecks("/ready", new HealthCheckOptions
            {
                Predicate = static registration => registration.Tags.Contains("ready"),
                AllowCachingResponses = false,
                ResponseWriter = async (context, report) =>
                {
                    if (report.Status == HealthStatus.Unhealthy)
                    {
                        context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
                    }

                    var result = new
                    {
                        checks = report.Entries.Select(entry => new
                        {
                            name = entry.Key,
                            status = entry.Value.Status.ToString(),
                            description = entry.Value.Description,
                            data = entry.Value.Data,
                            exception = entry.Value.Exception?.Message,
                            duration = entry.Value.Duration.ToString()
                        })
                    };

                    await context.Response.WriteAsJsonAsync(result, SerializationOptions.PrettyPrint);
                }
            });
        }

        return app;
    }
}