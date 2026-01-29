# Database Providers

DDAP supports multiple database providers, each implemented as a separate NuGet package. This guide covers the features, configuration, and best practices for each provider.

## Overview

| Provider | Package | Database | ORM | Status |
|----------|---------|----------|-----|--------|
| SQL Server | `Ddap.Data.Dapper` | SQL Server | Dapper | ✅ Stable |
| MySQL | `Ddap.Data.Dapper` | MySQL | Dapper | ✅ Stable |
| PostgreSQL | `Ddap.Data.Dapper` | PostgreSQL | Dapper | ✅ Stable |
| Entity Framework | `Ddap.Data.EntityFramework` | Any EF-supported | EF Core | ✅ Stable |

## SQL Server Provider

### Installation

```bash
dotnet add package Ddap.Data.Dapper
dotnet add package Microsoft.Data.SqlClient
```

### Configuration

```csharp
using Ddap.Data.Dapper;
using Microsoft.Data.SqlClient;

builder.Services
    .AddDdap(options =>
    {
        options.ConnectionString = "Server=localhost;Database=MyDb;Integrated Security=true;";
    })
    .AddDapper(() => new SqlConnection(connectionString));
```

### Features

- ✅ Tables, views, and schemas
- ✅ Primary keys (simple and composite)
- ✅ Foreign key relationships
- ✅ Indexes (clustered and non-clustered)
- ✅ Unique constraints
- ✅ Identity columns
- ✅ Computed columns
- ✅ Default values
- ✅ SQL Server-specific data types

### Connection String Options

```csharp
// Windows Authentication
"Server=localhost;Database=MyDb;Integrated Security=true;"

// SQL Server Authentication
"Server=localhost;Database=MyDb;User Id=sa;Password=YourPassword;"

// Connection pooling (recommended)
"Server=localhost;Database=MyDb;Integrated Security=true;Min Pool Size=10;Max Pool Size=100;"

// Azure SQL Database
"Server=tcp:myserver.database.windows.net,1433;Database=MyDb;User ID=user@myserver;Password=YourPassword;Encrypt=True;"
```

### Data Type Mapping

| SQL Server Type | .NET Type |
|----------------|-----------|
| `int` | `Int32` |
| `bigint` | `Int64` |
| `varchar`, `nvarchar` | `String` |
| `datetime`, `datetime2` | `DateTime` |
| `bit` | `Boolean` |
| `decimal`, `numeric` | `Decimal` |
| `uniqueidentifier` | `Guid` |
| `varbinary` | `Byte[]` |

### Best Practices

1. **Use connection pooling** for better performance
2. **Enable MARS** (Multiple Active Result Sets) if needed:
   ```
   Server=localhost;Database=MyDb;MultipleActiveResultSets=true;
   ```
3. **Set appropriate timeouts**:
   ```
   Server=localhost;Database=MyDb;Connection Timeout=30;
   ```

### Known Limitations

- Temporal tables are treated as regular tables
- System tables are excluded by default
- SQL Server 2016+ recommended for full feature support

## MySQL Provider

### Installation

```bash
dotnet add package Ddap.Data.Dapper
dotnet add package MySqlConnector
```

### Configuration

```csharp
using Ddap.Data.Dapper;
using MySqlConnector;

builder.Services
    .AddDdap(options =>
    {
        options.ConnectionString = "Server=localhost;Database=MyDb;User=root;Password=secret;";
    })
    .AddDapper(() => new MySqlConnection(connectionString));
```

### Features

- ✅ Tables and schemas (databases)
- ✅ Primary keys (simple and composite)
- ✅ Foreign key relationships
- ✅ Indexes (B-tree, hash, fulltext)
- ✅ Unique constraints
- ✅ Auto-increment columns
- ✅ Default values
- ✅ MySQL-specific data types

### Connection String Options

```csharp
// Basic connection
"Server=localhost;Database=MyDb;User=root;Password=secret;"

// With port
"Server=localhost;Port=3306;Database=MyDb;User=root;Password=secret;"

// SSL connection
"Server=localhost;Database=MyDb;User=root;Password=secret;SslMode=Required;"

// Connection pooling
"Server=localhost;Database=MyDb;User=root;Password=secret;MinimumPoolSize=5;MaximumPoolSize=100;"

// Character set
"Server=localhost;Database=MyDb;User=root;Password=secret;CharSet=utf8mb4;"
```

### Data Type Mapping

