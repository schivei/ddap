# Database Providers

DDAP provides **two unified packages** that work with any database you choose. You're in control of the database driver, connection string, and pooling configuration.

## Overview

| Package | Purpose | Database Support | You Provide |
|---------|---------|------------------|-------------|
| `Ddap.Data.Dapper` | Unified Dapper provider | **ANY** database with an `IDbConnection` | ADO.NET driver + connection factory |
| `Ddap.Data.EntityFramework` | Unified EF Core provider | **ANY** EF Core supported database | EF Core provider + DbContext |

## 🔥 Key Architecture Changes

### ✅ Before (Old - Database-Specific Packages)
```csharp
// ❌ OLD: Separate package per database
dotnet add package Ddap.Data.Dapper.SqlServer
dotnet add package Ddap.Data.Dapper.MySql
dotnet add package Ddap.Data.Dapper.PostgreSql
```

### ✅ Now (New - Unified Package)
```csharp
// ✅ NEW: ONE package for ALL databases
dotnet add package Ddap.Data.Dapper
dotnet add package Microsoft.Data.SqlClient  // YOU choose the driver
```

### What is Ddap.Data.Dapper?

**ONE package** that works with **ANY database** that has an ADO.NET driver (`IDbConnection` implementation).

### Core Concept

```csharp
.AddDapper(() => new YourDbConnection(connectionString))
           ↑               ↑
     Connection Factory  You choose this
```

You provide:
1. A **connection factory** - a lambda that creates `IDbConnection` instances
2. Your **database driver** - installed as a separate NuGet package
3. Your **connection string** - with pooling configured as you need

### Installation

#### Step 1: Install Ddap.Data.Dapper
```bash
dotnet add package Ddap.Data.Dapper
```

#### Step 2: Install YOUR Database Driver
```bash
# SQL Server
dotnet add package Microsoft.Data.SqlClient

# MySQL
dotnet add package MySqlConnector

# PostgreSQL
dotnet add package Npgsql

# SQLite
dotnet add package Microsoft.Data.Sqlite

# Oracle
dotnet add package Oracle.ManagedDataAccess.Core
```

---

## Database Examples

### SQL Server

```csharp
using Ddap.Data.Dapper;
using Microsoft.Data.SqlClient;

var connectionString = "Server=localhost;Database=MyDb;Integrated Security=true;";

builder.Services
    .AddDdap()
    .AddDapper(() => new SqlConnection(connectionString));
```

**Connection String Options:**
```csharp
// Windows Authentication
"Server=localhost;Database=MyDb;Integrated Security=true;"

// SQL Authentication
"Server=localhost;Database=MyDb;User Id=sa;Password=YourPassword;"

// Connection Pooling (recommended)
"Server=localhost;Database=MyDb;Integrated Security=true;Min Pool Size=5;Max Pool Size=100;"

// Azure SQL Database
"Server=tcp:myserver.database.windows.net,1433;Database=MyDb;User ID=admin;Password=pass;Encrypt=True;"

// Multiple Active Result Sets
"Server=localhost;Database=MyDb;Integrated Security=true;MultipleActiveResultSets=true;"
```

**Data Types:**
- Standard types: `int`, `bigint`, `varchar`, `nvarchar`, `datetime`, `datetime2`, `bit`, `decimal`
- SQL Server specific: `uniqueidentifier` → `Guid`, `hierarchyid`, `geography`, `geometry`

---

### MySQL

```csharp
using Ddap.Data.Dapper;
using MySqlConnector;

var connectionString = "Server=localhost;Database=MyDb;User=root;Password=secret;";

builder.Services
    .AddDdap()
    .AddDapper(() => new MySqlConnection(connectionString));
```

**Connection String Options:**
```csharp
// Basic
"Server=localhost;Database=MyDb;User=root;Password=secret;"

// Custom Port
"Server=localhost;Port=3306;Database=MyDb;User=root;Password=secret;"

// SSL Required
"Server=localhost;Database=MyDb;User=root;Password=secret;SslMode=Required;"

// Connection Pooling
"Server=localhost;Database=MyDb;User=root;Password=secret;MinimumPoolSize=5;MaximumPoolSize=100;"

// UTF-8 Encoding (recommended)
"Server=localhost;Database=MyDb;User=root;Password=secret;CharSet=utf8mb4;"
```

