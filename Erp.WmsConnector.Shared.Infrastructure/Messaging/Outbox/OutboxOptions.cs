using Erp.WmsConnector.Shared.Abstractions.Options;

namespace Erp.WmsConnector.Shared.Infrastructure.Messaging.Outbox;

internal sealed class OutboxOptions : IOptions
{
    public static string SectionName => "Messaging:Outbox";
    /// <summary>
    /// Prefix prepended to module keys from ModuleBatchSizes when querying outbox by message type
    /// (e.g. "Dictionaries" → "Erp.WmsConnector.Dictionaries" for DB LIKE match).
    /// </summary>
    public const string MessageTypePrefix = "Erp.WmsConnector";
    public bool Enabled { get; init; }
    public IReadOnlyDictionary<string, int> ModuleBatchSizes { get; init; } = new Dictionary<string, int>();
}