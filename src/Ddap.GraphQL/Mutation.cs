namespace Ddap.GraphQL;

/// <summary>
/// Represents the root GraphQL mutation type.
/// This class is partial to allow for extensibility.
/// </summary>
/// <example>
/// To extend this mutation, create a partial class:
/// <code>
/// public partial class Mutation
/// {
///     // Add custom mutation fields here
/// }
/// </code>
/// </example>
public partial class Mutation
{
    /// <summary>
    /// Placeholder mutation to satisfy GraphQL schema requirements.
    /// </summary>
    /// <returns>A success message.</returns>
    public string Ping()
    {
        return "Pong";
    }
}
