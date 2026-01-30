using System.Text.RegularExpressions;
using Ddap.Auth.Policies;

namespace Ddap.Grpc;

/// <summary>
/// Analyzes SQL queries to detect query types and potential security issues.
/// </summary>
public static class QueryAnalyzer
{
    private static readonly Regex SelectPattern = new(
        @"^\s*SELECT\s+",
        RegexOptions.IgnoreCase | RegexOptions.Compiled
    );
    private static readonly Regex InsertPattern = new(
        @"^\s*INSERT\s+",
        RegexOptions.IgnoreCase | RegexOptions.Compiled
    );
    private static readonly Regex UpdatePattern = new(
        @"^\s*UPDATE\s+",
        RegexOptions.IgnoreCase | RegexOptions.Compiled
    );
    private static readonly Regex DeletePattern = new(
        @"^\s*DELETE\s+",
        RegexOptions.IgnoreCase | RegexOptions.Compiled
    );
    private static readonly Regex CreatePattern = new(
        @"^\s*CREATE\s+",
        RegexOptions.IgnoreCase | RegexOptions.Compiled
    );
    private static readonly Regex DropPattern = new(
        @"^\s*DROP\s+",
        RegexOptions.IgnoreCase | RegexOptions.Compiled
    );
    private static readonly Regex AlterPattern = new(
        @"^\s*ALTER\s+",
        RegexOptions.IgnoreCase | RegexOptions.Compiled
    );
    private static readonly Regex TruncatePattern = new(
        @"^\s*TRUNCATE\s+",
        RegexOptions.IgnoreCase | RegexOptions.Compiled
    );
    private static readonly Regex MergePattern = new(
        @"^\s*MERGE\s+",
        RegexOptions.IgnoreCase | RegexOptions.Compiled
    );
    private static readonly Regex ExecutePattern = new(
        @"^\s*(EXEC|EXECUTE)\s+",
        RegexOptions.IgnoreCase | RegexOptions.Compiled
    );

    // SQL injection detection patterns (static for performance)
    private static readonly Regex OrAndInjectionPattern = new(
        @"\b(OR|AND)\b\s*\d+\s*=\s*\d+",
        RegexOptions.IgnoreCase | RegexOptions.Compiled
    );
    private static readonly Regex UnionSelectPattern = new(
        @"UNION\s+SELECT",
        RegexOptions.IgnoreCase | RegexOptions.Compiled
    );
    private static readonly Regex DropTablePattern = new(
        @"DROP\s+TABLE",
        RegexOptions.IgnoreCase | RegexOptions.Compiled
    );
    private static readonly Regex StatementTerminationPattern = new(
        @";.*--",
        RegexOptions.Compiled
    );
    private static readonly Regex ExtendedProcedurePattern = new(
        @"xp_\w+",
        RegexOptions.IgnoreCase | RegexOptions.Compiled
    );

    /// <summary>
    /// Determines the type of SQL query.
    /// </summary>
    /// <param name="query">The SQL query to analyze.</param>
    /// <returns>The detected query type.</returns>
    public static QueryType DetermineQueryType(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
            return QueryType.Unknown;

        if (SelectPattern.IsMatch(query))
            return QueryType.Select;
        if (InsertPattern.IsMatch(query))
            return QueryType.Insert;
        if (UpdatePattern.IsMatch(query))
            return QueryType.Update;
        if (DeletePattern.IsMatch(query))
            return QueryType.Delete;
        if (CreatePattern.IsMatch(query))
            return QueryType.Create;
        if (DropPattern.IsMatch(query))
            return QueryType.Drop;
        if (AlterPattern.IsMatch(query))
            return QueryType.Alter;
        if (TruncatePattern.IsMatch(query))
            return QueryType.Truncate;
        if (MergePattern.IsMatch(query))
            return QueryType.Merge;
        if (ExecutePattern.IsMatch(query))
            return QueryType.Execute;

        return QueryType.Unknown;
    }

    /// <summary>
    /// Checks if a query contains potential SQL injection patterns.
    /// Note: This is a basic check and should not be relied upon as the sole security measure.
    /// Always use parameterized queries.
    /// </summary>
    /// <param name="query">The SQL query to check.</param>
    /// <returns>True if potential injection patterns are detected; otherwise, false.</returns>
    public static bool HasPotentialInjection(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
            return false;

        // Check for suspicious patterns that indicate SQL injection attempts
        if (OrAndInjectionPattern.IsMatch(query))
            return true;

        if (UnionSelectPattern.IsMatch(query))
            return true;

        if (DropTablePattern.IsMatch(query))
            return true;

        if (StatementTerminationPattern.IsMatch(query))
            return true;

        if (ExtendedProcedurePattern.IsMatch(query))
            return true;

        return false;
    }

    /// <summary>
    /// Extracts the table name from a simple SQL query.
    /// This is a best-effort extraction and may not work for complex queries.
    /// </summary>
    /// <param name="query">The SQL query.</param>
    /// <returns>The extracted table name, or null if not found.</returns>
    public static string? ExtractTableName(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
            return null;

        var queryType = DetermineQueryType(query);

        return queryType switch
        {
            QueryType.Select => ExtractTableFromSelect(query),
            QueryType.Insert => ExtractTableFromInsert(query),
            QueryType.Update => ExtractTableFromUpdate(query),
            QueryType.Delete => ExtractTableFromDelete(query),
            _ => null,
        };
    }

    private static string? ExtractTableFromSelect(string query)
    {
        var fromMatch = Regex.Match(
            query,
            @"\bFROM\s+([\w\.\[\]]+)(?:\s+WHERE|\s+JOIN|\s+ORDER|\s+GROUP|\s+LIMIT|;|$)",
            RegexOptions.IgnoreCase
        );
        return fromMatch.Success ? fromMatch.Groups[1].Value.Trim().Trim('[', ']').Trim() : null;
    }

    private static string? ExtractTableFromInsert(string query)
    {
        var intoMatch = Regex.Match(
            query,
            @"\bINSERT\s+INTO\s+(\[?\w+\]?\.?\[?\w+\]?)",
            RegexOptions.IgnoreCase
        );
        return intoMatch.Success ? intoMatch.Groups[1].Value.Trim('[', ']') : null;
    }

    private static string? ExtractTableFromUpdate(string query)
    {
        var updateMatch = Regex.Match(
            query,
            @"\bUPDATE\s+(\[?\w+\]?\.?\[?\w+\]?)",
            RegexOptions.IgnoreCase
        );
        return updateMatch.Success ? updateMatch.Groups[1].Value.Trim('[', ']') : null;
    }

    private static string? ExtractTableFromDelete(string query)
    {
        var fromMatch = Regex.Match(
            query,
            @"\bFROM\s+(\[?\w+\]?\.?\[?\w+\]?)",
            RegexOptions.IgnoreCase
        );
        return fromMatch.Success ? fromMatch.Groups[1].Value.Trim('[', ']') : null;
    }
}
