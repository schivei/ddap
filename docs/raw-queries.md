# Raw SQL Queries via gRPC

> **Security First**: Execute raw SQL queries with policy-based authorization, audit logging, and injection prevention.

## Overview

DDAP's Raw Query feature allows developers to execute SQL queries directly via gRPC while maintaining strict security controls through policy-based authorization. This feature is designed for scenarios where the standard entity operations are insufficient and you need direct database access.

**Philosophy**: Developer in Control with Safe Defaults.

---

## 🔒 Security Model

### Default Policy: Read-Only

By default, DDAP only allows `SELECT` queries. All modification operations (INSERT, UPDATE, DELETE, etc.) are **denied**.

```csharp
// Default configuration - SELECT only
services.AddRawQueryServices();
```

### Available Policies

#### 1. DefaultRawQueryPolicy (Default)
- ✅ SELECT queries
- ❌ INSERT, UPDATE, DELETE, DROP, etc.

#### 2. AllowAllRawQueryPolicy
⚠️ **Warning**: Use with extreme caution - allows all query types.

```csharp
services.AddRawQueryServices(new AllowAllRawQueryPolicy());
```

#### 3. DenyAllRawQueryPolicy
Completely disables raw query execution.

```csharp
services.AddRawQueryServices(new DenyAllRawQueryPolicy());
```

#### 4. Custom Policies
Implement `IRawQueryPolicy` for custom authorization logic:

```csharp
public class CustomRawQueryPolicy : IRawQueryPolicy
{
    public Task<bool> CanExecuteQueryAsync(RawQueryContext context)
    {
        // Only allow SELECT on views
        if (context.QueryType == QueryType.Select && 
            context.TableName?.StartsWith("vw_") == true)
        {
            return Task.FromResult(true);
        }
        
        // Allow INSERT/UPDATE/DELETE only for admin role
        if (context.UserRoles?.Contains("admin") == true &&
            context.QueryType is QueryType.Insert or QueryType.Update or QueryType.Delete)
        {
            return Task.FromResult(true);
        }
        
        return Task.FromResult(false);
    }
}

// Register custom policy
services.AddRawQueryServices(new CustomRawQueryPolicy());
```

---

## 🚀 Getting Started

### 1. Server Setup (Dapper)

```csharp
using Ddap.Grpc;
using Ddap.Data.Dapper;
using Ddap.Auth.Policies;

var builder = WebApplication.CreateBuilder(args);

// Configure Dapper connection
builder.Services.AddSingleton(new DapperProviderOptions
{
    ConnectionFactory = () => new SqlConnection(connectionString),
    ProviderName = "SqlServer"
});

// Add raw query executor
builder.Services.AddSingleton<IRawQueryExecutor, DapperRawQueryExecutor>();

// Add raw query services with custom policy
builder.Services.AddRawQueryServices(new AllowAllRawQueryPolicy()); // Or your custom policy

// Add gRPC
builder.Services.AddGrpc();

var app = builder.Build();

// Map raw query service
app.MapGrpcService<RawQueryServiceImpl>();

app.Run();
```

### 2. Server Setup (Entity Framework)

```csharp
using Ddap.Grpc;
using Ddap.Data.EntityFramework;
using Ddap.Auth.Policies;

var builder = WebApplication.CreateBuilder(args);

// Configure DbContext
builder.Services.AddDbContextFactory<MyDbContext>(options =>
    options.UseSqlServer(connectionString));

// Add raw query executor
builder.Services.AddSingleton<IRawQueryExecutor, EntityFrameworkRawQueryExecutor<MyDbContext>>();

// Add raw query services
builder.Services.AddRawQueryServices(new DefaultRawQueryPolicy());

// Add gRPC
builder.Services.AddGrpc();

var app = builder.Build();

// Map raw query service
app.MapGrpcService<RawQueryServiceImpl>();

app.Run();
```

---

## 📊 Query Execution Types

### 1. Scalar Queries

Returns a single value (COUNT, SUM, MAX, etc.).

**gRPC Call:**
```protobuf
message RawQueryRequest {
  string query = 1;
  string parameters_json = 2;  // Optional: {"@param": "value"}
  ResultType result_type = 3;  // SCALAR
}
```

**Example:**
```csharp
var request = new RawQueryRequest
{
    Query = "SELECT COUNT(*) FROM Users WHERE Active = @Active",
    ParametersJson = "{\"Active\": true}",
    ResultType = ResultType.Scalar
};

var response = await client.ExecuteScalarAsync(request);
var count = BitConverter.ToInt32(response.Value.ToByteArray(), 0);
```

### 2. Single Row Queries

Returns a single row as a dictionary.

**gRPC Call:**
```protobuf
message SingleResult {
  bytes row_data = 1;
  repeated string column_names = 2;
  repeated string column_types = 3;
  bool is_empty = 4;
}
```

