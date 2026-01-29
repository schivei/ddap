# Ddap.Data.Dapper

Generic Dapper data provider for DDAP (Dynamic Data API Provider).

## Overview

This package provides a unified Dapper integration for DDAP that works with **ANY** database that provides an `IDbConnection` implementation. No database-specific dependencies required!

## Features

- ✅ **Database Agnostic**: Works with ANY IDbConnection (SQL Server, MySQL, PostgreSQL, SQLite, Oracle, etc.)
- ✅ **Zero Dependencies**: No database-specific NuGet packages required in this library
- ✅ **Developer Control**: You provide the connection, we do the rest
- ✅ **Lightweight**: Minimal ORM with maximum performance
- ✅ **Flexible**: Custom queries for database-specific features
- ✅ **Simple**: Just pass a connection factory function
- ✅ **Performant**: Uses Dapper's micro-ORM for maximum speed

## Installation

```bash
# Install DDAP Dapper provider
dotnet add package Ddap.Data.Dapper

# Install YOUR chosen database driver
dotnet add package Microsoft.Data.SqlClient        # SQL Server
dotnet add package MySqlConnector                  # MySQL
dotnet add package Npgsql                          # PostgreSQL
dotnet add package Microsoft.Data.Sqlite           # SQLite
dotnet add package Oracle.ManagedDataAccess.Core   # Oracle
# ... or any other ADO.NET provider
```

## Usage

### Basic Configuration

The beauty of the unified Dapper provider is its simplicity - just provide a connection factory:

#### SQL Server
```csharp
using Ddap.Core;
using Ddap.Data.Dapper;
using Microsoft.Data.SqlClient;

builder.Services.AddDdap(options => { })
    .AddDapper(() => new SqlConnection(connectionString))
    .AddGraphQL()
    .AddRest();
```

#### MySQL
```csharp
using MySqlConnector;

builder.Services.AddDdap(options => { })
    .AddDapper(() => new MySqlConnection(connectionString))
    .AddGraphQL()
    .AddRest();
```

#### PostgreSQL
```csharp
using Npgsql;

builder.Services.AddDdap(options => { })
    .AddDapper(() => new NpgsqlConnection(connectionString))
    .AddGraphQL()
    .AddRest();
```

#### SQLite
```csharp
using Microsoft.Data.Sqlite;

builder.Services.AddDdap(options => { })
    .AddDapper(() => new SqliteConnection("Data Source=app.db"))
    .AddGraphQL()
    .AddRest();
```

### Convenience Methods

For common databases, we provide convenience methods with optimized metadata queries:

#### SQL Server (Optimized)
```csharp
using Microsoft.Data.SqlClient;

builder.Services.AddDdap(options => { })
    .AddDapperSqlServer(() => new SqlConnection(connectionString));
```

#### MySQL (Optimized)
```csharp
using MySqlConnector;

builder.Services.AddDdap(options => { })
    .AddDapperMySql(() => new MySqlConnection(connectionString));
```

#### PostgreSQL (Optimized)
```csharp
using Npgsql;

builder.Services.AddDdap(options => { })
    .AddDapperPostgreSql(() => new NpgsqlConnection(connectionString));
```

These convenience methods include database-specific queries for indexes and foreign keys.

### Custom Configuration

Customize the provider with custom metadata queries:

```csharp
builder.Services.AddDdap(options => { })
    .AddDapper(() => new SqlConnection(connectionString), dapper =>
    {
        dapper.ProviderName = "My Custom Provider";
        
        // Custom query for loading tables
        dapper.CustomTableQuery = @"
            SELECT schema_name as Schema, table_name as TableName
            FROM my_custom_tables_view";
        
        // Custom query for loading columns
        dapper.CustomColumnQuery = @"
            SELECT 
                column_name as ColumnName,
                data_type as DataType,
                is_nullable as IsNullable,
                ...
            FROM my_custom_columns_view
            WHERE schema_name = @Schema AND table_name = @TableName";
        
        // Custom index query (optional)
        dapper.CustomIndexQuery = @"...";
        
        // Custom foreign key query (optional)
        dapper.CustomForeignKeyQuery = @"...";
    });
```

### Connection String from Configuration

```csharp
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDdap(options => { })
    .AddDapper(() => new SqlConnection(connectionString));
```

### Dynamic Connection Selection

```csharp
builder.Services.AddDdap(options => { })
    .AddDapper(() =>
    {
        var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        var connStr = env == "Development" 
            ? devConnectionString 
            : prodConnectionString;
        return new SqlConnection(connStr);
    });
```

## How It Works

1. **Connection Factory**: You provide a function that creates database connections
2. **Metadata Discovery**: Uses `INFORMATION_SCHEMA` views (standard SQL) to discover:
   - Tables and schemas
   - Columns and their types
   - Primary keys
