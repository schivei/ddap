using System.Collections.Generic;

namespace Ddap.CodeGen.Config;

/// <summary>
/// Configuration for DDAP source generator.
/// Can be loaded from ddap.config or other configuration sources.
/// </summary>
/// <example>
/// <code>
/// // ddap.config example (JSON):
/// {
///   "entities": [
///     {
///       "name": "User",
///       "generateController": true,
///       "generateService": true,
///       "generateGrpcService": true
///     }
///   ],
///   "namespace": "MyApp.Generated",
///   "outputPath": "Generated"
/// }
/// </code>
/// </example>
public class DdapGeneratorConfig
{
    /// <summary>
    /// Gets or sets the root namespace for generated code.
    /// </summary>
    public string Namespace { get; set; } = "Ddap.Generated";

    /// <summary>
    /// Gets or sets the output path for generated files.
    /// </summary>
    public string OutputPath { get; set; } = "Generated";

    /// <summary>
    /// Gets or sets the list of entity configurations.
    /// </summary>
    public List<EntityGeneratorConfig> Entities { get; set; } = new List<EntityGeneratorConfig>();

    /// <summary>
    /// Gets or sets a value indicating whether to generate REST controllers.
    /// </summary>
    public bool GenerateControllers { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether to generate gRPC services.
    /// </summary>
    public bool GenerateGrpcServices { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether to generate GraphQL types.
    /// </summary>
    public bool GenerateGraphQLTypes { get; set; } = true;
}

/// <summary>
/// Configuration for a specific entity's code generation.
/// </summary>
public class EntityGeneratorConfig
{
    /// <summary>
    /// Gets or sets the entity name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a value indicating whether to generate a REST controller for this entity.
    /// </summary>
    public bool GenerateController { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether to generate a gRPC service for this entity.
    /// </summary>
    public bool GenerateGrpcService { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether to generate GraphQL types for this entity.
    /// </summary>
    public bool GenerateGraphQLType { get; set; } = true;

    /// <summary>
    /// Gets or sets custom properties or metadata for this entity.
    /// </summary>
    public Dictionary<string, string> CustomProperties { get; set; } = new Dictionary<string, string>();
}
