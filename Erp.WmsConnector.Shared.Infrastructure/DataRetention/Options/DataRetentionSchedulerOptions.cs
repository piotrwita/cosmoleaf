using Erp.WmsConnector.Shared.Abstractions.Options;

namespace Erp.WmsConnector.Shared.Infrastructure.DataRetention.Options;

internal sealed class DataRetentionSchedulerOptions : IOptions
{
    public static string SectionName => "DataRetention";
    public bool Enabled { get; init; }
    public bool RunOnStartup { get; init; }
    public int StartAtHourUtc { get; init; } = 2;
    public const int IntervalHours = 24;
}
