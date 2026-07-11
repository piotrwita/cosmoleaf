using System.Reflection;

namespace Erp.WmsConnector.Shared.ModuleDefinition.Interfaces;

public interface IModuleDiscovery
{
    IReadOnlyList<Assembly> Assemblies { get; }
    IReadOnlyList<Type> FindModuleTypes();
}