using System.Data;
using Ddap.Core;
using Microsoft.EntityFrameworkCore;

namespace Ddap.Data.EntityFramework;

/// <summary>
/// Entity Framework-based implementation of raw query executor.
/// Executes SQL queries directly using Entity Framework with binary-efficient operations.
/// </summary>
/// <typeparam name="TContext">The DbContext type.</typeparam>
/// <example>
/// <code>
/// services.AddSingleton&lt;IRawQueryExecutor, EntityFrameworkRawQueryExecutor&lt;MyDbContext&gt;&gt;();
/// </code>
/// </example>
public class EntityFrameworkRawQueryExecutor<TContext> : IRawQueryExecutor
    where TContext : DbContext
{
    private readonly IDbContextFactory<TContext> _contextFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="EntityFrameworkRawQueryExecutor{TContext}"/> class.
    /// </summary>
    /// <param name="contextFactory">The DbContext factory.</param>
    public EntityFrameworkRawQueryExecutor(IDbContextFactory<TContext> contextFactory)
    {
        _contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
    }

    /// <inheritdoc />
    public async Task<T?> ExecuteScalarAsync<T>(
        string query,
        object? parameters = null,
        CancellationToken cancellationToken = default
    )
    {
        await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);
        using var connection = context.Database.GetDbConnection();
        await connection.OpenAsync(cancellationToken);

        using var command = connection.CreateCommand();
        command.CommandText = query;
        AddParameters(command, parameters);

        var result = await command.ExecuteScalarAsync(cancellationToken);
        return result != null && result != DBNull.Value
            ? (T?)Convert.ChangeType(result, typeof(T))
            : default;
    }

    /// <inheritdoc />
    public async Task<dynamic?> ExecuteSingleAsync(
        string query,
        object? parameters = null,
        CancellationToken cancellationToken = default
    )
    {
        var results = await ExecuteMultipleAsync(query, parameters, cancellationToken);
        return results.FirstOrDefault();
    }

    /// <inheritdoc />
    public async Task<IEnumerable<dynamic>> ExecuteMultipleAsync(
        string query,
        object? parameters = null,
        CancellationToken cancellationToken = default
    )
    {
        await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);
        using var connection = context.Database.GetDbConnection();
        await connection.OpenAsync(cancellationToken);

        using var command = connection.CreateCommand();
        command.CommandText = query;
        AddParameters(command, parameters);

        using var reader = await command.ExecuteReaderAsync(cancellationToken);

        var results = new List<dynamic>();
        while (await reader.ReadAsync(cancellationToken))
        {
            var row = new Dictionary<string, object?>();
            for (int i = 0; i < reader.FieldCount; i++)
            {
                row[reader.GetName(i)] = reader.IsDBNull(i) ? null : reader.GetValue(i);
            }
            results.Add(row);
        }

        return results;
    }

    /// <inheritdoc />
    public async Task<int> ExecuteNonQueryAsync(
        string query,
        object? parameters = null,
        CancellationToken cancellationToken = default
    )
    {
        await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);
        using var connection = context.Database.GetDbConnection();
        await connection.OpenAsync(cancellationToken);

        using var command = connection.CreateCommand();
        command.CommandText = query;
        AddParameters(command, parameters);

        return await command.ExecuteNonQueryAsync(cancellationToken);
    }

    private static void AddParameters(IDbCommand command, object? parameters)
    {
        if (parameters == null)
            return;

        var properties = parameters.GetType().GetProperties();
        foreach (var property in properties)
        {
            var parameter = command.CreateParameter();
            parameter.ParameterName = $"@{property.Name}";
            parameter.Value = property.GetValue(parameters) ?? DBNull.Value;
            command.Parameters.Add(parameter);
        }
    }
}