**Data Types:**
- Standard: `INT`, `BIGINT`, `VARCHAR`, `TEXT`, `DATETIME`, `TIMESTAMP`, `BOOLEAN`
- MySQL specific: `JSON` → `String`, `ENUM`, `SET`

**Best Practices:**
- Use `utf8mb4` character set for full Unicode support
- Use `InnoDB` engine for foreign key support
- MySQL 5.7+ for JSON column support

---

### PostgreSQL

```csharp
using Ddap.Data.Dapper;
using Npgsql;

var connectionString = "Host=localhost;Database=MyDb;Username=postgres;Password=secret;";

builder.Services
    .AddDdap()
    .AddDapper(() => new NpgsqlConnection(connectionString));
```

**Connection String Options:**
```csharp
// Basic
"Host=localhost;Database=MyDb;Username=postgres;Password=secret;"

// Custom Port
"Host=localhost;Port=5432;Database=MyDb;Username=postgres;Password=secret;"

// SSL Required
"Host=localhost;Database=MyDb;Username=postgres;Password=secret;SSL Mode=Require;"

// Connection Pooling
"Host=localhost;Database=MyDb;Username=postgres;Password=secret;Minimum Pool Size=5;Maximum Pool Size=100;"

// Timeouts
"Host=localhost;Database=MyDb;Username=postgres;Password=secret;Timeout=30;Command Timeout=30;"
```

**Data Types:**
- Standard: `integer`, `bigint`, `varchar`, `text`, `timestamp`, `boolean`, `decimal`
- PostgreSQL specific: `uuid` → `Guid`, `json`/`jsonb` → `String`, `array` → .NET arrays

**PostgreSQL-Specific Features:**
- Schemas for organization (`CREATE SCHEMA app;`)
- UUID primary keys
- Array types (`TEXT[]`, `INT[]`)
- JSONB for structured data
- Full-text search

---

### SQLite

```csharp
using Ddap.Data.Dapper;
using Microsoft.Data.Sqlite;

var connectionString = "Data Source=myapp.db;";

builder.Services
    .AddDdap()
    .AddDapper(() => new SqliteConnection(connectionString));
```

**Connection String Options:**
```csharp
// File database
"Data Source=myapp.db;"

// In-memory database
"Data Source=:memory:;"

// Read-only
"Data Source=myapp.db;Mode=ReadOnly;"

// Shared cache
"Data Source=myapp.db;Cache=Shared;"
```

**Best Practices:**
- Use `AUTOINCREMENT` sparingly (performance cost)
- Enable foreign keys: `PRAGMA foreign_keys = ON;`
- Use Write-Ahead Logging: `PRAGMA journal_mode=WAL;`

---

### Oracle

```csharp
using Ddap.Data.Dapper;
using Oracle.ManagedDataAccess.Client;

var connectionString = "User Id=system;Password=oracle;Data Source=localhost:1521/ORCLPDB1;";

builder.Services
    .AddDdap()
    .AddDapper(() => new OracleConnection(connectionString));
```

**Connection String Options:**
```csharp
// Basic
"User Id=system;Password=oracle;Data Source=localhost:1521/ORCLPDB1;"

// With TNS Name
"User Id=system;Password=oracle;Data Source=ORCL;"

// Connection Pooling
"User Id=system;Password=oracle;Data Source=ORCL;Pooling=true;Min Pool Size=5;Max Pool Size=100;"
```

---

## Advanced Configuration

### Custom Provider Options

```csharp
builder.Services
    .AddDdap()
    .AddDapper(() => new SqlConnection(connectionString), options =>
    {
        options.ProviderName = "MyCustomName";
        options.UseInformationSchema = true;  // Standard SQL metadata
        
        // Override metadata queries for specific database features
        options.CustomTableQuery = @"
            SELECT TABLE_SCHEMA, TABLE_NAME
            FROM INFORMATION_SCHEMA.TABLES
            WHERE TABLE_TYPE = 'BASE TABLE'";
    });
```

