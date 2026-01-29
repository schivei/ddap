namespace Ddap.Core;

/// <summary>
/// Options for configuring caching behavior.
/// </summary>
public class CachingOptions
{
    /// <summary>
    /// Gets or sets a value indicating whether caching is enabled.
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// Gets or sets the default cache duration.
    /// </summary>
    public TimeSpan DefaultDuration { get; set; } = TimeSpan.FromMinutes(10);

    /// <summary>
    /// Gets or sets the maximum cache size in megabytes.
    /// </summary>
    public int MaxSizeMb { get; set; } = 100;

    /// <summary>
    /// Gets or sets a value indicating whether to use distributed caching.
    /// </summary>
    public bool UseDistributedCache { get; set; } = false;
}
