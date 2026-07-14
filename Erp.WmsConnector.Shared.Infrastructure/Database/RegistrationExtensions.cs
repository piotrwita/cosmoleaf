using System.Reflection;
using Erp.WmsConnector.Shared.Abstractions.Database;
using Erp.WmsConnector.Shared.Abstractions.Options;
using Erp.WmsConnector.Shared.Infrastructure.Database.Interceptors;
using Erp.WmsConnector.Shared.Infrastructure.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Erp.WmsConnector.Shared.Infrastructure.Database;

public static class RegistrationExtensions
{
    public static async Task TryToMigrateDatabasesAsync(
        this IServiceProvider serviceProvider,
        IReadOnlyCollection<Assembly> assemblies,
        CancellationToken cancellationToken = default)
    {
        using IServiceScope scope = serviceProvider.CreateScope();
        var initializer = scope.ServiceProvider.GetRequiredService<IDatabaseInitializer>();

        Type baseDbContextType = typeof(BaseDbContext<>);
        Type migratableDbType = typeof(IMigratableDatabase);
        IEnumerable<Type> dbContextTypes = assemblies.SelectMany((Assembly a) => a.GetTypes())
            .Where(t => !t.IsAbstract && !t.IsInterface)
            .Where(t => baseDbContextType.IsAssignableFromGeneric(t) && migratableDbType.IsAssignableFrom(t));

        foreach (Type t in dbContextTypes)
        {
            IMigratableDatabase dbContext = (IMigratableDatabase)scope.ServiceProvider.GetRequiredService(t);

            await initializer.InitializeAsync(dbContext, cancellationToken);
        }
    }

    public static IServiceCollection AddDatabaseInitializer(this IServiceCollection services)
    {
        services.AddScoped<IDatabaseInitializer, DatabaseInitializer>();

        return services;
    }

    public static IServiceCollection AddDatabase<TDbContext>(this IServiceCollection services, IConfiguration config)
        where TDbContext : BaseDbContext<TDbContext>
    {
        return services.AddDatabase<TDbContext, DatabaseOptions>(config);
    }

    public static IServiceCollection AddDatabase<TDbContext, TDbOptions>(this IServiceCollection services, IConfiguration config)
        where TDbContext : BaseDbContext<TDbContext>
        where TDbOptions : class, IOptions, IDatabaseOptions
    {
        var dbOptions = config.GetRequiredOptions<TDbOptions>();

        services.TryAddSingleton<AuditableEntityInterceptor>();

        services.AddDbContext<TDbContext>((serviceProvider, options) =>
        {
            BaseDbContext<TDbContext>.ConfigureWithMigrationSchema(options, dbOptions.ConnectionString, dbOptions.MigrationTableSchema);
            options.AddInterceptors(serviceProvider.GetRequiredService<AuditableEntityInterceptor>());
        });

        services.TryAddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }

    private static bool IsAssignableFromGeneric(this Type genericType, Type type)
    {
        if (!genericType.IsGenericTypeDefinition)
        {
            return false;
        }

        Type? currentType = type;
        while (currentType != null)
        {
            if (currentType.IsGenericType && currentType.GetGenericTypeDefinition() == genericType)
            {
                return true;
            }

            currentType = currentType.BaseType;
        }

        return false;
    }
}
