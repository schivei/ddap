# DDAP Client Libraries - Getting Started

This guide will help you get started with DDAP client libraries for consuming DDAP APIs.

## Overview

DDAP provides client libraries for three different API types:

- **REST Client**: HTTP-based REST API consumer
- **gRPC Client**: High-performance gRPC client
- **GraphQL Client**: GraphQL query and mutation support

All clients share a common core that provides:
- HTTP client factory support
- Retry and resilience policies
- Consistent configuration
- Exception handling

## Installation

Install the client libraries you need:

```bash
# Core library (required)
dotnet add package Ddap.Client.Core

# REST client
dotnet add package Ddap.Client.Rest

# gRPC client
dotnet add package Ddap.Client.Grpc

# GraphQL client
dotnet add package Ddap.Client.GraphQL
```

## Quick Start

### REST Client

```csharp
using Ddap.Client.Core;
using Ddap.Client.Rest;
using Microsoft.Extensions.DependencyInjection;

// Configure services
var services = new ServiceCollection();
services.AddDdapRestClient(options =>
{
    options.BaseUrl = "https://api.example.com";
    options.Timeout = TimeSpan.FromSeconds(30);
    options.RetryCount = 3;
});

var serviceProvider = services.BuildServiceProvider();
var client = serviceProvider.GetRequiredService<DdapRestClient>();

// Use the client
var users = await client.GetAsync<User>("/api/users");
var user = await client.GetByIdAsync<User>("/api/users", 1);
var newUser = await client.CreateAsync("/api/users", new User { Name = "John" });
var updated = await client.UpdateAsync("/api/users", 1, user);
var deleted = await client.DeleteAsync("/api/users", 1);
```

### gRPC Client

```csharp
using Ddap.Client.Core;
using Ddap.Client.Grpc;

// Configure services
services.AddDdapGrpcClient(options =>
{
    options.BaseUrl = "https://api.example.com";
});

var client = serviceProvider.GetRequiredService<DdapGrpcClient>();

// Create a gRPC service client
var userService = client.CreateClient<UserServiceClient>();
```

### GraphQL Client

```csharp
using Ddap.Client.Core;
using Ddap.Client.GraphQL;

// Configure services
services.AddDdapGraphQLClient(options =>
{
    options.BaseUrl = "https://api.example.com";
});

var client = serviceProvider.GetRequiredService<DdapGraphQLClient>();

// Execute queries
var response = await client.QueryAsync<UsersData>(
    "{ users { id name email } }"
);

// Execute mutations
var mutationResponse = await client.MutationAsync<CreateUserData>(
    "mutation($name: String!) { createUser(name: $name) { id name } }",
    new { name = "John" }
);
```

## Configuration Options

All clients support the following configuration options:

- `BaseUrl`: The base URL of the API
- `Timeout`: Request timeout (default: 30 seconds)
- `RetryCount`: Number of retry attempts (default: 3)
- `RetryDelay`: Delay between retries (default: 1 second)
- `UseExponentialBackoff`: Use exponential backoff for retries (default: true)

## Exception Handling

All clients throw consistent exceptions:

- `DdapClientException`: Base exception for all client errors
- `DdapConnectionException`: Connection failures
- `DdapApiException`: API request failures (includes status code)

Example:

```csharp
try
{
    var user = await client.GetByIdAsync<User>("/api/users", 1);
}
catch (DdapApiException ex)
{
    Console.WriteLine($"API error: {ex.Message}, Status: {ex.StatusCode}");
}
catch (DdapConnectionException ex)
{
    Console.WriteLine($"Connection error: {ex.Message}");
}
```

## Testing Connection

All clients implement the `IDdapClient` interface with a `TestConnectionAsync` method:

```csharp
var isConnected = await client.TestConnectionAsync();
if (isConnected)
{
    Console.WriteLine("Successfully connected to API");
}
```

## Next Steps

- [REST Client Documentation](client-rest.md)
- [gRPC Client Documentation](client-grpc.md)
- [GraphQL Client Documentation](client-graphql.md)
