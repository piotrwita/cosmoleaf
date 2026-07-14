using Erp.WmsConnector.Shared.Abstractions.HealthChecks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Erp.WmsConnector.Shared.Infrastructure.HealthChecks;

internal sealed record HealthChecksRegisterer(IServiceCollection Services, IConfiguration Configuration) : IHealthChecksRegisterer;
