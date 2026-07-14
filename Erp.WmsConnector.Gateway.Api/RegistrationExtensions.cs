using Erp.WmsConnector.Integrations.Legacy.Core.Data.HealthChecks;
using Erp.WmsConnector.Shared.Infrastructure;
using Erp.WmsConnector.Shared.Infrastructure.Database;
using Erp.WmsConnector.Shared.Infrastructure.Database.HealthChecks;
using Erp.WmsConnector.Shared.Infrastructure.ErrorHandling;
using Erp.WmsConnector.Shared.Infrastructure.HealthChecks;
using Erp.WmsConnector.Shared.Infrastructure.Logging;
using Erp.WmsConnector.Shared.Infrastructure.Messaging.RabbitMq.HealthChecks;
using Erp.WmsConnector.Shared.Infrastructure.OpenApi;
using Erp.WmsConnector.Shared.ModuleDefinition;
using Microsoft.FeatureManagement;
using Serilog;

namespace Erp.WmsConnector.Gateway.Api;

internal static class RegistrationExtensions
{
    public static IServiceCollection AddApiDependencies(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddFeatureManagement();

        services.AddLogging(configuration);
        services.AddGlobalErrorHandling();

        services.TryAddModules(configuration);
        services.AddEndpointsApiExplorer();

        services.AddCoreInfrastructure(configuration);

        services.AddSwagger(configuration);

        services.AddHealthChecks(configuration,
            x => x.UseLegacyDatabase()
                .UseRabbitMq()
                .UseDatabase());

        return services;
    }

    public static WebApplication UseApiDependencies(this WebApplication app, IConfiguration configuration)
    {
        app.UseCoreInfrastructure();

        app.MapModules(configuration);

        app.UseSwagger();
        app.UseSwaggerUI();

        app.UseHealthChecks();

        return app;
    }

    public static async Task TryToMigrateDatabasesAsync(this WebApplication app, CancellationToken cancellationToken = default)
    {
        try
        {
            await app.Services.TryToMigrateDatabasesAsync(ModuleRegistry.Instance.EnabledAssemblies, cancellationToken);
        }
        catch
        {
            await Log.CloseAndFlushAsync();
            throw;
        }
    }
}