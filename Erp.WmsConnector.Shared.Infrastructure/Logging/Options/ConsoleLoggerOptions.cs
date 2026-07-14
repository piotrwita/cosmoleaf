using Erp.WmsConnector.Shared.Abstractions.Options;

namespace Erp.WmsConnector.Shared.Infrastructure.Logging.Options;

public sealed class ConsoleLoggerOptions : IOptions
{
    public static string SectionName => "Logger:Console";
    public bool Enabled { get; init; }
}