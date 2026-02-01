# DDAP gRPC Client

The DDAP gRPC Client provides high-performance, strongly-typed access to DDAP gRPC services.

## Installation

```bash
dotnet add package Ddap.Client.Grpc
```

## Configuration

### Basic Setup

```csharp
using Ddap.Client.Core;
using Ddap.Client.Grpc;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();
services.AddDdapGrpcClient(options =>
{
    options.BaseUrl = "https://api.example.com";
    options.Timeout = TimeSpan.FromSeconds(30);
});

var serviceProvider = services.BuildServiceProvider();
var client = serviceProvider.GetRequiredService<DdapGrpcClient>();
```

## Creating Service Clients

The gRPC client manages channel creation and provides factory methods for service clients:

```csharp
// Create a service client
var userService = client.CreateClient<UserServiceClient>();

// Use the service client
var response = await userService.GetUserAsync(new GetUserRequest { Id = 1 });
Console.WriteLine($"User: {response.Name}");
```

## Channel Management

The client automatically manages gRPC channels:

```csharp
// Get the underlying channel (for advanced scenarios)
var channel = client.GetChannel();

// The channel is reused for all service clients
var userService = client.CreateClient<UserServiceClient>();
var orderService = client.CreateClient<OrderServiceClient>();
```

## Streaming Operations

### Server Streaming

```csharp
var request = new GetUsersRequest();
var call = userService.GetUsers(request);

await foreach (var user in call.ResponseStream.ReadAllAsync())
{
    Console.WriteLine($"User: {user.Name}");
}
```

### Client Streaming

```csharp
var call = userService.CreateUsers();

foreach (var user in users)
{
    await call.RequestStream.WriteAsync(new CreateUserRequest
    {
        Name = user.Name,
        Email = user.Email
    });
}

await call.RequestStream.CompleteAsync();
var response = await call;
```

### Bidirectional Streaming

```csharp
var call = userService.Chat();

// Start reading responses
var readTask = Task.Run(async () =>
{
    await foreach (var message in call.ResponseStream.ReadAllAsync())
    {
        Console.WriteLine($"Received: {message.Text}");
    }
});

// Send messages
await call.RequestStream.WriteAsync(new ChatMessage { Text = "Hello" });
await call.RequestStream.WriteAsync(new ChatMessage { Text = "World" });
await call.RequestStream.CompleteAsync();

await readTask;
```

## Deadlines and Cancellation

### Setting Deadlines

```csharp
var options = new CallOptions(deadline: DateTime.UtcNow.AddSeconds(5));
var response = await userService.GetUserAsync(
    new GetUserRequest { Id = 1 },
    options
);
```

### Using Cancellation Tokens

```csharp
using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));

try
{
    var response = await userService.GetUserAsync(
        new GetUserRequest { Id = 1 },
        cancellationToken: cts.Token
    );
}
catch (OperationCanceledException)
{
    Console.WriteLine("Operation cancelled");
}
```

## Error Handling

### gRPC Status Codes

```csharp
using Grpc.Core;

try
{
    var response = await userService.GetUserAsync(new GetUserRequest { Id = 999 });
}
catch (RpcException ex) when (ex.StatusCode == StatusCode.NotFound)
{
    Console.WriteLine("User not found");
}
catch (RpcException ex)
{
    Console.WriteLine($"gRPC error: {ex.Status.Detail}");
}
```

## Metadata and Headers

### Adding Metadata

```csharp
var metadata = new Metadata
{
    { "authorization", "Bearer token123" },
    { "x-request-id", Guid.NewGuid().ToString() }
};

var options = new CallOptions(headers: metadata);
var response = await userService.GetUserAsync(
    new GetUserRequest { Id = 1 },
    options
);
```

### Reading Response Headers

```csharp
var call = userService.GetUserAsync(new GetUserRequest { Id = 1 });
var headers = await call.ResponseHeadersAsync;

foreach (var entry in headers)
{
    Console.WriteLine($"{entry.Key}: {entry.Value}");
}

var response = await call;
```

## Performance Optimization

### Channel Options

The client configures channels with optimized settings:

- Max message size: 16 MB
- Connection reuse
- HTTP/2 multiplexing

### Connection Pooling

The client reuses the same channel for all service clients:

```csharp
// Both services share the same channel
var userService = client.CreateClient<UserServiceClient>();
var orderService = client.CreateClient<OrderServiceClient>();
```

## Disposal

The client implements `IDisposable` for proper resource cleanup:

```csharp
using var client = new DdapGrpcClient(options);
var userService = client.CreateClient<UserServiceClient>();
// Use the service...
// Channel is automatically disposed
```

## Best Practices

1. **Reuse Channels**: Use a single client instance per application
2. **Use Streaming**: Leverage streaming for large datasets
3. **Set Deadlines**: Always specify deadlines for operations
4. **Handle Status Codes**: Catch and handle specific RPC status codes
5. **Dispose Properly**: Ensure the client is disposed to release resources

## Proto File Example

Define your gRPC services in `.proto` files:

```protobuf
syntax = "proto3";

package ddap.users;

service UserService {
    rpc GetUser (GetUserRequest) returns (User);
    rpc GetUsers (GetUsersRequest) returns (stream User);
    rpc CreateUser (CreateUserRequest) returns (User);
    rpc UpdateUser (UpdateUserRequest) returns (User);
    rpc DeleteUser (DeleteUserRequest) returns (DeleteUserResponse);
}

message User {
    int32 id = 1;
    string name = 2;
    string email = 3;
}

message GetUserRequest {
    int32 id = 1;
}

message GetUsersRequest {
    int32 page = 1;
    int32 page_size = 2;
}

message CreateUserRequest {
    string name = 1;
    string email = 2;
}

message UpdateUserRequest {
    int32 id = 1;
    string name = 2;
    string email = 3;
}

message DeleteUserRequest {
    int32 id = 1;
}

message DeleteUserResponse {
    bool success = 1;
}
```

## Next Steps

- [REST Client Documentation](client-rest.md)
- [GraphQL Client Documentation](client-graphql.md)
- [Getting Started Guide](client-getting-started.md)