### Multiple Databases

```csharp
// SQL Server (Primary)
builder.Services
    .AddDdap("Primary")
    .AddDapper(() => new SqlConnection(sqlServerConnString));

// PostgreSQL (Analytics)
builder.Services
    .AddDdap("Analytics")
    .AddDapper(() => new NpgsqlConnection(postgresConnString));

// SQLite (Cache)
builder.Services
    .AddDdap("Cache")
    .AddDapper(() => new SqliteConnection("Data Source=cache.db;"));
```

---

## When to Use Dapper

✅ **Use Dapper when:**
- Performance is critical
- You need maximum control over SQL
- Simple CRUD operations
- Database-first development
- Working with stored procedures
- Minimal dependencies

❌ **Avoid Dapper when:**
- You need change tracking
- Complex object graphs with lazy loading
- Code-first with migrations
- Heavy use of LINQ queries

---

## Entity Framework Provider


### What is Ddap.Data.EntityFramework?

**ONE package** that works with **ANY EF Core provider**. Your existing DbContext, your existing EF configuration.

### Core Concept

```csharp
.AddEntityFramework<MyDbContext>()
                    ↑
            Your existing DbContext
```

You provide:
1. A **DbContext** - already registered in DI
2. An **EF Core provider** - SQL Server, PostgreSQL, SQLite, etc.
3. Your **existing configuration** - we use it as-is

### Installation

```bash
# Install DDAP EF provider
dotnet add package Ddap.Data.EntityFramework

# Install YOUR EF Core provider (if not already installed)
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
# OR
dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL
# OR
dotnet add package Microsoft.EntityFrameworkCore.Sqlite
# OR
dotnet add package Pomelo.EntityFrameworkCore.MySql
```

---

## Configuration Examples

### SQL Server

```csharp
using Ddap.Data.EntityFramework;
using Microsoft.EntityFrameworkCore;

public class MyDbContext : DbContext
{
    public MyDbContext(DbContextOptions<MyDbContext> options) : base(options) { }
    
    public DbSet<User> Users { get; set; }
    public DbSet<Order> Orders { get; set; }
}

// Step 1: Register your DbContext (standard EF Core)
builder.Services.AddDbContext<MyDbContext>(options =>
    options.UseSqlServer(connectionString));

// Step 2: Add DDAP with Entity Framework provider
builder.Services
    .AddDdap()
    .AddEntityFramework<MyDbContext>();
```

---

### PostgreSQL

```csharp
using Ddap.Data.EntityFramework;
using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    
    public DbSet<Product> Products { get; set; }
}

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services
    .AddDdap()
    .AddEntityFramework<AppDbContext>();
```

---

### MySQL

```csharp
using Ddap.Data.EntityFramework;
using Microsoft.EntityFrameworkCore;

public class ShopContext : DbContext
{
    public ShopContext(DbContextOptions<ShopContext> options) : base(options) { }
    
    public DbSet<Customer> Customers { get; set; }
}

var serverVersion = new MySqlServerVersion(new Version(8, 0, 33));

builder.Services.AddDbContext<ShopContext>(options =>
    options.UseMySql(connectionString, serverVersion));

builder.Services
    .AddDdap()
    .AddEntityFramework<ShopContext>();
```

---

### SQLite

```csharp
using Ddap.Data.EntityFramework;
using Microsoft.EntityFrameworkCore;

public class LocalDbContext : DbContext
{
    public LocalDbContext(DbContextOptions<LocalDbContext> options) : base(options) { }
    
    public DbSet<Note> Notes { get; set; }
}

builder.Services.AddDbContext<LocalDbContext>(options =>
    options.UseSqlite("Data Source=app.db"));

builder.Services
    .AddDdap()
    .AddEntityFramework<LocalDbContext>();
```

---

### In-Memory (Testing)

