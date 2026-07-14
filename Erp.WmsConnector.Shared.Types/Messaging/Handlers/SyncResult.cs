namespace Erp.WmsConnector.Shared.Types.Messaging.Handlers;

public sealed record SyncResult<TData>(
    TData? Data,
    AlertInfo? Alert)
{
    public static SyncResult<TData> Success(TData data) => Success(data, default);
    public static SyncResult<TData> Success(TData data, AlertInfo? alert) => new(data, alert);
    public static SyncResult<TData> Empty(AlertInfo alert) => new(default, alert);
    public static SyncResult<TData> Empty() => new(default, default);
}
