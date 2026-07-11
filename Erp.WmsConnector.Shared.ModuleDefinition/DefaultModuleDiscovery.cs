using System.Reflection;
using Erp.WmsConnector.Shared.ModuleDefinition.Constants;
using Erp.WmsConnector.Shared.ModuleDefinition.Interfaces;

namespace Erp.WmsConnector.Shared.ModuleDefinition;

internal sealed class DefaultModuleDiscovery(IReadOnlyList<Assembly>? assemblies = null) : IModuleDiscovery
{

    private readonly IReadOnlyList<Assembly> _assemblies = assemblies ?? LoadAssemblies();

    public IReadOnlyList<Assembly> Assemblies => _assemblies;

    public IReadOnlyList<Type> FindModuleTypes() =>
        _assemblies
            .SelectMany(a => a.GetTypes())
            .Where(t => !t.IsAbstract && typeof(ModuleDefinition).IsAssignableFrom(t))
            .ToList();

    private static IReadOnlyList<Assembly> LoadAssemblies()
    {
        var list = new List<Assembly>();
        string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
        foreach (string file in Directory.GetFiles(baseDirectory, AssemblyConstants.DiscoveryPattern, SearchOption.TopDirectoryOnly))
        {
            string assemblyName = Path.GetFileNameWithoutExtension(file);
            list.Add(Assembly.Load(assemblyName));
        }

        IEnumerable<Assembly> loaded = AppDomain.CurrentDomain.GetAssemblies()
            .Where(a => a.GetName().Name?.Contains(AssemblyConstants.BaseName) == true)
            .Where(a => !list.Contains(a));

        list.AddRange(loaded);

        return list;
    }
}