```csharp
using Ddap.Data.EntityFramework;
using Microsoft.EntityFrameworkCore;

builder.Services.AddDbContext<TestDbContext>(options =>
    options.UseInMemoryDatabase("TestDb"));

builder.Services
    .AddDdap()
    .AddEntityFramework<TestDbContext>();
```

---

## Advanced Configuration

### Entity Filtering

```csharp
builder.Services
    .AddDdap()
    .AddEntityFramework<MyDbContext>(options =>
    {
        // Only expose specific entities
        options.EntityFilter = type => type.Namespace == "MyApp.Models.Public";
        
        // Discover from DbSet properties (default: true)
        options.DiscoverFromDbSets = true;
        
        // Use EF model metadata (default: true)
        options.UseModelMetadata = true;
    });
```

### Pooled DbContext Factory

For better performance in high-throughput scenarios:

```csharp
// Use pooled factory instead of AddDbContext
builder.Services.AddPooledDbContextFactory<MyDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services
    .AddDdap()
    .AddEntityFramework<MyDbContext>();
```

### Multiple DbContexts

```csharp
// Users database
builder.Services.AddDbContext<UsersDbContext>(options =>
    options.UseSqlServer(usersConnString));

builder.Services
    .AddDdap("Users")
    .AddEntityFramework<UsersDbContext>();

// Orders database
builder.Services.AddDbContext<OrdersDbContext>(options =>
    options.UseNpgsql(ordersConnString));

builder.Services
    .AddDdap("Orders")
    .AddEntityFramework<OrdersDbContext>();
```

---

## EF Core Features Support

| Feature | Supported | Notes |
|---------|-----------|-------|
| Code-first | ✅ | Full support |
| Database-first | ✅ | Full support |
| Migrations | ✅ | Works with existing migrations |
| Relationships | ✅ | One-to-many, many-to-many, etc. |
| Inheritance | ✅ | TPH, TPT, TPC strategies |
| Lazy Loading | ✅ | If enabled in your DbContext |
| Change Tracking | ✅ | Standard EF behavior |
| Transactions | ✅ | Full support |
| Bulk Operations | ⚠️ | Depends on provider extensions |

---

## When to Use Entity Framework

✅ **Use EF Core when:**
- You already have a DbContext
- Code-first development with migrations
- Complex object graphs with navigation properties
- Need change tracking
- Lazy loading is beneficial
- LINQ query composition
- Multiple database support needed

❌ **Avoid EF Core when:**
- Maximum performance is critical
- Memory footprint must be minimal
- Simple CRUD with hand-written SQL
- Working with legacy databases with unusual schemas

---

## Comparison: Dapper vs Entity Framework

### Performance

| Operation | Dapper | Entity Framework | Winner |
|-----------|--------|------------------|--------|
| Metadata Loading | ~50ms | ~150ms | 🏆 Dapper |
| Simple Query | ~5ms | ~8ms | 🏆 Dapper |
| Complex Query | ~15ms | ~25ms | 🏆 Dapper |
| Bulk Insert (1000 rows) | ~100ms | ~500ms | 🏆 Dapper |
| Insert with Relations | ~10ms | ~15ms | 🏆 Dapper |

*Benchmarks are approximate and vary by hardware, database, and configuration*

### Memory Usage

| Scenario | Dapper | Entity Framework | Difference |
|----------|--------|------------------|------------|
| Baseline | ~20 MB | ~50 MB | +150% |
| 10K entities loaded | ~45 MB | ~120 MB | +167% |
| With change tracking | N/A | ~200 MB | - |

### Feature Comparison

| Feature | Dapper | Entity Framework |
|---------|--------|------------------|
| **ORM Type** | Micro-ORM | Full ORM |
| **Learning Curve** | Low | Medium-High |
| **Performance** | ⚡ Excellent | Good |
| **SQL Control** | ✅ Full | ⚠️ Limited (LINQ) |
| **Relationships** | ✅ Manual | ✅ Automatic |
| **Change Tracking** | ❌ None | ✅ Yes |
| **Lazy Loading** | ❌ No | ✅ Yes |
| **Migrations** | ❌ No | ✅ Yes |
| **Database Agnostic** | ⚠️ Mostly | ✅ Yes |
| **Code-First** | ❌ No | ✅ Yes |
| **Stored Procedures** | ✅ Excellent | ⚠️ Limited |
| **Complex Types** | ⚠️ Manual | ✅ Automatic |
| **Memory Footprint** | 🏆 Very Low | Higher |
| **Maintenance** | Manual SQL | Automatic |

