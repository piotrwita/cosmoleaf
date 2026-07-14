namespace Erp.WmsConnector.Shared.Types.Exceptions;

public sealed record ErrorResponse(string ErrorCode, string Message, string CorrelationId);