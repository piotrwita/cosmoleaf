using Erp.WmsConnector.Shared.Abstractions.Options;

namespace Erp.WmsConnector.Shared.Infrastructure.HealthChecks.Options;


public sealed class HealthChecksOptions : IOptions
{
    public static string SectionName => "HealthChecks";
    public bool DatabaseEnabled { get; init; }
    public bool LegacyDatabaseEnabled { get; init; }
    public bool RabbitMqEnabled { get; init; }
    public int TimeoutSeconds { get; init; } = 5;
}