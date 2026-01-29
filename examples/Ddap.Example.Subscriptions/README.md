# DDAP Real-Time Subscriptions Example

This example demonstrates how to use DDAP with real-time subscriptions using the `Ddap.Subscriptions` package.

## Features Demonstrated

- SignalR integration for real-time updates
- Entity change notifications
- GraphQL subscriptions
- WebSocket connections
- Filtered subscriptions

## Prerequisites

- .NET 10 SDK
- SQL Server (or another supported database)

## Installation

```bash
dotnet add package Ddap.Core
dotnet add package Ddap.Subscriptions
dotnet add package Ddap.Data.Dapper
dotnet add package Microsoft.Data.SqlClient
dotnet add package Ddap.Rest
dotnet add package Ddap.GraphQL
```

## Basic Configuration

### Program.cs

```csharp
using Ddap.Core;
using Ddap.Subscriptions;
using Ddap.Data.Dapper;
using Ddap.Rest;
using Ddap.GraphQL;
using Microsoft.Data.SqlClient;

var builder = WebApplication.CreateBuilder(args);

// Configure DDAP with subscriptions
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services
    .AddDdap(options =>
    {
        options.ConnectionString = connectionString;
    })
    .AddDapper(() => new SqlConnection(connectionString))
    .AddRest()
    .AddGraphQL()
    .AddSubscriptions(subscriptionOptions =>
    {
        // Enable real-time notifications for specific entities
        subscriptionOptions.EnableFor("Products");
        subscriptionOptions.EnableFor("Orders");
        subscriptionOptions.EnableFor("Inventory");
        
        // Configure notification settings
        subscriptionOptions.NotifyOnCreate = true;
        subscriptionOptions.NotifyOnUpdate = true;
        subscriptionOptions.NotifyOnDelete = true;
    });

var app = builder.Build();

app.UseRouting();
app.MapControllers();
app.MapGraphQL("/graphql");
app.MapHub<DdapNotificationHub>("/notifications"); // SignalR hub

app.Run();
```

## SignalR Client Example (JavaScript)

### HTML + JavaScript

```html
<!DOCTYPE html>
<html>
<head>
    <title>DDAP Real-Time Updates</title>
    <script src="https://cdn.jsdelivr.net/npm/@microsoft/signalr@latest/dist/browser/signalr.min.js"></script>
</head>
<body>
    <h1>Real-Time Product Updates</h1>
    <div id="updates"></div>
    
    <script>
        const connection = new signalR.HubConnectionBuilder()
            .withUrl("/notifications")
            .build();
        
        // Subscribe to product updates
        connection.on("EntityCreated", (entityName, data) => {
            console.log(`New ${entityName} created:`, data);
            addUpdate(`✅ Created: ${entityName} - ${JSON.stringify(data)}`);
        });
        
        connection.on("EntityUpdated", (entityName, data) => {
            console.log(`${entityName} updated:`, data);
            addUpdate(`🔄 Updated: ${entityName} - ${JSON.stringify(data)}`);
        });
        
        connection.on("EntityDeleted", (entityName, id) => {
            console.log(`${entityName} deleted:`, id);
            addUpdate(`❌ Deleted: ${entityName} - ID: ${id}`);
        });
        
        connection.start()
            .then(() => {
                console.log("Connected to DDAP notifications");
                // Subscribe to specific entities
                connection.invoke("SubscribeToEntity", "Products");
            })
            .catch(err => console.error(err));
        
        function addUpdate(message) {
            const div = document.getElementById("updates");
            const p = document.createElement("p");
            p.textContent = `${new Date().toLocaleTimeString()}: ${message}`;
            div.insertBefore(p, div.firstChild);
        }
    </script>
</body>
</html>
```

## C# Client Example

