# Ddap.Data.EntityFramework

Entity Framework Core data provider for DDAP (Dynamic Data API Provider).

## Overview

This package provides Entity Framework Core integration for DDAP, allowing you to use any EF Core database provider with DDAP's automatic API generation capabilities.

## Features

- ✅ **Database Agnostic**: Works with ANY EF Core provider (SQL Server, MySQL, PostgreSQL, SQLite, InMemory, etc.)
- ✅ **Developer Control**: Configure your DbContext externally with full control
- ✅ **Automatic Discovery**: Discovers entities from DbSet<> properties
- ✅ **Metadata Integration**: Uses EF Core model metadata for schema discovery
- ✅ **Flexible Filtering**: Filter entities by type, name, or custom predicates
- ✅ **Pooled Context**: Uses `AddPooledDbContextFactory` for optimal performance
- ✅ **HotChocolate Ready**: Integrates seamlessly with GraphQL via HotChocolate

## Installation

```bash
dotnet add package Ddap.Data.EntityFramework
```

You'll also need your preferred EF Core database provider:

```bash
# Choose your database provider
dotnet add package Microsoft.EntityFrameworkCore.SqlServer  # SQL Server
dotnet add package Pomelo.EntityFrameworkCore.MySql         # MySQL
dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL    # PostgreSQL
dotnet add package Microsoft.EntityFrameworkCore.Sqlite     # SQLite
dotnet add package Microsoft.EntityFrameworkCore.InMemory   # In-Memory (testing)
```

## Usage

### Basic Configuration

```csharp
using Ddap.Core;
using Ddap.Data.EntityFramework;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 1. Register your DbContext with full control
builder.Services.AddPooledDbContextFactory<MyDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// 2. Add DDAP with EntityFramework provider
builder.Services.AddDdap(options => { })
    .AddEntityFramework<MyDbContext>()
    .AddGraphQL()
    .AddRest();

var app = builder.Build();
app.MapGraphQL();
app.MapControllers();
app.Run();
```

### MySQL with Retry Logic

```csharp
builder.Services.AddPooledDbContextFactory<MyDbContext>(options =>
{
    options.UseMySQL(connectionString, mysql =>
    {
        mysql.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(10),
            errorNumbersToAdd: null
        );
    });
});

builder.Services.AddDdap(options => { })
    .AddEntityFramework<MyDbContext>();
```

### PostgreSQL with Connection Pooling

```csharp
builder.Services.AddPooledDbContextFactory<MyDbContext>(options =>
{
    options.UseNpgsql(connectionString, npgsql =>
    {
        npgsql.EnableRetryOnFailure(5);
        npgsql.CommandTimeout(30);
    });
});

builder.Services.AddDdap(options => { })
    .AddEntityFramework<MyDbContext>();
```

### Entity Filtering

Filter which entities to include in your API:

```csharp
builder.Services.AddDdap(options =>
{
    // Global entity filter
    options.EntityFilter = type => !type.Name.StartsWith("Internal");
    
    // Table filtering
    options.ExcludeTables = new List<string> { "AuditLogs", "SystemConfig" };
})
.AddEntityFramework<MyDbContext>(ef =>
{
    // Provider-specific entity filter
    ef.EntityFilter = type => !type.Name.StartsWith("Audit");
});
```

### Multiple DbContexts

DDAP supports using multiple DbContexts (though typically one is sufficient):

```csharp
builder.Services.AddPooledDbContextFactory<CatalogDbContext>(options =>
    options.UseSqlServer(catalogConnectionString));

builder.Services.AddPooledDbContextFactory<IdentityDbContext>(options =>
    options.UseSqlServer(identityConnectionString));

builder.Services.AddDdap(options => { })
    .AddEntityFramework<CatalogDbContext>()
    .AddGraphQL()
    .AddRest();
```

### SQLite (Development)

