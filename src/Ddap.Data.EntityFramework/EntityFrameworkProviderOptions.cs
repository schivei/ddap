namespace Ddap.Data.EntityFramework;

/// <summary>
/// Options for configuring the EntityFramework data provider.
/// </summary>
public class EntityFrameworkProviderOptions
{
    /// <summary>
    /// Gets or sets a filter function for entity types.
    /// Return true to include the entity type, false to exclude it.
    /// </summary>
    public Func<Type, bool>? EntityFilter { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to discover entities from DbSet properties.
    /// Default is true.
    /// </summary>
    public bool DiscoverFromDbSets { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether to use the DbContext model metadata.
    /// Default is true.
    /// </summary>
    public bool UseModelMetadata { get; set; } = true;
}
