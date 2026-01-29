# Ddap.Core

Core abstractions and infrastructure for DDAP (Dynamic Data API Provider).

## Installation

```bash
dotnet add package Ddap.Core
```

## What's Included

This package provides the foundational abstractions and infrastructure for DDAP:

- **Entity Discovery** - Automatic database schema loading
- **Metadata Models** - Entity, property, index, and relationship definitions
- **Configuration Options** - Core DDAP configuration
- **Lifecycle Hooks** - Extension points for custom behavior
- **Repository Interfaces** - Core data access abstractions

## Quick Start

```csharp
using Ddap.Core;

var builder = WebApplication.CreateBuilder(args);

// Add DDAP core infrastructure
builder.Services.AddDdap(options =>
{
    options.ConnectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    // Additional configuration...
});

// Add a data provider (Dapper or Entity Framework)
// Add API providers (REST, GraphQL, gRPC)

var app = builder.Build();
app.Run();
```

## Documentation

Full documentation: **https://schivei.github.io/ddap**

## Related Packages

- `Ddap.Data.Dapper` - Dapper data provider (database-agnostic)
- `Ddap.Data.EntityFramework` - Entity Framework Core provider
- `Ddap.Rest` - REST APIs
- `Ddap.GraphQL` - GraphQL APIs
- `Ddap.Grpc` - gRPC services

## License

MIT - see [LICENSE](../../LICENSE)
