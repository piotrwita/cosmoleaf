namespace Erp.WmsConnector.Shared.Abstractions.FeatureManagement;

/// <summary>
/// Centralized feature flag definitions for the ERP WMS Connector service.
/// All feature flags should be defined here to ensure consistency across modules.
/// </summary>
public static class FeatureFlags
{
    /// <summary>
    /// Feature flags for shared capabilities
    /// </summary>
    public static class Shared
    {
        /// <summary>
        /// Enables periodic data retention cleanups
        /// </summary>
        public const string DataRetention = "Shared.DataRetention";
    }

    /// <summary>
    /// Feature flags for Dictionaries module
    /// </summary>
    public static class Dictionaries
    {
        /// <summary>
        /// Enables synchronization of articles on ERP side
        /// </summary>
        public const string ArticlesSync = "Dictionaries.ArticlesSync";
        /// <summary>
        /// Enables publishing notifications when articles are synchronized
        /// </summary>
        public const string ArticlesSyncNotification = "Dictionaries.ArticlesSyncNotification";

        /// <summary>
        /// Enables synchronization of contractors on ERP side
        /// </summary>
        public const string ContractorsSync = "Dictionaries.ContractorsSync";

        /// <summary>
        /// Enables publishing notifications when contractors are synchronized
        /// </summary>
        public const string ContractorsSyncNotification = "Dictionaries.ContractorsSyncNotification";
    }

    /// <summary>
    /// Feature flags for Notifications module
    /// </summary>
    public static class Notifications
    {
        /// <summary>
        /// Enables consumption of alerts from the Alerts queue
        /// </summary>
        public const string AlertsConsumption = "Notifications.AlertsConsumption";

        /// <summary>
        /// Enables the background service that sends alert emails
        /// </summary>
        public const string EmailSending = "Notifications.EmailSending";
    }

    /// <summary>
    /// Feature flags for Orders module
    /// </summary>
    public static class Orders
    {
        /// <summary>
        /// Enables consuming and processing of inbound order events
        /// </summary>
        public const string InboundSync = "Orders.InboundSync";

        /// <summary>
        /// Enables publishing sync notifications when orders are processed
        /// </summary>
        public const string SyncNotification = "Orders.SyncNotification";

        /// <summary>
        /// Enables the overall WMS order realization inbound flow.
        /// When enabled, realization messages from WMS are processed.
        /// </summary>
        public const string WmsRealizationInbound = "Orders.WmsRealizationInbound";

        /// <summary>
        /// When enabled, the Orders module handler processes realization messages directly.
        /// When disabled (default), messages are routed to Legacy system for processing.
        /// Requires <see cref="WmsRealizationInbound"/> to be enabled.
        /// </summary>
        public const string WmsRealizationInboundSerp = "Orders.WmsRealizationInboundSerp";

        /// <summary>
        /// Enables consuming and processing of outbound order events
        /// </summary>
        public const string OutboundSync = "Orders.OutboundSync";

        /// <summary>
        /// Enables publishing sync notifications when outbound orders are processed
        /// </summary>
        public const string OutboundSyncNotification = "Orders.OutboundSyncNotification";

        /// <summary>
        /// Enables the overall WMS order realization outbound flow.
        /// When enabled, realization messages from WMS are processed.
        /// </summary>
        public const string WmsRealizationOutbound = "Orders.WmsRealizationOutbound";

        /// <summary>
        /// When enabled, the Orders module handler processes outbound realization messages directly.
        /// When disabled (default), messages are routed to Legacy system for processing.
        /// Requires <see cref="WmsRealizationOutbound"/> to be enabled.
        /// </summary>
        public const string WmsRealizationOutboundSerp = "Orders.WmsRealizationOutboundSerp";
    }

    /// <summary>
    /// Feature flags for Webhooks module
    /// </summary>
    public static class Webhooks
    {
        /// <summary>
        /// Enables consumption of external event logs from the ExternalEventLogs queue
        /// </summary>
        public const string ExternalEventLogsConsumption = "Webhooks.ExternalEventLogsConsumption";
    }
}
