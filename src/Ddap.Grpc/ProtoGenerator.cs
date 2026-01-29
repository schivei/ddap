using System.Text;
using Ddap.Core;

namespace Ddap.Grpc;

/// <summary>
/// Generates Protocol Buffer (.proto) files dynamically from entity configurations.
/// </summary>
/// <example>
/// <code>
/// var generator = new ProtoGenerator();
/// var protoContent = generator.GenerateProtoFile(entityConfig);
///
/// // Save to file or return to client
/// File.WriteAllText("entity.proto", protoContent);
/// </code>
/// </example>
public class ProtoGenerator
{
    /// <summary>
    /// Generates a .proto file content for the specified entity.
    /// </summary>
    /// <param name="entity">The entity configuration.</param>
    /// <returns>The .proto file content as a string.</returns>
    public string GenerateProtoFile(IEntityConfiguration entity)
    {
        var sb = new StringBuilder();

        sb.AppendLine("syntax = \"proto3\";");
        sb.AppendLine();
        sb.AppendLine($"option csharp_namespace = \"Ddap.Grpc.Generated\";");
        sb.AppendLine();
        sb.AppendLine($"package {entity.SchemaName ?? "ddap"};");
        sb.AppendLine();

        // Generate message for the entity
        sb.AppendLine($"// Entity: {entity.EntityName}");
        sb.AppendLine($"message {entity.EntityName} {{");

        int fieldNumber = 1;
        foreach (var property in entity.Properties)
        {
            var protoType = MapToProtoType(property.PropertyType);
            var fieldName = ToCamelCase(property.PropertyName);
            sb.AppendLine($"  {protoType} {fieldName} = {fieldNumber};");
            fieldNumber++;
        }

        sb.AppendLine("}");
        sb.AppendLine();

        // Generate request/response messages
        sb.AppendLine($"message Get{entity.EntityName}Request {{");
        sb.AppendLine("  string id = 1;");
        sb.AppendLine("}");
        sb.AppendLine();

        sb.AppendLine($"message List{entity.EntityName}Request {{");
        sb.AppendLine("  int32 page_number = 1;");
        sb.AppendLine("  int32 page_size = 2;");
        sb.AppendLine("}");
        sb.AppendLine();

        sb.AppendLine($"message List{entity.EntityName}Response {{");
        sb.AppendLine($"  repeated {entity.EntityName} items = 1;");
        sb.AppendLine("  int32 total_count = 2;");
        sb.AppendLine("}");
        sb.AppendLine();

        sb.AppendLine($"message Create{entity.EntityName}Request {{");
        sb.AppendLine($"  {entity.EntityName} entity = 1;");
        sb.AppendLine("}");
        sb.AppendLine();

        sb.AppendLine($"message Update{entity.EntityName}Request {{");
        sb.AppendLine("  string id = 1;");
        sb.AppendLine($"  {entity.EntityName} entity = 2;");
        sb.AppendLine("}");
        sb.AppendLine();

        sb.AppendLine($"message Delete{entity.EntityName}Request {{");
        sb.AppendLine("  string id = 1;");
        sb.AppendLine("}");
        sb.AppendLine();

        sb.AppendLine("message Empty {}");
        sb.AppendLine();

        // Generate service
        sb.AppendLine($"service {entity.EntityName}Service {{");
        sb.AppendLine($"  rpc Get(Get{entity.EntityName}Request) returns ({entity.EntityName});");
        sb.AppendLine(
            $"  rpc List(List{entity.EntityName}Request) returns (List{entity.EntityName}Response);"
        );
        sb.AppendLine(
            $"  rpc Create(Create{entity.EntityName}Request) returns ({entity.EntityName});"
        );
        sb.AppendLine(
            $"  rpc Update(Update{entity.EntityName}Request) returns ({entity.EntityName});"
        );
        sb.AppendLine($"  rpc Delete(Delete{entity.EntityName}Request) returns (Empty);");
        sb.AppendLine("}");

        return sb.ToString();
    }

    /// <summary>
    /// Generates a combined .proto file for all entities.
    /// </summary>
    /// <param name="entities">The collection of entity configurations.</param>
    /// <returns>The .proto file content as a string.</returns>
    public string GenerateAllProtoFiles(IEnumerable<IEntityConfiguration> entities)
    {
        var sb = new StringBuilder();

        sb.AppendLine("syntax = \"proto3\";");
        sb.AppendLine();
        sb.AppendLine("option csharp_namespace = \"Ddap.Grpc.Generated\";");
        sb.AppendLine();
        sb.AppendLine("package ddap;");
        sb.AppendLine();

        foreach (var entity in entities)
        {
            sb.AppendLine($"// Entity: {entity.EntityName}");
            sb.AppendLine($"message {entity.EntityName} {{");

            int fieldNumber = 1;
            foreach (var property in entity.Properties)
            {
                var protoType = MapToProtoType(property.PropertyType);
                var fieldName = ToCamelCase(property.PropertyName);
                sb.AppendLine($"  {protoType} {fieldName} = {fieldNumber};");
                fieldNumber++;
            }

            sb.AppendLine("}");
            sb.AppendLine();
        }

        return sb.ToString();
    }

    private static string MapToProtoType(Type type)
    {
        var underlyingType = Nullable.GetUnderlyingType(type) ?? type;

        return underlyingType.Name switch
        {
            "String" => "string",
            "Int32" => "int32",
            "Int64" => "int64",
            "Boolean" => "bool",
            "Double" => "double",
            "Single" => "float",
            "Decimal" => "double",
            "DateTime" => "string",
            "Guid" => "string",
            "Byte" => "int32",
            "Int16" => "int32",
            _ => "string",
        };
    }

    private static string ToCamelCase(string name)
    {
        if (string.IsNullOrEmpty(name))
            return name;

        return char.ToLowerInvariant(name[0]) + name.Substring(1);
    }
}
