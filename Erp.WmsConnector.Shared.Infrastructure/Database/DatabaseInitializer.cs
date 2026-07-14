using System.Diagnostics;
using Erp.WmsConnector.Shared.Abstractions.Database;
using Erp.WmsConnector.Shared.Types.Exceptions;
using Microsoft.Extensions.Logging;

namespace Erp.WmsConnector.Shared.Infrastructure.Database;

internal class DatabaseInitializer(ILogger<DatabaseInitializer> logger) : IDatabaseInitializer
{
    public async Task InitializeAsync(IMigratableDatabase dbContext, CancellationToken cancellationToken = default)
    {
        var stopwatch = new Stopwatch();
        stopwatch.Start();

        logger.LogInformation("Started database migration...");

        try
        {
            await dbContext.MigrateAsync(cancellationToken);
        }
        catch (Exception exception)
        {
            logger.LogCritical(exception, "Cannot run database migration. Please check your database connection and migration scripts.");

            throw new DatabaseInitializationException(exception);
        }
        finally
        {
            stopwatch.Stop();

            logger.LogInformation("Completed database migration in {Elapsed}.", stopwatch.Elapsed);
        }
    }
}
