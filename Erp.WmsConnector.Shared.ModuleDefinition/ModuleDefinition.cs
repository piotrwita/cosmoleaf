using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Erp.WmsConnector.Shared.ModuleDefinition;

public abstract class ModuleDefinition
{
    /// <summary>
    /// Main route segment. If null/whitespace → will be derived from class name.
    /// </summary>
    public virtual string? BaseRoute => null;

    /// <summary>
    /// Default enabled, if no entry in configuration.
    /// </summary>
    public virtual bool DefaultEnabled { get; } = true;

    /// <summary>
    /// Api tag for grouping endpoints. If null → will use BaseRoute.
    /// </summary>
    public virtual string? ApiTag => null;

    /// <summary>
    /// Module key in configuration (defaults to class name, e.g. "OrdersModule").
    /// </summary>
    public virtual string ModuleKey => GetType().Name;

    public abstract void AddDependencies(IServiceCollection services, IConfiguration config);
    public abstract void CreateEndpoints(IEndpointRouteBuilder endpoints, IConfiguration config);
}

public record ModuleMetadata(string BaseAssemblyName, ModuleDefinition Module);