using Microsoft.Extensions.Logging;

namespace Erp.WmsConnector.Shared.Infrastructure.Logging;

public static class LoggerExtensions
{
    private static readonly string CodeColumn = Constants.CodePropertyName;

    public static IDisposable? BeginScopeWithCode(this ILogger logger, string code)
    {
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentNullException.ThrowIfNull(code);

        return logger.BeginScope(new[] { new KeyValuePair<string, object?>(CodeColumn, code) });
    }

    public static void LogWithCode(
        this ILogger logger,
        LogLevel logLevel,
        string code,
        string messageTemplate,
        params object?[] args)
        => logger.LogWithCode(logLevel, code, exception: null, messageTemplate, args);

    public static void LogWithCode(
        this ILogger logger,
        LogLevel logLevel,
        string code,
        Exception? exception,
        string messageTemplate,
        params object?[] args)
    {
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentNullException.ThrowIfNull(messageTemplate);

        if (string.IsNullOrWhiteSpace(code))
        {
            logger.Log(logLevel, exception, messageTemplate, args);
            return;
        }

        using IDisposable? _ = logger.BeginScopeWithCode(code);

        logger.Log(logLevel, exception, messageTemplate, args);
    }

    public static void LogInformationWithCode(
        this ILogger logger,
        string code,
        string messageTemplate,
        params object?[] args)
        => logger.LogWithCode(LogLevel.Information, code, messageTemplate, args);

    public static void LogInformationWithCode(
        this ILogger logger,
        Exception? exception,
        string code,
        string messageTemplate,
        params object?[] args)
        => logger.LogWithCode(LogLevel.Information, code, exception, messageTemplate, args);

    public static void LogErrorWithCode(
        this ILogger logger,
        string code,
        string messageTemplate,
        params object?[] args)
        => logger.LogWithCode(LogLevel.Error, code, messageTemplate, args);

    public static void LogErrorWithCode(
        this ILogger logger,
        Exception? exception,
        string code,
        string messageTemplate,
        params object?[] args)
        => logger.LogWithCode(LogLevel.Error, code, exception, messageTemplate, args);
}