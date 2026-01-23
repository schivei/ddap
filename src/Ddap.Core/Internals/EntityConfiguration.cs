using Ddap.Core;

namespace Ddap.Core.Internals;

/// <summary>
/// Concrete implementation of <see cref="IEntityConfiguration"/>.
/// </summary>
internal class EntityConfiguration : IEntityConfiguration
{
    /// <inheritdoc/>
    public required string EntityName { get; init; }

    /// <inheritdoc/>
    public string? SchemaName { get; init; }

    /// <inheritdoc/>
    public required IReadOnlyList<IPropertyConfiguration> Properties { get; init; }

    /// <inheritdoc/>
    public IReadOnlyList<IIndexConfiguration> Indexes { get; init; } =
        Array.Empty<IIndexConfiguration>();

    /// <inheritdoc/>
    public IReadOnlyList<IRelationshipConfiguration> Relationships { get; init; } =
        Array.Empty<IRelationshipConfiguration>();
}
