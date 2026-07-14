namespace Erp.WmsConnector.Shared.Infrastructure.Helpers;

public static class EnumHelper
{
    public static T ParseOrDefault<T>(string? option, T? defaultValue = null)
        where T : struct, Enum
    {
        if (string.IsNullOrWhiteSpace(option))
        {
            return GetFirstValue<T>();
        }

        if (Enum.TryParse(option, ignoreCase: true, out T result))
        {
            return result;
        }

        return defaultValue ?? GetFirstValue<T>();
    }

    public static T GetFirstValue<T>()
        where T : struct, Enum
    {
        T[] values = Enum.GetValues<T>();
        if (values.Length == 0)
        {
            throw new InvalidOperationException($"Enum type '{typeof(T).Name}' has no defined values.");
        }

        return values[0];
    }
}