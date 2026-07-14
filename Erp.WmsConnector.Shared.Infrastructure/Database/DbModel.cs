namespace Erp.WmsConnector.Shared.Infrastructure.Database;

/// <summary>
/// Base class for database entities providing audit columns.
/// </summary>
public abstract class DbModel
{
    /// <summary>
    /// Row version.
    /// </summary>
    public int Version { get; set; }

    /// <summary>
    /// Timestamp when the entity was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Timestamp when the entity was last updated.
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}

