using Erp.WmsConnector.Shared.Types.Exceptions;

namespace Erp.WmsConnector.Shared.Infrastructure.DataRetention;

internal sealed class DataRetentionServiceException(Exception innerException)
    : BaseException("Data retention service execution failed.", innerException);
