using Erp.WmsConnector.Shared.Infrastructure.Options;
using Kitayec.Abstractions.Messaging;
using Kitayec.Abstractions.Messaging.Outbox;
using Kitayec.Infrastructure.Messaging.Outbox.Decorators;
using Microsoft.Extensions.DependencyInjection;

namespace Erp.WmsConnector.Shared.Infrastructure.Messaging.Outbox;

public static class RegistrationExtensions
{
    public static IMessagingRegisterer UseOutbox(this IMessagingRegisterer registerer)
    {
        Kitayec.Infrastructure.Messaging.Outbox.RegistrationExtensions.UseOutbox(registerer);

        var msgOptions = registerer.Configuration.GetRequiredOptions<OutboxOptions>();
        if (!msgOptions.Enabled)
        {
            return registerer;
        }

        registerer.Services.ConfigureOptions<OutboxOptions>(registerer.Configuration);

        if (msgOptions.ModuleBatchSizes.Count > 0)
        {
            registerer.Services.TryDecorate<IMessageOutbox, ModuleScopedMessageOutboxDecorator>();
        }

        registerer.Services.TryDecorate<IMessagePublisher, ScopedOutboxMessagePublisherDecorator>();

        return registerer;
    }
}