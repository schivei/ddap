namespace Ddap.Core;

/// <summary>
/// Represents options for configuring DDAP services.
/// </summary>
public class DdapOptions
{
    /// <summary>
    /// Gets or sets the connection string for the database.
    /// </summary>
    public string ConnectionString { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a factory function for creating connection strings dynamically.
    /// This takes precedence over ConnectionString if provided.
    /// </summary>
    public Func<IServiceProvider, string>? ConnectionStringFactory { get; set; }

    /// <summary>
    /// Gets or sets the database provider type.
    /// </summary>
    public DatabaseProvider Provider { get; set; } = DatabaseProvider.SQLServer;

    /// <summary>
    /// Gets or sets a value indicating whether to load entities on startup.
    /// </summary>
    public bool LoadOnStartup { get; set; } = true;

    /// <summary>
    /// Gets or sets the schemas to include when loading entities.
    /// If null or empty, all schemas will be included.
    /// </summary>
    public List<string>? IncludeSchemas { get; set; }

    /// <summary>
    /// Gets or sets the schemas to exclude when loading entities.
    /// </summary>
    public List<string>? ExcludeSchemas { get; set; }

    /// <summary>
    /// Gets or sets the tables to include when loading entities.
    /// If null or empty, all tables will be included.
    /// </summary>
    public List<string>? IncludeTables { get; set; }

    /// <summary>
    /// Gets or sets the tables to exclude when loading entities.
    /// </summary>
    public List<string>? ExcludeTables { get; set; }

    /// <summary>
    /// Gets or sets a filter function for tables.
    /// Return true to include the table, false to exclude it.
    /// </summary>
    public Func<string, bool>? TableFilter { get; set; }

    /// <summary>
    /// Gets or sets a filter function for entities.
    /// Return true to include the entity type, false to exclude it.
    /// </summary>
    public Func<Type, bool>? EntityFilter { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to enable code generation.
    /// </summary>
    public bool EnableCodeGeneration { get; set; } = true;

    /// <summary>
    /// Gets or sets auto-reload options for schema changes.
    /// </summary>
    public AutoReloadOptions? AutoReload { get; set; }

    /// <summary>
    /// Gets or sets caching options.
    /// </summary>
    public CachingOptions Caching { get; set; } = new();

    /// <summary>
    /// Gets or sets observability options.
    /// </summary>
    public ObservabilityOptions Observability { get; set; } = new();

    /// <summary>
    /// Gets or sets an async callback invoked on application startup.
    /// </summary>
    public Func<IServiceProvider, Task>? OnStartupAsync { get; set; }

    /// <summary>
    /// Gets or sets an async callback invoked on application shutdown.
    /// </summary>
    public Func<IServiceProvider, Task>? OnShutdownAsync { get; set; }

    /// <summary>
    /// Gets or sets an async callback invoked when an error occurs.
    /// </summary>
    public Func<IServiceProvider, Exception, Task>? OnErrorAsync { get; set; }
}

/// <summary>
/// Defines supported database providers.
/// </summary>
public enum DatabaseProvider
{
    /// <summary>
    /// Microsoft SQL Server.
    /// </summary>
    SQLServer,

    /// <summary>
    /// MySQL database.
    /// </summary>
    MySQL,

    /// <summary>
    /// PostgreSQL database.
    /// </summary>
    PostgreSQL,
}
