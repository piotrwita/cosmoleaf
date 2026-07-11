using Erp.WmsConnector.Shared.ModuleDefinition.Helpers;
using Erp.WmsConnector.Shared.ModuleDefinition.Interfaces;

namespace Erp.WmsConnector.Shared.ModuleDefinition;

internal sealed class DefaultModuleFactory : IModuleFactory
{
    public ModuleMetadata? Create(Type t)
    {
        if (Activator.CreateInstance(t) is not ModuleDefinition module)
        {
            return null;
        }

        return new ModuleMetadata(AssemblyDiscoveryHelper.GetBaseModuleAssemblyName(t), module);
    }
}