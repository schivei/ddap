# API Providers

DDAP supports multiple API protocols through separate provider packages. This guide covers REST, gRPC, and GraphQL providers in detail.

## Overview

| Provider | Package | Protocol | Features |
|----------|---------|----------|----------|
| REST | `Ddap.Rest` | HTTP/REST | JSON, XML, Content Negotiation |
| gRPC | `Ddap.Grpc` | gRPC/HTTP2 | Binary protocol, streaming |
| GraphQL | `Ddap.GraphQL` | GraphQL | Queries, mutations, schema introspection |

## REST Provider

### Installation

```bash
dotnet add package Ddap.Rest
```

### Configuration

```csharp
using Ddap.Rest;

builder.Services
    .AddDdap(options => { /* ... */ })
    .AddDapper(() => new SqlConnection(connectionString))
    .AddRest();

var app = builder.Build();

app.UseRouting();
app.MapControllers();
```

### Features

#### Content Negotiation

DDAP REST APIs support multiple output formats via the `Accept` header:

**JSON (Default - Newtonsoft.Json)**
```bash
curl -H "Accept: application/json" http://localhost:5000/api/entity
```

**XML**
```bash
curl -H "Accept: application/xml" http://localhost:5000/api/entity
```

#### Generated Endpoints

The REST provider automatically generates the following endpoints:

**Entity Listing**
```http
GET /api/entity
```
Returns metadata about all entities in the database.

**Entity Metadata**
```http
GET /api/entity/{entityName}/metadata
```
Returns detailed metadata for a specific entity including properties, indexes, and relationships.

**CRUD Operations**

```http
# Create
POST /api/entity/{entityName}
Content-Type: application/json

{
  "property1": "value1",
  "property2": "value2"
}

# Read (single)
GET /api/entity/{entityName}/{id}

# Read (all)
GET /api/entity/{entityName}/data

# Update
PUT /api/entity/{entityName}/{id}
Content-Type: application/json

{
  "property1": "newValue"
}

# Delete
DELETE /api/entity/{entityName}/{id}
```

### Customization

#### Custom Controllers

Extend the generated controller:

```csharp
using Microsoft.AspNetCore.Mvc;

namespace Ddap.Rest;

public partial class EntityController
{
    [HttpGet("search")]
    public IActionResult Search([FromQuery] string query)
    {
        // Custom search logic
        return Ok(results);
    }

    [HttpGet("{entityName}/stats")]
    public IActionResult GetStatistics(string entityName)
    {
        var entity = _repository.GetEntity(entityName);
        return Ok(new {
            Name = entity.Name,
            PropertyCount = entity.Properties.Count,
            IndexCount = entity.Indexes.Count
        });
    }
}
```

#### JSON Serialization Settings

Configure Newtonsoft.Json:

```csharp
builder.Services.AddControllers()
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.ContractResolver = 
            new CamelCasePropertyNamesContractResolver();
        options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
        options.SerializerSettings.DateFormatString = "yyyy-MM-ddTHH:mm:ssZ";
    });
```

### Best Practices

1. **Use versioning** for API stability:
   ```csharp
   [ApiVersion("1.0")]
   [Route("api/v{version:apiVersion}/entity")]
   public partial class EntityController
   ```

2. **Add authentication**:
   ```csharp
   [Authorize]
   [HttpPost("{entityName}")]
   public IActionResult Create(string entityName, [FromBody] object data)
   ```

3. **Implement pagination**:
   ```csharp
   [HttpGet("{entityName}/data")]
   public IActionResult GetAll(
       string entityName,
       [FromQuery] int page = 1,
       [FromQuery] int pageSize = 20)
   ```

4. **Add caching**:
   ```csharp
   [ResponseCache(Duration = 60)]
   [HttpGet("{entityName}/metadata")]
   public IActionResult GetMetadata(string entityName)
   ```

## GraphQL Provider

### Installation

```bash
dotnet add package Ddap.GraphQL
```

### Configuration

