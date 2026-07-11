namespace Erp.WmsConnector.Shared.ModuleDefinition.Helpers;

internal static class ModuleRouteHelper
{
    internal static string DeriveRouteFromType(Type t)
    {
        const string moduleSuffix = "Module";
        string name = t.Name;
        if (name.EndsWith(moduleSuffix, StringComparison.Ordinal))
        {
            name = name[..^moduleSuffix.Length];
        }

        return name;
    }
}