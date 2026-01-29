# Ddap.Grpc

gRPC API provider for DDAP with high-performance binary protocol support.

## Installation

```bash
dotnet add package Ddap.Grpc
```

## Quick Start

```csharp
using Ddap.Core;
using Ddap.Data.Dapper;
using Ddap.Grpc;
using Microsoft.Data.SqlClient;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDdap()
    .AddDapper(() => new SqlConnection(
        builder.Configuration.GetConnectionString("DefaultConnection")
    ))
    .AddGrpc();

var app = builder.Build();
app.MapGrpcService<EntityService>();
app.Run();
```

## Features

- ✅ Automatic gRPC service generation
- ✅ HTTP/2 binary protocol
- ✅ High-performance RPC
- ✅ Streaming support (server, client, bidirectional)
- ✅ Protocol Buffers (.proto)
- ✅ Fully extensible via partial classes

## Generated Services

```protobuf
service EntityService {
  rpc GetEntities(Empty) returns (EntityListResponse);
  rpc GetEntity(GetEntityRequest) returns (EntityResponse);
  rpc GetEntityMetadata(GetEntityRequest) returns (EntityMetadataResponse);
}
```

## Client Example

```csharp
using Grpc.Net.Client;

var channel = GrpcChannel.ForAddress("https://localhost:5001");
var client = new EntityService.EntityServiceClient(channel);

var response = await client.GetEntitiesAsync(new Empty());
foreach (var entity in response.Entities)
{
    Console.WriteLine($"Entity: {entity.Name}");
}
```

## Extensibility

Extend via partial classes:

```csharp
using Grpc.Core;

namespace Ddap.Grpc;

public partial class EntityService
{
    public override async Task<CustomResponse> CustomMethod(
        CustomRequest request,
        ServerCallContext context)
    {
        // Your custom logic
        return new CustomResponse { Message = "Done" };
    }
}
```

## Documentation

Full documentation: **https://schivei.github.io/ddap/api-providers**

## Related Packages

- `Ddap.Core` - Core abstractions
- `Ddap.Data.Dapper` - Dapper provider
- `Ddap.Data.EntityFramework` - EF Core provider
- `Ddap.Rest` - REST APIs
- `Ddap.GraphQL` - GraphQL APIs

## License

MIT - see [LICENSE](../../LICENSE)
