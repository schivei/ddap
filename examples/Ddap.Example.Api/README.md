# DDAP Example API

This example demonstrates how to use DDAP (Dynamic Data API Provider) to automatically generate REST, gRPC, and GraphQL APIs from your database schema.

## Features

- **Automatic Entity Loading**: Loads database schema on startup
- **Multiple API Providers**: REST, gRPC, and GraphQL
- **Content Negotiation**: Supports JSON (Newtonsoft.Json) and XML output formats
- **Extensible**: Partial classes allow custom extensions
- **Modular Database Providers**: Choose your database provider (SQL Server, MySQL, PostgreSQL)

## Getting Started

1. Update the connection string in `appsettings.json` to point to your database
2. Choose your database provider (this example uses SQL Server with Dapper)
3. Run the application: `dotnet run`
4. Access the APIs:
   - REST: `http://localhost:5000/api/entity`
   - GraphQL: `http://localhost:5000/graphql`

## Database Provider Options

DDAP now supports modular database providers. Choose the one that fits your needs:

### SQL Server with Dapper
```csharp
using Ddap.Data.Dapper;
using Microsoft.Data.SqlClient;

var connectionString = "Server=localhost;Database=MyDb;...";
services.AddDdap(options => {
    options.ConnectionString = connectionString;
})
.AddDapper(() => new SqlConnection(connectionString));
```

### MySQL with Dapper
```csharp
using Ddap.Data.Dapper;
using MySqlConnector;

var connectionString = "Server=localhost;Database=MyDb;User=root;Password=...";
services.AddDdap(options => {
    options.ConnectionString = connectionString;
})
.AddDapper(() => new MySqlConnection(connectionString));
```

### PostgreSQL with Dapper
```csharp
using Ddap.Data.Dapper;
using Npgsql;

var connectionString = "Host=localhost;Database=MyDb;Username=postgres;Password=...";
services.AddDdap(options => {
    options.ConnectionString = connectionString;
})
.AddDapper(() => new NpgsqlConnection(connectionString));
```

### Entity Framework Core (Database Agnostic)
```csharp
using Ddap.Data.EntityFramework;

services.AddDdap(options => {
    options.ConnectionString = "...";
})
.AddEntityFramework<MyDbContext>();
```

## REST API Examples

### Get all entities (JSON - default with Newtonsoft.Json)
```bash
curl -H "Accept: application/json" http://localhost:5000/api/entity
```

### Get all entities (XML)
```bash
curl -H "Accept: application/xml" http://localhost:5000/api/entity
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
- How to choose a database provider (SQL Server, MySQL, PostgreSQL)
- How to enable multiple API providers
- How to chain provider configurations

## Package Requirements

Install the packages you need:

```bash
# Core (always required)
dotnet add package Ddap.Core

# Choose your database provider:
dotnet add package Ddap.Data.Dapper                # Generic Dapper provider
dotnet add package Ddap.Data.EntityFramework      # For EF Core

# Choose your API providers:
dotnet add package Ddap.Rest                       # REST with JSON/XML
dotnet add package Ddap.Grpc                       # gRPC
dotnet add package Ddap.GraphQL                    # GraphQL
```

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

## Internal Organization

The new modular structure means:
- Each database provider is in its own NuGet package
- Internal implementation classes are in `Internals/` folders
- Choose only the providers you need to minimize dependencies
