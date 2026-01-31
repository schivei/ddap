# DDAP REST Client

The DDAP REST Client provides a simple, strongly-typed HTTP client for consuming RESTful DDAP APIs.

## Installation

```bash
dotnet add package Ddap.Client.Rest
```

## Configuration

### Basic Setup

```csharp
using Ddap.Client.Core;
using Ddap.Client.Rest;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();
services.AddDdapRestClient(options =>
{
    options.BaseUrl = "https://api.example.com";
    options.Timeout = TimeSpan.FromSeconds(30);
    options.RetryCount = 3;
    options.UseExponentialBackoff = true;
});

var serviceProvider = services.BuildServiceProvider();
var client = serviceProvider.GetRequiredService<DdapRestClient>();
```

## CRUD Operations

### Get All Entities

```csharp
// Get all users
var users = await client.GetAsync<User>("/api/users");

foreach (var user in users)
{
    Console.WriteLine($"{user.Id}: {user.Name}");
}
```

### Get Single Entity by ID

```csharp
// Get user by ID
var user = await client.GetByIdAsync<User>("/api/users", 1);

if (user != null)
{
    Console.WriteLine($"Found: {user.Name}");
}
```

### Create Entity

```csharp
// Create a new user
var newUser = new User
{
    Name = "John Doe",
    Email = "john@example.com"
};

var created = await client.CreateAsync("/api/users", newUser);
Console.WriteLine($"Created user with ID: {created.Id}");
```

### Update Entity

```csharp
// Update existing user
var user = await client.GetByIdAsync<User>("/api/users", 1);
user.Name = "Jane Doe";

var updated = await client.UpdateAsync("/api/users", 1, user);
Console.WriteLine($"Updated: {updated.Name}");
```

### Delete Entity

```csharp
// Delete user
var deleted = await client.DeleteAsync("/api/users", 1);

if (deleted)
{
    Console.WriteLine("User deleted successfully");
}
```

## Resilience Policies

The REST client automatically includes retry policies with exponential backoff:

```csharp
services.AddDdapRestClient(options =>
{
    options.RetryCount = 5; // Retry up to 5 times
    options.RetryDelay = TimeSpan.FromSeconds(2); // Initial delay
    options.UseExponentialBackoff = true; // Double delay each retry
});
```

## Error Handling

### Handling API Errors

```csharp
try
{
    var user = await client.GetByIdAsync<User>("/api/users", 999);
}
catch (DdapApiException ex) when (ex.StatusCode == 404)
{
    Console.WriteLine("User not found");
}
catch (DdapApiException ex)
{
    Console.WriteLine($"API Error: {ex.Message} (Status: {ex.StatusCode})");
}
```

### Handling Connection Errors

```csharp
try
{
    var users = await client.GetAsync<User>("/api/users");
}
catch (DdapConnectionException ex)
{
    Console.WriteLine($"Connection failed: {ex.Message}");
}
```

## Custom Entity Types

Define your entity types:

```csharp
public class User
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Stock { get; set; }
}
```

## Async/Await Patterns

All operations are fully async:

```csharp
// Sequential operations
var user = await client.GetByIdAsync<User>("/api/users", 1);
var updated = await client.UpdateAsync("/api/users", 1, user);

// Parallel operations
var usersTask = client.GetAsync<User>("/api/users");
var productsTask = client.GetAsync<Product>("/api/products");

await Task.WhenAll(usersTask, productsTask);

var users = await usersTask;
var products = await productsTask;
```

## Cancellation Support

All operations support cancellation tokens:

```csharp
using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));

try
{
    var users = await client.GetAsync<User>("/api/users", cts.Token);
}
catch (OperationCanceledException)
{
    Console.WriteLine("Operation cancelled");
}
```

## Best Practices

1. **Reuse Client Instances**: Register the client as a singleton in DI
2. **Use Cancellation Tokens**: Always pass cancellation tokens for long operations
3. **Handle Specific Exceptions**: Catch and handle specific exception types
4. **Configure Timeouts**: Set appropriate timeouts based on your API
5. **Use Retry Policies**: Enable exponential backoff for transient failures

## Next Steps

- [GraphQL Client Documentation](client-graphql.md)
- [gRPC Client Documentation](client-grpc.md)
- [Getting Started Guide](client-getting-started.md)
