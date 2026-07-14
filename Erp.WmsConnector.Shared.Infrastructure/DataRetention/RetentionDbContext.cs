using Microsoft.EntityFrameworkCore;

namespace Erp.WmsConnector.Shared.Infrastructure.DataRetention;

internal sealed class RetentionDbContext(DbContextOptions<RetentionDbContext> options) : DbContext(options);
