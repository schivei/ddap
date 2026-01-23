using System.Collections.Concurrent;

namespace Ddap.Core;

/// <summary>
/// Default implementation of <see cref="IEntityRepository"/>.
/// Stores entity configurations in memory.
/// </summary>
internal class EntityRepository : IEntityRepository
{
    private readonly ConcurrentDictionary<string, IEntityConfiguration> _entities = new();

    /// <inheritdoc/>
    public IReadOnlyList<IEntityConfiguration> GetAllEntities()
    {
        return _entities.Values.ToList();
    }

    /// <inheritdoc/>
    public IEntityConfiguration? GetEntity(string entityName)
    {
        _entities.TryGetValue(entityName, out var entity);
        return entity;
    }

    /// <inheritdoc/>
    public bool EntityExists(string entityName)
    {
        return _entities.ContainsKey(entityName);
    }

    /// <summary>
    /// Adds or updates an entity configuration in the repository.
    /// </summary>
    /// <param name="entity">The entity configuration to add or update.</param>
    internal void AddOrUpdateEntity(IEntityConfiguration entity)
    {
        _entities.AddOrUpdate(entity.EntityName, entity, (_, _) => entity);
    }

    /// <summary>
    /// Clears all entity configurations from the repository.
    /// </summary>
    internal void Clear()
    {
        _entities.Clear();
    }
}
