using System.Data;

namespace Ddap.Data.Dapper;

/// <summary>
/// Options for configuring the Dapper data provider.
/// </summary>
public class DapperProviderOptions
{
    /// <summary>
    /// Gets or sets the connection factory that creates database connections.
    /// This allows using ANY database that provides an IDbConnection implementation.
    /// </summary>
    public required Func<IDbConnection> ConnectionFactory { get; set; }

    /// <summary>
    /// Gets or sets the provider name for display purposes.
    /// Default is "Dapper".
    /// </summary>
    public string ProviderName { get; set; } = "Dapper";

    /// <summary>
    /// Gets or sets the query for loading table information.
    /// If not provided, uses INFORMATION_SCHEMA which works for most databases.
    /// </summary>
    public string? CustomTableQuery { get; set; }

    /// <summary>
    /// Gets or sets the query for loading column information.
    /// If not provided, uses INFORMATION_SCHEMA which works for most databases.
    /// </summary>
    public string? CustomColumnQuery { get; set; }

    /// <summary>
    /// Gets or sets the query for loading index information.
    /// Database-specific - may need customization.
    /// </summary>
    public string? CustomIndexQuery { get; set; }

    /// <summary>
    /// Gets or sets the query for loading foreign key information.
    /// Database-specific - may need customization.
    /// </summary>
    public string? CustomForeignKeyQuery { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to use standard INFORMATION_SCHEMA queries.
    /// Default is true. Set to false if using custom queries for all metadata.
    /// </summary>
    public bool UseInformationSchema { get; set; } = true;
}
