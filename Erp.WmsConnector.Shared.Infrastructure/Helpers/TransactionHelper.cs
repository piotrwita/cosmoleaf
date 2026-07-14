using System.Transactions;
using Erp.WmsConnector.Shared.Infrastructure.Database;

namespace Erp.WmsConnector.Shared.Infrastructure.Helpers;

public static class TransactionHelper
{
    public static TransactionScope? TryBeginTransactionScope()
    {
        var options = new TransactionOptions
        {
            IsolationLevel = IsolationLevel.ReadCommitted,
            Timeout = TimeSpan.FromSeconds(TransactionExtensions.CommitTimeoutSeconds),
        };

        if (IsInExternalTransaction())
        {
            return null;
        }
        else
        {
            return new TransactionScope(TransactionScopeOption.Required, options, TransactionScopeAsyncFlowOption.Enabled);
        }
    }

    public static bool IsInExternalTransaction()
        => Transaction.Current != null;
}