### Use Case Recommendations

| Scenario | Recommended |
|----------|-------------|
| High-performance API | 🏆 **Dapper** |
| Microservices | 🏆 **Dapper** |
| CRUD with complex relationships | 🏆 **Entity Framework** |
| Existing EF Core application | 🏆 **Entity Framework** |
| Code-first + migrations | 🏆 **Entity Framework** |
| Legacy database | 🏆 **Dapper** |
| Read-heavy workloads | 🏆 **Dapper** |
| Write-heavy with tracking | 🏆 **Entity Framework** |
| Maximum control over SQL | 🏆 **Dapper** |
| Rapid development | 🏆 **Entity Framework** |

---

## Best Practices

### Connection String Management

#### Use Configuration

```csharp
// appsettings.json
{
  "ConnectionStrings": {
    "Default": "Server=localhost;Database=MyDb;Integrated Security=true;"
  }
}

// Program.cs
var connectionString = builder.Configuration.GetConnectionString("Default");
```

#### Use Environment Variables

```bash
# .env or environment
export ConnectionStrings__Default="Server=prod;Database=MyDb;User Id=app;Password=***;"
```

#### Use Azure Key Vault / Secrets

```csharp
builder.Configuration.AddAzureKeyVault(
    new Uri($"https://{keyVaultName}.vault.azure.net/"),
    new DefaultAzureCredential());

var connectionString = builder.Configuration["ConnectionStrings:Default"];
```

#### Never Hardcode Secrets

```csharp
// ❌ BAD
var connStr = "Server=prod;Password=SuperSecret123;";

// ✅ GOOD
var connStr = builder.Configuration.GetConnectionString("Default");
```

---

### Connection Pooling

#### Dapper (via ADO.NET Driver)

Connection pooling is **automatic** in most ADO.NET drivers:

```csharp
// SQL Server
"Server=localhost;Database=MyDb;Integrated Security=true;Min Pool Size=5;Max Pool Size=100;"

// PostgreSQL
"Host=localhost;Database=MyDb;Username=user;Password=pass;Minimum Pool Size=5;Maximum Pool Size=100;"

// MySQL
"Server=localhost;Database=MyDb;User=root;Password=pass;MinimumPoolSize=5;MaximumPoolSize=100;"
```

**Best Practices:**
- Set `Min Pool Size` to handle baseline load (typically 5-10)
- Set `Max Pool Size` based on database limits (typically 50-100)
- Monitor pool exhaustion with connection timeout errors
- Don't set pool size too high (wastes resources)

#### Entity Framework

```csharp
// Use pooled DbContext for better performance
builder.Services.AddPooledDbContextFactory<MyDbContext>(options =>
    options.UseSqlServer(connectionString));
```

---

### Performance Tips

#### Dapper

1. **Reuse connections from pool** (automatic)
2. **Use parameterized queries** (prevents SQL injection + caching)
   ```csharp
   connection.Query("SELECT * FROM Users WHERE Id = @Id", new { Id = userId });
   ```
3. **Use buffered queries** for large result sets:
   ```csharp
   var users = connection.Query<User>("SELECT * FROM Users", buffered: false);
   ```
4. **Avoid N+1 queries** with manual joins or multiple result sets

#### Entity Framework

1. **Disable change tracking** for read-only queries:
   ```csharp
   dbContext.Users.AsNoTracking().ToList();
   ```
2. **Use projection** to select only needed columns:
   ```csharp
   dbContext.Users.Select(u => new { u.Id, u.Name }).ToList();
   ```
3. **Eager load relationships** to avoid N+1:
   ```csharp
   dbContext.Orders.Include(o => o.Customer).ThenInclude(c => c.Address).ToList();
   ```
