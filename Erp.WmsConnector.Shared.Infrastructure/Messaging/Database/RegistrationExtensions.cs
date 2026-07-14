using Erp.WmsConnector.Shared.Infrastructure.Options;
using Kitayec.Abstractions.Messaging;
using Microsoft.Extensions.DependencyInjection;

namespace Erp.WmsConnector.Shared.Infrastructure.Messaging.Database;

public static class RegistrationExtensions
{
    public static IMessagingRegisterer UseUnifiedDatabase(this IMessagingRegisterer registerer)
    {
        var fallbackOptions = registerer.Configuration.GetRequiredOptions<DatabaseOptions>();

        string connectionKey = $"{MessagingDatabaseOptions.SectionName}:{nameof(MessagingDatabaseOptions.ConnectionString)}";
        if (string.IsNullOrWhiteSpace(registerer.Configuration[connectionKey]))
        {
            registerer.Configuration[connectionKey] = fallbackOptions.ConnectionString;
        }

        string schemaKey = $"{MessagingDatabaseOptions.SectionName}:{nameof(MessagingDatabaseOptions.MigrationTableSchema)}";
        if (string.IsNullOrWhiteSpace(registerer.Configuration[schemaKey]))
        {
            registerer.Configuration[schemaKey] = fallbackOptions.MigrationTableSchema;
        }

        registerer.Services.AddOptions<MessagingDatabaseOptions>()
            .BindConfiguration(MessagingDatabaseOptions.SectionName);

        return registerer;
    }
}