using Ddap.Core;

namespace Ddap.Grpc;

/// <summary>
/// Base gRPC service for dynamically generated entity services.
/// This class is partial to allow for extensibility.
/// </summary>
/// <example>
/// To extend this service, create a partial class:
/// <code>
/// public partial class EntityService
/// {
///     // Add custom RPC methods here
/// }
/// </code>
/// </example>
public partial class EntityService
{
    private readonly IEntityRepository _entityRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="EntityService"/> class.
    /// </summary>
    /// <param name="entityRepository">The entity repository.</param>
    public EntityService(IEntityRepository entityRepository)
    {
        _entityRepository = entityRepository;
    }

    /// <summary>
    /// Gets all available entities metadata via gRPC.
    /// </summary>
    /// <returns>A response containing entity metadata.</returns>
    public virtual EntityListResponse GetEntities()
    {
        var entities = _entityRepository.GetAllEntities();
        var response = new EntityListResponse
        {
            Entities = entities.Select(e => e.EntityName).ToList()
        };

        return response;
    }
}

/// <summary>
/// Represents a response containing a list of entities.
/// </summary>
public class EntityListResponse
{
    /// <summary>
    /// Gets or sets the list of entity names.
    /// </summary>
    public List<string> Entities { get; set; } = new();
}
