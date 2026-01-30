# Auto-Reload System

## Overview

The Auto-Reload System in DDAP automatically detects changes to your database schema and reloads the GraphQL schema without requiring application restarts.

**Why Auto-Reload is Useful:**
- Zero-downtime schema updates
- Faster development cycles
- Reduced deployment complexity
- Continuous availability during schema updates

## Quick Start

```csharp
builder.Services.AddDdap(options =>
{
    options.ConnectionString = "Server=localhost;Database=MyDb;...";
    options.AutoReload = new AutoReloadOptions
    {
        Enabled = true,
        IdleTimeout = TimeSpan.FromMinutes(5)
    };
});
```

## Configuration Options

```csharp
public class AutoReloadOptions
{
    public bool Enabled { get; set; } = false;
    public TimeSpan IdleTimeout { get; set; } = TimeSpan.FromMinutes(5);
    public ReloadStrategy Strategy { get; set; } = ReloadStrategy.InvalidateAndRebuild;
    public ReloadBehavior Behavior { get; set; } = ReloadBehavior.ServeOldSchema;
    public ChangeDetection ChangeDetection { get; set; } = ChangeDetection.CheckHash;
    public Func<IServiceProvider, Task<bool>>? OnBeforeReloadAsync { get; set; }
    public Func<IServiceProvider, Task>? OnAfterReloadAsync { get; set; }
    public Func<IServiceProvider, Exception, Task>? OnReloadErrorAsync { get; set; }
}
```

## Reload Strategies

### InvalidateAndRebuild
Complete schema rebuild from scratch - most thorough but slowest.

```csharp
Strategy = ReloadStrategy.InvalidateAndRebuild
```

**Trade-offs:**
- ✅ Handles all schema changes reliably
- ❌ Slowest (1-5 seconds)

### HotReload
Incremental updates without full rebuild - fastest but may not handle all changes.

```csharp
Strategy = ReloadStrategy.HotReload
```

**Trade-offs:**
- ✅ Fastest (100-500ms)
- ❌ Cannot handle breaking changes or renames

### RestartExecutor
Restart GraphQL executor - balanced approach.

```csharp
Strategy = ReloadStrategy.RestartExecutor
```

**Trade-offs:**
- ✅ Moderate speed (500ms-2s)
- ✅ Handles most changes

## Reload Behaviors

### ServeOldSchema
Continue serving requests with old schema during reload - zero downtime.

```csharp
Behavior = ReloadBehavior.ServeOldSchema
```

**Trade-offs:**
- ✅ Zero downtime
- ❌ 2x memory during reload

### BlockRequests
Reject new requests during reload - returns HTTP 503.

```csharp
Behavior = ReloadBehavior.BlockRequests
```

**Trade-offs:**
- ✅ Lower memory usage
- ❌ Requests fail during reload

### QueueRequests
Queue requests during reload and process after - balanced approach.

```csharp
Behavior = ReloadBehavior.QueueRequests
```

**Trade-offs:**
- ✅ No failed requests
- ❌ Increased latency during reload

## Change Detection

### AlwaysReload
Always reload after idle timeout without checking for changes.

```csharp
ChangeDetection = ChangeDetection.AlwaysReload
```

**Trade-offs:**
- ✅ Simple and predictable
- ❌ Wastes resources on unnecessary reloads

### CheckHash
Compare schema hash to detect changes - reliable and database-agnostic.

```csharp
ChangeDetection = ChangeDetection.CheckHash
```

**Trade-offs:**
- ✅ Catches all schema changes
- ❌ Higher overhead than timestamps

**Hash includes:** Tables, columns, types, keys, indexes, constraints

### CheckTimestamps
Check database object modification timestamps - fastest method.

```csharp
ChangeDetection = ChangeDetection.CheckTimestamps
```

**Trade-offs:**
- ✅ Fastest detection
- ❌ Database-specific, may miss some changes

