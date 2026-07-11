using Erp.WmsConnector.Shared.ModuleDefinition.Helpers;
using Erp.WmsConnector.Shared.ModuleDefinition.Interfaces;
using Humanizer;
using Microsoft.Extensions.Configuration;

namespace Erp.WmsConnector.Shared.ModuleDefinition;

internal sealed class DefaultModuleConfig : IModuleConfig
{
    const string ModuleSectionPrefix = "Modules:";
    const string EnabledKey = "Enabled";

    public bool IsEnabled(ModuleDefinition module, IConfiguration config)
    {
        var section = config.GetSection($"{ModuleSectionPrefix}{module.ModuleKey}");
        bool? enabled = section.GetValue<bool?>(EnabledKey);
        return enabled ?? module.DefaultEnabled;
    }

    public string ResolveBaseRoute(ModuleDefinition module)
    {
        string candidate;
        if (string.IsNullOrWhiteSpace(module.BaseRoute))
        {
            candidate = ModuleRouteHelper.DeriveRouteFromType(module.GetType());
        }
        else
        {
            candidate = module.BaseRoute!;
        }

        return candidate.Kebaberize();
    }
}
