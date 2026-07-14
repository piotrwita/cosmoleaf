using Erp.WmsConnector.Shared.Abstractions.Options;
using Erp.WmsConnector.Shared.Types.Exceptions.NotFound;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Erp.WmsConnector.Shared.Infrastructure.Options;

public static class OptionsExtensions
{
    public static T GetRequiredOptions<T>(this IConfiguration config)
        where T : class, IOptions
    {
        T? options = config.GetOptions<T>();

        NotFoundThrowHelper.ThrowIfNull(options, $"Configuration section '{T.SectionName}' is missing or invalid");

        return options;
    }

    public static T? GetOptions<T>(this IConfiguration config)
        where T : class, IOptions => config.GetOptions<T>(default);

    public static T? GetOptions<T>(this IConfiguration config, Action<BinderOptions>? configureOptions)
        where T : class, IOptions => config.GetSection(T.SectionName).Get<T>(configureOptions);

    public static IServiceCollection ConfigureOptions<T>(this IServiceCollection services, IConfiguration config)
        where T : class, IOptions
    {
        _ = config.GetRequiredOptions<T>();

        IConfigurationSection section = config.GetSection(T.SectionName);

        return services.Configure<T>(section);
    }
}