using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Erp.WmsConnector.Shared.Abstractions.HealthChecks;

public interface IHealthChecksRegisterer
{
    IServiceCollection Services { get; }
    IConfiguration Configuration { get; }
}