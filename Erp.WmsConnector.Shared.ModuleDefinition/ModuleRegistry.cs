using System.Collections.Concurrent;
using System.Reflection;
using Erp.WmsConnector.Shared.ModuleDefinition.Constants;
using Erp.WmsConnector.Shared.ModuleDefinition.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Erp.WmsConnector.Shared.ModuleDefinition;

public sealed class ModuleRegistry
{
    private static ModuleRegistry? _instance;
    public static ModuleRegistry Instance => _instance ??= Create();
    public static ModuleRegistry Create(
        IModuleDiscovery? discovery = null,
        IModuleFactory? factory = null,
        IModuleConfig? config = null)
        => new ModuleRegistry(
            discovery ?? new DefaultModuleDiscovery(),
            factory ?? new DefaultModuleFactory(),
            config ?? new DefaultModuleConfig());

    private readonly IModuleDiscovery _discovery;
    private readonly IModuleFactory _factory;
    private readonly IModuleConfig _config;
    private bool _hasModulesBeenMapped;

    private readonly ConcurrentDictionary<string, ModuleMetadata> _registered = new();

    private readonly Lazy<IReadOnlyCollection<ModuleDefinition>> _registeredModules;
    private readonly Lazy<IReadOnlyCollection<Assembly>> _enabledAssemblies;

    private ModuleRegistry(
        IModuleDiscovery discovery,
        IModuleFactory factory,
        IModuleConfig config)
    {
        _discovery = discovery;
        _factory = factory;
        _config = config;
        _registeredModules = new Lazy<IReadOnlyCollection<ModuleDefinition>>(GetRegisteredModules);
        _enabledAssemblies = new Lazy<IReadOnlyCollection<Assembly>>(GetEnabledAssemblies);
    }

    public IReadOnlyCollection<ModuleDefinition> RegisteredModules => _registeredModules.Value;
    public IReadOnlyCollection<Assembly> EnabledAssemblies => _enabledAssemblies.Value;

    /// <summary>
    /// Should be invoked after the method TryAddModules otherwise it will not find module assemblies.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when the method TryAddModules has not been called yet.</exception>
    private IReadOnlyCollection<ModuleDefinition> GetRegisteredModules()
    {
        ThrowIfModulesNotMapped();

        return _registered.Values.Select(x => x.Module).ToArray();
    }

    /// <inheritdoc cref="GetRegisteredModules"/>
    private IReadOnlyCollection<Assembly> GetEnabledAssemblies()
    {
        ThrowIfModulesNotMapped();

        var enabledModuleAssemblies = Instance.GetEnabledModuleAssemblies();
        var restAssemblies = AppDomain.CurrentDomain.GetAssemblies()
            .Where(a => a.GetName().Name?.StartsWith(AssemblyConstants.SharedPrefix) == true ||
                        a.GetName().Name?.StartsWith(AssemblyConstants.GatewayPrefix) == true)
            .ToArray();

        return [.. enabledModuleAssemblies, .. restAssemblies];
    }

    private void ThrowIfModulesNotMapped()
    {
        if (!_hasModulesBeenMapped)
        {
            throw new InvalidOperationException("Modules have not been mapped yet. Call the method TryAddModules first.");
        }
    }

    private IReadOnlyCollection<Assembly> GetEnabledModuleAssemblies()
    {
        List<string> enabledModuleBaseNames = _registered.Values.Select(x => x.BaseAssemblyName).ToList();

        List<Assembly> enabledAssemblies = _discovery.Assemblies
            .Where(a => enabledModuleBaseNames.Any(baseName => a.GetName().Name?.StartsWith(baseName) == true))
            .ToList();

        return enabledAssemblies;
    }

    public IServiceCollection TryAddModules(IServiceCollection services, IConfiguration config)
    {
        if (_hasModulesBeenMapped)
        {
            return services;
        }

        var moduleTypes = _discovery.FindModuleTypes();
        foreach (Type type in moduleTypes)
        {
            ModuleMetadata? metadata = _factory.Create(type);
            if (metadata is null)
            {
                throw new InvalidOperationException(
                    $"Failed to create instance of module type '{type.FullName}'. " +
                    "Ensure the type has a public parameterless constructor.");
            }

            if (!_config.IsEnabled(metadata.Module, config))
            {
                continue;
            }

            if (_registered.TryAdd(type.Name, metadata))
            {
                metadata.Module.AddDependencies(services, config);
            }
        }

        _hasModulesBeenMapped = true;

        return services;
    }

    public IEndpointRouteBuilder MapModules(IEndpointRouteBuilder app, IConfiguration config)
    {
        foreach (ModuleDefinition module in _registered.Values.Select(m => m.Module))
        {
            string baseRoute = _config.ResolveBaseRoute(module).Trim('/');
            var group = app.MapGroup("/" + baseRoute).WithTags(module.ApiTag ?? baseRoute);
            module.CreateEndpoints(group, config);
        }

        return app;
    }
}