4. **Use compiled queries** for frequently-executed queries
5. **Split complex queries** to avoid Cartesian explosion

---

### Security

#### Always Use Parameterized Queries

```csharp
// ❌ DANGEROUS - SQL Injection vulnerability
var sql = $"SELECT * FROM Users WHERE Name = '{userName}'";

// ✅ SAFE - Parameterized query
var sql = "SELECT * FROM Users WHERE Name = @Name";
connection.Query<User>(sql, new { Name = userName });
```

#### Least Privilege Database Users

```sql
-- Create app-specific user with minimal permissions
CREATE LOGIN app_user WITH PASSWORD = 'StrongPassword!';
CREATE USER app_user FOR LOGIN app_user;

-- Grant only necessary permissions
GRANT SELECT, INSERT, UPDATE, DELETE ON SCHEMA::dbo TO app_user;
-- DENY DROP, ALTER, CREATE
```

#### Encrypt Connections

```csharp
// SQL Server
"Server=prod;Database=MyDb;User Id=app;Password=***;Encrypt=True;TrustServerCertificate=False;"

// PostgreSQL
"Host=prod;Database=MyDb;Username=app;Password=***;SSL Mode=Require;"

// MySQL
"Server=prod;Database=MyDb;User=app;Password=***;SslMode=Required;"
```

---

## Migration Guide

### From Old Database-Specific Packages

#### Before (Old Architecture)

```csharp
// ❌ OLD: Multiple database-specific packages
dotnet add package Ddap.Data.Dapper.SqlServer
dotnet add package Ddap.Data.Dapper.MySql
dotnet add package Ddap.Data.Dapper.PostgreSql

builder.Services
    .AddDdap()
    .AddDapperSqlServer(connectionString);
```

#### After (New Architecture)

```csharp
// ✅ NEW: One package + your choice of driver
dotnet add package Ddap.Data.Dapper
dotnet add package Microsoft.Data.SqlClient

builder.Services
    .AddDdap()
    .AddDapper(() => new SqlConnection(connectionString));
```

### Migration Steps

1. **Uninstall old packages:**
   ```bash
   dotnet remove package Ddap.Data.Dapper.SqlServer
   dotnet remove package Ddap.Data.Dapper.MySql
   dotnet remove package Ddap.Data.Dapper.PostgreSql
   ```

2. **Install new unified package:**
   ```bash
   dotnet add package Ddap.Data.Dapper
   ```

3. **Ensure database driver is installed:**
   ```bash
   # If not already installed
   dotnet add package Microsoft.Data.SqlClient
   ```

4. **Update configuration:**
   ```csharp
   // OLD
   .AddDapperSqlServer(connectionString)
   
   // NEW
   .AddDapper(() => new SqlConnection(connectionString))
   ```

5. **Update usings (if needed):**
   ```csharp
   using Ddap.Data.Dapper;  // Same namespace
   using Microsoft.Data.SqlClient;  // Add driver namespace
   ```

### Breaking Changes

| Old API | New API | Notes |
|---------|---------|-------|
| `.AddDapperSqlServer(connStr)` | `.AddDapper(() => new SqlConnection(connStr))` | Pass factory function |
| `.AddDapperMySql(connStr)` | `.AddDapper(() => new MySqlConnection(connStr))` | Pass factory function |
| `.AddDapperPostgreSql(connStr)` | `.AddDapper(() => new NpgsqlConnection(connStr))` | Pass factory function |
| Connection string in method | Connection string in factory | More control over lifetime |

### Benefits of Migration

✅ **Flexibility**: Choose any database driver, any version  
✅ **Control**: Manage connection creation and pooling  
✅ **Simplicity**: One package to maintain  
✅ **Future-proof**: Add new databases without waiting for DDAP updates  
✅ **Testability**: Inject mock connections easily

---

## Troubleshooting

### Cannot Connect to Database

**Problem**: Connection errors or timeouts

