# Ddap.GraphQL

GraphQL API provider for DDAP, powered by HotChocolate.

## Installation

```bash
dotnet add package Ddap.GraphQL
```

## Quick Start

```csharp
using Ddap.Core;
using Ddap.Data.Dapper;
using Ddap.GraphQL;
using Microsoft.Data.SqlClient;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDdap()
    .AddDapper(() => new SqlConnection(
        builder.Configuration.GetConnectionString("DefaultConnection")
    ))
    .AddGraphQL(graphql =>
    {
        // You configure HotChocolate
        graphql
            .AddFiltering()
            .AddSorting()
            .AddProjections();
    });

var app = builder.Build();
app.MapGraphQL();
app.Run();
```

## Features

- ✅ Automatic GraphQL schema generation
- ✅ Powered by HotChocolate 13
- ✅ Queries and mutations
- ✅ Schema introspection
- ✅ GraphQL Playground
- ✅ Fully extensible via partial classes

## Example Query

```graphql
query {
  entities {
    name
    schema
    properties {
      name
      dataType
      isRequired
    }
  }
}
```

## Extensibility

Add custom queries and mutations:

```csharp
namespace Ddap.GraphQL;

public partial class Query
{
    public string Version() => "1.0.0";
    
    public IEnumerable<Entity> SearchEntities(string searchTerm)
    {
        // Your custom logic
        return results;
    }
}

public partial class Mutation
{
    public bool RefreshMetadata()
    {
        // Your custom logic
        return true;
    }
}
```

## GraphQL Playground

Access at `http://localhost:5000/graphql` to:
- Explore the schema
- Run queries interactively
- View auto-generated documentation

## Documentation

Full documentation: **https://schivei.github.io/ddap/api-providers**

## Related Packages

- `Ddap.Core` - Core abstractions
- `Ddap.Data.Dapper` - Dapper provider
- `Ddap.Data.EntityFramework` - EF Core provider
- `Ddap.Rest` - REST APIs
- `Ddap.Grpc` - gRPC services

## License

MIT - see [LICENSE](../../LICENSE)
