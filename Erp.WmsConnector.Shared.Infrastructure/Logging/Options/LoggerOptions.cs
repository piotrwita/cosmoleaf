using Erp.WmsConnector.Shared.Abstractions.Options;

namespace Erp.WmsConnector.Shared.Infrastructure.Logging.Options;

public sealed class LoggerOptions : IOptions
{
    public static string SectionName => "Logger";
    public string Level { get; init; } = string.Empty;
    public Dictionary<string, string> Overrides { get; init; } = [];
    public ConsoleLoggerOptions? Console { get; init; }
    public FileLoggerOptions? File { get; init; }
    public DatabaseLoggerOptions? Database { get; init; }
}