**Solutions**:
1. Verify connection string format for your database
2. Check firewall rules allow connections
3. Ensure database service is running
4. Test connection with database-specific tool:
   ```bash
   sqlcmd -S localhost -U sa -P Password    # SQL Server
   mysql -h localhost -u root -p            # MySQL
   psql -h localhost -U postgres            # PostgreSQL
   ```

### "No connection factory registered"

**Problem**: `InvalidOperationException` when starting app

**Solution**: Ensure you call `.AddDapper(...)` with a factory:
```csharp
builder.Services
    .AddDdap()
    .AddDapper(() => new SqlConnection(connectionString));  // ← Required
```

### "Type 'X' is not registered as a DbContext"

**Problem**: EF provider can't find DbContext

**Solution**: Register DbContext **before** DDAP:
```csharp
// Register DbContext first
builder.Services.AddDbContext<MyDbContext>(options => ...);

// Then add DDAP
builder.Services
    .AddDdap()
    .AddEntityFramework<MyDbContext>();
```

### Poor Performance
**Problem**: Slow queries or metadata loading

**Solutions**:

**For Dapper:**
1. Enable connection pooling in connection string
2. Add database indexes on frequently-queried columns
3. Use buffered queries for large result sets
4. Profile queries with database tools (`EXPLAIN`, `ANALYZE`)

**For Entity Framework:**
1. Disable change tracking: `.AsNoTracking()`
2. Use projection: `Select(x => new { x.Id, x.Name })`
3. Eager load: `.Include(x => x.Related)`
4. Use compiled queries for repeated queries
5. Consider splitting complex queries

### "Table has no primary key"

**Problem**: DDAP requires primary keys for entity operations

**Solution**: Add primary key to table or use views for read-only access:
```sql
-- Add primary key
ALTER TABLE MyTable ADD CONSTRAINT PK_MyTable PRIMARY KEY (Id);

-- Or create view with virtual key (read-only)
CREATE VIEW MyTableView AS SELECT ROW_NUMBER() OVER (ORDER BY CreatedAt) AS Id, * FROM MyTable;
```

### Connection Pool Exhaustion

**Problem**: `Timeout expired. The timeout period elapsed prior to obtaining a connection from the pool.`

**Solutions**:
1. Increase `Max Pool Size` in connection string
2. Ensure connections are properly disposed (use `using` statements)
3. Check for connection leaks in application code
4. Monitor active connections in database
5. Reduce `Connection Timeout` to fail faster

```csharp
// Increase pool size
"Server=localhost;Database=MyDb;Min Pool Size=10;Max Pool Size=200;"

// Ensure proper disposal
using (var connection = new SqlConnection(connectionString))
{
    // Use connection
} // Automatically returned to pool
```

---

## Next Steps

- **[API Providers](./api-providers.md)** - Expose your data via REST, gRPC, or GraphQL
- **[Advanced Usage](./advanced.md)** - Optimize performance and extend functionality  
- **[Get Started](./get-started.md)** - Quick start guide
- **[Troubleshooting](./troubleshooting.md)** - Common issues and solutions

---

## Additional Resources

### Database Documentation
- [SQL Server Connection Strings](https://docs.microsoft.com/sql/connect/ado-net/connection-string-syntax)
- [MySQL Connection Strings](https://dev.mysql.com/doc/connector-net/en/connector-net-connections-string.html)
- [PostgreSQL Connection Strings](https://www.npgsql.org/doc/connection-string-parameters.html)
- [SQLite Connection Strings](https://docs.microsoft.com/dotnet/standard/data/sqlite/connection-strings)

### Driver Documentation
- [Microsoft.Data.SqlClient](https://github.com/dotnet/SqlClient)
- [MySqlConnector](https://mysqlconnector.net/)
- [Npgsql](https://www.npgsql.org/)
- [Microsoft.Data.Sqlite](https://docs.microsoft.com/dotnet/standard/data/sqlite/)
- [Oracle.ManagedDataAccess](https://www.oracle.com/database/technologies/appdev/dotnet/odp.html)

### ORM Documentation
- [Dapper Documentation](https://github.com/DapperLib/Dapper)
- [Entity Framework Core Documentation](https://docs.microsoft.com/ef/core/)
