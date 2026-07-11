namespace Erp.WmsConnector.Shared.ModuleDefinition.Constants;

/// <summary>
/// Contains shared assembly naming constants used across the module discovery and registration system.
/// </summary>
internal static class AssemblyConstants
{
    /// <summary>
    /// The base name for all ERP WMS Connector assemblies.
    /// </summary>
    public const string BaseName = "Erp.WmsConnector";

    /// <summary>
    /// The shared infrastructure assembly name prefix.
    /// </summary>
    public const string SharedPrefix = $"{BaseName}.Shared";

    /// <summary>
    /// The gateway assembly name prefix.
    /// </summary>
    public const string GatewayPrefix = $"{BaseName}.Gateway";

    /// <summary>
    /// The file pattern for discovering ERP WMS Connector assemblies.
    /// </summary>
    public const string DiscoveryPattern = $"{BaseName}.*.dll";
}
