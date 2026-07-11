namespace Erp.WmsConnector.Shared.ModuleDefinition.Helpers;

internal static class AssemblyDiscoveryHelper
{
    internal static string GetBaseModuleAssemblyName(Type t)
    {
        const string apiSuffix = ".Api";
        string? assemblyName = t.Assembly.GetName().Name;
        if (assemblyName?.EndsWith(apiSuffix, StringComparison.Ordinal) == true)
        {
            return assemblyName[..^apiSuffix.Length];
        }

        return assemblyName ?? string.Empty;
    }
}