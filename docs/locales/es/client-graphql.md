# DDAP GraphQL Client

The DDAP GraphQL Client provides a simple interface for executing GraphQL queries and mutations against DDAP APIs.

## Installation

```bash
dotnet add package Ddap.Client.GraphQL
```

## Configuration

### Basic Setup

```csharp
using Ddap.Client.Core;
using Ddap.Client.GraphQL;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();
services.AddDdapGraphQLClient(options =>
{
    options.BaseUrl = "https://api.example.com";
    options.Timeout = TimeSpan.FromSeconds(30);
    options.RetryCount = 3;
});

var serviceProvider = services.BuildServiceProvider();
var client = serviceProvider.GetRequiredService<DdapGraphQLClient>();
```

## Executing Queries

### Simple Query

```csharp
// Define response type
public class UsersData
{
    public User[] Users { get; set; } = Array.Empty<User>();
}

public class User
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}

// Execute query
var response = await client.QueryAsync<UsersData>(
    "{ users { id name email } }"
);

foreach (var user in response.Data?.Users ?? Array.Empty<User>())
{
    Console.WriteLine($"{user.Id}: {user.Name}");
}
```

### Query with Variables

```csharp
public class UserData
{
    public User User { get; set; } = new();
}

var query = @"
    query GetUser($id: Int!) {
        user(id: $id) {
            id
            name
            email
        }
    }
";

var variables = new { id = 1 };

var response = await client.QueryAsync<UserData>(query, variables);
Console.WriteLine($"User: {response.Data?.User.Name}");
```

### Complex Query with Nested Data

```csharp
public class UserWithOrdersData
{
    public UserWithOrders User { get; set; } = new();
}

public class UserWithOrders
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public Order[] Orders { get; set; } = Array.Empty<Order>();
}

public class Order
{
    public int Id { get; set; }
    public decimal Total { get; set; }
    public DateTime CreatedAt { get; set; }
}

var query = @"
    query GetUserWithOrders($userId: Int!) {
        user(id: $userId) {
            id
            name
            orders {
                id
                total
                createdAt
            }
        }
    }
";

var response = await client.QueryAsync<UserWithOrdersData>(
    query,
    new { userId = 1 }
);

var user = response.Data?.User;
Console.WriteLine($"User: {user?.Name}");
Console.WriteLine($"Orders: {user?.Orders.Length ?? 0}");
```

## Executing Mutations

### Create Mutation

```csharp
public class CreateUserData
{
    public User CreateUser { get; set; } = new();
}

var mutation = @"
    mutation CreateUser($name: String!, $email: String!) {
        createUser(name: $name, email: $email) {
            id
            name
            email
        }
    }
";

var variables = new
{
    name = "John Doe",
    email = "john@example.com"
};

var response = await client.MutationAsync<CreateUserData>(mutation, variables);
Console.WriteLine($"Created user with ID: {response.Data?.CreateUser.Id}");
```

### Update Mutation

```csharp
public class UpdateUserData
{
    public User UpdateUser { get; set; } = new();
}

var mutation = @"
    mutation UpdateUser($id: Int!, $name: String!) {
        updateUser(id: $id, name: $name) {
            id
            name
            email
        }
    }
";

var response = await client.MutationAsync<UpdateUserData>(
    mutation,
    new { id = 1, name = "Jane Doe" }
);
```

### Delete Mutation

```csharp
public class DeleteUserData
{
    public bool DeleteUser { get; set; }
}

var mutation = @"
    mutation DeleteUser($id: Int!) {
        deleteUser(id: $id)
    }
";

var response = await client.MutationAsync<DeleteUserData>(
    mutation,
    new { id = 1 }
);

if (response.Data?.DeleteUser == true)
{
    Console.WriteLine("User deleted successfully");
}
```

## Error Handling

### Checking for GraphQL Errors

```csharp
var response = await client.QueryAsync<UsersData>(
    "{ users { id name email } }"
);

if (response.Errors != null && response.Errors.Length > 0)
{
    foreach (var error in response.Errors)
    {
        Console.WriteLine($"GraphQL Error: {error.Message}");
        
        if (error.Locations != null)
        {
            foreach (var location in error.Locations)
            {
                Console.WriteLine($"  at line {location.Line}, column {location.Column}");
            }
        }
    }
}
else if (response.Data != null)
{
    // Process data
    foreach (var user in response.Data.Users)
    {
        Console.WriteLine($"{user.Id}: {user.Name}");
    }
}
```

