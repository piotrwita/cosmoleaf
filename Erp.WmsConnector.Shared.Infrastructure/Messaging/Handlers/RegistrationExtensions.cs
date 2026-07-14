using System.Diagnostics.CodeAnalysis;
using Erp.WmsConnector.Shared.Abstractions.Messaging.Handlers;
using Kitayec.Abstractions.Messaging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.FeatureManagement;

namespace Erp.WmsConnector.Shared.Infrastructure.Messaging.Handlers;

public static class RegistrationExtensions
{
    /// <summary>
    /// Registers a sync handler with its notification publisher and orchestrator for a specific message type.
    /// </summary>
    [SuppressMessage(
        "Major Code Smell",
        "S2436:Types should not have too many generic parameters",
        Justification = "All 4 type parameters are necessary to enforce compile-time type safety. " +
                        "TData must be explicit to ensure TSyncHandler and TNotificationPublisher " +
                        "use matching types, preventing subtle bugs that would otherwise only surface at runtime.")]
    public static IServiceCollection AddSyncHandler<TMessage, TData, TSyncHandler, TNotificationPublisher>(this IServiceCollection services)
        where TMessage : class, IMessage
        where TSyncHandler : class, ISyncHandler<TMessage, TData>
        where TNotificationPublisher : class, INotificationPublisher<TMessage, TData>
    {
        services.AddScoped<ISyncHandler<TMessage, TData>, TSyncHandler>();
        services.AddScoped<INotificationPublisher<TMessage, TData>, TNotificationPublisher>();
        services.AddScoped<IMessageHandler<TMessage>>(sp =>
        {
            var syncHandler = sp.GetRequiredService<ISyncHandler<TMessage, TData>>();
            var notificationPublisher = sp.GetRequiredService<INotificationPublisher<TMessage, TData>>();
            var alertPublisher = sp.GetRequiredService<ISyncHandlerAlertPublisher<TMessage>>();
            var featureManager = sp.GetRequiredService<IFeatureManager>();
            var logger = sp.GetRequiredService<ILogger<SyncHandlerOrchestrator<TMessage, TData>>>();
            return new SyncHandlerOrchestrator<TMessage, TData>(
                syncHandler, notificationPublisher, alertPublisher, featureManager, logger);
        });

        return services;
    }
}
