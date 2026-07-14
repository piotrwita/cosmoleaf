using Erp.WmsConnector.Shared.Abstractions.HealthChecks;
using Erp.WmsConnector.Shared.Infrastructure.HealthChecks.Options;
using Erp.WmsConnector.Shared.Infrastructure.Options;
using Kitayec.Infrastructure.Messaging.RabbitMq.HealthChecks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Erp.WmsConnector.Shared.Infrastructure.Messaging.RabbitMq.HealthChecks;

public static class RegistrationExtensions
{
    public static IHealthChecksRegisterer UseRabbitMq(this IHealthChecksRegisterer registerer)
    {
        IConfiguration config = registerer.Configuration;
        IServiceCollection services = registerer.Services;

        var options = config.GetRequiredOptions<HealthChecksOptions>();
        if (options.RabbitMqEnabled)
        {
            services.UseRabbitMq(config);
        }

        return registerer;
    }
}