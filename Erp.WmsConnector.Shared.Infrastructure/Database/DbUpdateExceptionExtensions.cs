using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Erp.WmsConnector.Shared.Infrastructure.Database;

public static class DbUpdateExceptionExtensions
{
    /// <summary>
    /// Returns <see langword="true"/> when the exception was caused by a SQL Server unique
    /// constraint or unique index violation (error codes 2627 and 2601, respectively).
    /// </summary>
    /// <remarks>
    /// Use this in <c>when</c> filter clauses to distinguish duplicate-key failures from
    /// other <see cref="DbUpdateException"/> causes without catching and re-throwing:
    /// <code>
    /// catch (DbUpdateException ex) when (ex.IsUniqueConstraintViolation())
    /// {
    ///     // handle duplicate
    /// }
    /// </code>
    /// </remarks>
    public static bool IsUniqueConstraintViolation(this DbUpdateException exception)
        => exception.InnerException is SqlException { Number: 2627 or 2601 };
}
