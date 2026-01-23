namespace Ddap.Core;

/// <summary>
/// Represents the configuration for a relationship between entities.
/// </summary>
public interface IRelationshipConfiguration
{
    /// <summary>
    /// Gets the name of the relationship.
    /// </summary>
    string RelationshipName { get; }

    /// <summary>
    /// Gets the name of the related entity.
    /// </summary>
    string RelatedEntityName { get; }

    /// <summary>
    /// Gets the type of relationship (one-to-one, one-to-many, many-to-many).
    /// </summary>
    RelationshipType RelationshipType { get; }

    /// <summary>
    /// Gets the foreign key property names in the current entity.
    /// </summary>
    IReadOnlyList<string> ForeignKeyProperties { get; }

    /// <summary>
    /// Gets the principal key property names in the related entity.
    /// </summary>
    IReadOnlyList<string> PrincipalKeyProperties { get; }

    /// <summary>
    /// Gets a value indicating whether this relationship is required.
    /// </summary>
    bool IsRequired { get; }
}

/// <summary>
/// Defines the type of relationship between entities.
/// </summary>
public enum RelationshipType
{
    /// <summary>
    /// One-to-one relationship.
    /// </summary>
    OneToOne,

    /// <summary>
    /// One-to-many relationship.
    /// </summary>
    OneToMany,

    /// <summary>
    /// Many-to-many relationship.
    /// </summary>
    ManyToMany
}
