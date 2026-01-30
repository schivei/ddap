# Ddap.CodeGen

Source code generator for DDAP that provides compile-time code generation for entity types and API endpoints.

## Installation

```bash
dotnet add package Ddap.CodeGen
```

## What's Included

This package provides Roslyn-based source generation for DDAP:

- **Compile-Time Generation** - Generate code at build time
- **Entity Type Generation** - Automatic entity class generation
- **API Endpoint Generation** - Generate API controllers/handlers
- **Roslyn Analyzer** - Code analysis and diagnostics

## Quick Start

The source generator runs automatically at build time. Simply add the package and configure DDAP:

```csharp
// No explicit code needed - generation happens at compile time

using Ddap.Core;
using Ddap.CodeGen; // Triggers source generation

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDdap(options =>
{
    options.ConnectionString = builder.Configuration.GetConnectionString("DefaultConnection");
});

// Generated entities and endpoints are automatically available
```

## Generated Code

The code generator creates:
- Entity type definitions from database schema
- Repository implementations
- API controller/handler classes
- DTO mappings

## Documentation

Full documentation: **https://schivei.github.io/ddap**

## Related Packages

- `Ddap.Core` - Core abstractions and infrastructure
- `Ddap.Data.Dapper` - Dapper data provider
- `Ddap.Data.EntityFramework` - Entity Framework Core provider

## License

MIT - see [LICENSE](../../LICENSE)
