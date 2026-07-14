using Erp.WmsConnector.Shared.Abstractions.HealthChecks;
using Erp.WmsConnector.Shared.Infrastructure.HealthChecks;
using Erp.WmsConnector.Shared.Infrastructure.HealthChecks.Options;
using Erp.WmsConnector.Shared.Infrastructure.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Erp.WmsConnector.Shared.Infrastructure.Database.HealthChecks;

public static class RegistrationExtensions
{
    public static IHealthChecksRegisterer UseDatabase(this IHealthChecksRegisterer registerer)
    {
        IConfiguration config = registerer.Configuration;
        IServiceCollection services = registerer.Services;

        var options = config.GetRequiredOptions<HealthChecksOptions>();
        if (options.DatabaseEnabled)
        {
            var timeout = TimeSpan.FromSeconds(options.TimeoutSeconds);
            var dbOptions = config.GetRequiredOptions<DatabaseOptions>();

            services.AddHealthChecks().AddSqlServer(
                connectionString: dbOptions.ConnectionString,
                name: DatabaseOptions.SectionName,
                failureStatus: HealthStatus.Unhealthy,
                timeout: timeout,
                tags: DatabaseTags.Primary);
        }

        return registerer;
    }
}