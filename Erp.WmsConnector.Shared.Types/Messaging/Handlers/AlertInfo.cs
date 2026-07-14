namespace Erp.WmsConnector.Shared.Types.Messaging.Handlers;

public sealed record AlertInfo(
    string AlertType,
    object Content,
    string? RoutingKey = null);
