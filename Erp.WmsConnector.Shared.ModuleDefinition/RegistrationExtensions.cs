using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Erp.WmsConnector.Shared.ModuleDefinition;

public static class RegistrationExtensions
{
    public static IServiceCollection TryAddModules(this IServiceCollection services, IConfiguration config)
    {
        return ModuleRegistry.Instance.TryAddModules(services, config);
    }

    public static IEndpointRouteBuilder MapModules(this IEndpointRouteBuilder app, IConfiguration config)
    {
        return ModuleRegistry.Instance.MapModules(app, config);
    }
}