```csharp
using Ddap.GraphQL;

builder.Services
    .AddDdap(options => { /* ... */ })
    .AddDapper(() => new SqlConnection(connectionString))
    .AddGraphQL();

var app = builder.Build();

app.MapGraphQL("/graphql");
```

### Features

#### Generated Schema

DDAP automatically generates a GraphQL schema based on your database entities:

```graphql
type Query {
  entities: [EntityMetadata!]!
  entity(entityName: String!): EntityMetadata
}

type EntityMetadata {
  name: String!
  schema: String!
  propertyCount: Int!
  properties: [PropertyMetadata!]!
  indexes: [IndexMetadata!]!
  relationships: [RelationshipMetadata!]!
}

type PropertyMetadata {
  name: String!
  dataType: String!
  isRequired: Boolean!
  isPrimaryKey: Boolean!
  isAutoIncrement: Boolean!
}
```

#### Query Examples

**List all entities:**
```graphql
query {
  entities {
    name
    schema
    propertyCount
  }
}
```

**Get specific entity:**
```graphql
query {
  entity(entityName: "Users") {
    name
    properties {
      name
      dataType
      isRequired
      isPrimaryKey
    }
    relationships {
      name
      targetEntityName
      relationshipType
    }
  }
}
```

### Customization

#### Custom Queries

Add custom queries to the generated Query type:

```csharp
using HotChocolate;

namespace Ddap.GraphQL;

public partial class Query
{
    public string Version() => "1.0.0";

    public async Task<EntityStatistics> GetStatistics(
        [Service] IEntityRepository repository)
    {
        return new EntityStatistics
        {
            TotalEntities = repository.Entities.Count,
            TotalProperties = repository.Entities.Sum(e => e.Properties.Count),
            TotalIndexes = repository.Entities.Sum(e => e.Indexes.Count)
        };
    }

    public IEnumerable<IEntityConfiguration> SearchEntities(
        string searchTerm,
        [Service] IEntityRepository repository)
    {
        return repository.Entities
            .Where(e => e.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase));
    }
}
```

#### Custom Mutations

Add custom mutations:

```csharp
namespace Ddap.GraphQL;

public partial class Mutation
{
    public async Task<bool> RefreshMetadata(
        [Service] IDataProvider provider,
        [Service] IEntityRepository repository)
    {
        await provider.LoadEntitiesAsync(repository);
        return true;
    }

    public string TestMutation(string input)
    {
        return $"Received: {input}";
    }
}
```

#### Custom Types

Define custom GraphQL types:

```csharp
public class EntityStatistics
{
    public int TotalEntities { get; set; }
    public int TotalProperties { get; set; }
    public int TotalIndexes { get; set; }
}
```

### Best Practices

1. **Use DataLoader** for efficient data loading:
   ```csharp
   public async Task<IEntityConfiguration> GetEntity(
       string entityName,
       [Service] IEntityRepository repository,
       EntityByNameDataLoader dataLoader)
   {
       return await dataLoader.LoadAsync(entityName);
   }
   ```

2. **Add authorization**:
   ```csharp
   [Authorize(Roles = "Admin")]
   public async Task<bool> DeleteEntity(string entityName)
   ```

3. **Implement field-level authorization**:
   ```csharp
   [Authorize(Policy = "ReadMetadata")]
   public IEntityConfiguration GetEntity(string entityName)
   ```

4. **Add validation**:
   ```csharp
   public async Task<bool> CreateEntity(
       [Required] string entityName,
       [Service] IEntityRepository repository)
   ```

### GraphQL Playground

Access the GraphQL Playground at `http://localhost:5000/graphql` to:
- Explore the schema
- Run queries interactively
- View documentation
- Test mutations

## gRPC Provider

### Installation

```bash
dotnet add package Ddap.Grpc
```

### Configuration

```csharp
using Ddap.Grpc;

builder.Services
    .AddDdap(options => { /* ... */ })
    .AddDapper(() => new SqlConnection(connectionString))
    .AddGrpc();

var app = builder.Build();

app.MapGrpcService<EntityService>();
```