```csharp
using Microsoft.AspNetCore.SignalR.Client;

var connection = new HubConnectionBuilder()
    .WithUrl("http://localhost:5000/notifications")
    .Build();

// Handle entity created events
connection.On<string, object>("EntityCreated", (entityName, data) =>
{
    Console.WriteLine($"New {entityName} created: {data}");
});

// Handle entity updated events
connection.On<string, object>("EntityUpdated", (entityName, data) =>
{
    Console.WriteLine($"{entityName} updated: {data}");
});

// Handle entity deleted events
connection.On<string, string>("EntityDeleted", (entityName, id) =>
{
    Console.WriteLine($"{entityName} deleted: {id}");
});

await connection.StartAsync();

// Subscribe to specific entities
await connection.InvokeAsync("SubscribeToEntity", "Products");
await connection.InvokeAsync("SubscribeToEntity", "Orders");

Console.WriteLine("Listening for updates. Press any key to exit.");
Console.ReadKey();

await connection.DisposeAsync();
```

## GraphQL Subscriptions

### Schema

```graphql
subscription {
  entityCreated(entityName: "Products") {
    id
    name
    price
  }
  
  entityUpdated(entityName: "Products") {
    id
    name
    price
  }
  
  entityDeleted(entityName: "Products") {
    id
  }
}
```

### GraphQL Client Example

```javascript
import { createClient } from 'graphql-ws';

const client = createClient({
  url: 'ws://localhost:5000/graphql',
});

// Subscribe to product updates
client.subscribe(
  {
    query: `
      subscription {
        entityUpdated(entityName: "Products") {
          id
          name
          price
        }
      }
    `,
  },
  {
    next: (data) => {
      console.log('Product updated:', data);
    },
    error: (err) => {
      console.error('Subscription error:', err);
    },
    complete: () => {
      console.log('Subscription completed');
    },
  }
);
```

## Filtered Subscriptions

Subscribe only to specific records:

```csharp
subscriptionOptions.AddFilter("Products", (entity, user) =>
{
    // Only notify about products in user's department
    var userDepartment = user.FindFirst("department")?.Value;
    return entity["Department"]?.ToString() == userDepartment;
});
```

## Advanced Features

### Custom Event Handlers

```csharp
public class ProductNotificationHandler : IEntityNotificationHandler
{
    public async Task OnEntityCreatedAsync(string entityName, object entity)
    {
        // Send email notification
        // Log to analytics
        // Trigger webhooks
    }
    
    public async Task OnEntityUpdatedAsync(string entityName, object entity)
    {
        // Custom logic
    }
    
    public async Task OnEntityDeletedAsync(string entityName, string id)
    {
        // Custom logic
    }
}

// Register handler
builder.Services.AddSingleton<IEntityNotificationHandler, ProductNotificationHandler>();
```

### Batching Updates

```csharp
subscriptionOptions.EnableBatching(batchOptions =>
{
    batchOptions.MaxBatchSize = 10;
    batchOptions.MaxWaitTimeMs = 1000;
});
```

## Testing

### Trigger Updates via REST API

```bash
# Create a product (triggers notification)
curl -X POST http://localhost:5000/api/entity/Products \
  -H "Content-Type: application/json" \
  -d '{"name":"New Product","price":29.99}'

# Update a product (triggers notification)
curl -X PUT http://localhost:5000/api/entity/Products/1 \
  -H "Content-Type: application/json" \
  -d '{"name":"Updated Product","price":39.99}'

# Delete a product (triggers notification)
curl -X DELETE http://localhost:5000/api/entity/Products/1
```

## Performance Considerations

1. **Scale with Redis backplane** for multi-server deployments
2. **Use filters** to reduce notification volume
3. **Implement connection limits** to prevent resource exhaustion
4. **Monitor SignalR metrics** for connection health

## Learn More

- [SignalR Documentation](https://docs.microsoft.com/aspnet/core/signalr/)
- [GraphQL Subscriptions](https://graphql.org/blog/subscriptions-in-graphql-and-relay/)
- [DDAP Documentation](../../docs/get-started.md)
