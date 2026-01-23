using Ddap.Core;

namespace Ddap.Core.Internals;

/// <summary>
/// Concrete implementation of <see cref="IIndexConfiguration"/>.
/// </summary>
internal class IndexConfiguration : IIndexConfiguration
{
    /// <inheritdoc/>
    public required string IndexName { get; init; }

    /// <inheritdoc/>
    public required IReadOnlyList<string> PropertyNames { get; init; }

    /// <inheritdoc/>
    public bool IsUnique { get; init; }

    /// <inheritdoc/>
    public bool IsClustered { get; init; }
}