### Features

#### Generated Services

DDAP generates gRPC services based on your entities:

```protobuf
service EntityService {
  rpc GetEntities(Empty) returns (EntityListResponse);
  rpc GetEntity(GetEntityRequest) returns (EntityResponse);
  rpc GetEntityMetadata(GetEntityRequest) returns (EntityMetadataResponse);
}

message EntityResponse {
  string name = 1;
  string schema = 2;
  int32 property_count = 3;
}
```

#### Client Example

```csharp
using Grpc.Net.Client;

// Create channel
var channel = GrpcChannel.ForAddress("https://localhost:5001");
var client = new EntityService.EntityServiceClient(channel);

// Call service
var response = await client.GetEntitiesAsync(new Empty());
foreach (var entity in response.Entities)
{
    Console.WriteLine($"Entity: {entity.Name}");
}
```

### Customization

#### Custom gRPC Services

```csharp
using Grpc.Core;

namespace Ddap.Grpc;

public partial class EntityService
{
    public override async Task<CustomResponse> CustomMethod(
        CustomRequest request,
        ServerCallContext context)
    {
        // Custom logic
        return new CustomResponse
        {
            Message = "Custom response"
        };
    }
}
```

#### Server Streaming

```csharp
public override async Task StreamEntities(
    Empty request,
    IServerStreamWriter<EntityResponse> responseStream,
    ServerCallContext context)
{
    foreach (var entity in _repository.Entities)
    {
        await responseStream.WriteAsync(new EntityResponse
        {
            Name = entity.Name,
            Schema = entity.Schema
        });
    }
}
```

### Best Practices

1. **Use deadlines**:
   ```csharp
   var response = await client.GetEntitiesAsync(
       new Empty(),
       deadline: DateTime.UtcNow.AddSeconds(5));
   ```

2. **Implement health checks**:
   ```csharp
   builder.Services.AddGrpcHealthChecks()
       .AddCheck("entity_service", () => HealthCheckResult.Healthy());
   ```

3. **Add interceptors**:
   ```csharp
   builder.Services.AddGrpc(options =>
   {
       options.Interceptors.Add<LoggingInterceptor>();
   });
   ```

4. **Enable compression**:
   ```csharp
   builder.Services.AddGrpc(options =>
   {
       options.ResponseCompressionLevel = CompressionLevel.Optimal;
   });
   ```

## Provider Comparison

| Feature | REST | GraphQL | gRPC |
|---------|------|---------|------|
| Protocol | HTTP/1.1 | HTTP/1.1 | HTTP/2 |
| Format | JSON/XML | JSON | Binary (Protocol Buffers) |
| Schema | OpenAPI/Swagger | GraphQL Schema | .proto files |
| Query Flexibility | Limited | High | Medium |
| Performance | Medium | Medium | High |
| Browser Support | Excellent | Excellent | Limited |
| Streaming | Limited | Subscriptions | Excellent |
| Caching | Easy | Complex | Medium |
| Learning Curve | Easy | Medium | Medium |

## Multiple Providers

You can enable multiple providers simultaneously:

```csharp
builder.Services
    .AddDdap(options => { /* ... */ })
    .AddDapper(() => new SqlConnection(connectionString))
    .AddRest()      // REST at /api/*
    .AddGraphQL()   // GraphQL at /graphql
    .AddGrpc();     // gRPC on configured port

var app = builder.Build();

app.UseRouting();
app.MapControllers();           // REST
app.MapGraphQL("/graphql");     // GraphQL
app.MapGrpcService<EntityService>();  // gRPC
```

## Next Steps

- [Advanced Usage](./advanced.md) - Extensibility and patterns
- [Troubleshooting](./troubleshooting.md) - Common issues
- [Database Providers](./database-providers.md) - Database-specific features

## Additional Resources

- [REST API Best Practices](https://restfulapi.net/)
- [GraphQL Documentation](https://graphql.org/learn/)
- [gRPC Documentation](https://grpc.io/docs/)
