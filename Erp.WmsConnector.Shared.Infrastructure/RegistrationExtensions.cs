using System.Reflection;
using Erp.WmsConnector.Shared.Infrastructure.Authorization;
using Erp.WmsConnector.Shared.Infrastructure.DataRetention;
using Erp.WmsConnector.Shared.Infrastructure.Database;
using Erp.WmsConnector.Shared.Infrastructure.Dispatching;
using Erp.WmsConnector.Shared.Infrastructure.ErrorHandling;
using Erp.WmsConnector.Shared.Infrastructure.Logging.Middleware;
using Erp.WmsConnector.Shared.Infrastructure.Messaging.Decorators;
using Erp.WmsConnector.Shared.Infrastructure.Messaging.Database;
using Erp.WmsConnector.Shared.Infrastructure.Messaging.Outbox;
using Erp.WmsConnector.Shared.Infrastructure.Messaging.RabbitMq;
using Erp.WmsConnector.Shared.Infrastructure.Serialization;
using Kitayec.Abstractions.Messaging;
using Kitayec.Infrastructure.Messaging;
using Kitayec.Infrastructure.Messaging.Deduplication;
using Kitayec.Infrastructure.Messaging.RabbitMq;
using Kitayec.Infrastructure.Messaging.Resiliency;
using Kitayec.Infrastructure.Observability;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Erp.WmsConnector.Shared.Infrastructure;

public static class RegistrationExtensions
{
    public static IServiceCollection AddCoreInfrastructure(this IServiceCollection services, IConfiguration configuration, IReadOnlyCollection<Assembly>? assemblies = null)
    {
        services.AddAuth(configuration);

        services.AddCqrs(assemblies);

        services.AddDatabaseInitializer();

        services.AddSerialization();

        services.AddMessaging(configuration,
            x => x.UseRabbitMq()
                .UseMessagePublisherConvention<RabbitMqCustomMessageConventionProvider>()
                .UseMessageConsumerConvention<RabbitMqCustomMessageConventionProvider>()
                .UseUnifiedDatabase()
                .UseDeduplication()
                .UseOutbox()
                .UseResiliency());

        services.TryDecorate(typeof(IMessageHandler<>), typeof(CorrelationIdMessageHandlerDecorator<>));

        services.AddObservability(configuration);
        services.AddDataRetention(configuration);

        return services;
    }

    public static WebApplication UseCoreInfrastructure(this WebApplication app)
    {
        app.UseGlobalErrorHandling();
        app.UseCorrelationId();
        app.UseAuth();

        return app;
    }
}