| MySQL Type | .NET Type |
|-----------|-----------|
| `INT`, `INTEGER` | `Int32` |
| `BIGINT` | `Int64` |
| `VARCHAR`, `TEXT` | `String` |
| `DATETIME`, `TIMESTAMP` | `DateTime` |
| `BOOLEAN`, `TINYINT(1)` | `Boolean` |
| `DECIMAL`, `NUMERIC` | `Decimal` |
| `BINARY`, `BLOB` | `Byte[]` |
| `JSON` | `String` |

### Best Practices

1. **Use utf8mb4** for full Unicode support:
   ```sql
   CREATE DATABASE mydb CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
   ```
2. **Enable connection pooling** by default
3. **Set appropriate timeouts**:
   ```
   Server=localhost;Database=MyDb;User=root;Password=secret;ConnectionTimeout=30;
   ```
4. **Use InnoDB engine** for foreign key support

### Known Limitations

- Stored procedures are not automatically discovered
- Views are treated as read-only entities
- MySQL 5.7+ recommended for JSON support

## PostgreSQL Provider

### Installation

```bash
dotnet add package Ddap.Data.Dapper
dotnet add package Npgsql
```

### Configuration

```csharp
using Ddap.Data.Dapper;
using Npgsql;

builder.Services
    .AddDdap(options =>
    {
        options.ConnectionString = "Host=localhost;Database=MyDb;Username=postgres;Password=secret;";
    })
    .AddDapper(() => new NpgsqlConnection(connectionString));
```

### Features

- ✅ Tables and schemas
- ✅ Primary keys (simple and composite)
- ✅ Foreign key relationships
- ✅ Indexes (B-tree, hash, GiST, GIN, etc.)
- ✅ Unique constraints
- ✅ Serial/identity columns
- ✅ Default values
- ✅ PostgreSQL-specific types (UUID, JSON, arrays, etc.)
- ✅ Materialized views

### Connection String Options

```csharp
// Basic connection
"Host=localhost;Database=MyDb;Username=postgres;Password=secret;"

// With port
"Host=localhost;Port=5432;Database=MyDb;Username=postgres;Password=secret;"

// SSL connection
"Host=localhost;Database=MyDb;Username=postgres;Password=secret;SSL Mode=Require;"

// Connection pooling
"Host=localhost;Database=MyDb;Username=postgres;Password=secret;Minimum Pool Size=5;Maximum Pool Size=100;"

// Timeout settings
"Host=localhost;Database=MyDb;Username=postgres;Password=secret;Timeout=30;Command Timeout=30;"
```

### Data Type Mapping

| PostgreSQL Type | .NET Type |
|----------------|-----------|
| `integer`, `int4` | `Int32` |
| `bigint`, `int8` | `Int64` |
| `varchar`, `text` | `String` |
| `timestamp`, `timestamptz` | `DateTime` |
| `boolean` | `Boolean` |
| `decimal`, `numeric` | `Decimal` |
| `uuid` | `Guid` |
| `bytea` | `Byte[]` |
| `json`, `jsonb` | `String` |
| `array` | Depends on element type |

### Best Practices

1. **Use schemas** for organization:
   ```sql
   CREATE SCHEMA app;
   CREATE TABLE app.users (...);
   ```
2. **Enable connection pooling** (enabled by default)
3. **Use appropriate data types**:
   - `UUID` instead of `GUID` strings
   - `JSONB` instead of `JSON` for better performance
   - `TIMESTAMPTZ` for timestamps with timezone
4. **Create indexes** on frequently queried columns

### PostgreSQL-Specific Features

#### UUID Primary Keys

```csharp
// PostgreSQL automatically handles UUID generation
options.ConnectionString = "Host=localhost;Database=MyDb;Username=postgres;Password=secret;";
```

#### Array Types

PostgreSQL arrays are supported and mapped to .NET arrays:

```sql
CREATE TABLE items (
    id SERIAL PRIMARY KEY,
    tags TEXT[]
);
```

#### JSONB Support

JSONB columns are treated as strings in DDAP but can be extended:

```sql
CREATE TABLE documents (
    id SERIAL PRIMARY KEY,
    data JSONB
);
```

### Known Limitations

- Complex custom types require additional mapping
- Materialized views are treated as regular tables
- PostgreSQL 11+ recommended for full feature support

## Entity Framework Provider

### Installation

```bash
dotnet add package Ddap.Data.EntityFramework
```

### Configuration

The Entity Framework provider is database-agnostic and works with any database supported by EF Core:

```csharp
using Ddap.Data.EntityFramework;

builder.Services
    .AddDdap(options =>
    {
        options.ConnectionString = "Server=localhost;Database=MyDb;...";
    })
    .AddEntityFramework<TContext>();
```

