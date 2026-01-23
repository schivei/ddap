using Ddap.Core;

namespace Ddap.GraphQL;

/// <summary>
/// Represents the root GraphQL query type.
/// This class is partial to allow for extensibility.
/// </summary>
/// <example>
/// To extend this query, create a partial class:
/// <code>
/// public partial class Query
/// {
///     // Add custom query fields here
/// }
/// </code>
/// </example>
public partial class Query
{
    /// <summary>
    /// Gets all available entities.
    /// </summary>
    /// <param name="entityRepository">The entity repository.</param>
    /// <returns>A list of entity metadata.</returns>
    public IEnumerable<EntityMetadata> GetEntities([Service] IEntityRepository entityRepository)
    {
        var entities = entityRepository.GetAllEntities();
        return entities.Select(
            e =>
                new EntityMetadata
                {
                    Name = e.EntityName,
                    Schema = e.SchemaName,
                    PropertyCount = e.Properties.Count
                }
        );
    }

    /// <summary>
    /// Gets metadata for a specific entity.
    /// </summary>
    /// <param name="entityName">The name of the entity.</param>
    /// <param name="entityRepository">The entity repository.</param>
    /// <returns>The entity metadata or null if not found.</returns>
    public EntityMetadata? GetEntity(
        string entityName,
        [Service] IEntityRepository entityRepository
    )
    {
        var entity = entityRepository.GetEntity(entityName);
        if (entity == null)
            return null;

        return new EntityMetadata
        {
            Name = entity.EntityName,
            Schema = entity.SchemaName,
            PropertyCount = entity.Properties.Count
        };
    }
}

/// <summary>
/// Represents entity metadata for GraphQL responses.
/// </summary>
public class EntityMetadata
{
    /// <summary>
    /// Gets or sets the entity name.
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Gets or sets the schema name.
    /// </summary>
    public string? Schema { get; set; }

    /// <summary>
    /// Gets or sets the count of properties.
    /// </summary>
    public int PropertyCount { get; set; }
}
