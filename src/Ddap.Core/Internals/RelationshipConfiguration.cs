using Ddap.Core;

namespace Ddap.Core.Internals;

/// <summary>
/// Concrete implementation of <see cref="IRelationshipConfiguration"/>.
/// </summary>
internal class RelationshipConfiguration : IRelationshipConfiguration
{
    /// <inheritdoc/>
    public required string RelationshipName { get; init; }

    /// <inheritdoc/>
    public required string RelatedEntityName { get; init; }

    /// <inheritdoc/>
    public required RelationshipType RelationshipType { get; init; }

    /// <inheritdoc/>
    public required IReadOnlyList<string> ForeignKeyProperties { get; init; }

    /// <inheritdoc/>
    public required IReadOnlyList<string> PrincipalKeyProperties { get; init; }

    /// <inheritdoc/>
    public bool IsRequired { get; init; }
}
