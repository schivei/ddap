# Why DDAP?

Choosing the right API framework can make or break your project. DDAP takes a radically different approach to API generation—one that empowers you instead of constraining you.

## The Problem: Framework Lock-In

Most API frameworks promise to speed up development, but they come with hidden costs:

### 🔒 Forced Dependencies

You're locked into specific libraries whether you like them or not:
- **Serializers**: Forced to use Newtonsoft.Json when you want System.Text.Json
- **ORMs**: Locked into Entity Framework when Dapper is better for your use case  
- **Logging**: Can't switch from the framework's built-in logger
- **Validation**: Stuck with their validation library

**Example Problem**: Your framework uses Newtonsoft.Json internally. You want to switch to System.Text.Json for a 3x performance boost. **You can't**—it's hardcoded everywhere. You're stuck.

### 🔒 Hidden Magic

Frameworks do "magic" behind the scenes that you can't see, debug, or modify:
- Auto-generated code you can't inspect
- Convention-based behavior that breaks when you deviate
- Black-box query generation
- Opaque caching strategies

**Example Problem**: Your API is slow. The framework generates inefficient queries. You can't optimize them because you can't see or control the query generation. **You're stuck**.

### 🔒 Database Coupling

Many frameworks are tightly coupled to specific database providers:
- MySQL framework won't work with PostgreSQL
- Different packages for each database
- Migration pain when switching databases
- Can't support multiple databases in same app

**Example Problem**: You built with MySQL. Now you need to support PostgreSQL for a major client. **You're rewriting everything**.

### 🔒 Migration Nightmares

When you outgrow the framework or need to change direction:
- Rewrite large portions of your codebase
- Vendor lock-in makes switching expensive
- Technical debt accumulates over time
- "It works, don't touch it" mentality sets in

**Example Problem**: You started with a simple CRUD API. Now you need GraphQL. Your framework doesn't support it. **You're building a second API from scratch**.

---

## The DDAP Solution: Infrastructure, Not Opinion

DDAP takes a completely different approach. We provide **infrastructure** without forcing any decisions.

### ✅ Developer Empowerment

| You Choose | Not the Framework |
|-----------|------------------|
| 🎯 Database type (SQL Server, MySQL, PostgreSQL, SQLite, custom) | |
| 🎯 ORM (Dapper or Entity Framework) | |
| 🎯 JSON serializer (System.Text.Json, Newtonsoft.Json, or any) | |
| 🎯 API protocols (REST, GraphQL, gRPC, or all three) | |
| 🎯 Validation library (FluentValidation, DataAnnotations, custom) | |
| 🎯 Authentication scheme (JWT, OAuth, custom) | |
| 🎯 Logging framework (Serilog, NLog, built-in, custom) | |
| 🎯 **Everything else!** | |

### ✅ Explicit, Not Magic

Everything DDAP does is transparent and under your control:

```csharp
// You see and control everything
var ddapBuilder = builder.Services.AddDdap(options =>
{
    options.ConnectionString = connectionString;
    options.AutoReload = new AutoReloadOptions
    {
        Enabled = true,
        IdleTimeout = TimeSpan.FromMinutes(5)
    };
});

// You choose your data access
ddapBuilder.AddDapper();  // Or AddEntityFramework()

// You choose your API protocols
ddapBuilder.AddRest();     // Or not
ddapBuilder.AddGraphQL();  // Or not  
ddapBuilder.AddGrpc();     // Or not
```

No hidden configuration. No magic. **You're in control.**

### ✅ Database Agnostic

One package works with **any** database:

```xml
<!-- Single generic Dapper package -->
<PackageReference Include="Ddap.Data.Dapper" Version="1.*" />

<!-- Choose YOUR database driver -->
<PackageReference Include="Microsoft.Data.SqlClient" Version="5.*" />
<!-- OR -->
<PackageReference Include="MySqlConnector" Version="2.*" />
<!-- OR -->
<PackageReference Include="Npgsql" Version="8.*" />
<!-- OR any IDbConnection provider -->
```