**Example:**
```csharp
var request = new RawQueryRequest
{
    Query = "SELECT Id, Name, Email FROM Users WHERE Id = @Id",
    ParametersJson = "{\"Id\": 123}",
    ResultType = ResultType.Single
};

var response = await client.ExecuteSingleAsync(request);
if (!response.IsEmpty)
{
    var row = RawQueryBinarySerializer.DeserializeSingleRow(response.RowData.ToByteArray());
    Console.WriteLine($"User: {row["Name"]} ({row["Email"]})");
}
```

### 3. Multiple Row Queries

Returns multiple rows.

**gRPC Call:**
```protobuf
message MultipleResult {
  bytes rows_data = 1;
  repeated string column_names = 2;
  repeated string column_types = 3;
  int32 row_count = 4;
}
```

**Example:**
```csharp
var request = new RawQueryRequest
{
    Query = "SELECT Id, Name FROM Users WHERE Active = @Active ORDER BY Name",
    ParametersJson = "{\"Active\": true}",
    ResultType = ResultType.Multiple
};

var response = await client.ExecuteMultipleAsync(request);
var rows = RawQueryBinarySerializer.DeserializeMultipleRows(response.RowsData.ToByteArray());

foreach (var row in rows)
{
    Console.WriteLine($"User: {row["Name"]} (ID: {row["Id"]})");
}
```

### 4. Non-Query Commands (INSERT, UPDATE, DELETE)

Executes commands that don't return data.

**gRPC Call:**
```protobuf
message VoidResult {
  int32 rows_affected = 1;
}
```

**Example:**
```csharp
var request = new RawQueryRequest
{
    Query = "UPDATE Users SET LastLogin = @LoginTime WHERE Id = @UserId",
    ParametersJson = "{\"LoginTime\": \"2024-01-30T10:00:00Z\", \"UserId\": 123}",
    ResultType = ResultType.Void
};

var response = await client.ExecuteNonQueryAsync(request);
Console.WriteLine($"{response.RowsAffected} rows updated");
```

---

## 🛡️ Security Features

### 1. SQL Injection Prevention

The query analyzer automatically detects potential SQL injection patterns:

```csharp
// ❌ This will be rejected
"SELECT * FROM Users WHERE Id = 1 OR 1=1"

// ✅ This is safe (parameterized)
"SELECT * FROM Users WHERE Id = @Id"
```

**Always use parameterized queries:**
```csharp
var request = new RawQueryRequest
{
    Query = "SELECT * FROM Users WHERE Name = @Name",
    ParametersJson = "{\"Name\": \"John\"}",  // Safe!
    ResultType = ResultType.Multiple
};
```

### 2. Query Type Detection

The system automatically detects query types:
- SELECT
- INSERT
- UPDATE
- DELETE
- CREATE
- DROP
- ALTER
- TRUNCATE
- MERGE
- EXECUTE

### 3. Table Name Extraction

For policy evaluation, the system attempts to extract the target table name:

```csharp
// Context available to policies
public class RawQueryContext
{
    public string Query { get; init; }
    public QueryType QueryType { get; init; }
    public string? TableName { get; init; }      // Extracted automatically
    public string? DatabaseName { get; init; }
    public string? UserId { get; init; }
    public IEnumerable<string>? UserRoles { get; init; }
}
```

### 4. Audit Logging

All query executions are automatically logged:

```
INFO: Raw query scalar execution requested: SELECT COUNT(*) FROM Users
INFO: Query authorized. User: john@example.com, QueryType: Select, Table: Users
INFO: Raw query scalar executed successfully
```

Failed attempts are also logged:

```
WARNING: Query denied by policy. User: guest@example.com, QueryType: Delete, Table: Users
WARNING: Potential SQL injection detected: SELECT * FROM Users WHERE 1=1
```

---

## 📈 Best Practices

### ✅ DO

1. **Always use parameterized queries**
   ```csharp
   Query = "SELECT * FROM Users WHERE Id = @Id",
   ParametersJson = "{\"Id\": 123}"
   ```

2. **Use the most restrictive policy possible**
   ```csharp
   // Production: Read-only by default
   services.AddRawQueryServices(); // DefaultRawQueryPolicy
   ```

3. **Implement custom policies for fine-grained control**
   ```csharp
   public class ProductionRawQueryPolicy : IRawQueryPolicy
   {
       public Task<bool> CanExecuteQueryAsync(RawQueryContext context)
       {
           // Only allow SELECT on specific tables
           if (context.QueryType == QueryType.Select)
           {
               var allowedTables = new[] { "Users", "Orders", "Products" };
               return Task.FromResult(
                   allowedTables.Contains(context.TableName)
               );
           }
           return Task.FromResult(false);
       }
   }
   ```

