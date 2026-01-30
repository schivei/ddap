# DDAP Project Templates

DDAP provides project templates to quickly scaffold new APIs with your preferred database and API providers. This guide covers installing, using, and customizing DDAP templates.

## Overview

The `Ddap.Templates` package provides `dotnet new` templates for creating DDAP-powered APIs with:

- **Pre-configured** database providers (Dapper or Entity Framework)
- **Multiple database** support (SQL Server, MySQL, PostgreSQL, SQLite)
- **Multi-protocol** APIs (REST, GraphQL, gRPC)
- **Optional features** (Authentication, Subscriptions, .NET Aspire)
- **Best practices** configuration out of the box

### Available Templates

| Template | Description |
|----------|-------------|
| `ddap-api` | Full-featured DDAP API with configurable providers |

## Installation

Install the DDAP templates package globally:

```bash
dotnet new install Ddap.Templates
```

Verify installation:

```bash
dotnet new list ddap
```

You should see:

```
Template Name    Short Name    Language    Tags
---------------  ------------  ----------  ------------------------
DDAP API         ddap-api      [C#]        Web/API/Database/DDAP
```

### Updating Templates

To update to the latest version:

```bash
dotnet new install Ddap.Templates --force
```

### Uninstalling Templates

```bash
dotnet new uninstall Ddap.Templates
```

## Interactive Mode

The simplest way to create a new DDAP project is using interactive mode. Run the template without arguments:

```bash
dotnet new ddap-api
```

You'll be prompted for:

1. **Project name** - Name of your API project
2. **Database provider** - Choose `dapper` or `entityframework`
3. **Database type** - Choose `sqlserver`, `mysql`, `postgresql`, or `sqlite`
4. **API providers** - Select which APIs to enable (REST, GraphQL, gRPC)
5. **Connection string** - Your database connection string
6. **Include authentication** - Add JWT authentication support
7. **Include subscriptions** - Add real-time subscription support
8. **Use .NET Aspire** - Include Aspire orchestration

The template will generate a complete project with all configurations set up.

## CLI Mode

For automation or CI/CD, use command-line options to skip interactive prompts:

```bash
dotnet new ddap-api --name MyApi [options]
```

### Basic Syntax

```bash
dotnet new ddap-api \
  --name <project-name> \
  --database-provider <dapper|entityframework> \
  --database-type <sqlserver|mysql|postgresql|sqlite> \
  --api-providers <rest,graphql,grpc> \
  [--connection-string <connection-string>] \
  [--include-auth true|false] \
  [--include-subscriptions true|false] \
  [--use-aspire true|false]
```

## Options Reference

### `--name`

**Type:** `string`  
**Required:** Yes  
**Description:** Name of the project and generated files

```bash
dotnet new ddap-api --name ProductCatalogApi
```

Creates:
- Project file: `ProductCatalogApi.csproj`
- Namespace: `ProductCatalogApi`
- Folder: `ProductCatalogApi/`

---

### `--database-provider`

**Type:** `string`  
**Values:** `dapper` | `entityframework`  
**Default:** `dapper`  
**Description:** ORM/data access provider to use

**Dapper:**
- Lightweight, high-performance
- Works with any `IDbConnection`
- Direct SQL control
- Smaller package footprint

**Entity Framework:**
- Full ORM with change tracking
- LINQ queries
- Migrations support
- Database-agnostic

```bash
# Dapper (default)
dotnet new ddap-api --name MyApi --database-provider dapper

# Entity Framework
dotnet new ddap-api --name MyApi --database-provider entityframework
```

---

### `--database-type`

**Type:** `string`  
**Values:** `sqlserver` | `mysql` | `postgresql` | `sqlite`  
**Default:** `sqlserver`  
**Description:** Target database system

Automatically configures:
- Appropriate NuGet packages
- Connection factory
- Database-specific connection strings

```bash
dotnet new ddap-api --name MyApi --database-type mysql
```

---

### `--api-providers`