```csharp
builder.Services.AddPooledDbContextFactory<MyDbContext>(options =>
{
    options.UseSqlite("Data Source=app.db");
});

builder.Services.AddDdap(options => { })
    .AddEntityFramework<MyDbContext>();
```

### InMemory (Testing)

```csharp
builder.Services.AddPooledDbContextFactory<MyDbContext>(options =>
{
    options.UseInMemoryDatabase("TestDb");
});

builder.Services.AddDdap(options => { })
    .AddEntityFramework<MyDbContext>();
```

## Configuration Options

### EntityFrameworkProviderOptions

```csharp
.AddEntityFramework<MyDbContext>(ef =>
{
    // Filter entity types
    ef.EntityFilter = type => !type.Namespace?.Contains("Internal") ?? true;
    
    // Discover entities from DbSet properties (default: true)
    ef.DiscoverFromDbSets = true;
    
    // Use EF Core model metadata (default: true)
    ef.UseModelMetadata = true;
});
```

## How It Works

1. **Entity Discovery**: Scans your DbContext for `DbSet<>` properties
2. **Metadata Extraction**: Uses EF Core's model metadata to extract:
   - Entity names and schemas
   - Properties and their types
   - Primary keys and indexes
   - Relationships (foreign keys, navigations)
3. **API Generation**: Generates REST and GraphQL endpoints automatically
4. **Query Optimization**: Uses pooled DbContext factories for performance

## Why Use EntityFramework Provider?

### vs. Dapper Provider

| Feature | EntityFramework | Dapper |
|---------|----------------|--------|
| Schema Discovery | From EF Model | From Database |
| Type Safety | Compile-time | Runtime |
| Migrations | Built-in | Manual |
| Relationship Handling | Automatic | Manual |
| Performance | Good (with pooling) | Excellent |
| Learning Curve | Moderate | Low |

**Use EntityFramework when:**
- You already use EF Core in your project
- You want type-safe models and migrations
- You need automatic relationship handling
- You prefer code-first or model-first approaches

**Use Dapper when:**
- You want maximum performance
- You prefer database-first approach
- You want lightweight ORM
- You need fine-grained SQL control

## Advanced Scenarios

### Custom DbContext Configuration

```csharp
builder.Services.AddPooledDbContextFactory<MyDbContext>(options =>
{
    options.UseSqlServer(connectionString, sqlServer =>
    {
        sqlServer.EnableRetryOnFailure();
        sqlServer.CommandTimeout(60);
        sqlServer.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
    });
    
    options.EnableSensitiveDataLogging(isDevelopment);
    options.EnableDetailedErrors(isDevelopment);
    options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
});
```

### Integration with HotChocolate GraphQL

DDAP automatically integrates with HotChocolate for GraphQL support:

```csharp
builder.Services.AddDdap(options => { })
    .AddEntityFramework<MyDbContext>()
    .AddGraphQL(graphql =>
    {
        graphql
            .AddFiltering()
            .AddSorting()
            .AddProjections()
            .ModifyRequestOptions(opt => opt.IncludeExceptionDetails = isDevelopment);
    });
```

## Best Practices

1. **Always use pooled context factories** for production
2. **Configure retry logic** for transient failures
3. **Filter sensitive entities** (audit logs, internal tables)
4. **Use proper connection strings** with pooling enabled
5. **Enable query splitting** for complex queries
6. **Disable tracking** for read-only scenarios

## Troubleshooting

### No entities loaded

- Ensure your DbContext has `DbSet<>` properties
- Check entity and table filters
- Verify DbContext is properly configured

### Performance issues

- Use `AddPooledDbContextFactory` instead of `AddDbContext`
- Enable connection pooling in your connection string
- Consider using Dapper provider for read-heavy scenarios

### Migration issues

- EntityFramework provider only reads metadata
- Migrations are handled by EF Core normally
- DDAP doesn't interfere with EF Core migrations

## License

This project is licensed under the MIT License.
