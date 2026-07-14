using Erp.WmsConnector.Shared.Abstractions.Options;

namespace Erp.WmsConnector.Shared.Infrastructure.Options;

public sealed class GatewayMetadata : IOptions
{
    public static string SectionName => "Gateway";
    public string? Name { get; init; }
    public string? Version { get; init; }
}