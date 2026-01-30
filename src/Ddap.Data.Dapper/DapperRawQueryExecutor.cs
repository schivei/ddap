using System.Data;
using Dapper;
using Ddap.Core;

namespace Ddap.Data.Dapper;

/// <summary>
/// Dapper-based implementation of raw query executor.
/// Executes SQL queries directly using Dapper with binary-efficient operations.
/// </summary>
/// <example>
/// <code>
/// services.AddSingleton&lt;IRawQueryExecutor, DapperRawQueryExecutor&gt;();
/// </code>
/// </example>
public class DapperRawQueryExecutor : IRawQueryExecutor
{
    private readonly DapperProviderOptions _options;

    /// <summary>
    /// Initializes a new instance of the <see cref="DapperRawQueryExecutor"/> class.
    /// </summary>
    /// <param name="options">The Dapper provider options containing the connection factory.</param>
    public DapperRawQueryExecutor(DapperProviderOptions options)
    {
        _options = options ?? throw new ArgumentNullException(nameof(options));
    }

    /// <inheritdoc />
    public async Task<T?> ExecuteScalarAsync<T>(
        string query,
        object? parameters = null,
        CancellationToken cancellationToken = default
    )
    {
        using var connection = _options.ConnectionFactory();
        await OpenConnectionAsync(connection, cancellationToken);
        return await connection.ExecuteScalarAsync<T>(query, parameters);
    }

    /// <inheritdoc />
    public async Task<dynamic?> ExecuteSingleAsync(
        string query,
        object? parameters = null,
        CancellationToken cancellationToken = default
    )
    {
        using var connection = _options.ConnectionFactory();
        await OpenConnectionAsync(connection, cancellationToken);
        return await connection.QueryFirstOrDefaultAsync(query, parameters);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<dynamic>> ExecuteMultipleAsync(
        string query,
        object? parameters = null,
        CancellationToken cancellationToken = default
    )
    {
        using var connection = _options.ConnectionFactory();
        await OpenConnectionAsync(connection, cancellationToken);
        return await connection.QueryAsync(query, parameters);
    }

    /// <inheritdoc />
    public async Task<int> ExecuteNonQueryAsync(
        string query,
        object? parameters = null,
        CancellationToken cancellationToken = default
    )
    {
        using var connection = _options.ConnectionFactory();
        await OpenConnectionAsync(connection, cancellationToken);
        return await connection.ExecuteAsync(query, parameters);
    }

    private static async Task OpenConnectionAsync(
        IDbConnection connection,
        CancellationToken cancellationToken
    )
    {
        if (connection.State != ConnectionState.Open)
        {
            if (connection is System.Data.Common.DbConnection dbConnection)
            {
                await dbConnection.OpenAsync(cancellationToken);
            }
            else
            {
                connection.Open();
            }
        }
    }
}
