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
    /// Gets or sets a value indicating whether to enable code generation.
    /// </summary>
    public bool EnableCodeGeneration { get; set; } = true;
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
    PostgreSQL
}
