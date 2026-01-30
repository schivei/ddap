# Ddap.Subscriptions

Real-time subscription support for DDAP using SignalR and GraphQL subscriptions for live data updates.

## Installation

```bash
dotnet add package Ddap.Subscriptions
```

## What's Included

This package provides real-time data subscription features:

- **SignalR Integration** - WebSocket-based real-time communication
- **GraphQL Subscriptions** - Subscribe to data changes via GraphQL
- **Live Data Updates** - Automatic notifications on data changes
- **Pub/Sub Pattern** - Event-driven architecture support

## Quick Start

```csharp
using Ddap.Subscriptions;

var builder = WebApplication.CreateBuilder(args);

// Add DDAP with subscriptions
builder.Services.AddDdap(options =>
{
    options.ConnectionString = builder.Configuration.GetConnectionString("DefaultConnection");
})
.AddDdapSubscriptions();

// Add SignalR
builder.Services.AddSignalR();

var app = builder.Build();
app.MapHub<DdapSubscriptionHub>("/ddap-subscriptions");
app.Run();
```

## Client Usage

```javascript
// JavaScript/TypeScript client
const connection = new signalR.HubConnectionBuilder()
    .withUrl("/ddap-subscriptions")
    .build();

connection.on("EntityUpdated", (entityName, data) => {
    console.log(`${entityName} updated:`, data);
});

await connection.start();
await connection.invoke("SubscribeToEntity", "Users");
```

## GraphQL Subscription

```graphql
subscription OnUserUpdated {
  userUpdated {
    id
    name
    email
    updatedAt
  }
}
```

## Documentation

Full documentation: **https://schivei.github.io/ddap**

## Related Packages

- `Ddap.Core` - Core abstractions and infrastructure
- `Ddap.GraphQL` - GraphQL API provider
- `Ddap.Rest` - REST API provider

## License

MIT - see [LICENSE](../../LICENSE)
