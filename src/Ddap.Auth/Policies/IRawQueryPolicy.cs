namespace Ddap.Auth.Policies;

/// <summary>
/// Defines the contract for policies that control raw SQL query execution.
/// Implement this interface to restrict query types, databases, and tables.
/// </summary>
/// <example>
/// <code>
/// public class MyRawQueryPolicy : IRawQueryPolicy
/// {
///     public Task&lt;bool&gt; CanExecuteQueryAsync(RawQueryContext context)
///     {
///         // Only allow SELECT queries on specific tables
///         return Task.FromResult(
///             context.QueryType == QueryType.Select &amp;&amp;
///             context.TableName?.StartsWith("vw_") == true
///         );
///     }
/// }
/// </code>
/// </example>
public interface IRawQueryPolicy
{
    /// <summary>
    /// Determines whether the specified query can be executed based on policy rules.
    /// </summary>
    /// <param name="context">The context containing query information.</param>
    /// <returns>True if the query is allowed; otherwise, false.</returns>
    Task<bool> CanExecuteQueryAsync(RawQueryContext context);
}

/// <summary>
/// Provides context information for raw query policy evaluation.
/// </summary>
public class RawQueryContext
{
    /// <summary>
    /// Gets or sets the SQL query to be executed.
    /// </summary>
    public required string Query { get; init; }

    /// <summary>
    /// Gets or sets the type of query operation.
    /// </summary>
    public QueryType QueryType { get; init; }

    /// <summary>
    /// Gets or sets the database name, if applicable.
    /// </summary>
    public string? DatabaseName { get; init; }

    /// <summary>
    /// Gets or sets the table name, if applicable.
    /// </summary>
    public string? TableName { get; init; }

    /// <summary>
    /// Gets or sets the user identifier making the request.
    /// </summary>
    public string? UserId { get; init; }

    /// <summary>
    /// Gets or sets the user roles.
    /// </summary>
    public IEnumerable<string>? UserRoles { get; init; }
}

/// <summary>
/// Defines the types of SQL queries that can be executed.
/// </summary>
public enum QueryType
{
    /// <summary>
    /// Unknown or unparsable query type.
    /// </summary>
    Unknown = 0,

    /// <summary>
    /// SELECT query - retrieves data.
    /// </summary>
    Select = 1,

    /// <summary>
    /// INSERT query - adds new data.
    /// </summary>
    Insert = 2,

    /// <summary>
    /// UPDATE query - modifies existing data.
    /// </summary>
    Update = 3,

    /// <summary>
    /// DELETE query - removes data.
    /// </summary>
    Delete = 4,

    /// <summary>
    /// CREATE query - creates database objects.
    /// </summary>
    Create = 5,

    /// <summary>
    /// DROP query - removes database objects.
    /// </summary>
    Drop = 6,

    /// <summary>
    /// ALTER query - modifies database objects.
    /// </summary>
    Alter = 7,

    /// <summary>
    /// TRUNCATE query - removes all data from a table.
    /// </summary>
    Truncate = 8,

    /// <summary>
    /// MERGE query - combines insert/update operations.
    /// </summary>
    Merge = 9,

    /// <summary>
    /// Stored procedure execution.
    /// </summary>
    Execute = 10,
}
