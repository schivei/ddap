# Advanced DDAP Usage

This guide covers advanced patterns, extensibility, and best practices for using DDAP in production applications.

## Table of Contents

- [Extensibility](#extensibility)
- [Custom Endpoints](#custom-endpoints)
- [Custom Queries and Mutations](#custom-queries-and-mutations)
- [Advanced Configuration](#advanced-configuration)
- [Auto-Reload Patterns](#auto-reload-patterns)
- [Template Customization](#template-customization)
- [Lifecycle Hooks](#lifecycle-hooks)
- [Performance Optimization](#performance-optimization)
- [Production Best Practices](#production-best-practices)
- [Integration Patterns](#integration-patterns)

## Extensibility

DDAP is designed to be extensible. All generated controllers, services, queries, and mutations are partial classes, allowing you to add custom logic without modifying generated code.

### Extending REST Controllers

Add custom endpoints to the generated `EntityController`:

```csharp
using Microsoft.AspNetCore.Mvc;
using Ddap.Core;

namespace Ddap.Rest;

public partial class EntityController
{
    [HttpGet("search")]
    public async Task<IActionResult> SearchEntities([FromQuery] string query)
    {
        // Custom search logic
        var results = await _repository.SearchAsync(query);
        return Ok(results);
    }

    [HttpPost("{entityName}/bulk")]
    public async Task<IActionResult> BulkCreate(string entityName, [FromBody] List<object> items)
    {
        // Custom bulk creation logic
        foreach (var item in items)
        {
            // Process each item
        }
        return Ok();
    }

    [HttpGet("{entityName}/export")]
    public async Task<IActionResult> Export(string entityName, [FromQuery] string format)
    {
        var entity = _repository.GetEntity(entityName);
        if (entity == null)
            return NotFound();

        // Export logic based on format (CSV, Excel, etc.)
        var data = await ExportData(entity, format);
        return File(data, $"application/{format}");
    }
}
```

### Extending GraphQL Queries

Add custom GraphQL queries:

```csharp
using HotChocolate;
using Ddap.Core;

namespace Ddap.GraphQL;

public partial class Query
{
    public async Task<List<EntityStatistics>> GetEntityStatistics(
        [Service] IEntityRepository repository)
    {
        var stats = new List<EntityStatistics>();
        
        foreach (var entity in repository.Entities)
        {
            stats.Add(new EntityStatistics
            {
                EntityName = entity.Name,
                PropertyCount = entity.Properties.Count,
                IndexCount = entity.Indexes.Count,
                RelationshipCount = entity.Relationships.Count
            });
        }
        
        return stats;
    }

    public string CustomQuery([Service] IEntityRepository repository) 
    {
        return $"Total entities: {repository.Entities.Count}";
    }
}

public class EntityStatistics
{
    public string EntityName { get; set; }
    public int PropertyCount { get; set; }
    public int IndexCount { get; set; }
    public int RelationshipCount { get; set; }
}
```

### Extending GraphQL Mutations

Add custom mutations:

```csharp
namespace Ddap.GraphQL;

public partial class Mutation
{
    public async Task<bool> BulkImport(
        string entityName, 
        List<Dictionary<string, object>> data,
        [Service] IEntityRepository repository)
    {
        var entity = repository.GetEntity(entityName);
        if (entity == null)
            return false;

        // Custom bulk import logic
        foreach (var item in data)
        {
            // Process each item
        }

        return true;
    }
}
```

## Custom Endpoints

### Adding Authentication

Protect endpoints with authentication:

```csharp
using Microsoft.AspNetCore.Authorization;

namespace Ddap.Rest;

public partial class EntityController
{
    [Authorize]
    [HttpGet("{entityName}/secure")]
    public IActionResult SecureEndpoint(string entityName)
    {
        // Only authenticated users can access
        var entity = _repository.GetEntity(entityName);
        return Ok(entity);
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{entityName}/all")]
    public async Task<IActionResult> DeleteAll(string entityName)
    {
        // Only admins can delete all records
        return Ok();
    }
}
```

### Adding Validation

Add custom validation logic:

```csharp
namespace Ddap.Rest;

public partial class EntityController
{
    [HttpPost("{entityName}")]
    public async Task<IActionResult> CreateWithValidation(
        string entityName, 
        [FromBody] Dictionary<string, object> data)
    {
        var entity = _repository.GetEntity(entityName);
        if (entity == null)
            return NotFound();

        // Custom validation
        var validationResult = ValidateData(entity, data);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        // Proceed with creation
        return Created($"/api/entity/{entityName}", data);
    }

    private ValidationResult ValidateData(
        IEntityConfiguration entity, 
        Dictionary<string, object> data)
    {
        var result = new ValidationResult();

        // Check required fields
        foreach (var property in entity.Properties)
        {
            if (property.IsRequired && !data.ContainsKey(property.Name))
            {
                result.AddError($"{property.Name} is required");
            }
        }

        // Custom business rules
        if (entity.Name == "Users" && data.ContainsKey("Email"))
        {
            var email = data["Email"].ToString();
            if (!IsValidEmail(email))
            {
                result.AddError("Invalid email format");
            }
        }

        return result;
    }
}
```

## Custom Queries and Mutations

### Complex GraphQL Queries

Implement complex queries with relationships:

```csharp
namespace Ddap.GraphQL;

public partial class Query
{
    public async Task<EntityDetails> GetEntityDetails(
        string entityName,
        [Service] IEntityRepository repository)
    {
        var entity = repository.GetEntity(entityName);
        if (entity == null)
            throw new GraphQLException($"Entity {entityName} not found");

        return new EntityDetails
        {
            Name = entity.Name,
            Schema = entity.Schema,
            Properties = entity.Properties.Select(p => new PropertyDetails
            {
                Name = p.Name,
                Type = p.DataType,
                IsRequired = p.IsRequired,
                IsPrimaryKey = p.IsPrimaryKey
            }).ToList(),
            Relationships = entity.Relationships.Select(r => new RelationshipDetails
            {
                Name = r.Name,
                TargetEntity = r.TargetEntityName,
                Type = r.RelationshipType.ToString()
            }).ToList()
        };
    }
}
```

### Batch Operations

Implement efficient batch operations:

```csharp
namespace Ddap.GraphQL;

public partial class Mutation
{
    public async Task<BatchResult> BatchOperation(
        List<BatchItem> items,
        [Service] IEntityRepository repository)
    {
        var result = new BatchResult();

        foreach (var item in items)
        {
            try
            {
                switch (item.Operation)
                {
                    case "CREATE":
                        // Create logic
                        result.SuccessCount++;
                        break;
                    case "UPDATE":
                        // Update logic
                        result.SuccessCount++;
                        break;
                    case "DELETE":
                        // Delete logic
                        result.SuccessCount++;
                        break;
                }
            }
            catch (Exception ex)
            {
                result.Errors.Add($"{item.Operation} failed: {ex.Message}");
            }
        }

        return result;
    }
}

public class BatchItem
{
    public string Operation { get; set; }
    public string EntityName { get; set; }
    public Dictionary<string, object> Data { get; set; }
}

public class BatchResult
{
    public int SuccessCount { get; set; }
    public List<string> Errors { get; set; } = new();
}
```

## Advanced Configuration

### Custom JSON Serialization

Configure Newtonsoft.Json settings:

```csharp
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

builder.Services.AddControllers()
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
        options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
        options.SerializerSettings.DateFormatString = "yyyy-MM-ddTHH:mm:ssZ";
        options.SerializerSettings.Converters.Add(new StringEnumConverter());
    });
```

### Multiple Database Connections

Support multiple databases in one application:

```csharp
builder.Services
    .AddDdap(options =>
    {
        options.ConnectionString = builder.Configuration.GetConnectionString("PrimaryDb");
    })
    .AddDapper(() => new SqlConnection(connectionString))
    .AddRest()
    .AddGraphQL();

// Second database
var secondaryConnectionString = builder.Configuration.GetConnectionString("SecondaryDb");
builder.Services
    .AddDdap("SecondaryDb", options =>
    {
        options.ConnectionString = secondaryConnectionString;
    })
    .AddDapper(() => new MySqlConnection(secondaryConnectionString))
    .AddRest();
```

### Environment-Specific Configuration

Different configurations per environment:

```csharp
var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsDevelopment())
{
    var devConnectionString = builder.Configuration.GetConnectionString("Development");
    builder.Services
        .AddDdap(options =>
        {
            options.ConnectionString = devConnectionString;
            options.EnableDetailedErrors = true;
            options.AutoReload = new AutoReloadOptions
            {
                Enabled = true,
                IdleTimeout = TimeSpan.FromSeconds(30),
                Strategy = ReloadStrategy.HotReload  // Fast for dev
            };
        })
        .AddDapper(() => new SqlConnection(devConnectionString))
        .AddRest()
        .AddGraphQL()
        .AddGrpc();
}
else if (builder.Environment.IsProduction())
{
    var prodConnectionString = builder.Configuration.GetConnectionString("Production");
    builder.Services
        .AddDdap(options =>
        {
            options.ConnectionString = prodConnectionString;
            options.EnableDetailedErrors = false;
            options.AutoReload = new AutoReloadOptions
            {
                Enabled = true,
                IdleTimeout = TimeSpan.FromMinutes(10),
                Strategy = ReloadStrategy.InvalidateAndRebuild,  // Thorough for prod
                Behavior = ReloadBehavior.ServeOldSchema  // Zero downtime
            };
        })
        .AddDapper(() => new SqlConnection(prodConnectionString))
        .AddRest()
        .AddGraphQL();
}
```

## Auto-Reload Patterns

The Auto-Reload System enables zero-downtime schema updates. Here are advanced patterns for production use.

### Pattern 1: Conditional Reload

Control when reloads happen based on business logic:

```csharp
builder.Services.AddDdap(options =>
{
    options.AutoReload = new AutoReloadOptions
    {
        Enabled = true,
        IdleTimeout = TimeSpan.FromMinutes(5),
        
        OnBeforeReloadAsync = async (sp) =>
        {
            var logger = sp.GetRequiredService<ILogger<Program>>();
            var config = sp.GetRequiredService<IConfiguration>();
            
            // Check maintenance window
            var now = DateTime.UtcNow;
            if (now.Hour >= 2 && now.Hour <= 4)
            {
                logger.LogInformation("Allowing reload during maintenance window");
                return true;
            }
            
            // Check if manually blocked
            if (config.GetValue<bool>("BlockAutoReload"))
            {
                logger.LogWarning("Auto-reload blocked by configuration");
                return false;
            }
            
            return true;
        }
    };
});
```

### Pattern 2: Coordinated Multi-Instance Reload

In a load-balanced environment, stagger reloads across instances:

```csharp
OnBeforeReloadAsync = async (sp) =>
{
    var cache = sp.GetRequiredService<IDistributedCache>();
    var instanceId = Environment.GetEnvironmentVariable("INSTANCE_ID") ?? "default";
    
    // Check if another instance is reloading
    var lockKey = "ddap:reload:lock";
    var existingLock = await cache.GetStringAsync(lockKey);
    
    if (existingLock != null && existingLock != instanceId)
    {
        // Another instance is reloading, wait
        return false;
    }
    
    // Acquire lock for this instance
    await cache.SetStringAsync(lockKey, instanceId, new DistributedCacheEntryOptions
    {
        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
    });
    
    return true;
}
```

### Pattern 3: Health Check Integration

Integrate reload status with health checks:

```csharp
public class SchemaReloadHealthCheck : IHealthCheck
{
    private readonly IEntityRepository _repository;
    private static DateTime? _lastReload;
    private static bool _isReloading;
    
    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        if (_isReloading)
        {
            return HealthCheckResult.Degraded("Schema reload in progress");
        }
        
        var data = new Dictionary<string, object>
        {
            { "entityCount", _repository.Entities.Count },
            { "lastReload", _lastReload ?? DateTime.UtcNow }
        };
        
        return HealthCheckResult.Healthy("Schema is current", data);
    }
    
    public static void MarkReloadStart() => _isReloading = true;
    public static void MarkReloadComplete()
    {
        _isReloading = false;
        _lastReload = DateTime.UtcNow;
    }
}

// Configure in Program.cs
builder.Services.AddDdap(options =>
{
    options.AutoReload = new AutoReloadOptions
    {
        OnBeforeReloadAsync = async (sp) =>
        {
            SchemaReloadHealthCheck.MarkReloadStart();
            return true;
        },
        OnAfterReloadAsync = async (sp) =>
        {
            SchemaReloadHealthCheck.MarkReloadComplete();
        }
    };
});
```

### Pattern 4: Monitoring and Alerting

Track reload metrics and send alerts:

```csharp
public class ReloadMetrics
{
    public int TotalReloads { get; set; }
    public int FailedReloads { get; set; }
    public List<TimeSpan> ReloadDurations { get; set; } = new();
}

builder.Services.AddSingleton<ReloadMetrics>();
builder.Services.AddDdap(options =>
{
    options.AutoReload = new AutoReloadOptions
    {
        OnBeforeReloadAsync = async (sp) =>
        {
            var metrics = sp.GetRequiredService<ReloadMetrics>();
            var startTime = DateTime.UtcNow;
            
            // Store start time for duration calculation
            sp.GetRequiredService<IMemoryCache>()
                .Set("reload:start", startTime);
            
            return true;
        },
        
        OnAfterReloadAsync = async (sp) =>
        {
            var metrics = sp.GetRequiredService<ReloadMetrics>();
            var cache = sp.GetRequiredService<IMemoryCache>();
            
            if (cache.TryGetValue("reload:start", out DateTime startTime))
            {
                var duration = DateTime.UtcNow - startTime;
                metrics.ReloadDurations.Add(duration);
                metrics.TotalReloads++;
                
                // Alert if reload took too long
                if (duration > TimeSpan.FromSeconds(10))
                {
                    var logger = sp.GetRequiredService<ILogger<Program>>();
                    logger.LogWarning("Slow schema reload: {Duration}ms", duration.TotalMilliseconds);
                }
            }
        },
        
        OnReloadErrorAsync = async (sp, exception) =>
        {
            var metrics = sp.GetRequiredService<ReloadMetrics>();
            metrics.FailedReloads++;
            
            var logger = sp.GetRequiredService<ILogger<Program>>();
            logger.LogError(exception, "Schema reload failed");
            
            // Send alert for production failures
            if (builder.Environment.IsProduction())
            {
                var alerting = sp.GetService<IAlertingService>();
                await alerting?.SendAlertAsync(
                    "DDAP Schema Reload Failure",
                    $"Reload failed: {exception.Message}\nTotal failures: {metrics.FailedReloads}"
                );
            }
        }
    };
});
```

## Template Customization

DDAP templates generate starting projects that you can fully customize. Here are common customization patterns.

### Custom Template Options

Create your own template variants by modifying generated projects:

```bash
# Generate a base project
dotnet new ddap-api --name MyTemplate

# Customize it:
# - Add your authentication strategy
# - Configure logging providers
# - Set up middleware pipeline
# - Add domain services

# Package as a template
dotnet pack -c Release
dotnet new install ./bin/Release/MyTemplate.Templates.1.0.0.nupkg
```

### Post-Generation Scripts

Add scripts to `scripts/` folder in generated projects:

```bash
# scripts/setup.sh
#!/bin/bash
echo "Setting up development environment..."

# Initialize user secrets
dotnet user-secrets init

# Set default connection string
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost;Database=Dev;Integrated Security=true;"

# Restore packages
dotnet restore

# Run migrations (if using EF)
dotnet ef database update

echo "Setup complete!"
```

### Extending Generated Code

All generated code uses partial classes for extension:

```csharp
// Generated/EntityController.g.cs (generated - don't modify)
public abstract partial class EntityController : ControllerBase
{
    // Generated methods...
}

// Controllers/EntityController.cs (your customizations)
public partial class EntityController
{
    private readonly ILogger<EntityController> _logger;
    private readonly IMemoryCache _cache;
    
    public EntityController(
        IEntityRepository repository,
        ILogger<EntityController> logger,
        IMemoryCache cache)
    {
        _repository = repository;
        _logger = logger;
        _cache = cache;
    }
    
    [HttpGet("custom/health")]
    public IActionResult Health()
    {
        return Ok(new { Status = "Healthy", Entities = _repository.Entities.Count });
    }
}
```

## Lifecycle Hooks

DDAP provides lifecycle hooks at various points for custom logic.

### Application Lifecycle

```csharp
public class DdapApplicationLifecycle : IHostedService
{
    private readonly IEntityRepository _repository;
    private readonly ILogger<DdapApplicationLifecycle> _logger;
    
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("DDAP starting - {EntityCount} entities loaded", 
            _repository.Entities.Count);
        
        // Pre-warm caches
        foreach (var entity in _repository.Entities)
        {
            // Load metadata into cache
            await PreloadEntityMetadata(entity);
        }
    }
    
    public async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("DDAP shutting down gracefully");
        
        // Flush any pending operations
        await FlushPendingOperations();
    }
}

// Register in Program.cs
builder.Services.AddHostedService<DdapApplicationLifecycle>();
```

### Request Lifecycle

Hook into request processing:

```csharp
public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;
    
    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }
    
    public async Task InvokeAsync(HttpContext context)
    {
        var startTime = DateTime.UtcNow;
        
        try
        {
            await _next(context);
        }
        finally
        {
            var duration = DateTime.UtcNow - startTime;
            _logger.LogInformation(
                "Request: {Method} {Path} - {StatusCode} - {Duration}ms",
                context.Request.Method,
                context.Request.Path,
                context.Response.StatusCode,
                duration.TotalMilliseconds);
        }
    }
}

// Register in Program.cs
app.UseMiddleware<RequestLoggingMiddleware>();
```

### Entity Repository Events

Subscribe to repository events:

```csharp
public interface IEntityRepositoryEventHandler
{
    Task OnEntityAddedAsync(IEntityConfiguration entity);
    Task OnEntityRemovedAsync(string entityName);
    Task OnRepositoryReloadedAsync();
}

public class EntityChangeLogger : IEntityRepositoryEventHandler
{
    private readonly ILogger<EntityChangeLogger> _logger;
    
    public async Task OnEntityAddedAsync(IEntityConfiguration entity)
    {
        _logger.LogInformation("Entity added: {EntityName} with {PropertyCount} properties",
            entity.Name, entity.Properties.Count);
    }
    
    public async Task OnEntityRemovedAsync(string entityName)
    {
        _logger.LogWarning("Entity removed: {EntityName}", entityName);
    }
    
    public async Task OnRepositoryReloadedAsync()
    {
        _logger.LogInformation("Entity repository reloaded");
    }
}

// Register in Program.cs
builder.Services.AddSingleton<IEntityRepositoryEventHandler, EntityChangeLogger>();
```

## Performance Optimization

### Caching Strategies

Implement caching for frequently accessed data:

```csharp
using Microsoft.Extensions.Caching.Memory;

namespace Ddap.Rest;

public partial class EntityController
{
    private readonly IMemoryCache _cache;

    public EntityController(IEntityRepository repository, IMemoryCache cache)
    {
        _repository = repository;
        _cache = cache;
    }

    [HttpGet("{entityName}/cached")]
    public async Task<IActionResult> GetCached(string entityName)
    {
        var cacheKey = $"entity_{entityName}";
        
        if (!_cache.TryGetValue(cacheKey, out IEntityConfiguration entity))
        {
            entity = _repository.GetEntity(entityName);
            
            var cacheOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromMinutes(5));
            
            _cache.Set(cacheKey, entity, cacheOptions);
        }

        return Ok(entity);
    }
}
```

### Connection Pooling

Optimize database connections:

```csharp
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services
    .AddDdap(options =>
    {
        options.ConnectionString = connectionString;
        options.MaxPoolSize = 100;
        options.MinPoolSize = 10;
        options.ConnectionTimeout = 30;
    })
    .AddDapper(() => new SqlConnection(connectionString));
```

### Lazy Loading

Implement lazy loading for relationships:

```csharp
public class EntityData
{
    public string Name { get; set; }
    
    private List<RelatedEntity> _relatedEntities;
    public List<RelatedEntity> RelatedEntities
    {
        get
        {
            if (_relatedEntities == null)
            {
                // Load only when accessed
                _relatedEntities = LoadRelatedEntities();
            }
            return _relatedEntities;
        }
    }
}
```

## Production Best Practices

### Health Checks

Add health checks for monitoring:

```csharp
builder.Services.AddHealthChecks()
    .AddSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));

var app = builder.Build();

app.MapHealthChecks("/health");
```

### Logging

Configure structured logging:

```csharp
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/ddap-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();
```

### Error Handling

Implement global error handling:

```csharp
public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "An error occurred");

        var response = new
        {
            Message = "An error occurred while processing your request",
            Details = exception.Message
        };

        httpContext.Response.StatusCode = 500;
        await httpContext.Response.WriteAsJsonAsync(response, cancellationToken);

        return true;
    }
}

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
```

### Rate Limiting

Implement rate limiting:

```csharp
using AspNetCoreRateLimit;

builder.Services.AddMemoryCache();
builder.Services.Configure<IpRateLimitOptions>(builder.Configuration.GetSection("IpRateLimiting"));
builder.Services.AddInMemoryRateLimiting();
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

var app = builder.Build();
app.UseIpRateLimiting();
```

### CORS Configuration

Configure CORS for cross-origin requests:

```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        policy =>
        {
            policy.WithOrigins("https://example.com")
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

var app = builder.Build();
app.UseCors("AllowSpecificOrigin");
```

## Integration Patterns

### Message Queue Integration

Integrate with message queues for async processing:

```csharp
using RabbitMQ.Client;

namespace Ddap.Rest;

public partial class EntityController
{
    private readonly IConnection _connection;

    [HttpPost("{entityName}/async")]
    public async Task<IActionResult> CreateAsync(
        string entityName, 
        [FromBody] Dictionary<string, object> data)
    {
        // Queue the operation
        using var channel = _connection.CreateModel();
        channel.QueueDeclare(queue: "entity-operations", durable: true);
        
        var message = JsonConvert.SerializeObject(new
        {
            EntityName = entityName,
            Operation = "CREATE",
            Data = data
        });
        
        var body = Encoding.UTF8.GetBytes(message);
        channel.BasicPublish(exchange: "", routingKey: "entity-operations", body: body);
        
        return Accepted();
    }
}
```

### Event Sourcing

Implement event sourcing pattern:

```csharp
public interface IEventStore
{
    Task AppendAsync(string entityName, object @event);
    Task<List<object>> GetEventsAsync(string entityName);
}

namespace Ddap.Rest;

public partial class EntityController
{
    private readonly IEventStore _eventStore;

    [HttpPost("{entityName}/with-events")]
    public async Task<IActionResult> CreateWithEvents(
        string entityName,
        [FromBody] Dictionary<string, object> data)
    {
        // Store event
        await _eventStore.AppendAsync(entityName, new EntityCreatedEvent
        {
            EntityName = entityName,
            Data = data,
            Timestamp = DateTime.UtcNow
        });

        // Create entity
        // ...

        return Created($"/api/entity/{entityName}", data);
    }
}
```

### Webhook Notifications

Send webhook notifications on entity changes:

```csharp
public interface IWebhookService
{
    Task NotifyAsync(string eventType, object data);
}

namespace Ddap.Rest;

public partial class EntityController
{
    private readonly IWebhookService _webhookService;

    [HttpPost("{entityName}")]
    public async Task<IActionResult> CreateWithWebhook(
        string entityName,
        [FromBody] Dictionary<string, object> data)
    {
        // Create entity
        // ...

        // Send webhook notification
        await _webhookService.NotifyAsync("entity.created", new
        {
            EntityName = entityName,
            Data = data
        });

        return Created($"/api/entity/{entityName}", data);
    }
}
```

## Testing Custom Extensions

### Unit Testing Custom Endpoints

```csharp
using Xunit;
using Moq;

public class EntityControllerTests
{
    [Fact]
    public async Task CustomEndpoint_ReturnsOk()
    {
        // Arrange
        var mockRepository = new Mock<IEntityRepository>();
        var controller = new EntityController(mockRepository.Object);

        // Act
        var result = await controller.CustomEndpoint("test");

        // Assert
        Assert.IsType<OkObjectResult>(result);
    }
}
```

### Integration Testing

```csharp
using Microsoft.AspNetCore.Mvc.Testing;

public class ApiIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public ApiIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task CustomEndpoint_ReturnsSuccess()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/entity/custom/test");

        // Assert
        response.EnsureSuccessStatusCode();
    }
}
```

## Next Steps

- [Philosophy](./philosophy.md) - Developer in Control philosophy
- [Templates](./templates.md) - Project templates and quick start
- [Auto-Reload](./auto-reload.md) - Auto-reload configuration and patterns
- [Troubleshooting](./troubleshooting.md) - Common issues and solutions
- [Database Providers](./database-providers.md) - Database-specific features
- [API Providers](./api-providers.md) - API-specific features

## Additional Resources

- [ASP.NET Core Best Practices](https://docs.microsoft.com/aspnet/core/)
- [GraphQL Best Practices](https://graphql.org/learn/best-practices/)
- [gRPC Best Practices](https://grpc.io/docs/guides/)
