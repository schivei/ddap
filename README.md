# DDAP - Dynamic Data API Provider

[![Build and Test](https://github.com/schivei/ddap/actions/workflows/build.yml/badge.svg)](https://github.com/schivei/ddap/actions/workflows/build.yml)
[![NuGet](https://img.shields.io/nuget/v/Ddap.Core.svg)](https://www.nuget.org/packages?q=ddap)
[![License](https://img.shields.io/github/license/schivei/ddap)](LICENSE)

**DDAP** (Dynamic Data API Provider) is a .NET 10 library that automatically generates REST, gRPC, and GraphQL APIs from your database schema. Load your database metadata at runtime and instantly expose your data through multiple API protocols.

## Features

- 🚀 **Automatic API Generation**: Load database schema and automatically create API endpoints
- 🗄️ **Multiple Database Support**: SQL Server, MySQL, PostgreSQL
- 🌐 **Multiple API Protocols**: REST, gRPC, GraphQL
- 📋 **Content Negotiation**: REST APIs support JSON (Newtonsoft.Json), XML, and YAML
- 🔧 **Extensible**: Partial classes for custom controllers, services, queries, and mutations
- 📦 **Modular**: Separate libraries for each provider
- 🎯 **Builder Pattern**: Fluent API for easy configuration
- 📊 **Code Coverage**: Generated code automatically excluded from coverage reports
- 🔄 **Source Generators**: Compile-time code generation support

## Quick Start

### Installation

Choose the packages you need based on your database and API requirements:

```bash
# Core (always required)
dotnet add package Ddap.Core

# Database Providers (choose one or more):
dotnet add package Ddap.Data.Dapper.SqlServer      # SQL Server with Dapper
dotnet add package Ddap.Data.Dapper.MySQL          # MySQL with Dapper
dotnet add package Ddap.Data.Dapper.PostgreSQL     # PostgreSQL with Dapper
dotnet add package Ddap.Data.EntityFramework       # Entity Framework Core (database-agnostic)

# API Providers (choose one or more):
dotnet add package Ddap.Rest                       # REST API (JSON/XML/YAML)
dotnet add package Ddap.Grpc                       # gRPC
dotnet add package Ddap.GraphQL                    # GraphQL
```

### Configuration

Choose your database provider and configure DDAP:

#### SQL Server with Dapper
```csharp
using Ddap.Core;
using Ddap.Data.Dapper.SqlServer;
using Ddap.Rest;
using Ddap.Grpc;
using Ddap.GraphQL;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddDdap(options =>
    {
        options.ConnectionString = "Server=localhost;Database=MyDb;...";
    })
    .AddSqlServerDapper()  // SQL Server with Dapper
    .AddRest()             // Enable REST API (JSON/XML/YAML)
    .AddGrpc()             // Enable gRPC
    .AddGraphQL();         // Enable GraphQL

var app = builder.Build();

app.UseRouting();
app.MapControllers();
app.MapGraphQL("/graphql");

app.Run();
```

#### MySQL with Dapper
```csharp
using Ddap.Data.Dapper.MySQL;

builder.Services
    .AddDdap(options =>
    {
        options.ConnectionString = "Server=localhost;Database=MyDb;User=root;Password=...";
    })
    .AddMySqlDapper()      // MySQL with Dapper
    .AddRest()
    .AddGraphQL();
```

#### PostgreSQL with Dapper
```csharp
using Ddap.Data.Dapper.PostgreSQL;

builder.Services
    .AddDdap(options =>
    {
        options.ConnectionString = "Host=localhost;Database=MyDb;Username=postgres;Password=...";
    })
    .AddPostgreSqlDapper()  // PostgreSQL with Dapper
    .AddRest()
    .AddGrpc();
```

## Content Negotiation (REST API)

DDAP REST APIs support multiple output formats using **Newtonsoft.Json** for JSON serialization:

### JSON (Default - Newtonsoft.Json)
```bash
curl -H "Accept: application/json" http://localhost:5000/api/entity
```

### XML
```bash
curl -H "Accept: application/xml" http://localhost:5000/api/entity
```

### YAML
```bash
curl -H "Accept: application/yaml" http://localhost:5000/api/entity
```

## API Examples

### REST API

```bash
# Get all entities
GET /api/entity

# Get entity metadata
GET /api/entity/{entityName}/metadata
```

### GraphQL

```graphql
query {
  entities {
    name
    schema
    propertyCount
  }
}
```

## Extensibility

All controllers, services, queries, and mutations are partial classes:

```csharp
namespace Ddap.Rest;

public partial class EntityController
{
    [HttpGet("custom")]
    public IActionResult CustomEndpoint()
    {
        return Ok("Custom logic here");
    }
}
```

```csharp
namespace Ddap.GraphQL;

public partial class Query
{
    public string CustomQuery() => "Custom GraphQL query";
}
```

## Project Structure

```
ddap/
├── src/
│   ├── Ddap.Core/                      # Core abstractions and interfaces
│   │   └── Internals/                  # Internal implementation classes
│   ├── Ddap.Data/                      # Legacy data provider (deprecated)
│   ├── Ddap.Data.EntityFramework/      # EF Core provider (database-agnostic)
│   ├── Ddap.Data.Dapper.SqlServer/     # SQL Server with Dapper
│   ├── Ddap.Data.Dapper.MySQL/         # MySQL with Dapper
│   ├── Ddap.Data.Dapper.PostgreSQL/    # PostgreSQL with Dapper
│   ├── Ddap.Memory/                    # In-memory entity management
│   ├── Ddap.CodeGen/                   # Source generators
│   ├── Ddap.Rest/                      # REST API provider (JSON/XML/YAML)
│   ├── Ddap.Grpc/                      # gRPC provider
│   └── Ddap.GraphQL/                   # GraphQL provider
├── tests/
│   └── Ddap.Tests/                     # Unit and integration tests
├── examples/
│   └── Ddap.Example.Api/               # Example application
└── docs/                               # Documentation
```

## Database Support

- **SQL Server** - Full support with indexes, relationships, composite keys
- **MySQL** - Full support
- **PostgreSQL** - Full support

All providers support:
- Complex indexes
- Foreign key relationships
- Composite primary keys
- UUID/GUID keys
- Auto-increment columns

## CI/CD

The project includes GitHub Actions workflows for:

- **Build & Test**: Automated testing with SQL Server, MySQL, PostgreSQL containers
- **Release**: Manual release creation with automatic versioning
- **Documentation**: Automatic documentation deployment to GitHub Pages

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Roadmap

- [ ] Enhanced source generator support
- [ ] Dynamic .proto file download endpoint for gRPC
- [ ] REST endpoints derived from gRPC services
- [ ] Advanced query filtering and pagination
- [ ] Real-time subscriptions (GraphQL/SignalR)
- [ ] Authentication and authorization support

## Documentation

Full documentation is available at [https://schivei.github.io/ddap](https://schivei.github.io/ddap)

## Support

For issues, questions, or feature requests, please [open an issue](https://github.com/schivei/ddap/issues) on GitHub.