### Features

- ✅ Database-agnostic (works with any EF Core provider)
- ✅ All standard EF Core features
- ✅ Code-first and database-first support
- ✅ Migrations support
- ✅ Complex relationships
- ✅ Inheritance (TPH, TPT, TPC)

### Supported Databases

- SQL Server
- MySQL/MariaDB
- PostgreSQL
- SQLite
- Oracle
- Cosmos DB
- In-Memory (for testing)

### Best Practices

1. **Register EF Core provider** before DDAP:
   ```csharp
   builder.Services.AddDbContext<MyDbContext>(options =>
       options.UseSqlServer(connectionString));

   builder.Services
       .AddDdap(options => { /* ... */ })
       .AddEntityFramework();
   ```

2. **Use lazy loading** cautiously (can impact performance)

3. **Configure change tracking**:
   ```csharp
   options.UseSqlServer(connectionString)
          .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
   ```

### When to Use EF vs Dapper

| Scenario | Recommendation |
|----------|---------------|
| Simple CRUD | Dapper (faster) |
| Complex relationships | Entity Framework |
| Existing EF Core app | Entity Framework |
| Maximum performance | Dapper |
| Code-first development | Entity Framework |
| Database-first development | Either |
| Multiple database types | Entity Framework |

## Provider Comparison

### Performance

**Benchmarks** (relative performance):

| Operation | SQL Server (Dapper) | MySQL (Dapper) | PostgreSQL (Dapper) | Entity Framework |
|-----------|---------------------|----------------|---------------------|------------------|
| Metadata Loading | 100% | 95% | 90% | 70% |
| Simple Query | 100% | 98% | 95% | 80% |
| Complex Query | 100% | 97% | 96% | 75% |
| Bulk Insert | 100% | 90% | 85% | 60% |

*Note: Benchmarks are approximate and vary by hardware and configuration*

### Memory Usage

- **Dapper Providers**: Lower memory footprint (~50MB baseline)
- **Entity Framework**: Higher memory footprint (~100MB baseline)

### Feature Support

| Feature | Dapper Providers | Entity Framework |
|---------|-----------------|------------------|
| Relationships | ✅ | ✅ |
| Indexes | ✅ | ✅ |
| Inheritance | ❌ | ✅ |
| Lazy Loading | ❌ | ✅ |
| Change Tracking | ❌ | ✅ |
| Migrations | ❌ | ✅ |

## Multiple Database Support

You can use multiple database providers in the same application:

```csharp
// Primary database (SQL Server)
var sqlConnectionString = builder.Configuration.GetConnectionString("SqlServer");
builder.Services
    .AddDdap("Primary", options =>
    {
        options.ConnectionString = sqlConnectionString;
    })
    .AddDapper(() => new SqlConnection(sqlConnectionString))
    .AddRest();

// Secondary database (PostgreSQL)
var pgConnectionString = builder.Configuration.GetConnectionString("PostgreSQL");
builder.Services
    .AddDdap("Secondary", options =>
    {
        options.ConnectionString = pgConnectionString;
    })
    .AddDapper(() => new NpgsqlConnection(pgConnectionString))
    .AddRest();
```

## Troubleshooting

### Connection Issues

**Problem**: Cannot connect to database

**Solutions**:
1. Verify connection string
2. Check firewall settings
3. Ensure database is running
4. Test with a simple connection:
   ```bash
   # SQL Server
   sqlcmd -S localhost -U sa -P YourPassword
   
   # MySQL
   mysql -h localhost -u root -p
   
   # PostgreSQL
   psql -h localhost -U postgres
   ```

### Metadata Loading Errors

**Problem**: Some tables are not discovered

**Solutions**:
1. Check table permissions
2. Verify schema access
3. Ensure tables have primary keys
4. Check for system tables being excluded

### Performance Issues

**Problem**: Slow metadata loading

**Solutions**:
1. Add indexes on system tables
2. Use connection pooling
3. Increase connection timeout
4. Consider caching metadata

## Next Steps

- [API Providers](./api-providers.md) - Learn about REST, gRPC, and GraphQL
- [Advanced Usage](./advanced.md) - Optimize performance and extend functionality
- [Troubleshooting](./troubleshooting.md) - Common issues and solutions

## Additional Resources

- [SQL Server Documentation](https://docs.microsoft.com/sql/)
- [MySQL Documentation](https://dev.mysql.com/doc/)
- [PostgreSQL Documentation](https://www.postgresql.org/docs/)
- [Entity Framework Core Documentation](https://docs.microsoft.com/ef/core/)
