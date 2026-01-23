# DDAP Aspire Example

This example demonstrates how to use DDAP with .NET Aspire for rapid API development with automatic schema refresh.

## Project Structure

- **AppHost** - Aspire orchestrator project
- **ApiService** - DDAP API service

## Features Demonstrated

- .NET Aspire integration
- Automatic database connection configuration
- REST, GraphQL, and gRPC APIs
- Auto-refresh for schema changes during development
- Service discovery and orchestration

## Prerequisites

- .NET 10 SDK
- Docker (for SQL Server container)
- .NET Aspire workload

Install Aspire workload:
```bash
dotnet workload install aspire
```

## Running the Example

### Using Visual Studio 2022
1. Open the solution
2. Set `AppHost` as the startup project
3. Press F5 to run

### Using Command Line
```bash
cd examples/Ddap.Example.Aspire/AppHost
dotnet run
```

This will:
1. Start the Aspire dashboard
2. Launch SQL Server in a container
3. Start the DDAP API service
4. Display the dashboard URL (typically http://localhost:15000)

## Accessing the APIs

Once running, you can access:

- **Aspire Dashboard**: http://localhost:15000
- **REST API**: Check dashboard for the service URL
- **GraphQL**: {service-url}/graphql

## Configuration Example

### AppHost (Program.cs)

```csharp
using Aspire.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

// Add SQL Server with database
var sqlserver = builder.AddSqlServer("sqlserver")
                      .WithDataVolume();
                      
var db = sqlserver.AddDatabase("catalogdb");

// Add DDAP API service
var api = builder.AddProject<Projects.Ddap_Example_Aspire_ApiService>("ddap-api")
                 .WithReference(db);

builder.Build().Run();
```

### API Service (Program.cs)

```csharp
using Ddap.Core;
using Ddap.Data.Dapper.SqlServer;
using Ddap.Rest;
using Ddap.GraphQL;

var builder = WebApplication.CreateBuilder(args);

// Add Aspire service defaults
builder.AddServiceDefaults();

// Configure DDAP
builder.Services
    .AddDdap(options =>
    {
        options.ConnectionString = builder.Configuration.GetConnectionString("catalogdb");
    })
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

## Auto-Refresh Feature

For development, you can enable auto-refresh to automatically detect schema changes:

```csharp
builder.Services
    .AddDdap(options =>
    {
        options.ConnectionString = builder.Configuration.GetConnectionString("catalogdb");
        options.EnableAutoRefresh = true;
        options.RefreshIntervalSeconds = 30;
    })
    .AddSqlServerDapper()
    .AddRest()
    .AddGraphQL();
```

## Observability

View in the Aspire dashboard:
- Service health status
- Logs from all services
- Distributed tracing
- Metrics and performance data
- Database connection status

## Learn More

- [DDAP Aspire Package Documentation](../../src/Ddap.Aspire/README.md)
- [.NET Aspire Documentation](https://learn.microsoft.com/dotnet/aspire/)
- [DDAP Documentation](../../docs/get-started.md)
