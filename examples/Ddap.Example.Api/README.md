# DDAP Example API

This example demonstrates how to use DDAP (Dynamic Data API Provider) to automatically generate REST, gRPC, and GraphQL APIs from your database schema.

## Features

- **Automatic Entity Loading**: Loads database schema on startup
- **Multiple API Providers**: REST, gRPC, and GraphQL
- **Content Negotiation**: Supports JSON (Newtonsoft.Json), XML, and YAML output formats
- **Extensible**: Partial classes allow custom extensions

## Getting Started

1. Update the connection string in `appsettings.json` to point to your database
2. Run the application: `dotnet run`
3. Access the APIs:
   - REST: `http://localhost:5000/api/entity`
   - GraphQL: `http://localhost:5000/graphql`

## REST API Examples

### Get all entities (JSON - default with Newtonsoft.Json)
```bash
curl -H "Accept: application/json" http://localhost:5000/api/entity
```

### Get all entities (XML)
```bash
curl -H "Accept: application/xml" http://localhost:5000/api/entity
```

### Get all entities (YAML)
```bash
curl -H "Accept: application/yaml" http://localhost:5000/api/entity
```

### Get entity metadata
```bash
curl http://localhost:5000/api/entity/{entityName}/metadata
```

## GraphQL Examples

### Query all entities
```graphql
query {
  entities {
    name
    schema
    propertyCount
  }
}
```

### Query specific entity
```graphql
query {
  entity(entityName: "Users") {
    name
    schema
    propertyCount
  }
}
```

## Configuration

The DDAP configuration in `Program.cs` shows:
- How to configure the database provider (SQL Server, MySQL, PostgreSQL)
- How to enable multiple API providers
- How to chain provider configurations

## Extending the API

All controllers, services, queries, and mutations are partial classes. You can extend them:

```csharp
namespace Ddap.Rest;

public partial class EntityController
{
    [HttpGet("custom")]
    public IActionResult CustomEndpoint()
    {
        return Ok("Custom endpoint");
    }
}
```