**Switch databases?** Just swap the driver. No code changes needed.

**Support multiple databases?** Use different connections. DDAP doesn't care.

### ✅ Official Packages First

Unlike other frameworks that force community packages, DDAP prioritizes **official vendor packages**:

- ✅ SQL Server: `Microsoft.Data.SqlClient` (Microsoft official)
- ✅ MySQL: `MySql.EntityFrameworkCore` (Oracle official)
- ✅ PostgreSQL: `Npgsql` (official)
- ✅ SQLite: `Microsoft.Data.Sqlite` (Microsoft official)

**Want a community package instead?** (e.g., Pomelo for MySQL)  
Fine! Switch it. We don't force anything. We just default to official packages.

### ✅ Zero Migration Pain

DDAP grows with your needs:

**Start simple:**
```csharp
ddapBuilder.AddRest();
```

**Add GraphQL later:**
```csharp
ddapBuilder.AddRest();
ddapBuilder.AddGraphQL();  // Just add this line
```

**Add gRPC too:**
```csharp
ddapBuilder.AddRest();
ddapBuilder.AddGraphQL();
ddapBuilder.AddGrpc();     // Just add this line
```

**Same codebase. Same entities. Multiple protocols.**

---

## Real-World Scenarios

### Scenario 1: Performance Optimization

**Other Framework:**
> "We need to optimize this query, but the framework generates it automatically and we can't modify it without hacking the framework internals."

**DDAP:**
> "Let's switch from Entity Framework to Dapper for this hot path."

```csharp
// Keep EF for most of your app
ddapBuilder.AddEntityFramework();

// Use Dapper for specific hot paths
public class PerformanceRepository
{
    private readonly IDbConnection _connection;
    
    public async Task<List<Order>> GetTopOrdersAsync()
    {
        // Raw Dapper query for maximum performance
        return await _connection.QueryAsync<Order>(
            "SELECT TOP 100 * FROM Orders ORDER BY Total DESC");
    }
}
```

**Result:** 10x faster queries where it matters, without rewriting your entire app.

### Scenario 2: Multi-Database Support

**Other Framework:**
> "We built with MySQL. Now a major client requires SQL Server. We need to rewrite the entire data layer."

**DDAP:**
> "Just swap the connection."

```csharp
// Before (MySQL)
services.AddDdap(options => options.ConnectionString = mySqlConnection)
    .AddDapper(() => new MySqlConnection(mySqlConnection));

// After (SQL Server)
services.AddDdap(options => options.ConnectionString = sqlServerConnection)
    .AddDapper(() => new SqlConnection(sqlServerConnection));
```

**Result:** Same code, different database. Deploy to both clients.

### Scenario 3: API Evolution

**Other Framework:**
> "Our REST API is working great, but now we need GraphQL for the mobile team and gRPC for microservices. Time to build separate APIs."

**DDAP:**
> "Enable more protocols."

```csharp
// Week 1: REST only
ddapBuilder.AddRest();

// Week 5: Add GraphQL for mobile
ddapBuilder.AddRest();
ddapBuilder.AddGraphQL();

// Week 10: Add gRPC for microservices
ddapBuilder.AddRest();
ddapBuilder.AddGraphQL();
ddapBuilder.AddGrpc();
```

**Result:** One codebase, three protocols. Same entities, same logic.

---

## The DDAP Philosophy

### "Developer in Control"

DDAP was built on a single principle: **You should control your architecture, not your framework.**

We believe:
- ✅ **Explicit is better than implicit** - No magic, no surprises
- ✅ **Choice is better than constraint** - You pick your stack
- ✅ **Official is better than community** - Use vendor packages by default
- ✅ **Infrastructure, not opinion** - We provide tools, you make decisions

### What DDAP Provides

