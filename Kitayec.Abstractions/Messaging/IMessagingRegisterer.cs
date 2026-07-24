using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Kitayec.Abstractions.Messaging;

public interface IMessagingRegisterer
{
    IServiceCollection Services { get; }
    IConfiguration Configuration { get; }
}