**Database Support:**
- SQL Server: `sys.objects.modify_date`
- PostgreSQL: `pg_class.relfilenode`
- MySQL: `information_schema.tables.update_time`

## Lifecycle Hooks

Execute custom logic at specific points in the reload process.

### OnBeforeReloadAsync
Invoked before reload starts. Return `true` to proceed, `false` to cancel.

```csharp
OnBeforeReloadAsync = async (sp) =>
{
    var logger = sp.GetRequiredService<ILogger<Program>>();
    logger.LogInformation("Schema reload starting");
    
    // Check if reload should proceed
    var cache = sp.GetService<IMemoryCache>();
    if (cache?.Get("BlockReload") is true)
        return false; // Cancel reload
    
    return true; // Proceed
}
```

### OnAfterReloadAsync
Invoked after reload completes successfully.

```csharp
OnAfterReloadAsync = async (sp) =>
{
    var logger = sp.GetRequiredService<ILogger<Program>>();
    logger.LogInformation("Schema reload completed");
    
    // Warm up caches with new schema
    var repository = sp.GetRequiredService<IEntityRepository>();
    await repository.WarmCacheAsync();
}
```

### OnReloadErrorAsync
Invoked when reload fails.

```csharp
OnReloadErrorAsync = async (sp, exception) =>
{
    var logger = sp.GetRequiredService<ILogger<Program>>();
    logger.LogError(exception, "Schema reload failed");
    
    // Send alert
    var alerting = sp.GetService<IAlertingService>();
    await alerting?.SendAlertAsync("Schema Reload Failed", exception.Message);
}
```

## Events

Subscribe to reload events for monitoring and diagnostics.

```csharp
builder.Services.AddSingleton<IReloadEventHandler, MyReloadEventHandler>();

public class MyReloadEventHandler : IReloadEventHandler
{
    public Task OnReloadStartedAsync(ReloadStartedEvent evt)
    {
        // Log reload start
        return Task.CompletedTask;
    }
    
    public Task OnReloadCompletedAsync(ReloadCompletedEvent evt)
    {
        // Log duration and entity count
        return Task.CompletedTask;
    }
    
    public Task OnReloadFailedAsync(ReloadFailedEvent evt)
    {
        // Log error
        return Task.CompletedTask;
    }
}
```

## Complete Example

Production-ready configuration:

```csharp
builder.Services.AddDdap(options =>
{
    options.ConnectionString = connectionString;
    options.AutoReload = new AutoReloadOptions
    {
        Enabled = true,
        IdleTimeout = TimeSpan.FromMinutes(3),
        Strategy = ReloadStrategy.InvalidateAndRebuild,
        Behavior = ReloadBehavior.ServeOldSchema,
        ChangeDetection = ChangeDetection.CheckHash,
        
        OnBeforeReloadAsync = async (sp) =>
        {
            var logger = sp.GetRequiredService<ILogger<Program>>();
            logger.LogInformation("Preparing for schema reload");
            
            // Check if blocked by configuration
            var config = sp.GetRequiredService<IConfiguration>();
            if (config.GetValue<bool>("BlockAutoReload"))
                return false;
            
            return true;
        },
        
        OnAfterReloadAsync = async (sp) =>
        {
            var logger = sp.GetRequiredService<ILogger<Program>>();
            logger.LogInformation("Schema reload completed");
            
            // Clear distributed cache
            var cache = sp.GetService<IDistributedCache>();
            await cache?.RemoveAsync("schema-metadata");
            
            // Warm up new schema
            var repository = sp.GetRequiredService<IEntityRepository>();
            await repository.PreloadMetadataAsync();
        },
        
        OnReloadErrorAsync = async (sp, exception) =>
        {
            var logger = sp.GetRequiredService<ILogger<Program>>();
            logger.LogError(exception, "Schema reload failed");
            
            // Send alert in production
            if (builder.Environment.IsProduction())
            {
                var alerting = sp.GetService<IAlertingService>();
                await alerting?.SendAlertAsync(
                    "DDAP Schema Reload Failed",
                    exception.Message
                );
            }
        }
    };
});
```

