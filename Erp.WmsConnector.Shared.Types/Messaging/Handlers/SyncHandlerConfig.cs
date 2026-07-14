namespace Erp.WmsConnector.Shared.Types.Messaging.Handlers;

public sealed record SyncHandlerConfig(
    string SyncFeatureFlag,
    string NotificationFeatureFlag);
