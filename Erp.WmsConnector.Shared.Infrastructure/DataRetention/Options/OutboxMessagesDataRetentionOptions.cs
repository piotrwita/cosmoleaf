using Erp.WmsConnector.Shared.Abstractions.Options;

namespace Erp.WmsConnector.Shared.Infrastructure.DataRetention.Options;

internal sealed class OutboxMessagesDataRetentionOptions : IOptions
{
    public static string SectionName => "DataRetention:Modules:Messaging:OutboxMessages";
    public int RetentionDays { get; init; } = 30;
}
