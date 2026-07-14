using Erp.WmsConnector.Shared.Abstractions.Serialization;
using Microsoft.Extensions.DependencyInjection;

namespace Erp.WmsConnector.Shared.Infrastructure.Serialization;

public static class RegistrationExtensions
{
    public static IServiceCollection AddSerialization(this IServiceCollection services)
    {
        services.AddSingleton<ISerializer, JsonSerializer>();
        return services;
    }
}
