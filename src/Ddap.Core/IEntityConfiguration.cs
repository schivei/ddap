namespace Ddap.Core;

/// <summary>
/// Represents the configuration for a database entity.
/// This interface defines the metadata and structure of entities loaded from the database.
/// </summary>
public interface IEntityConfiguration
{
    /// <summary>
    /// Gets the name of the entity (typically the table name).
    /// </summary>
    string EntityName { get; }

    /// <summary>
    /// Gets the schema name where the entity is located.
    /// </summary>
    string? SchemaName { get; }

    /// <summary>
    /// Gets the collection of properties/columns for this entity.
    /// </summary>
    IReadOnlyList<IPropertyConfiguration> Properties { get; }

    /// <summary>
    /// Gets the collection of indexes defined for this entity.
    /// </summary>
    IReadOnlyList<IIndexConfiguration> Indexes { get; }

    /// <summary>
    /// Gets the collection of relationships for this entity.
    /// </summary>
    IReadOnlyList<IRelationshipConfiguration> Relationships { get; }
}