Think of DDAP as **scaffolding**:

```
┌─────────────────────────────────────┐
│         YOUR APPLICATION            │
│  (Your choices, your code)          │
├─────────────────────────────────────┤
│            DDAP Layer               │
│  (Infrastructure only)              │
│  - Entity discovery                 │
│  - Metadata mapping                 │
│  - Base API types                   │
│  - Auto-reload system               │
│  - Lifecycle hooks                  │
├─────────────────────────────────────┤
│         YOUR CHOICES                │
│  - Database driver                  │
│  - ORM (Dapper/EF)                  │
│  - Serializer                       │
│  - API protocols                    │
│  - Everything else                  │
└─────────────────────────────────────┘
```

DDAP sits in the middle, providing infrastructure without opinions.

### What DDAP Doesn't Do

We intentionally **don't**:
- ❌ Force specific libraries on you
- ❌ Hide what's happening
- ❌ Lock you into patterns
- ❌ Make architectural decisions for you
- ❌ Generate code you can't modify
- ❌ Prevent you from doing what you need

**If DDAP gets in your way, that's a bug.** [Report it](https://github.com/schivei/ddap/issues).

---

## Comparison: DDAP vs Others

| Feature | Other Frameworks | DDAP |
|---------|------------------|------|
| **Serializer** | Forced (usually Newtonsoft) | Your choice |
| **ORM** | Forced (usually EF) | Dapper or EF, your choice |
| **Database** | Database-specific packages | One package, any database |
| **API Style** | Pick one: REST OR GraphQL | All three: REST + GraphQL + gRPC |
| **Code Generation** | Hidden, can't modify | Partial classes, extend anywhere |
| **Configuration** | Convention-based, hidden | Explicit, in your Program.cs |
| **Query Control** | Framework-generated only | Write raw SQL when needed |
| **Migration Path** | Rewrite when outgrown | Add features incrementally |
| **Philosophy** | "Our way or highway" | "Developer in Control" |

---

## Who Should Use DDAP?

### ✅ Perfect For:

- **Experienced developers** who know what they want
- **Teams** who need flexibility for diverse requirements
- **Projects** that need to evolve without rewrites
- **Organizations** supporting multiple databases
- **Performance-critical** applications needing fine-grained control
- **Architects** who want to own their technical decisions

### ❌ Maybe Not For:

- **Beginners** wanting everything decided for them
- **Throwaway prototypes** (though DDAP works great here too)
- **Projects** where "just works" is more important than control

---

## Getting Started

Ready to take control?

```bash
# Install template

# Create project with YOUR choices
    --database-type sqlserver \
    --database-provider dapper \
    --include-rest \
    --include-graphql

# You're in control from day one
cd MyApi
dotnet run
```

---

## Learn More

- [Get Started](get-started.md) - Quick start guide
- [Philosophy](philosophy.md) - Deep dive into "Developer in Control"
- [Database Providers](database-providers.md) - Database setup guide
- [API Providers](api-providers.md) - REST, GraphQL, gRPC details
- [GitHub](https://github.com/schivei/ddap) - Source code and issues

---

## Questions?

**"Isn't having choices more complex?"**  
Yes, you make decisions. But you're making them anyway—other frameworks just hide them and make them for you. DDAP makes them explicit and changeable.

**"Why not just use [other framework]?"**  
If it works for you, great! Use it. DDAP is for developers who've been burned by framework lock-in and want control.

**"Is DDAP production-ready?"**  
Yes. DDAP is built for production. We have real applications running on it. See [GitHub](https://github.com/schivei/ddap) for status.

**"Does DDAP have a community?"**  
Growing! Check [GitHub Discussions](https://github.com/schivei/ddap/discussions) and [Issues](https://github.com/schivei/ddap/issues).

---

<div align="center">

**Ready to take control?**

[Get Started](get-started.md) | [View on GitHub](https://github.com/schivei/ddap)

</div>