**Type:** `string` (comma-separated)  
**Values:** `rest`, `graphql`, `grpc`  
**Default:** `rest`  
**Description:** API protocols to enable

```bash
# Single provider
dotnet new ddap-api --name MyApi --api-providers rest

# Multiple providers
dotnet new ddap-api --name MyApi --api-providers "rest,graphql,grpc"
```

Adds:
- `Ddap.Rest` for REST APIs
- `Ddap.GraphQL` for GraphQL (includes HotChocolate)
- `Ddap.Grpc` for gRPC services

---

### `--connection-string`

**Type:** `string`  
**Optional:** Yes  
**Description:** Database connection string (added to appsettings.json)

```bash
dotnet new ddap-api --name MyApi \
  --connection-string "Server=localhost;Database=MyDb;User Id=sa;Password=YourStrong!Pass;"
```

**Note:** Connection strings with special characters should be quoted. For sensitive environments, use User Secrets instead (see User Secrets section).

---

### `--include-auth`

**Type:** `boolean`  
**Default:** `false`  
**Description:** Include JWT authentication and authorization

```bash
dotnet new ddap-api --name MyApi --include-auth true
```

Adds:
- `Ddap.Auth` package
- JWT token validation
- Authentication middleware
- Authorization policies
- `/auth/login` and `/auth/token` endpoints

---

### `--include-subscriptions`

**Type:** `boolean`  
**Default:** `false`  
**Description:** Include real-time subscriptions support

```bash
dotnet new ddap-api --name MyApi --include-subscriptions true
```

Adds:
- `Ddap.Subscriptions` package
- WebSocket support
- Real-time change notifications
- Subscription infrastructure

---

### `--use-aspire`

**Type:** `boolean`  
**Default:** `false`  
**Description:** Include .NET Aspire orchestration and observability

```bash
dotnet new ddap-api --name MyApi --use-aspire true
```

Adds:
- `Ddap.Aspire` package
- Aspire AppHost project
- Service defaults (logging, metrics, tracing)
- Orchestration configuration

Creates additional projects:
- `<ProjectName>.AppHost` - Aspire orchestration
- `<ProjectName>.ServiceDefaults` - Shared configuration

## Examples

### Example 1: Basic REST API with SQL Server

Create a simple REST API with SQL Server and Dapper:

```bash
dotnet new ddap-api \
  --name ProductApi \
  --database-provider dapper \
  --database-type sqlserver \
  --api-providers rest
```

**Generated structure:**
```
ProductApi/
├── ProductApi.csproj
├── Program.cs
├── appsettings.json
└── appsettings.Development.json
```

**To run:**
```bash
cd ProductApi
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost;Database=Products;Integrated Security=true;"
dotnet run
```

**Test:**
```bash
curl http://localhost:5000/api/entity
```

---

### Example 2: Full-Stack with GraphQL, gRPC, and MySQL

Create a comprehensive API with multiple protocols:

```bash
dotnet new ddap-api \
  --name EcommerceApi \
  --database-provider dapper \
  --database-type mysql \
  --api-providers "rest,graphql,grpc" \
  --include-auth true
```

**Includes:**
- REST endpoints at `/api/*`
- GraphQL playground at `/graphql`
- gRPC services
- JWT authentication

**To run:**
```bash
cd EcommerceApi
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost;Database=ecommerce;User=root;Password=secret;"
dotnet user-secrets set "Jwt:SecretKey" "your-256-bit-secret-key-here-32chars"
dotnet run
```

**Test REST:**
```bash
curl http://localhost:5000/api/entity
```

**Test GraphQL:**
```bash
curl -X POST http://localhost:5000/graphql \
  -H "Content-Type: application/json" \
  -d '{"query": "{ entities { name propertyCount } }"}'
```

Or open `http://localhost:5000/graphql` in a browser for the GraphQL playground.

---

### Example 3: Microservice with PostgreSQL and Aspire

Create a cloud-native microservice with observability:

