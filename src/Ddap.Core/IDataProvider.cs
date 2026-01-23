namespace Ddap.Core;

/// <summary>
/// Represents a data provider that loads entity configurations from a database.
/// </summary>
public interface IDataProvider
{
    /// <summary>
    /// Loads entity configurations from the database.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation, containing the loaded entity configurations.</returns>
    Task<IReadOnlyList<IEntityConfiguration>> LoadEntitiesAsync(
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Gets the database provider name (e.g., "MySQL", "PostgreSQL", "SQLServer").
    /// </summary>
    string ProviderName { get; }
}
