namespace Ddap.Core;

/// <summary>
/// Represents a repository for managing in-memory entity metadata.
/// This interface provides access to entity configurations loaded from the database.
/// </summary>
public interface IEntityRepository
{
    /// <summary>
    /// Gets all entity configurations available in the repository.
    /// </summary>
    /// <returns>A read-only collection of entity configurations.</returns>
    IReadOnlyList<IEntityConfiguration> GetAllEntities();

    /// <summary>
    /// Gets an entity configuration by its name.
    /// </summary>
    /// <param name="entityName">The name of the entity to retrieve.</param>
    /// <returns>The entity configuration if found; otherwise, null.</returns>
    IEntityConfiguration? GetEntity(string entityName);

    /// <summary>
    /// Checks if an entity exists in the repository.
    /// </summary>
    /// <param name="entityName">The name of the entity to check.</param>
    /// <returns>True if the entity exists; otherwise, false.</returns>
    bool EntityExists(string entityName);

    /// <summary>
    /// Adds or updates an entity configuration in the repository.
    /// </summary>
    /// <param name="entity">The entity configuration to add or update.</param>
    void AddOrUpdateEntity(IEntityConfiguration entity);
}
