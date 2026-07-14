using Erp.WmsConnector.Shared.Abstractions.Options;

namespace Erp.WmsConnector.Shared.Infrastructure.DataRetention.Options;

internal sealed class DeduplicatedMessagesDataRetentionOptions : IOptions
{
    public static string SectionName => "DataRetention:Modules:Messaging:DeduplicatedMessages";
    public int RetentionDays { get; init; } = 30;
}
