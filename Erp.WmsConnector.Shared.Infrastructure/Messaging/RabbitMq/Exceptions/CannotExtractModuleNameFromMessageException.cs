using Erp.WmsConnector.Shared.Types.Exceptions;

namespace Erp.WmsConnector.Shared.Infrastructure.Messaging.RabbitMq.Exceptions;
internal sealed class CannotExtractModuleNameFromMessageException(string messageTypeName) : BaseException($"Cannot extract module name from type: {messageTypeName}");