```bash
dotnet new ddap-api \
  --name InventoryService \
  --database-provider entityframework \
  --database-type postgresql \
  --api-providers "rest,grpc" \
  --use-aspire true
```

**Generated structure:**
```
InventoryService/
├── InventoryService/
│   ├── InventoryService.csproj
│   ├── Program.cs
│   └── appsettings.json
├── InventoryService.AppHost/
│   ├── InventoryService.AppHost.csproj
│   └── Program.cs
└── InventoryService.ServiceDefaults/
    ├── InventoryService.ServiceDefaults.csproj
    └── Extensions.cs
```

**To run with Aspire:**
```bash
cd InventoryService.AppHost
dotnet run
```

Aspire dashboard opens automatically at `http://localhost:15888` with:
- Distributed tracing
- Metrics and telemetry
- Log aggregation
- Resource management

---

### Example 4: SQLite API for Development

Create a lightweight API perfect for local development:

```bash
dotnet new ddap-api \
  --name DevApi \
  --database-provider dapper \
  --database-type sqlite \
  --api-providers rest \
  --connection-string "Data Source=dev.db"
```

**Benefits:**
- No external database required
- File-based database (`dev.db`)
- Perfect for prototyping
- Easy to share

**To run:**
```bash
cd DevApi
dotnet run
```

The SQLite database file will be created automatically on first run.

## User Secrets

For security, never store connection strings in `appsettings.json` in production. Use User Secrets for development:

### Initialize User Secrets

```bash
cd YourProject
dotnet user-secrets init
```

This adds a `<UserSecretsId>` to your `.csproj` file.

### Set Connection String

```bash
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost;Database=MyDb;User Id=sa;Password=MyPassword;"
```

### Set JWT Secret (if using authentication)

```bash
dotnet user-secrets set "Jwt:SecretKey" "your-super-secret-256-bit-key-that-is-32-characters-long"
dotnet user-secrets set "Jwt:Issuer" "https://myapi.com"
dotnet user-secrets set "Jwt:Audience" "myapi-client"
```

### List All Secrets

```bash
dotnet user-secrets list
```

### Remove a Secret

```bash
dotnet user-secrets remove "ConnectionStrings:DefaultConnection"
```

### Clear All Secrets

```bash
dotnet user-secrets clear
```

### Using in Production

For production, use:
- **Azure Key Vault** - Azure App Service
- **AWS Secrets Manager** - AWS services
- **Environment Variables** - Docker/Kubernetes
- **Configuration Providers** - Custom solutions

## Customization

Generated projects are designed to be customized. Here are common modifications:

### Adding Custom Endpoints

Extend generated controllers with partial classes:

```csharp
// Create a new file: Controllers/EntityController.Custom.cs
namespace YourProject.Controllers;

public partial class EntityController
{
    [HttpGet("custom/stats")]
    public IActionResult GetStats()
    {
        // Your custom logic
        return Ok(new { TotalEntities = 42 });
    }
}
```

### Customizing GraphQL Schema

Modify `Program.cs` to configure HotChocolate:

```csharp
builder.Services
    .AddDdap(/* ... */)
    .AddGraphQL(graphql =>
    {
        graphql
            .AddFiltering()
            .AddSorting()
            .AddProjections()
            .AddMaxExecutionDepthRule(10)
            .AddQueryType<CustomQueries>()
            .AddMutationType<CustomMutations>();
    });
```

### Adding Middleware

Insert custom middleware in `Program.cs`:

```csharp
var app = builder.Build();

app.UseHttpsRedirection();
app.UseCors(/* your CORS policy */);
app.UseAuthentication();
app.UseAuthorization();

// Your custom middleware
app.Use(async (context, next) =>
{
    // Pre-processing
    await next();
    // Post-processing
});

app.MapControllers();
app.Run();
```

### Configuring Auto-Reload

Enable automatic schema reloading:

