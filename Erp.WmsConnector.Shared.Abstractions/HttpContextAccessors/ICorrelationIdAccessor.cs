namespace Erp.WmsConnector.Shared.Abstractions.HttpContextAccessors;

public interface ICorrelationIdAccessor
{
    string GetCorrelationId();
    void SetCorrelationId(string correlationId);
    static string GenerateCorrelationId() => Guid.NewGuid().ToString("N")[..16];
}