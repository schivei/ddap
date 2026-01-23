using System.Text;
using Ddap.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Ddap.Grpc.Controllers;

/// <summary>
/// Controller for downloading dynamically generated .proto files.
/// This class is partial to allow for extensibility.
/// </summary>
/// <example>
/// <code>
/// // Download .proto for specific entity:
/// // GET /proto/User
///
/// // Download .proto for all entities:
/// // GET /proto
/// </code>
/// </example>
[ApiController]
[Route("[controller]")]
public partial class ProtoFileController : ControllerBase
{
    private readonly IEntityRepository _entityRepository;
    private readonly ProtoGenerator _protoGenerator;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProtoFileController"/> class.
    /// </summary>
    /// <param name="entityRepository">The entity repository.</param>
    public ProtoFileController(IEntityRepository entityRepository)
    {
        _entityRepository = entityRepository;
        _protoGenerator = new ProtoGenerator();
    }

    /// <summary>
    /// Downloads the .proto file for all entities.
    /// </summary>
    /// <returns>The .proto file content.</returns>
    /// <response code="200">Returns the .proto file</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Produces("text/plain")]
    public IActionResult GetAllProtos()
    {
        var entities = _entityRepository.GetAllEntities();
        var protoContent = _protoGenerator.GenerateAllProtoFiles(entities);

        return File(Encoding.UTF8.GetBytes(protoContent), "text/plain", "ddap.proto");
    }

    /// <summary>
    /// Downloads the .proto file for a specific entity.
    /// </summary>
    /// <param name="entityName">The name of the entity.</param>
    /// <returns>The .proto file content.</returns>
    /// <response code="200">Returns the .proto file</response>
    /// <response code="404">If the entity is not found</response>
    [HttpGet("{entityName}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Produces("text/plain")]
    public IActionResult GetProto(string entityName)
    {
        var entity = _entityRepository.GetEntity(entityName);

        if (entity == null)
        {
            return NotFound(new { message = $"Entity '{entityName}' not found" });
        }

        var protoContent = _protoGenerator.GenerateProtoFile(entity);

        return File(Encoding.UTF8.GetBytes(protoContent), "text/plain", $"{entityName}.proto");
    }
}
