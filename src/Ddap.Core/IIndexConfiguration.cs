namespace Ddap.Core;

/// <summary>
/// Represents the configuration for an index on an entity.
/// </summary>
public interface IIndexConfiguration
{
    /// <summary>
    /// Gets the name of the index.
    /// </summary>
    string IndexName { get; }

    /// <summary>
    /// Gets the collection of property names included in the index.
    /// </summary>
    IReadOnlyList<string> PropertyNames { get; }

    /// <summary>
    /// Gets a value indicating whether this is a unique index.
    /// </summary>
    bool IsUnique { get; }

    /// <summary>
    /// Gets a value indicating whether this is a clustered index.
    /// </summary>
    bool IsClustered { get; }
}
