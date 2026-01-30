# DDAP - Dynamic Data API Provider

[![Build and Test](https://github.com/schivei/ddap/actions/workflows/build.yml/badge.svg)](https://github.com/schivei/ddap/actions/workflows/build.yml)
[![NuGet](https://img.shields.io/nuget/v/Ddap.Core.svg)](https://www.nuget.org/packages?q=ddap)
[![License](https://img.shields.io/github/license/schivei/ddap)](LICENSE)

**DDAP** (Dynamic Data API Provider) is a .NET 10 library that automatically generates REST, gRPC, and GraphQL APIs from your database schema. Load your database metadata at runtime and instantly expose your data through multiple API protocols.

## Features

- 🚀 **Automatic API Generation**: Load database schema and automatically create API endpoints
- 🗄️ **Multiple Database Options**: Supports SQL Server, MySQL, and PostgreSQL (one at a time)
- 🌐 **Multiple API Protocols**: REST, gRPC, GraphQL simultaneously
- 📋 **Content Negotiation**: REST APIs support JSON (Newtonsoft.Json) and XML
- 🔧 **Extensible**: Partial classes for custom controllers, services, queries, and mutations
- 📦 **Modular**: Separate libraries for each provider
- 🎯 **Builder Pattern**: Fluent API for easy configuration
- 📊 **Code Coverage**: Generated code automatically excluded from coverage reports
- 🔄 **Source Generators**: Compile-time code generation support with `Ddap.CodeGen`
- 🔐 **Authentication & Authorization**: JWT, role-based access, entity-level security with `Ddap.Auth`
- 🔔 **Real-Time Subscriptions**: SignalR and GraphQL subscriptions with `Ddap.Subscriptions`
- ☁️ **.NET Aspire Integration**: Seamless integration with `Ddap.Aspire` for cloud-native apps

## Quick Start

### Installation

Choose the packages you need based on your database and API requirements:

```bash
# Core (always required)
dotnet add package Ddap.Core

# Database Providers (choose one):
dotnet add package Ddap.Data.Dapper                # Generic Dapper provider
dotnet add package Ddap.Data.EntityFramework       # Entity Framework Core (database-agnostic)

# API Providers (choose one or more):
dotnet add package Ddap.Rest                       # REST API (JSON/XML)
dotnet add package Ddap.Grpc                       # gRPC
dotnet add package Ddap.GraphQL                    # GraphQL
```

### Configuration

Choose your database provider and configure DDAP:

#### SQL Server with Dapper
```csharp
using Ddap.Core;
using Ddap.Data.Dapper;
using Ddap.Rest;
using Ddap.Grpc;
using Ddap.GraphQL;
using Microsoft.Data.SqlClient;

var builder = WebApplication.CreateBuilder(args);

var connectionString = "Server=localhost;Database=MyDb;...";
builder.Services
    .AddDdap(options =>
    {
        options.ConnectionString = connectionString;
    })
    .AddDapper(() => new SqlConnection(connectionString))  // SQL Server with Dapper
    .AddRest()             // Enable REST API (JSON/XML)
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
using Ddap.Data.Dapper;
using MySqlConnector;

var connectionString = "Server=localhost;Database=MyDb;User=root;Password=...";
builder.Services
    .AddDdap(options =>
    {
        options.ConnectionString = connectionString;
    })
    .AddDapper(() => new MySqlConnection(connectionString))      // MySQL with Dapper
    .AddRest()
    .AddGraphQL();
```

#### PostgreSQL with Dapper
```csharp
using Ddap.Data.Dapper;
using Npgsql;

var connectionString = "Host=localhost;Database=MyDb;Username=postgres;Password=...";
builder.Services
    .AddDdap(options =>
    {
        options.ConnectionString = connectionString;
    })
    .AddDapper(() => new NpgsqlConnection(connectionString))  // PostgreSQL with Dapper
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
│   ├── Ddap.Data.Dapper/               # Generic Dapper provider for any database
│   ├── Ddap.CodeGen/                   # Source generators
│   ├── Ddap.Rest/                      # REST API provider (JSON/XML)
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

## Advanced Features

### Authentication & Authorization (`Ddap.Auth`)

Secure your APIs with JWT authentication and role-based authorization:

```csharp
builder.Services
    .AddDdap(options => { /* ... */ })
    .AddDapper(() => new SqlConnection(connectionString))
    .AddAuth(authOptions =>
    {
        authOptions.RequireAuthenticationByDefault = true;
        authOptions.AddEntityPolicy("Users", policy => policy.RequireRole("Admin"));
        authOptions.AddEntityPolicy("Products", policy => policy.RequireAuthenticatedUser());
    })
    .AddRest();
```

**Features:**
- JWT Bearer authentication
- Role-based authorization
- Entity-level security policies
- Field-level access control
- Multi-tenant support

[📖 See full Auth example](examples/Ddap.Example.Auth)

### Real-Time Subscriptions (`Ddap.Subscriptions`)

Get real-time notifications when data changes:

```csharp
builder.Services
    .AddDdap(options => { /* ... */ })
    .AddDapper(() => new SqlConnection(connectionString))
    .AddRest()
    .AddGraphQL()
    .AddSubscriptions(subscriptionOptions =>
    {
        subscriptionOptions.EnableFor("Products");
        subscriptionOptions.NotifyOnCreate = true;
        subscriptionOptions.NotifyOnUpdate = true;
    });
```

**Features:**
- SignalR integration
- GraphQL subscriptions
- WebSocket support
- Filtered notifications
- Custom event handlers

[📖 See full Subscriptions example](examples/Ddap.Example.Subscriptions)

### Source Generator (`Ddap.CodeGen`)

Generate strongly-typed entity classes at compile-time:

```csharp
[GenerateEntity("Products")]
public partial class Product
{
    // Properties generated from database schema at compile-time
    // Full IntelliSense support!
}
```

**Benefits:**
- Type-safe property access
- IntelliSense support
- Compile-time checking
- No reflection overhead
- Auto-generated DTOs

[📖 See full CodeGen example](examples/Ddap.Example.CodeGen)

### .NET Aspire Integration (`Ddap.Aspire`)

Seamless integration with .NET Aspire for cloud-native applications:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var db = builder.AddSqlServer("sqlserver").AddDatabase("catalogdb");

var api = builder.AddDdapApi("ddap-api")
                 .WithReference(db)
                 .WithRestApi()
                 .WithGraphQL()
                 .WithAutoRefresh(30);

builder.Build().Run();
```

**Features:**
- Automatic service discovery
- Database connection management
- Auto-refresh for schema changes
- Observability dashboard
- Easy scaling

[📖 See full Aspire example](examples/Ddap.Example.Aspire)

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

We welcome contributions! Please see our [Contributing Guide](CONTRIBUTING.md) for details on:

- How to report bugs
- How to suggest enhancements
- Development setup
- Pull request process
- Coding standards
- Testing guidelines

Quick start for contributors:

```bash
# Fork and clone the repository
git clone https://github.com/YOUR-USERNAME/ddap.git
cd ddap

# Install dependencies
dotnet restore

# Build the solution
dotnet build

# Run tests
dotnet test
```

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Roadmap

### Completed ✅
- ✅ **Authentication and authorization support** - Available via `Ddap.Auth` package
- ✅ **Real-time subscriptions** - Available via `Ddap.Subscriptions` package with SignalR
- ✅ **Source generator support** - Available via `Ddap.CodeGen` package
- ✅ **.NET Aspire integration** - Available via `Ddap.Aspire` package

### In Progress 🚧
- [ ] Enhanced query filtering and pagination
- [ ] Dynamic .proto file download endpoint for gRPC
- [ ] REST endpoints derived from gRPC services

### Planned 📋
- [ ] OData support
- [ ] Webhook notifications
- [ ] Rate limiting and throttling
- [ ] Advanced caching strategies
- [ ] Multi-tenancy support

## Documentation

### 📚 Comprehensive Guides

- **[Getting Started](docs/get-started.md)** - Quick start guide and basic setup
- **[Architecture](docs/architecture.md)** - Understand DDAP's design and components
- **[Advanced Usage](docs/advanced.md)** - Extensibility, custom endpoints, and patterns
- **[Database Providers](docs/database-providers.md)** - SQL Server, MySQL, PostgreSQL, EF Core
- **[API Providers](docs/api-providers.md)** - REST, gRPC, and GraphQL documentation
- **[Troubleshooting](docs/troubleshooting.md)** - Common issues and solutions

### 🌐 Online Documentation

Full documentation is available at [https://schivei.github.io/ddap](https://schivei.github.io/ddap)

## Examples

Comprehensive examples demonstrating each feature:

- **[Basic API Example](examples/Ddap.Example.Api)** - REST, gRPC, and GraphQL with multiple database providers
- **[Aspire Integration](examples/Ddap.Example.Aspire)** - .NET Aspire orchestration with auto-refresh
- **[Authentication & Authorization](examples/Ddap.Example.Auth)** - JWT authentication, role-based access, entity-level security
- **[Real-Time Subscriptions](examples/Ddap.Example.Subscriptions)** - SignalR and GraphQL subscriptions for live updates
- **[Source Generator](examples/Ddap.Example.CodeGen)** - Compile-time entity generation with strong typing

Each example includes:
- Complete source code
- Step-by-step setup instructions
- Configuration examples
- Usage demonstrations
- Best practices

## Support

For issues, questions, or feature requests, please [open an issue](https://github.com/schivei/ddap/issues) on GitHub.

### Getting Help

- **📖 Documentation:** Check the [docs](docs/) directory
- **💬 Discussions:** [GitHub Discussions](https://github.com/schivei/ddap/discussions)
- **🐛 Bug Reports:** [GitHub Issues](https://github.com/schivei/ddap/issues)
- **💡 Feature Requests:** [GitHub Issues](https://github.com/schivei/ddap/issues) with enhancement label
