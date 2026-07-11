using Microsoft.Extensions.Configuration;

namespace Erp.WmsConnector.Shared.ModuleDefinition.Interfaces;

public interface IModuleConfig
{
    bool IsEnabled(ModuleDefinition module, IConfiguration config);
    string ResolveBaseRoute(ModuleDefinition module);
}