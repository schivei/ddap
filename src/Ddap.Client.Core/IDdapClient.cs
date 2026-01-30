namespace Ddap.Client.Core;

/// <summary>
/// Base interface for all DDAP API clients
/// </summary>
public interface IDdapClient
{
    /// <summary>
    /// Gets the base URL of the API
    /// </summary>
    string BaseUrl { get; }

    /// <summary>
    /// Gets a value indicating whether the client is connected
    /// </summary>
    bool IsConnected { get; }

    /// <summary>
    /// Tests the connection to the API
    /// </summary>
    Task<bool> TestConnectionAsync(CancellationToken cancellationToken = default);
}
