# Ddap.Rest

REST API provider for DDAP with content negotiation support.

## Installation

```bash
dotnet add package Ddap.Rest
```

## Quick Start

```csharp
using Ddap.Core;
using Ddap.Data.Dapper;
using Ddap.Rest;
using Microsoft.Data.SqlClient;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDdap()
    .AddDapper(() => new SqlConnection(
        builder.Configuration.GetConnectionString("DefaultConnection")
    ))
    .AddRest();

var app = builder.Build();
app.MapControllers();
app.Run();
```

## Features

- ✅ Automatic REST endpoint generation
- ✅ Content negotiation (JSON, XML)
- ✅ Standard HTTP verbs (GET, POST, PUT, DELETE)
- ✅ Entity metadata endpoints
- ✅ Fully extensible via partial classes

## Generated Endpoints

```
GET    /api/entity                     # List all entities
GET    /api/entity/{entityName}/metadata # Entity metadata
GET    /api/entity/{entityName}/data   # Get all records
GET    /api/entity/{entityName}/{id}   # Get single record
POST   /api/entity/{entityName}        # Create record
PUT    /api/entity/{entityName}/{id}   # Update record
DELETE /api/entity/{entityName}/{id}   # Delete record
```

## Extensibility

Extend via partial classes:

```csharp
namespace Ddap.Rest;

public partial class EntityController
{
    [HttpGet("search")]
    public IActionResult Search([FromQuery] string query)
    {
        // Your custom logic
        return Ok(results);
    }
}
```

## Documentation

Full documentation: **https://schivei.github.io/ddap/api-providers**

## Related Packages

- `Ddap.Core` - Core abstractions
- `Ddap.Data.Dapper` - Dapper provider
- `Ddap.Data.EntityFramework` - EF Core provider
- `Ddap.GraphQL` - GraphQL APIs
- `Ddap.Grpc` - gRPC services

## License

MIT - see [LICENSE](../../LICENSE)
