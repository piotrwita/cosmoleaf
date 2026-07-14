namespace Erp.WmsConnector.Shared.Types.Exceptions;

public class MessageEventHandlingException(Exception innerEx)
    : BaseException("Message event handling failed", innerEx)
{ }
