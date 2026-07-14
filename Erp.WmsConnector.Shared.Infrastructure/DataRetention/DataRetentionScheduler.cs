namespace Erp.WmsConnector.Shared.Infrastructure.DataRetention;

internal static class DataRetentionScheduler
{
    public static DateTime CalculateNextRunUtc(DateTime nowUtc, int startAtHourUtc)
    {
        DateTime today = new(nowUtc.Year, nowUtc.Month, nowUtc.Day, startAtHourUtc, 0, 0, DateTimeKind.Utc);
        return nowUtc < today ? today : today.AddDays(1);
    }
}
