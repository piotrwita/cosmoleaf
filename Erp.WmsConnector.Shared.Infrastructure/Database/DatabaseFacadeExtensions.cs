using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Erp.WmsConnector.Shared.Infrastructure.Database;

internal static class DatabaseFacadeExtensions
{
    public const int DefaultBatchSize = 5000;

    /// <summary>
    /// Executes a SQL query in batches using the provided <paramref name="buildSql"/> function to construct
    /// the SQL statement for each batch, continuing until fewer rows than <paramref name="batchSize"/> are affected.
    /// Returns the total number of affected rows.
    /// </summary>
    /// <param name="database">The <see cref="DatabaseFacade"/> to execute SQL commands against.</param>
    /// <param name="buildSql">
    /// A function that takes the batch size and returns a <see cref="FormattableString"/>
    /// representing the SQL command to execute for that batch.
    /// </param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <param name="batchSize">The maximum number of rows to process in each batch. Defaults to <see cref="DefaultBatchSize"/>.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing
    /// the total number of rows affected across all executed batches.
    /// </returns>
    internal static async Task<int> ExecuteSqlBatchedAsync(
        this DatabaseFacade database,
        Func<int, FormattableString> buildSql,
        CancellationToken cancellationToken,
        int batchSize = DefaultBatchSize)
    {
        FormattableString sql = buildSql(batchSize);
        int totalAffected = 0;
        int affected;

        do
        {
            affected = await database.ExecuteSqlInterpolatedAsync(sql, cancellationToken);

            totalAffected += affected;
        } while (affected == batchSize);

        return totalAffected;
    }
}
