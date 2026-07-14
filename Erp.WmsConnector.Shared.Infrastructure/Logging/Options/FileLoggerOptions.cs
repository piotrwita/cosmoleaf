using Erp.WmsConnector.Shared.Abstractions.Options;

namespace Erp.WmsConnector.Shared.Infrastructure.Logging.Options;

public sealed class FileLoggerOptions : IOptions
{
    public static string SectionName => "Logger:File";
    public bool Enabled { get; init; }
    public string Path { get; init; } = string.Empty;
    public string Interval { get; init; } = string.Empty;
    public int? RetainedFileCountLimit { get; init; }
}