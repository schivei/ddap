# Troubleshooting

This guide covers common issues, error messages, and their solutions when working with DDAP.

## Table of Contents

- [Installation Issues](#installation-issues)
- [Configuration Issues](#configuration-issues)
- [Database Connection Issues](#database-connection-issues)
- [Metadata Loading Issues](#metadata-loading-issues)
- [API Issues](#api-issues)
- [Performance Issues](#performance-issues)
- [Deployment Issues](#deployment-issues)

## Installation Issues

### Issue: Package Not Found

**Error:**
```
error NU1101: Unable to find package Ddap.Core
```

**Solutions:**
1. Ensure you're using the correct package name
2. Check NuGet.org availability: https://www.nuget.org/packages/Ddap.Core
3. Clear NuGet cache:
   ```bash
   dotnet nuget locals all --clear
   ```
4. Add NuGet source explicitly:
   ```bash
   dotnet nuget add source https://api.nuget.org/v3/index.json
   ```

### Issue: Version Compatibility

**Error:**
```
Package Ddap.Core requires .NET 10.0
```

**Solution:**
Ensure you're using .NET 10 SDK:
```bash
dotnet --version  # Should be 10.0.x
```

Download from: https://dotnet.microsoft.com/download

### Issue: Conflicting Dependencies

**Error:**
```
Package 'X' is not compatible with 'netcoreapp3.1'
```

**Solution:**
Update your project to target .NET 10:
```xml
<Project Sdk="Microsoft.NET.Sdk.Web">
  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
  </PropertyGroup>
</Project>
```

## Configuration Issues

### Issue: AddDdap Not Found

**Error:**
```
'IServiceCollection' does not contain a definition for 'AddDdap'
```

**Solution:**
Add the using directive:
```csharp
using Ddap.Core;
```

### Issue: Provider Not Registered

**Error:**
```
No data provider registered
```

**Solution:**
Register a database provider after AddDdap:
```csharp
builder.Services
    .AddDdap(options => { /* ... */ })
    .AddSqlServerDapper();  // Don't forget this!
```

### Issue: Connection String Not Found

**Error:**
```
ArgumentNullException: Connection string cannot be null
```

**Solutions:**

1. Check appsettings.json:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=localhost;..."
     }
   }
   ```

2. Verify configuration access:
   ```csharp
   var connStr = builder.Configuration.GetConnectionString("DefaultConnection");
   if (string.IsNullOrEmpty(connStr))
   {
       throw new InvalidOperationException("Connection string not found");
   }
   ```

## Database Connection Issues

### Issue: SQL Server Connection Failed

**Error:**
```
A network-related or instance-specific error occurred
```

**Solutions:**

1. **Verify SQL Server is running:**
   ```bash
   # Windows
   Get-Service MSSQLSERVER
   
   # Linux/Docker
   docker ps | grep sqlserver
   ```

2. **Test connection:**
   ```bash
   sqlcmd -S localhost -U sa -P YourPassword
   ```

3. **Check firewall:**
   - Allow TCP port 1433
   - Disable Windows Firewall temporarily to test

4. **Enable TCP/IP:**
   - Open SQL Server Configuration Manager
   - Enable TCP/IP protocol
   - Restart SQL Server service

5. **Try different connection string formats:**
   ```csharp
   // Windows Authentication
   "Server=localhost;Database=MyDb;Integrated Security=true;"
   
   // SQL Authentication
   "Server=localhost;Database=MyDb;User Id=sa;Password=YourPassword;"
   
   // With explicit port
   "Server=localhost,1433;Database=MyDb;User Id=sa;Password=YourPassword;"
   
   // Named instance
   "Server=localhost\\SQLEXPRESS;Database=MyDb;Integrated Security=true;"
   ```

### Issue: MySQL Connection Failed

**Error:**
```
Unable to connect to any of the specified MySQL hosts
```

**Solutions:**

1. **Verify MySQL is running:**
   ```bash
   mysql --version
   systemctl status mysql
   ```

2. **Test connection:**
   ```bash
   mysql -h localhost -u root -p
   ```

3. **Check bind-address in my.cnf:**
   ```ini
   bind-address = 127.0.0.1  # Restrict to local connections (recommended)
   # For remote access, bind to a specific internal IP or CIDR (not 0.0.0.0) and secure the network.
   ```

4. **Create a dedicated application user (recommended):**
   ```sql
   -- Replace 'your-strong-password' with a secure password (min 16 chars, mixed case, numbers, symbols)
   -- Replace 'your_database' with your actual database name
   CREATE USER 'ddap_app'@'localhost' IDENTIFIED BY 'your-strong-password';
   GRANT SELECT, INSERT, UPDATE, DELETE ON your_database.* TO 'ddap_app'@'localhost';
   FLUSH PRIVILEGES;
   ```

   **⚠️ Development-only (DO NOT USE IN PRODUCTION):**
   ```sql
   -- Only for local development with proper firewall protection
   GRANT ALL PRIVILEGES ON *.* TO 'root'@'%' IDENTIFIED BY 'password';
   FLUSH PRIVILEGES;
   ```

### Issue: PostgreSQL Connection Failed

**Error:**
```
Connection refused
```

**Solutions:**

1. **Verify PostgreSQL is running:**
   ```bash
   pg_isready
   systemctl status postgresql
   ```

2. **Test connection:**
   ```bash
   psql -h localhost -U postgres
   ```

3. **Edit postgresql.conf:**
   ```ini
   # In production, restrict PostgreSQL to localhost or a specific trusted interface.
   # Example (local-only):
   listen_addresses = 'localhost'
   # Example (single interface or subnet, adjust as needed):
   # listen_addresses = '192.0.2.10'
   ```

4. **Edit pg_hba.conf:**
   ```
   # Allow local connections only (recommended default)
   host    all             all             127.0.0.1/32         scram-sha-256
   host    all             all             ::1/128              scram-sha-256

   # For remote access, restrict to the minimal required subnet or IP and a least-privilege role:
   # host   mydb           app_user        203.0.113.0/24       scram-sha-256

   # Development-only: allow all IPv4 addresses (DO NOT USE IN PRODUCTION)
   # host   all            all             0.0.0.0/0            md5
   ```

5. **Restart PostgreSQL:**
   ```bash
   systemctl restart postgresql
   ```

## Metadata Loading Issues

### Issue: No Entities Loaded

**Error:**
```
Repository contains no entities
```

**Solutions:**

1. **Check database has tables:**
   ```sql
   -- SQL Server
   SELECT * FROM INFORMATION_SCHEMA.TABLES;
   
   -- MySQL
   SHOW TABLES;
   
   -- PostgreSQL
   \dt
   ```

2. **Verify table permissions:**
   ```sql
   -- SQL Server
   SELECT * FROM fn_my_permissions(NULL, 'DATABASE');
   ```

3. **Check schema filter:**
   ```csharp
   builder.Services.AddDdap(options =>
   {
       options.ConnectionString = "...";
       options.IncludeSchemas = new[] { "dbo", "myschema" };
   });
   ```

### Issue: Some Tables Missing

**Problem:** Only some tables are loaded

**Solutions:**

1. **Check primary keys:**
   - DDAP requires tables to have primary keys
   - Add primary keys to tables

2. **Verify table ownership:**
   ```sql
   -- SQL Server
   SELECT * FROM sys.tables WHERE is_ms_shipped = 0;
   ```

3. **Check schema exclusions:**
   ```csharp
   options.ExcludeSchemas = new[] { "sys", "internal" };
   ```

### Issue: Relationships Not Detected

**Problem:** Foreign keys not showing up

**Solutions:**

1. **Verify foreign keys exist:**
   ```sql
   -- SQL Server
   SELECT * FROM sys.foreign_keys;
   
   -- MySQL
   SELECT * FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE 
   WHERE REFERENCED_TABLE_NAME IS NOT NULL;
   
   -- PostgreSQL
   SELECT * FROM information_schema.table_constraints 
   WHERE constraint_type = 'FOREIGN KEY';
   ```

2. **Create missing foreign keys:**
   ```sql
   ALTER TABLE Orders
   ADD CONSTRAINT FK_Orders_Customers
   FOREIGN KEY (CustomerId) REFERENCES Customers(Id);
   ```

## API Issues

### Issue: 404 Not Found on /api/entity

**Error:**
```
HTTP 404 - /api/entity
```

**Solutions:**

1. **Ensure routing is configured:**
   ```csharp
   app.UseRouting();
   app.MapControllers();
   ```

2. **Check REST provider is registered:**
   ```csharp
   .AddRest()
   ```

3. **Verify controllers are discovered:**
   ```csharp
   builder.Services.AddControllers()
       .AddApplicationPart(typeof(EntityController).Assembly);
   ```

### Issue: GraphQL Endpoint Not Working

**Error:**
```
HTTP 404 - /graphql
```

**Solutions:**

1. **Map GraphQL endpoint:**
   ```csharp
   app.MapGraphQL("/graphql");
   ```

2. **Check GraphQL provider is registered:**
   ```csharp
   .AddGraphQL()
   ```

3. **Add GraphQL services:**
   ```csharp
   builder.Services
       .AddGraphQLServer()
       .AddQueryType<Query>();
   ```

### Issue: Content Negotiation Not Working

**Problem:** Always returns JSON even with Accept: application/xml

**Solutions:**

1. **Add XML formatter:**
   ```csharp
   builder.Services.AddControllers()
       .AddXmlSerializerFormatters();
   ```

2. **Add YAML formatter:**
   ```csharp
   builder.Services.AddControllers()
       .AddYamlFormatters();
   ```

3. **Check Accept header:**
   ```bash
   curl -v -H "Accept: application/xml" http://localhost:5000/api/entity
   ```

### Issue: CORS Errors

**Error:**
```
Access to fetch at 'http://localhost:5000' from origin 'http://localhost:3000' 
has been blocked by CORS policy
```

**Solution:**
```csharp
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();
app.UseCors();
```

## Performance Issues

### Issue: Slow Metadata Loading

**Problem:** Application takes long to start

**Solutions:**

1. **Add indexes to system tables:**
   ```sql
   -- SQL Server
   CREATE INDEX IX_Columns ON INFORMATION_SCHEMA.COLUMNS(TABLE_NAME);
   ```

2. **Increase connection timeout:**
   ```csharp
   options.ConnectionString = "...;Connection Timeout=60;";
   ```

3. **Use caching:**
   ```csharp
   builder.Services.AddMemoryCache();
   builder.Services.AddSingleton<IEntityRepository, CachedEntityRepository>();
   ```

### Issue: High Memory Usage

**Problem:** Application consuming too much memory

**Solutions:**

1. **Enable garbage collection:**
   ```csharp
   GCSettings.LatencyMode = GCLatencyMode.Batch;
   ```

2. **Reduce entity cache:**
   ```csharp
   options.MaxCachedEntities = 100;
   ```

3. **Use server GC:**
   ```xml
   <PropertyGroup>
     <ServerGarbageCollection>true</ServerGarbageCollection>
   </PropertyGroup>
   ```

### Issue: Slow API Responses

**Problem:** API requests are slow

**Solutions:**

1. **Enable response caching:**
   ```csharp
   builder.Services.AddResponseCaching();
   app.UseResponseCaching();
   
   [ResponseCache(Duration = 60)]
   [HttpGet("metadata")]
   public IActionResult GetMetadata()
   ```

2. **Use connection pooling:**
   ```csharp
   options.ConnectionString = "...;Min Pool Size=10;Max Pool Size=100;";
   ```

3. **Add indexes to database:**
   ```sql
   CREATE INDEX IX_Entity_Name ON Entities(Name);
   ```

## Deployment Issues

### Issue: Application Won't Start in Production

**Error:**
```
Connection refused
```

**Solutions:**

1. **Check environment variables:**
   ```bash
   export ConnectionStrings__DefaultConnection="..."
   ```

2. **Verify connection string in production:**
   ```csharp
   if (app.Environment.IsProduction())
   {
       var connStr = builder.Configuration.GetConnectionString("DefaultConnection");
       if (string.IsNullOrEmpty(connStr))
           throw new InvalidOperationException("Production connection string missing");
   }
   ```

3. **Check application URL bindings:**
   ```csharp
   var app = builder.Build();
   app.Urls.Add("http://0.0.0.0:5000");
   ```

### Issue: Docker Container Fails to Connect

**Error:**
```
Cannot connect to host.docker.internal
```

**Solutions:**

1. **Use correct host:**
   ```
   # Instead of localhost, use:
   Server=host.docker.internal;...
   ```

2. **Use Docker network:**
   ```yaml
   # docker-compose.yml
   services:
     api:
       depends_on:
         - sqlserver
       environment:
         - ConnectionStrings__DefaultConnection=Server=sqlserver;...
   ```

3. **Check network connectivity:**
   ```bash
   docker exec -it container_name ping sqlserver
   ```

### Issue: SSL/TLS Errors

**Error:**
```
The certificate chain was issued by an authority that is not trusted
```

**Solutions:**

1. **Trust certificate:**
   ```csharp
   // Development only!
   options.ConnectionString = "...;TrustServerCertificate=True;";
   ```

2. **Install proper certificate:**
   ```bash
   dotnet dev-certs https --trust
   ```

## Getting Help

If you're still experiencing issues:

1. **Check GitHub Issues:** https://github.com/schivei/ddap/issues
2. **Review Examples:** https://github.com/schivei/ddap/tree/main/examples
3. **Enable logging:**
   ```csharp
   builder.Logging.SetMinimumLevel(LogLevel.Debug);
   ```
4. **Open a new issue** with:
   - DDAP version
   - .NET version
   - Database type and version
   - Error message and stack trace
   - Minimal reproduction code

## Common Error Messages

| Error | Likely Cause | Quick Fix |
|-------|--------------|-----------|
| `NullReferenceException` in EntityRepository | Provider not registered | Add `.AddSqlServerDapper()` etc. |
| `InvalidOperationException: No service for type IDataProvider` | Missing provider | Register data provider |
| `SqlException: Login failed` | Wrong credentials | Check connection string |
| `TimeoutException` | Slow query or network | Increase timeout |
| `ArgumentException: Connection string is invalid` | Malformed connection string | Validate format |
| `FileNotFoundException: Could not load file or assembly` | Missing package | Run `dotnet restore` |

## Debug Mode

Enable detailed error messages in development:

```csharp
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    
    builder.Services.AddDdap(options =>
    {
        options.EnableDetailedErrors = true;
        options.EnableSensitiveDataLogging = true;
    });
}
```

## Next Steps

- [Get Started](./get-started.md) - Basic setup and configuration
- [Advanced Usage](./advanced.md) - Extensibility patterns
- [Database Providers](./database-providers.md) - Database-specific documentation
- [API Providers](./api-providers.md) - API protocol details
