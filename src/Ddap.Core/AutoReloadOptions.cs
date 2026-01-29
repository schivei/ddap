namespace Ddap.Core;

/// <summary>
/// Options for configuring automatic schema reload behavior.
/// </summary>
public class AutoReloadOptions
{
    /// <summary>
    /// Gets or sets a value indicating whether auto-reload is enabled.
    /// </summary>
    public bool Enabled { get; set; } = false;

    /// <summary>
    /// Gets or sets the idle timeout before reload is allowed.
    /// No requests should be received during this period for reload to occur.
    /// </summary>
    public TimeSpan IdleTimeout { get; set; } = TimeSpan.FromMinutes(5);

    /// <summary>
    /// Gets or sets the reload strategy to use.
    /// </summary>
    public ReloadStrategy Strategy { get; set; } = ReloadStrategy.InvalidateAndRebuild;

    /// <summary>
    /// Gets or sets the reload behavior during reload.
    /// </summary>
    public ReloadBehavior Behavior { get; set; } = ReloadBehavior.ServeOldSchema;

    /// <summary>
    /// Gets or sets the change detection method.
    /// </summary>
    public ChangeDetection ChangeDetection { get; set; } = ChangeDetection.CheckHash;

    /// <summary>
    /// Gets or sets an async callback invoked before reload starts.
    /// Return true to proceed with reload, false to cancel.
    /// </summary>
    public Func<IServiceProvider, Task<bool>>? OnBeforeReloadAsync { get; set; }

    /// <summary>
    /// Gets or sets an async callback invoked after reload completes.
    /// </summary>
    public Func<IServiceProvider, Task>? OnAfterReloadAsync { get; set; }

    /// <summary>
    /// Gets or sets an async callback invoked when reload fails.
    /// </summary>
    public Func<IServiceProvider, Exception, Task>? OnReloadErrorAsync { get; set; }
}

/// <summary>
/// Defines the strategy for reloading the schema.
/// </summary>
public enum ReloadStrategy
{
    /// <summary>
    /// Invalidate the current schema and rebuild from scratch.
    /// Most thorough but slowest option.
    /// </summary>
    InvalidateAndRebuild,

    /// <summary>
    /// Attempt to hot-reload the schema without rebuilding.
    /// Faster but may not work for all changes.
    /// </summary>
    HotReload,

    /// <summary>
    /// Restart the GraphQL request executor.
    /// Moderate speed and reliability.
    /// </summary>
    RestartExecutor,
}

/// <summary>
/// Defines behavior during schema reload.
/// </summary>
public enum ReloadBehavior
{
    /// <summary>
    /// Continue serving requests using the old schema during reload.
    /// </summary>
    ServeOldSchema,

    /// <summary>
    /// Block incoming requests until reload completes.
    /// </summary>
    BlockRequests,

    /// <summary>
    /// Queue incoming requests and process them after reload.
    /// </summary>
    QueueRequests,
}

/// <summary>
/// Defines the method for detecting schema changes.
/// </summary>
public enum ChangeDetection
{
    /// <summary>
    /// Always reload regardless of whether changes are detected.
    /// </summary>
    AlwaysReload,

    /// <summary>
    /// Check a hash of the schema metadata to detect changes.
    /// </summary>
    CheckHash,

    /// <summary>
    /// Check timestamps of database objects to detect changes.
    /// </summary>
    CheckTimestamps,
}
