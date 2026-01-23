using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Linq;
using System.Text;

namespace Ddap.CodeGen.Generators;

/// <summary>
/// Source generator that creates partial controller and service classes for DDAP entities.
/// Generates code based on entity configurations and ddap.config settings.
/// </summary>
/// <example>
/// <code>
/// // Generated code example for User entity:
/// [GeneratedCode("Ddap.CodeGen", "1.0.0")]
/// public partial class UserController : EntityController
/// {
///     // Generated CRUD methods
/// }
/// 
/// [GeneratedCode("Ddap.CodeGen", "1.0.0")]
/// public partial class UserService : EntityService
/// {
///     // Generated gRPC methods
/// }
/// </code>
/// </example>
[Generator]
public class EntitySourceGenerator : IIncrementalGenerator
{
    /// <summary>
    /// Called to initialize the generator and set up the generation pipeline.
    /// </summary>
    /// <param name="context">The initialization context.</param>
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // Register for additional files (ddap.config)
        var configFiles = context.AdditionalTextsProvider
            .Where(file => file.Path.EndsWith("ddap.config"))
            .Select((file, ct) => file.GetText(ct)?.ToString() ?? string.Empty);

        // Register source generation
        context.RegisterSourceOutput(configFiles, (spc, configContent) =>
        {
            try
            {
                var config = LoadConfiguration(configContent);

                // Generate code for each entity
                foreach (var entityConfig in config.Entities)
                {
                    if (config.GenerateControllers && entityConfig.GenerateController)
                    {
                        GenerateController(spc, config.Namespace, entityConfig.Name);
                    }

                    if (config.GenerateGrpcServices && entityConfig.GenerateGrpcService)
                    {
                        GenerateGrpcService(spc, config.Namespace, entityConfig.Name);
                    }

                    if (config.GenerateGraphQLTypes && entityConfig.GenerateGraphQLType)
                    {
                        GenerateGraphQLType(spc, config.Namespace, entityConfig.Name);
                    }
                }
            }
            catch
            {
                // Silently fail if configuration not found or invalid
            }
        });
    }

    private Config.DdapGeneratorConfig LoadConfiguration(string configContent)
    {
        var config = new Config.DdapGeneratorConfig();

        // In a real implementation, parse JSON configuration
        // For now, return default config

        return config;
    }

    private void GenerateController(SourceProductionContext context, string namespaceName, string entityName)
    {
        var source = new StringBuilder();

        source.AppendLine("using System.CodeDom.Compiler;");
        source.AppendLine("using Microsoft.AspNetCore.Mvc;");
        source.AppendLine("using Ddap.Rest;");
        source.AppendLine("using Ddap.Rest.Models;");
        source.AppendLine("using System.Threading.Tasks;");
        source.AppendLine();
        source.AppendLine($"namespace {namespaceName}");
        source.AppendLine("{");
        source.AppendLine("    /// <summary>");
        source.AppendLine($"    /// Generated REST controller for {entityName} entity.");
        source.AppendLine("    /// </summary>");
        source.AppendLine($"    [GeneratedCode(\"Ddap.CodeGen\", \"1.0.0\")]");
        source.AppendLine($"    [ApiController]");
        source.AppendLine($"    [Route(\"api/[controller]\")]");
        source.AppendLine($"    public partial class {entityName}Controller : EntityController");
        source.AppendLine("    {");
        source.AppendLine("        /// <summary>");
        source.AppendLine($"        /// Gets all {entityName} entities with pagination.");
        source.AppendLine("        /// </summary>");
        source.AppendLine("        /// <param name=\"parameters\">Query parameters.</param>");
        source.AppendLine($"        /// <returns>Paginated list of {entityName} entities.</returns>");
        source.AppendLine("        [HttpGet(\"all\")]");
        source.AppendLine($"        public virtual async Task<IActionResult> GetAll{entityName}([FromQuery] QueryParameters parameters)");
        source.AppendLine("        {");
        source.AppendLine("            // Implementation would be provided by runtime or custom partial class");
        source.AppendLine("            return Ok();");
        source.AppendLine("        }");
        source.AppendLine("    }");
        source.AppendLine("}");

        context.AddSource($"{entityName}Controller.g.cs", SourceText.From(source.ToString(), Encoding.UTF8));
    }

    private void GenerateGrpcService(SourceProductionContext context, string namespaceName, string entityName)
    {
        var source = new StringBuilder();

        source.AppendLine("using System.CodeDom.Compiler;");
        source.AppendLine("using Grpc.Core;");
        source.AppendLine("using Ddap.Grpc;");
        source.AppendLine("using System.Threading.Tasks;");
        source.AppendLine();
        source.AppendLine($"namespace {namespaceName}");
        source.AppendLine("{");
        source.AppendLine("    /// <summary>");
        source.AppendLine($"    /// Generated gRPC service for {entityName} entity.");
        source.AppendLine("    /// </summary>");
        source.AppendLine($"    [GeneratedCode(\"Ddap.CodeGen\", \"1.0.0\")]");
        source.AppendLine($"    public partial class {entityName}Service : EntityService");
        source.AppendLine("    {");
        source.AppendLine("        /// <summary>");
        source.AppendLine($"        /// Gets a {entityName} by ID.");
        source.AppendLine("        /// </summary>");
        source.AppendLine("        /// <param name=\"request\">The request.</param>");
        source.AppendLine("        /// <param name=\"context\">The server call context.</param>");
        source.AppendLine($"        /// <returns>The {entityName} entity.</returns>");
        source.AppendLine($"        public virtual Task<{entityName}Response> Get{entityName}(Get{entityName}Request request, ServerCallContext context)");
        source.AppendLine("        {");
        source.AppendLine("            // Implementation would be provided by runtime or custom partial class");
        source.AppendLine($"            return Task.FromResult(new {entityName}Response());");
        source.AppendLine("        }");
        source.AppendLine("    }");
        source.AppendLine("}");

        context.AddSource($"{entityName}Service.g.cs", SourceText.From(source.ToString(), Encoding.UTF8));
    }

    private void GenerateGraphQLType(SourceProductionContext context, string namespaceName, string entityName)
    {
        var source = new StringBuilder();

        source.AppendLine("using System.CodeDom.Compiler;");
        source.AppendLine("using HotChocolate.Types;");
        source.AppendLine();
        source.AppendLine($"namespace {namespaceName}");
        source.AppendLine("{");
        source.AppendLine("    /// <summary>");
        source.AppendLine($"    /// Generated GraphQL type for {entityName} entity.");
        source.AppendLine("    /// </summary>");
        source.AppendLine($"    [GeneratedCode(\"Ddap.CodeGen\", \"1.0.0\")]");
        source.AppendLine($"    public partial class {entityName}Type : ObjectType");
        source.AppendLine("    {");
        source.AppendLine("        /// <summary>");
        source.AppendLine("        /// Configures the GraphQL type.");
        source.AppendLine("        /// </summary>");
        source.AppendLine("        /// <param name=\"descriptor\">The type descriptor.</param>");
        source.AppendLine("        protected override void Configure(IObjectTypeDescriptor descriptor)");
        source.AppendLine("        {");
        source.AppendLine($"            descriptor.Name(\"{entityName}\");");
        source.AppendLine($"            descriptor.Description(\"GraphQL type for {entityName} entity\");");
        source.AppendLine("            base.Configure(descriptor);");
        source.AppendLine("        }");
        source.AppendLine("    }");
        source.AppendLine("}");

        context.AddSource($"{entityName}Type.g.cs", SourceText.From(source.ToString(), Encoding.UTF8));
    }
}
