namespace Ddap.Core;

/// <summary>
/// Defines the contract for executing raw SQL queries with different result types.
/// </summary>
/// <example>
/// <code>
/// var executor = serviceProvider.GetRequiredService&lt;IRawQueryExecutor&gt;();
/// var result = await executor.ExecuteScalarAsync&lt;int&gt;("SELECT COUNT(*) FROM Users");
/// </code>
/// </example>
public interface IRawQueryExecutor
{
    /// <summary>
    /// Executes a query and returns a single scalar value.
    /// </summary>
    /// <typeparam name="T">The type of the scalar result.</typeparam>
    /// <param name="query">The SQL query to execute.</param>
    /// <param name="parameters">Optional query parameters.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The scalar result.</returns>
    Task<T?> ExecuteScalarAsync<T>(
        string query,
        object? parameters = null,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Executes a query and returns a single row as a dynamic object.
    /// </summary>
    /// <param name="query">The SQL query to execute.</param>
    /// <param name="parameters">Optional query parameters.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A dynamic object representing the single row, or null if no rows.</returns>
    Task<dynamic?> ExecuteSingleAsync(
        string query,
        object? parameters = null,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Executes a query and returns multiple rows as dynamic objects.
    /// </summary>
    /// <param name="query">The SQL query to execute.</param>
    /// <param name="parameters">Optional query parameters.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>An enumerable of dynamic objects representing the rows.</returns>
    Task<IEnumerable<dynamic>> ExecuteMultipleAsync(
        string query,
        object? parameters = null,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Executes a non-query command (INSERT, UPDATE, DELETE) and returns the number of affected rows.
    /// </summary>
    /// <param name="query">The SQL query to execute.</param>
    /// <param name="parameters">Optional query parameters.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The number of rows affected.</returns>
    Task<int> ExecuteNonQueryAsync(
        string query,
        object? parameters = null,
        CancellationToken cancellationToken = default
    );
}

/// <summary>
/// Represents the result type for a raw query execution.
/// </summary>
public enum RawQueryResultType
{
    /// <summary>
    /// Query returns a single scalar value.
    /// </summary>
    Scalar = 0,

    /// <summary>
    /// Query returns a single row.
    /// </summary>
    Single = 1,

    /// <summary>
    /// Query returns multiple rows.
    /// </summary>
    Multiple = 2,

    /// <summary>
    /// Query returns no data (void/non-query).
    /// </summary>
    Void = 3,
}
