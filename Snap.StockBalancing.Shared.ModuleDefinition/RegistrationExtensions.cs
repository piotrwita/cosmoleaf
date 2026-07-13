using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Concurrent;

namespace Snap.StockBalancing.Shared.ModuleDefinition;

public static class Modules
{
    private static readonly ConcurrentDictionary<string, ModuleDefinition> RegisteredModules = [];

    public static void RegisterModule<TModule>(Func<TModule> moduleFactory = default) where TModule : ModuleDefinition
    {
        var moduleDefinition = moduleFactory is not null
            ? moduleFactory()
            : Activator.CreateInstance<TModule>();

        RegisteredModules.TryAdd(moduleDefinition.ModuleName, moduleDefinition);
    }

    public static void AddModules(this IServiceCollection services, IConfiguration configuration)
    {
        foreach (var module in RegisteredModules.Values)
        {
            module.AddDependencies(services, configuration);
        }
    }

    public static WebApplication UseModulesEndpoints(this WebApplication app)
    {
        foreach (var module in RegisteredModules.Values)
        {
            module.CreateEndpoints(app.MapGroup(module.ModulePrefix));
        }
        return app;
    }
}