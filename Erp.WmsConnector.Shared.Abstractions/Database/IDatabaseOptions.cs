namespace Erp.WmsConnector.Shared.Abstractions.Database;

public interface IDatabaseOptions
{
    string ConnectionString { get; set; }
    string MigrationTableSchema { get; set; }
}
