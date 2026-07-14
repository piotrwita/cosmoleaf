namespace Erp.WmsConnector.Shared.Infrastructure.DataRetention;

internal static class DataRetentionLogCodes
{
    public const string DisabledFeature = "data_retention_disabled_feature";
    public const string DisabledConfig = "data_retention_disabled_config";
    public const string InvalidSettings = "data_retention_invalid_settings";
    public const string Started = "data_retention_started";
    public const string NextRun = "data_retention_next_run";
    public const string Stopping = "data_retention_stopping";
    public const string Failed = "data_retention_failed";
    public const string SweepStarted = "data_retention_sweep_started";
    public const string TaskSkipped = "data_retention_task_skipped";
    public const string TaskCompleted = "data_retention_task_completed";
    public const string TaskFailed = "data_retention_task_failed";
}
