namespace Erp.WmsConnector.Shared.ModuleDefinition.Interfaces;

public interface IModuleFactory
{
    ModuleMetadata? Create(Type type);
}