using Erp.WmsConnector.Shared.Abstractions.HttpContextAccessors;
using Serilog.Core;
using Serilog.Events;

namespace Erp.WmsConnector.Shared.Infrastructure.Logging.Enrichers;

public sealed class CorrelationIdEnricher : ILogEventEnricher
{
    private readonly ICorrelationIdAccessor? _correlationIdAccessor;

    public CorrelationIdEnricher()
    { }

    public CorrelationIdEnricher(ICorrelationIdAccessor correlationIdAccessor)
        => _correlationIdAccessor = correlationIdAccessor;

    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        if (logEvent.Properties.ContainsKey(Constants.CorrelationPropertyName))
        {
            return;
        }

        string correlationId = _correlationIdAccessor?.GetCorrelationId() ?? ICorrelationIdAccessor.GenerateCorrelationId();
        LogEventProperty correlationIdProperty = propertyFactory.CreateProperty(Constants.CorrelationPropertyName, correlationId);

        logEvent.AddPropertyIfAbsent(correlationIdProperty);
    }
}
