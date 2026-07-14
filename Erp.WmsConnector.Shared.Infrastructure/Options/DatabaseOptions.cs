using Erp.WmsConnector.Shared.Abstractions.Database;
using Erp.WmsConnector.Shared.Abstractions.Options;

namespace Erp.WmsConnector.Shared.Infrastructure.Options;

internal class DatabaseOptions : IOptions, IDatabaseOptions
{
    public static string SectionName => "Database";
    public required string ConnectionString { get; set; }
    public required string MigrationTableSchema { get; set; }
}