3. **Optional Metadata**: For database-specific features (indexes, foreign keys):
   - Provide custom queries for your database
   - Or use our convenience methods (SQL Server, MySQL, PostgreSQL)
   - Or skip them (they're optional)
4. **Type Mapping**: Automatically maps database types to CLR types
5. **API Generation**: DDAP generates REST and GraphQL endpoints

## Supported Databases

Theoretically, **ANY** database with an ADO.NET provider. Tested with:

- ✅ Microsoft SQL Server
- ✅ MySQL / MariaDB
- ✅ PostgreSQL
- ✅ SQLite
- ⚠️ Oracle (should work, provide custom queries)
- ⚠️ IBM DB2 (should work, provide custom queries)
- ⚠️ Firebird (should work, provide custom queries)
- ⚠️ Any other ADO.NET provider

## Migration from Old Packages

### Before (Old Design)
```csharp
// Had to install database-specific package
dotnet add package Ddap.Data.Dapper.SqlServer
dotnet add package Ddap.Data.Dapper.MySQL
dotnet add package Ddap.Data.Dapper.PostgreSQL

// Different extension methods per database
builder.Services.AddDdap(...)
    .AddSqlServerDapper()      // SQL Server specific
    .AddMySqlDapper()          // MySQL specific
    .AddPostgreSqlDapper()     // PostgreSQL specific
```

### After (New Design)
```csharp
// Single unified package
dotnet add package Ddap.Data.Dapper

// Your database driver (you choose)
dotnet add package Microsoft.Data.SqlClient

// Single unified method
builder.Services.AddDdap(...)
    .AddDapper(() => new SqlConnection(connectionString))

// Or use convenience methods if you want optimized queries
builder.Services.AddDdap(...)
    .AddDapperSqlServer(() => new SqlConnection(connectionString))
```

## Configuration Options

### DapperProviderOptions

```csharp
.AddDapper(() => new SqlConnection(connectionString), options =>
{
    // Display name for the provider
    options.ProviderName = "My Database";
    
    // Use standard INFORMATION_SCHEMA (default: true)
    options.UseInformationSchema = true;
    
    // Custom queries for metadata (optional)
    options.CustomTableQuery = "...";
    options.CustomColumnQuery = "...";
    options.CustomIndexQuery = "...";
    options.CustomForeignKeyQuery = "...";
});
```

## Advanced Scenarios

### Connection Pooling

Most ADO.NET providers support connection pooling via connection strings:

```csharp
// SQL Server
var connectionString = "Server=...;Database=...;Pooling=true;Min Pool Size=5;Max Pool Size=100";

// MySQL
var connectionString = "Server=...;Database=...;Pooling=true;MinimumPoolSize=5;MaximumPoolSize=100";

// PostgreSQL
var connectionString = "Host=...;Database=...;Pooling=true;MinPoolSize=5;MaxPoolSize=100";
```

### Read-Only Connections

```csharp
builder.Services.AddDdap(options => { })
    .AddDapper(() =>
    {
        var connection = new SqlConnection(connectionString);
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = "SET TRANSACTION READ ONLY";
        command.ExecuteNonQuery();
        return connection;
    });
```

### Multiple Databases

```csharp
// Not directly supported - use separate DDAP instances or EntityFramework provider
// For multiple databases, consider using multiple microservices
```

### Schema Filtering

```csharp
builder.Services.AddDdap(options =>
{
    // Filter schemas
    options.IncludeSchemas = new List<string> { "dbo", "app" };
    options.ExcludeSchemas = new List<string> { "sys", "audit" };
    
    // Filter tables
    options.ExcludeTables = new List<string> { "AuditLog", "__MigrationHistory" };
    
    // Custom table filter
    options.TableFilter = tableName => !tableName.StartsWith("tmp_");
})
.AddDapper(() => new SqlConnection(connectionString));
```

## Why Use Dapper Provider?

### vs. EntityFramework Provider

| Feature | Dapper | EntityFramework |
|---------|--------|-----------------|
| Performance | Excellent | Good |
| Memory Usage | Low | Moderate |
| Simplicity | High | Moderate |
| Type Safety | Runtime | Compile-time |
| Migrations | Manual | Built-in |
| Relationship Handling | Manual | Automatic |
| Database Support | Any IDbConnection | EF Core providers |

**Use Dapper when:**
- ✅ You want maximum performance
- ✅ You prefer database-first approach
- ✅ You want lightweight ORM
- ✅ You need fine-grained control
- ✅ You're working with legacy databases

**Use EntityFramework when:**
- ✅ You already use EF Core
- ✅ You want type-safe models
- ✅ You need migrations
- ✅ You prefer code-first approach

## Type Mapping

The provider automatically maps common SQL types to CLR types:

| SQL Type | CLR Type |
|----------|----------|
| int, integer, int4 | int |
| bigint, int8, long | long |
| smallint, int2 | short |
| tinyint | byte |
| int unsigned | uint |
| bigint unsigned | ulong |
| smallint unsigned | ushort |
| tinyint unsigned | byte |
| bit, bool, boolean | bool |
| decimal, numeric, money | decimal |
| float, double | double |
| real, float4 | float |
| datetime, timestamp | DateTime |
| datetimeoffset, timestamptz | DateTimeOffset |
| date | DateOnly |
| time | TimeOnly |
| interval | TimeSpan |
| uniqueidentifier, uuid | Guid |
| binary, varbinary, bytea | byte[] |
| char, varchar, text, nvarchar | string |

## Troubleshooting

### No entities loaded

- Check connection string is valid
- Verify database tables exist
- Check schema and table filters
- Ensure tables are in `INFORMATION_SCHEMA.TABLES`

### Performance issues

- Enable connection pooling
- Use parameterized queries (we do this automatically)
- Consider indexes on your tables
- Check database query performance

### Custom database not working

- Provide custom metadata queries for your database
- Check if `INFORMATION_SCHEMA` is supported
- Consult your database documentation

### Type mapping issues

- The `MapSqlTypeToClrType` method handles common types
- For custom types, extend the provider or use custom queries

## Best Practices

1. **Always use connection pooling** in production
2. **Store connection strings** in configuration, not code
3. **Use parameterized queries** (we do this for you)
4. **Filter unnecessary tables** to improve startup time
5. **Use convenience methods** for SQL Server, MySQL, PostgreSQL
6. **Provide custom queries** for optimal performance on your database

## License

This project is licensed under the MIT License.