4. **Monitor and alert on raw query usage**
   ```csharp
   // Add custom logging
   services.AddLogging(logging =>
   {
       logging.AddFilter("Ddap.Grpc.Services.RawQueryServiceImpl", LogLevel.Information);
   });
   ```

### ❌ DON'T

1. **Don't use string concatenation for queries**
   ```csharp
   // ❌ NEVER DO THIS
   Query = $"SELECT * FROM Users WHERE Name = '{userName}'"
   
   // ✅ DO THIS INSTEAD
   Query = "SELECT * FROM Users WHERE Name = @Name",
   ParametersJson = "{\"Name\": \"" + userName + "\"}"
   ```

2. **Don't use AllowAllRawQueryPolicy in production** without proper authorization
   ```csharp
   // ❌ Dangerous in production
   services.AddRawQueryServices(new AllowAllRawQueryPolicy());
   
   // ✅ Safe approach
   services.AddRawQueryServices(sp =>
   {
       var env = sp.GetRequiredService<IWebHostEnvironment>();
       return env.IsProduction()
           ? new DefaultRawQueryPolicy()
           : new AllowAllRawQueryPolicy();
   });
   ```

3. **Don't expose raw query endpoints without authentication**
   ```csharp
   // Require authentication
   [Authorize]
   public class SecureRawQueryService : RawQueryService.RawQueryServiceBase
   {
       // ...
   }
   ```

---

## 🔧 Advanced Configuration

### Role-Based Access Control

```csharp
public class RoleBasedRawQueryPolicy : IRawQueryPolicy
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    
    public RoleBasedRawQueryPolicy(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }
    
    public Task<bool> CanExecuteQueryAsync(RawQueryContext context)
    {
        var user = _httpContextAccessor.HttpContext?.User;
        
        if (user?.IsInRole("Admin") == true)
        {
            // Admins can do anything
            return Task.FromResult(true);
        }
        
        if (user?.IsInRole("Analyst") == true)
        {
            // Analysts can only SELECT
            return Task.FromResult(context.QueryType == QueryType.Select);
        }
        
        // Everyone else: no access
        return Task.FromResult(false);
    }
}
```

### Database-Specific Policies

```csharp
public class DatabaseSpecificPolicy : IRawQueryPolicy
{
    public Task<bool> CanExecuteQueryAsync(RawQueryContext context)
    {
        // Allow SELECT on reporting database
        if (context.DatabaseName == "ReportingDB" && 
            context.QueryType == QueryType.Select)
        {
            return Task.FromResult(true);
        }
        
        // Deny everything else
        return Task.FromResult(false);
    }
}
```

### Time-Based Restrictions

```csharp
public class TimeBasedPolicy : IRawQueryPolicy
{
    public Task<bool> CanExecuteQueryAsync(RawQueryContext context)
    {
        var hour = DateTime.UtcNow.Hour;
        
        // Allow modifications only during maintenance window (2-4 AM UTC)
        if (hour >= 2 && hour < 4)
        {
            return Task.FromResult(true);
        }
        
        // Allow SELECT queries anytime
        return Task.FromResult(context.QueryType == QueryType.Select);
    }
}
```

---

## ⚠️ Warnings and Limitations

### SQL Injection Detection

The built-in injection detection is **basic** and should not be your only line of defense. Always:
- Use parameterized queries
- Implement proper authentication and authorization
- Monitor and audit query execution
- Use database-level permissions

### Query Type Detection

Table name extraction is **best-effort** and may not work for:
- Complex queries with subqueries
- Queries with multiple tables (JOINs)
- Stored procedure calls
- Dynamic SQL

For such cases, rely on database-level permissions.

### Performance

Raw queries bypass DDAP's caching and optimization layers. Use them only when necessary:
- Complex analytical queries
- Bulk operations
- Database maintenance
- Reporting

For standard CRUD operations, use DDAP's entity repositories.

---

## 🧪 Testing

See the test files for comprehensive examples:
- `tests/Ddap.Tests/RawQuery/RawQueryPolicyTests.cs` - Policy tests
- `tests/Ddap.Tests/RawQuery/QueryAnalyzerTests.cs` - Security tests
- `tests/Ddap.Tests/RawQuery/RawQueryBinarySerializerTests.cs` - Serialization tests

---

## 📚 Related Documentation

- [DDAP Core Documentation](./index.md)
- [Authorization Policies](./advanced.md)
- [gRPC Services](./api-providers.md)
- [Data Providers](./database-providers.md)

---

## 🤝 Contributing

Found a security issue or have suggestions for improving the raw query feature? Please open an issue or submit a pull request on [GitHub](https://github.com/schivei/ddap).

---

**Remember**: With great power comes great responsibility. Use raw queries wisely! 🎯
