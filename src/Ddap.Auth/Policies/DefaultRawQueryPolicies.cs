namespace Ddap.Auth.Policies;

/// <summary>
/// Default policy that only allows SELECT queries.
/// This is a safe default that prevents data modification.
/// </summary>
/// <example>
/// <code>
/// services.AddSingleton&lt;IRawQueryPolicy, DefaultRawQueryPolicy&gt;();
/// </code>
/// </example>
public class DefaultRawQueryPolicy : IRawQueryPolicy
{
    /// <summary>
    /// Determines whether the query can be executed.
    /// Only allows SELECT queries by default.
    /// </summary>
    /// <param name="context">The query context.</param>
    /// <returns>True if the query is a SELECT; otherwise, false.</returns>
    public Task<bool> CanExecuteQueryAsync(RawQueryContext context)
    {
        // By default, only allow SELECT queries
        return Task.FromResult(context.QueryType == QueryType.Select);
    }
}

/// <summary>
/// Policy that allows all query types.
/// WARNING: Use with caution - this allows data modification and schema changes.
/// </summary>
/// <example>
/// <code>
/// services.AddSingleton&lt;IRawQueryPolicy, AllowAllRawQueryPolicy&gt;();
/// </code>
/// </example>
public class AllowAllRawQueryPolicy : IRawQueryPolicy
{
    /// <summary>
    /// Allows all queries without restriction.
    /// </summary>
    /// <param name="context">The query context.</param>
    /// <returns>Always returns true.</returns>
    public Task<bool> CanExecuteQueryAsync(RawQueryContext context)
    {
        return Task.FromResult(true);
    }
}

/// <summary>
/// Policy that denies all raw queries.
/// Use this to completely disable raw query execution.
/// </summary>
/// <example>
/// <code>
/// services.AddSingleton&lt;IRawQueryPolicy, DenyAllRawQueryPolicy&gt;();
/// </code>
/// </example>
public class DenyAllRawQueryPolicy : IRawQueryPolicy
{
    /// <summary>
    /// Denies all queries.
    /// </summary>
    /// <param name="context">The query context.</param>
    /// <returns>Always returns false.</returns>
    public Task<bool> CanExecuteQueryAsync(RawQueryContext context)
    {
        return Task.FromResult(false);
    }
}
