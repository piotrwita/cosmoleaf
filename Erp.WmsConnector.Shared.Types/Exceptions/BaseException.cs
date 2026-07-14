using Humanizer;

namespace Erp.WmsConnector.Shared.Types.Exceptions;

/// <summary>
/// Base exception for all custom exceptions in the ERP-WMS Connector solution.
/// Provides common properties and structured error handling.
/// </summary>
public class BaseException : Exception
{
    private const string DefaultErrorCode = "internal_server";

    public string ErrorCode { get; init; }
    public virtual int StatusCode { get; init; } = 400;

    protected BaseException(string message)
        : base(message) => ErrorCode = GetErrorCodeFromType(GetType());

    protected BaseException(string message, Exception innerException)
        : base(message, innerException) => ErrorCode = GetErrorCodeFromType(GetType());

    public static string GetErrorCodeFromType(Type type)
    {
        const string exStr = "exception";
        const string suffix = $"_{exStr}";
        string name = type.Name.Underscore();
        if (name.EndsWith(suffix, StringComparison.Ordinal))
        {
            name = name[..^suffix.Length];
        }

        return string.IsNullOrEmpty(name) || name == exStr ? DefaultErrorCode : name;
    }
}