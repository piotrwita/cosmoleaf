using Erp.WmsConnector.Shared.Abstractions.DataRetention;
using Erp.WmsConnector.Shared.Infrastructure.DataRetention.Options;
using Erp.WmsConnector.Shared.Infrastructure.DataRetention.Tasks;
using Erp.WmsConnector.Shared.Infrastructure.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Erp.WmsConnector.Shared.Infrastructure.DataRetention;

internal static class RegistrationExtensions
{
    public static IServiceCollection AddDataRetention(this IServiceCollection services, IConfiguration configuration)
    {
        var databaseOptions = configuration.GetRequiredOptions<DatabaseOptions>();

        services.ConfigureOptions<DataRetentionSchedulerOptions>(configuration);
        services.ConfigureOptions<AppLogsDataRetentionOptions>(configuration);
        services.ConfigureOptions<OutboxMessagesDataRetentionOptions>(configuration);
        services.ConfigureOptions<DeduplicatedMessagesDataRetentionOptions>(configuration);

        services.AddDbContext<RetentionDbContext>(options =>
            options.UseSqlServer(databaseOptions.ConnectionString));
        services.AddScoped<IDataRetentionTask, AppLogsDataRetentionTask>();
        services.AddScoped<IDataRetentionTask, OutboxMessagesDataRetentionTask>();
        services.AddScoped<IDataRetentionTask, DeduplicatedMessagesDataRetentionTask>();
        services.AddSingleton<DataRetentionProcessor>();
        services.AddHostedService<DataRetentionService>();

        return services;
    }
}
