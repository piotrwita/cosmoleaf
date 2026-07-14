using Erp.WmsConnector.Shared.Abstractions.Options;

namespace Erp.WmsConnector.Shared.Infrastructure.DataRetention.Options;

internal sealed class AppLogsDataRetentionOptions : IOptions
{
    public static string SectionName => "DataRetention:Modules:Logger:AppLogs";
    public int InformationRetentionDays { get; init; } = 7;
    public int OtherRetentionDays { get; init; } = 30;
}