### Handling HTTP Errors

```csharp
try
{
    var response = await client.QueryAsync<UsersData>(
        "{ users { id name email } }"
    );
}
catch (DdapApiException ex)
{
    Console.WriteLine($"API Error: {ex.Message} (Status: {ex.StatusCode})");
}
catch (DdapConnectionException ex)
{
    Console.WriteLine($"Connection Error: {ex.Message}");
}
```

## Query Builder Helpers

### Building Queries Programmatically

```csharp
// Simple helper for building queries
public static class GraphQLQueryBuilder
{
    public static string BuildQuery(string entityName, params string[] fields)
    {
        var fieldsList = string.Join(" ", fields);
        return $"{{ {entityName} {{ {fieldsList} }} }}";
    }
    
    public static string BuildQueryWithArgs(
        string entityName,
        string args,
        params string[] fields)
    {
        var fieldsList = string.Join(" ", fields);
        return $"{{ {entityName}({args}) {{ {fieldsList} }} }}";
    }
}

// Usage
var query = GraphQLQueryBuilder.BuildQuery("users", "id", "name", "email");
var response = await client.QueryAsync<UsersData>(query);

var queryWithArgs = GraphQLQueryBuilder.BuildQueryWithArgs(
    "user",
    "id: 1",
    "id", "name", "email"
);
var userResponse = await client.QueryAsync<UserData>(queryWithArgs);
```

## Fragments

### Using Fragments

```csharp
var query = @"
    fragment UserFields on User {
        id
        name
        email
    }
    
    query GetUsers {
        users {
            ...UserFields
        }
    }
";

var response = await client.QueryAsync<UsersData>(query);
```

## Pagination

### Cursor-based Pagination

```csharp
public class PagedUsersData
{
    public UserConnection Users { get; set; } = new();
}

public class UserConnection
{
    public User[] Edges { get; set; } = Array.Empty<User>();
    public PageInfo PageInfo { get; set; } = new();
}

public class PageInfo
{
    public bool HasNextPage { get; set; }
    public string? EndCursor { get; set; }
}

var query = @"
    query GetUsers($after: String, $first: Int!) {
        users(after: $after, first: $first) {
            edges {
                id
                name
                email
            }
            pageInfo {
                hasNextPage
                endCursor
            }
        }
    }
";

string? cursor = null;
var allUsers = new List<User>();

do
{
    var response = await client.QueryAsync<PagedUsersData>(
        query,
        new { after = cursor, first = 10 }
    );
    
    if (response.Data?.Users.Edges != null)
    {
        allUsers.AddRange(response.Data.Users.Edges);
    }
    
    var pageInfo = response.Data?.Users.PageInfo;
    if (pageInfo?.HasNextPage == true)
    {
        cursor = pageInfo.EndCursor;
    }
    else
    {
        break;
    }
} while (true);

Console.WriteLine($"Loaded {allUsers.Count} users");
```

## Cancellation Support

```csharp
using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));

try
{
    var response = await client.QueryAsync<UsersData>(
        "{ users { id name email } }",
        cancellationToken: cts.Token
    );
}
catch (OperationCanceledException)
{
    Console.WriteLine("Query cancelled");
}
```

## Best Practices

1. **Define Response Types**: Always create strongly-typed response models
2. **Use Variables**: Parameterize queries with variables instead of string interpolation
3. **Handle Errors**: Check both GraphQL errors and HTTP exceptions
4. **Request Only Needed Fields**: Minimize data transfer by requesting only required fields
5. **Use Fragments**: Share field definitions across queries with fragments
6. **Implement Pagination**: Use cursor-based pagination for large datasets

## Future: LINQ Support

The GraphQL client is designed to integrate with Linq2GraphQL.Client for LINQ-based query building:

```csharp
// Future LINQ support (via Linq2GraphQL.Client)
var users = await client
    .Query<User>()
    .Where(u => u.Name.Contains("John"))
    .Select(u => new { u.Id, u.Name })
    .ToListAsync();
```

This functionality will be available when Linq2GraphQL.Client integration is complete.

## Next Steps

- [REST Client Documentation](client-rest.md)
- [gRPC Client Documentation](client-grpc.md)
- [Getting Started Guide](client-getting-started.md)
