namespace Erp.WmsConnector.Shared.Abstractions.Messaging.Handlers;

public interface ICorrelatedMessage
{
    Guid EventId { get; }
    Guid? CorrelationId { get; }
}
