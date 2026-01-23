# Ddap.Aspire - .NET Aspire Integration

This library provides seamless integration between DDAP (Dynamic Data API Provider) and .NET Aspire for rapid agile development.

## Features

- 🚀 **Quick Setup**: Automatic API generation in Aspire applications
- 🔄 **Auto-Refresh**: Automatically detect and reload database schema changes
- 🎯 **Service Discovery**: Automatic connection string resolution from Aspire
- 📦 **Multi-Provider**: Support for REST, gRPC, and GraphQL in one service
- ⚡ **Development Speed**: Perfect for agile development and rapid prototyping

## Installation

```bash
dotnet add package Ddap.Aspire
dotnet add package Ddap.Core
dotnet add package Ddap.Data.Dapper.SqlServer  # or MySQL/PostgreSQL
dotnet add package Ddap.Rest
dotnet add package Ddap.GraphQL
```

## Usage

### AppHost Project

Configure DDAP in your Aspire AppHost:

```csharp
using Ddap.Aspire;

var builder = DistributedApplication.CreateBuilder(args);

// Add database
var db = builder.AddSqlServer("sql")
               .AddDatabase("mydb");

// Add DDAP API service with automatic database discovery
builder.AddDdapApi("api")
       .WithReference(db)
       .WithRestApi()           // Enable REST (JSON/XML/YAML)
       .WithGraphQL()           // Enable GraphQL
       .WithGrpc()              // Enable gRPC
       .WithAutoRefresh(30);    // Auto-reload schema every 30 seconds

builder.Build().Run();
```

### API Service Project

Configure DDAP in your service:

```csharp
using Ddap.Aspire;
using Ddap.Data.Dapper.SqlServer;
using Ddap.Rest;
using Ddap.GraphQL;

var builder = WebApplication.CreateBuilder(args);

// Add Aspire service defaults
builder.AddServiceDefaults();

// Add DDAP with Aspire integration
builder.Services.AddDdapForAspire(builder.Configuration)
       .AddSqlServerDapper()
       .AddRest()
       .AddGraphQL();

var app = builder.Build();

app.MapDefaultEndpoints();
app.UseRouting();
app.MapControllers();
app.MapGraphQL("/graphql");

app.Run();
```

### With Auto-Refresh (Agile Development)

For development environments where the schema changes frequently:

```csharp
builder.Services.AddDdapForAspireWithAutoRefresh(
    builder.Configuration,
    refreshIntervalSeconds: 30  // Check for schema changes every 30 seconds
)
.AddSqlServerDapper()
.AddRest()
.AddGraphQL();
```

## Complete Example

### 1. Create Aspire App

```bash
dotnet new aspire -n MyDdapApp
cd MyDdapApp
```

### 2. AppHost/Program.cs

```csharp
using Ddap.Aspire;

var builder = DistributedApplication.CreateBuilder(args);

// Add SQL Server
var sql = builder.AddSqlServer("sql");
var db = sql.AddDatabase("catalogdb");

// Add DDAP API
var api = builder.AddDdapApi("ddap-api")
                 .WithReference(db)
                 .WithRestApi(5000)
                 .WithGraphQL()
                 .WithAutoRefresh(30);

// Add frontend that references the API
builder.AddProject<Projects.MyDdapApp_Frontend>("frontend")
       .WithReference(api);

builder.Build().Run();
```

### 3. API Service

```csharp
var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services
    .AddDdapForAspireWithAutoRefresh(builder.Configuration, 30)
    .AddSqlServerDapper()
    .AddRest()
    .AddGraphQL()
    .AddGrpc();

var app = builder.Build();

app.MapDefaultEndpoints();
app.UseRouting();
app.MapControllers();
app.MapGraphQL();

app.Run();
```

## Benefits for Agile Development

### Rapid Prototyping
- Create API instantly from your database schema
- No need to write controllers or resolvers manually
- Focus on database design, get APIs automatically

### Schema Evolution
- Auto-refresh detects database changes
- APIs update automatically
- No service restart needed (with auto-refresh)

### Multi-Protocol Support
- One database, three API types (REST, gRPC, GraphQL)
- Client can choose their preferred protocol
- Easy to add new protocols

### Aspire Integration
- Automatic service discovery
- Built-in observability
- Seamless connection string management
- Dashboard shows all endpoints

## Configuration Options

### DdapResource Methods

- `WithRestApi(port?)` - Enable REST API with optional port
- `WithGraphQL(path?)` - Enable GraphQL with custom path
- `WithGrpc(port?)` - Enable gRPC with optional port
- `WithAutoRefresh(intervalSeconds)` - Enable automatic schema refresh

### Service Methods

- `AddDdapForAspire(configuration)` - Basic Aspire integration
- `AddDdapForAspireWithAutoRefresh(configuration, intervalSeconds)` - With auto-refresh

## Dashboard Features

When running in Aspire, the dashboard shows:

- **Endpoints**: REST, GraphQL, gRPC URLs
- **Health**: Service health status
- **Logs**: Real-time logs including schema refresh
- **Traces**: Distributed tracing for requests
- **Metrics**: Request counts, latencies

## Best Practices

### Development
```csharp
// Use auto-refresh in development
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddDdapForAspireWithAutoRefresh(
        builder.Configuration, 
        refreshIntervalSeconds: 10  // Faster refresh in dev
    );
}
else
{
    builder.Services.AddDdapForAspire(builder.Configuration);
}
```

### Production
- Disable auto-refresh in production
- Use manual schema refresh triggers
- Cache entity configurations

### Database Providers

Choose based on your database:

```csharp
// SQL Server
.AddSqlServerDapper()

// MySQL
.AddMySqlDapper()

// PostgreSQL
.AddPostgreSqlDapper()

// Entity Framework (any provider)
.AddEntityFramework()
```

## Troubleshooting

### Connection String Not Found
Ensure database is referenced:
```csharp
builder.AddDdapApi("api").WithReference(database);
```

### Auto-Refresh Not Working
Check logs for errors. Ensure data provider is registered:
```csharp
.AddSqlServerDapper()  // Don't forget the data provider!
```

### Port Conflicts
Specify custom ports:
```csharp
.WithRestApi(5001)
.WithGrpc(5002)
```

## Learn More

- [.NET Aspire Documentation](https://learn.microsoft.com/dotnet/aspire/)
- [DDAP Documentation](../README.md)
- [Examples](../examples/)

## Contributing

Contributions are welcome! Please see the main [CONTRIBUTING.md](../../CONTRIBUTING.md) file.

## License

This project is licensed under the MIT License - see the [LICENSE](../../LICENSE) file for details.
