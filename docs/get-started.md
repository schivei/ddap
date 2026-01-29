# Getting Started with DDAP

Welcome to DDAP (Dynamic Data API Provider)! This guide will help you get started with creating automatic REST, gRPC, and GraphQL APIs from your database schema.

## What is DDAP?

DDAP is a .NET 10 library that automatically generates APIs from your database schema. Point it at your database, and it will:

- Load your database metadata (tables, columns, relationships, indexes)
- Generate REST, gRPC, and/or GraphQL endpoints
- Support JSON, XML, and YAML content negotiation
- Provide extensibility through partial classes

## Prerequisites

Before you begin, make sure you have:

- **.NET 10 SDK** or later installed
- A database (SQL Server, MySQL, or PostgreSQL)
- Basic knowledge of ASP.NET Core
- Your database connection string

## Installation

### Step 1: Create a New ASP.NET Core Project

```bash
dotnet new webapi -n MyDdapApi
cd MyDdapApi
```

### Step 2: Install DDAP Packages

Install the core package and your choice of providers:

```bash
# Core package (required)
dotnet add package Ddap.Core

# Choose your database provider
dotnet add package Ddap.Data.Dapper      # Generic Dapper provider
# OR
dotnet add package Ddap.Data.Dapper.MySQL          # For MySQL
# OR
dotnet add package Ddap.Data.Dapper.PostgreSQL     # For PostgreSQL

# Choose your API providers (one or more)
dotnet add package Ddap.Rest                       # REST API
dotnet add package Ddap.GraphQL                    # GraphQL
dotnet add package Ddap.Grpc                       # gRPC
```

### Step 3: Configure DDAP in Program.cs

Here's a minimal example using SQL Server:

```csharp
using Ddap.Core;
using Ddap.Data.Dapper;
using Ddap.Rest;
using Microsoft.Data.SqlClient;

var builder = WebApplication.CreateBuilder(args);

// Configure DDAP
var connectionString = "Server=localhost;Database=MyDb;Integrated Security=true;";
builder.Services
    .AddDdap(options =>
    {
        options.ConnectionString = connectionString;
    })
    .AddDapper(() => new SqlConnection(connectionString))  // Use SQL Server with Dapper
    .AddRest();            // Enable REST API

var app = builder.Build();

app.UseRouting();
app.MapControllers();

app.Run();
```

### Step 4: Run Your Application

```bash
dotnet run
```

That's it! Your API is now running and automatically exposing your database entities.

## Testing Your API

Once your application is running, you can test the generated endpoints:

### Get All Entities

```bash
curl http://localhost:5000/api/entity
```

This returns metadata about all tables/entities in your database.

### Get Entity Metadata

```bash
curl http://localhost:5000/api/entity/Users/metadata
```

Replace `Users` with the name of any table in your database.

### Content Negotiation (REST)

DDAP supports multiple output formats:

```bash
# JSON (default - using Newtonsoft.Json)
curl -H "Accept: application/json" http://localhost:5000/api/entity

# XML
curl -H "Accept: application/xml" http://localhost:5000/api/entity
```

## Adding More API Providers

DDAP supports multiple API protocols. You can add GraphQL and gRPC:

```csharp
using Ddap.Core;
using Ddap.Data.Dapper;
using Ddap.Rest;
using Ddap.GraphQL;
using Ddap.Grpc;
using Microsoft.Data.SqlClient;

var builder = WebApplication.CreateBuilder(args);

var connectionString = "Server=localhost;Database=MyDb;...";
builder.Services
    .AddDdap(options =>
    {
        options.ConnectionString = connectionString;
    })
    .AddDapper(() => new SqlConnection(connectionString))
    .AddRest()      // REST API
    .AddGraphQL()   // GraphQL
    .AddGrpc();     // gRPC

var app = builder.Build();

app.UseRouting();
app.MapControllers();
app.MapGraphQL("/graphql");  // GraphQL endpoint

app.Run();
```

### Testing GraphQL

Navigate to `http://localhost:5000/graphql` in your browser to access the GraphQL playground, or use curl:

```bash
curl -X POST http://localhost:5000/graphql \
  -H "Content-Type: application/json" \
  -d '{"query": "{ entities { name schema propertyCount } }"}'
```

## Using Different Database Providers

### MySQL

```csharp
using Ddap.Data.Dapper;
using MySqlConnector;

var connectionString = "Server=localhost;Database=MyDb;User=root;Password=secret;";
builder.Services
    .AddDdap(options =>
    {
        options.ConnectionString = connectionString;
    })
    .AddDapper(() => new MySqlConnection(connectionString))
    .AddRest();
```

### PostgreSQL

```csharp
using Ddap.Data.Dapper;
using Npgsql;

var connectionString = "Host=localhost;Database=MyDb;Username=postgres;Password=secret;";
builder.Services
    .AddDdap(options =>
    {
        options.ConnectionString = connectionString;
    })
    .AddDapper(() => new NpgsqlConnection(connectionString))
    .AddRest();
```

### Entity Framework Core

For database-agnostic Entity Framework Core support:

```csharp
using Ddap.Data.EntityFramework;

builder.Services
    .AddDdap(options =>
    {
        options.ConnectionString = "...";
    })
    .AddEntityFramework<MyDbContext>()
    .AddRest();
```

## Configuration from appsettings.json

It's recommended to store your connection string in `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=MyDb;Integrated Security=true;"
  }
}
```

Then reference it in your code:

```csharp
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services
    .AddDdap(options =>
    {
        options.ConnectionString = connectionString;
    })
    .AddDapper(() => new SqlConnection(connectionString))
    .AddRest();
```

## Next Steps

Now that you have DDAP running, explore these topics:

- **[Architecture](./architecture.md)** - Learn about DDAP's architecture and design
- **[Advanced Usage](./advanced.md)** - Extensibility, custom endpoints, and advanced patterns
- **[Database Providers](./database-providers.md)** - Deep dive into database provider options
- **[API Providers](./api-providers.md)** - Learn about REST, gRPC, and GraphQL providers
- **[Troubleshooting](./troubleshooting.md)** - Common issues and solutions

## Example Application

Check out the complete example application at [`examples/Ddap.Example.Api`](https://github.com/schivei/ddap/tree/main/examples/Ddap.Example.Api) for a working reference implementation.

## Need Help?

- **Documentation**: [https://schivei.github.io/ddap](https://schivei.github.io/ddap)
- **Issues**: [GitHub Issues](https://github.com/schivei/ddap/issues)
- **Contributing**: See [CONTRIBUTING.md](https://github.com/schivei/ddap/blob/main/CONTRIBUTING.md)
