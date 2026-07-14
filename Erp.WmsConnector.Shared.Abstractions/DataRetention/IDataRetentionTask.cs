namespace Erp.WmsConnector.Shared.Abstractions.DataRetention;

public interface IDataRetentionTask
{
    const int MinimumRetentionDays = 7;

    string Name { get; }
    int MinConfiguredRetentionDays { get; }
    Task<int> ExecuteAsync(CancellationToken cancellationToken);
}
