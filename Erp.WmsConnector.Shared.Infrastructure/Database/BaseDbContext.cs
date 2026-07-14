using System.Diagnostics.CodeAnalysis;
using Erp.WmsConnector.Shared.Abstractions.Database;
using Microsoft.EntityFrameworkCore;

namespace Erp.WmsConnector.Shared.Infrastructure.Database;

[ExcludeFromCodeCoverage(Justification = "Abstract Base infrastructure class for DbContext")]
public abstract class BaseDbContext<T>(DbContextOptions<T> options) : DbContext(options), ISaveableContext
    where T : BaseDbContext<T>
{
    private const string MigrationsHistoryTable = "MigrationsHistory";

    protected abstract string Schema { get; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly,
            type => !typeof(IUnmigratable).IsAssignableFrom(type));

        modelBuilder.HasDefaultSchema(Schema);
    }

    public static void ConfigureWithMigrationSchema(DbContextOptionsBuilder options, string connectionString, string migrationSchema)
    {
        migrationSchema = string.IsNullOrWhiteSpace(migrationSchema) ? "dbo" : migrationSchema;
        options.UseSqlServer(connectionString, sqlOptions => sqlOptions.MigrationsHistoryTable(MigrationsHistoryTable, migrationSchema));
    }

    public virtual async Task MigrateAsync(CancellationToken cancellationToken = default)
        => await Database.MigrateAsync(cancellationToken);

    public bool HasChanges() => ChangeTracker.HasChanges();
}