using Erp.WmsConnector.Shared.Infrastructure.Options;

namespace Erp.WmsConnector.Shared.Infrastructure.Messaging.Database;

internal sealed class MessagingDatabaseOptions : DatabaseOptions
{
    public new static string SectionName => "Messaging:Database";
}