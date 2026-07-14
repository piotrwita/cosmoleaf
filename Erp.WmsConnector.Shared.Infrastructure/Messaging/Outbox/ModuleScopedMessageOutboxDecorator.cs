using Kitayec.Abstractions.Messaging;
using Kitayec.Abstractions.Messaging.Outbox;
using Kitayec.Types;
using Erp.WmsConnector.Shared.Infrastructure.Logging;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Erp.WmsConnector.Shared.Infrastructure.Messaging.Outbox;

internal sealed class ModuleScopedMessageOutboxDecorator(
    IMessageOutbox messageOutbox,
    IOptions<OutboxOptions> outboxOptions,
    ILogger<ModuleScopedMessageOutboxDecorator> logger) : IMessageOutbox
{
    private const string OutboxFetchByModuleCode = "outbox_fetch_by_module";

    public async Task<IReadOnlyList<OutboxMessage>> GetUnsentAsync(
        int batchSize = default,
        CancellationToken cancellationToken = default)
    {
        var moduleLimits = outboxOptions.Value.ModuleBatchSizes;

        if (moduleLimits.Count == 0)
        {
            return await messageOutbox.GetUnsentAsync(batchSize, cancellationToken);
        }

        using var _ = logger.BeginScopeWithCode(OutboxFetchByModuleCode);

        var allOutboxMessages = new List<OutboxMessage>();
        foreach (var (moduleKey, limit) in moduleLimits)
        {
            string messageTypePrefix = $"{OutboxOptions.MessageTypePrefix}.{moduleKey}";
            var outboxMessages = await messageOutbox.GetUnsentByMessageTypeAsync(limit, messageTypePrefix, cancellationToken);
            allOutboxMessages.AddRange(outboxMessages);

            logger.LogInformation("Module={Module} UnsentMessages={MessageCount} Limit={Limit}", moduleKey, outboxMessages.Count, limit);        
        }

        return allOutboxMessages;
    }

    public Task AddAsync<TMessage>(TMessage message, string messageId, string? destination = default, string? routingKey = default,
        IDictionary<string, object>? headers = default, CancellationToken cancellationToken = default) where TMessage : IMessage
        => messageOutbox.AddAsync(message, messageId, destination, routingKey, headers, cancellationToken);

    public Task MarkAsProcessedAsync(OutboxMessage outboxMessage, CancellationToken cancellationToken = default)
        => messageOutbox.MarkAsProcessedAsync(outboxMessage, cancellationToken);
}
