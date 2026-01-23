namespace Ddap.Rest;

/// <summary>
/// Represents a provider for REST API functionality.
/// </summary>
public interface IRestApiProvider
{
    /// <summary>
    /// Gets a value indicating whether the REST API is enabled.
    /// </summary>
    bool IsEnabled { get; }

    /// <summary>
    /// Gets the base path for REST API endpoints.
    /// </summary>
    string BasePath { get; }
}

/// <summary>
/// Default implementation of <see cref="IRestApiProvider"/>.
/// </summary>
internal class RestApiProvider : IRestApiProvider
{
    /// <inheritdoc/>
    public bool IsEnabled => true;

    /// <inheritdoc/>
    public string BasePath => "/api";
}
