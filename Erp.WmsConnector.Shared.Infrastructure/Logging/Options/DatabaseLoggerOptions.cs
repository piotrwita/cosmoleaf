using Erp.WmsConnector.Shared.Abstractions.Options;

namespace Erp.WmsConnector.Shared.Infrastructure.Logging.Options;

public sealed class DatabaseLoggerOptions : IOptions
{
    public static string SectionName => "Logger:Database";
    public bool Enabled { get; init; }
    public string Table { get; init; } = string.Empty;
    public string Schema { get; init; } = string.Empty;
    public string ConnectionString { get; init; } = string.Empty;
}