```csharp
builder.Services.AddDdap(options =>
{
    options.ConnectionString = connectionString;
    options.AutoReload = new AutoReloadOptions
    {
        Enabled = true,
        IdleTimeout = TimeSpan.FromMinutes(5),
        Strategy = ReloadStrategy.InvalidateAndRebuild,
        Behavior = ReloadBehavior.ServeOldSchema,
        Detection = ChangeDetection.CheckHash
    };
});
```

## Troubleshooting

### Template Not Found

**Problem:** `dotnet new ddap-api` says template not found

**Solution:**
```bash
dotnet new install Ddap.Templates
dotnet new list ddap  # Verify installation
```

---

### Connection String Error

**Problem:** Database connection fails on startup

**Solutions:**

1. **Check connection string format:**
   - SQL Server: `Server=localhost;Database=MyDb;Integrated Security=true;`
   - MySQL: `Server=localhost;Database=MyDb;User=root;Password=secret;`
   - PostgreSQL: `Host=localhost;Database=MyDb;Username=postgres;Password=secret;`
   - SQLite: `Data Source=mydb.db`

2. **Use User Secrets for development:**
   ```bash
   dotnet user-secrets set "ConnectionStrings:DefaultConnection" "your-connection-string"
   ```

3. **Verify database server is running:**
   ```bash
   # SQL Server
   docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=YourStrong!Pass" -p 1433:1433 -d mcr.microsoft.com/mssql/server:2022-latest
   
   # MySQL
   docker run -e MYSQL_ROOT_PASSWORD=secret -p 3306:3306 -d mysql:8
   
   # PostgreSQL
   docker run -e POSTGRES_PASSWORD=secret -p 5432:5432 -d postgres:16
   ```

---

### Missing Package Error

**Problem:** `Package Ddap.Rest not found` or similar

**Solution:**
```bash
# Ensure you have the correct package feed
dotnet nuget list source
dotnet restore
```

---

### GraphQL Playground Not Loading

**Problem:** `/graphql` endpoint returns 404

**Solution:**

1. Ensure GraphQL is enabled:
   ```bash
   dotnet new ddap-api --name MyApi --api-providers "rest,graphql"
   ```

2. Check `Program.cs` includes:
   ```csharp
   app.MapGraphQL("/graphql");
   ```

3. Navigate to the correct URL (usually `http://localhost:5000/graphql`)

---

### Port Already in Use

**Problem:** `Address already in use` error

**Solution:**

1. **Change port in `appsettings.json`:**
   ```json
   {
     "Urls": "http://localhost:5001"
   }
   ```

2. **Or use command line:**
   ```bash
   dotnet run --urls "http://localhost:5001"
   ```

---

### Aspire Dashboard Won't Start

**Problem:** Aspire AppHost fails to launch

**Solution:**

1. **Install .NET Aspire workload:**
   ```bash
   dotnet workload install aspire
   ```

2. **Update Aspire packages:**
   ```bash
   dotnet add package Aspire.Hosting --version 9.0.0
   ```

3. **Run from AppHost project:**
   ```bash
   cd YourProject.AppHost
   dotnet run
   ```

## Next Steps

After creating your project:

1. **Read the generated README** - Project-specific instructions
2. **[Database Providers](./database-providers.md)** - Deep dive into Dapper vs EF
3. **[API Providers](./api-providers.md)** - REST, GraphQL, gRPC details
4. **[Auto-Reload](./auto-reload.md)** - Configure schema refresh
5. **[Advanced Usage](./advanced.md)** - Extensibility and customization
6. **[Troubleshooting](./troubleshooting.md)** - Common issues and solutions

## Additional Resources

- **[DDAP Documentation](https://schivei.github.io/ddap)** - Full documentation
- **[GitHub Repository](https://github.com/schivei/ddap)** - Source code and examples
- **[Example Projects](https://github.com/schivei/ddap/tree/main/examples)** - Working examples
- **[NuGet Packages](https://www.nuget.org/packages?q=Ddap)** - All DDAP packages

---

**Happy building with DDAP! 🚀**