## Best Practices

**1. Environment-Specific Configuration**
```csharp
if (builder.Environment.IsDevelopment())
{
    options.AutoReload = new AutoReloadOptions
    {
        Strategy = ReloadStrategy.HotReload,
        IdleTimeout = TimeSpan.FromSeconds(30)
    };
}
else
{
    options.AutoReload = new AutoReloadOptions
    {
        Strategy = ReloadStrategy.InvalidateAndRebuild,
        IdleTimeout = TimeSpan.FromMinutes(5)
    };
}
```

**2. Appropriate Idle Timeouts**
- Development: 30s-1m (fast feedback)
- Staging: 2-3m (balance)
- Production: 5-10m (stability)

**3. Implement Monitoring**
Track reload frequency, duration, and failure rate using event handlers.

**4. Consider Database Size**
For large schemas (1000+ tables):
- Use `CheckTimestamps` instead of `CheckHash`
- Increase `IdleTimeout` to reduce check frequency
- Consider `HotReload` for minor changes

**5. Test Reload Scenarios**
Test adding/removing tables, changing columns, and heavy traffic during reload.

## Troubleshooting

### Schema Not Reloading
**Problem:** Database changes not reflected in API.

**Solutions:**
- Verify `Enabled = true`
- Check idle timeout has passed
- Test with `ChangeDetection.AlwaysReload`
- Check logs for reload errors
- Ensure database permissions for metadata queries

### Frequent Unnecessary Reloads
**Problem:** Schema reloads without changes.

**Solutions:**
- Switch to `CheckHash` or `CheckTimestamps`
- Increase `IdleTimeout`
- Investigate what's triggering changes

### High Memory Usage
**Problem:** Memory spikes during reload.

**Solutions:**
- Use `BlockRequests` instead of `ServeOldSchema`
- Switch to `HotReload` strategy
- Increase `IdleTimeout` to reduce frequency

### Requests Failing During Reload
**Problem:** 503 errors with `BlockRequests`.

**Solutions:**
- Switch to `ServeOldSchema` for zero downtime
- Use `QueueRequests` to buffer requests
- Implement client retry logic

### Reload Takes Too Long
**Problem:** Unacceptable reload duration.

**Solutions:**
- Use faster strategy (`HotReload` > `RestartExecutor` > `InvalidateAndRebuild`)
- Filter tables to reduce schema size
- Optimize database metadata queries

## Performance Considerations

### Memory Usage

| Behavior | Memory Usage | Notes |
|----------|--------------|-------|
| `ServeOldSchema` | ~2x during reload | Two complete schemas in memory |
| `QueueRequests` | 1x + queue | Queue size depends on request rate |
| `BlockRequests` | 1x | Minimal overhead |

### Reload Duration

| Strategy | Typical Duration | Scales With |
|----------|------------------|-------------|
| `HotReload` | 100-500ms | Number of changes |
| `RestartExecutor` | 500ms-2s | Total tables |
| `InvalidateAndRebuild` | 1-5s | Total tables + columns |

### Change Detection Overhead

| Method | Check Time | Database Load |
|--------|------------|---------------|
| `AlwaysReload` | 0ms | None |
| `CheckHash` | 50-500ms | Reads all metadata |
| `CheckTimestamps` | 10-50ms | Reads system tables only |

### Recommendations

**Small Schemas (<100 tables):**
- Use `InvalidateAndRebuild` + `CheckHash`
- Impact is minimal, reliability is worth it

**Medium Schemas (100-500 tables):**
- Use `RestartExecutor` + `CheckHash`
- Balance between speed and reliability

**Large Schemas (500+ tables):**
- Use `HotReload` + `CheckTimestamps`
- Prioritize performance
- Test thoroughly for edge cases

---

For more information, see:
- [Architecture Documentation](./architecture.md)
- [Advanced Usage Guide](./advanced.md)
- [Troubleshooting Guide](./troubleshooting.md)
