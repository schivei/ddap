# 🎛️ DDAP - Developer in Control

> Dynamic Data API Provider. You control everything. We handle the boilerplate.

[![NuGet](https://img.shields.io/nuget/v/Ddap.Core)](https://www.nuget.org/packages/Ddap.Core)
[![License](https://img.shields.io/github/license/schivei/ddap)](LICENSE)
[![Build](https://img.shields.io/github/actions/workflow/status/schivei/ddap/build.yml)](https://github.com/schivei/ddap/actions)

## ⚡ What is DDAP?

DDAP automatically generates REST, GraphQL, and gRPC APIs from your database schema—but **without forcing any decisions on you**.

Unlike other frameworks that lock you into specific libraries, databases, or patterns, DDAP provides **infrastructure only**. You choose:
- 🗄️ Your database (SQL Server, MySQL, PostgreSQL, SQLite, or custom)
- 🔧 Your ORM (Dapper or Entity Framework)
- 🎨 Your serializer (System.Text.Json, Newtonsoft.Json, or any)
- 🌐 Your API style (REST, GraphQL, gRPC, or all three)

**DDAP discovers your schema, generates base types, and gets out of your way.**

---

## 🎯 Developer in Control

| What DDAP Provides | What You Control |
|--------------------|------------------|
| ✅ Entity discovery from database | 🎯 Database type (SQL Server, MySQL, etc.) |
| ✅ Metadata mapping (tables, columns, keys) | 🎯 ORM choice (Dapper or Entity Framework) |
| ✅ Base API types (controllers, queries, services) | 🎯 Serialization library (any JSON library) |
| ✅ Auto-Reload infrastructure | 🎯 Auto-Reload configuration (when, how) |
| ✅ Hooks and lifecycle callbacks | 🎯 GraphQL configuration (complete control) |
| ✅ Partial classes for extension | 🎯 REST configuration (formatters, routing) |
| ✅ Project templates (`dotnet new`) | 🎯 gRPC configuration (services, options) |
| | 🎯 **Everything else!** |

### ❌ Other Frameworks vs ✅ DDAP

```
┌─────────────────────────────────────┐
│  🚫 Opinionated Frameworks          │
│  ❌ Force Newtonsoft.Json           │
│  ❌ Hardcode XML/YAML formatters    │
│  ❌ Database-specific packages      │
│  ❌ Hidden configurations           │
│  ❌ Lock you into patterns          │
└─────────────────────────────────────┘

┌─────────────────────────────────────┐
│  ✅ DDAP - Developer in Control     │
│  ✅ You choose serializer           │
│  ✅ You configure formatters        │
│  ✅ Single Dapper, ANY database     │
│  ✅ Everything explicit             │
│  ✅ You own the architecture        │
└─────────────────────────────────────┘
```

---

## 🚀 Quick Start

### 1. Install packages

```bash
dotnet add package Ddap.Core
dotnet add package Ddap.Data.Dapper  # OR Ddap.Data.EntityFramework
dotnet add package Ddap.GraphQL      # Optional
dotnet add package Ddap.Rest         # Optional
dotnet add package Ddap.Grpc         # Optional
```

### 2. Configure (Dapper example)

```csharp
using Microsoft.Data.SqlClient;

var builder = WebApplication.CreateBuilder(args);

// YOU choose the database connection
builder.Services.AddDdap()
    .AddDapper(() => new SqlConnection(
        builder.Configuration.GetConnectionString("DefaultConnection")
    ))
    .AddRest()
    .AddGraphQL(graphql =>
    {
        // YOU configure HotChocolate
        graphql
            .AddFiltering()
            .AddSorting()
            .AddProjections();
    });

var app = builder.Build();
app.MapControllers();
app.MapGraphQL();
app.Run();
```

### 3. Done! 🎉

- REST: `GET /api/entity`
- GraphQL: `POST /graphql { entities { name } }`

### OR Use the Template

```bash
dotnet new install Ddap.Templates
dotnet new ddap-api --name MyApi
cd MyApi
dotnet run
```

---

## ✨ Features

### 🗄️ Database Agnostic
- **Dapper:** Works with ANY `IDbConnection` (SQL Server, MySQL, PostgreSQL, SQLite, Oracle, etc.)
- **Entity Framework:** Use your existing `DbContext`

### 🌐 Multi-Protocol APIs
- **REST:** Standard HTTP/JSON endpoints with full controller customization
- **GraphQL:** Powered by HotChocolate, fully configurable
- **gRPC:** High-performance RPC, configurable services

### 🔄 Auto-Reload System
Automatically reloads database schema after idle periods:
- ✅ **3 Strategies:** InvalidateAndRebuild, HotReloadIncremental, RestartExecutor
- ✅ **3 Behaviors:** ServeOldSchema, BlockRequests, QueueRequests
- ✅ **3 Detection Methods:** AlwaysReload, CheckHash, CheckTimestamps
- ✅ **Lifecycle Hooks:** OnBeforeReloadAsync, OnAfterReloadAsync

```csharp
options.AutoReload = new AutoReloadOptions
{
    Enabled = true,
    IdleTimeout = TimeSpan.FromMinutes(5),
    Strategy = ReloadStrategy.InvalidateAndRebuild,
    Behavior = ReloadBehavior.ServeOldSchema,
    Detection = ChangeDetection.CheckHash
};
```

### 📦 Project Templates
```bash
dotnet new ddap-api --database-provider dapper --database-type mysql --api-providers "rest,graphql"
```

### 🎛️ Zero Opinions
- No forced dependencies
- No hidden configurations
- No magic behavior
- **You configure everything**

### 🔧 Fully Extensible
```csharp
// Extend via partial classes
namespace Ddap.Rest;

public partial class EntityController
{
    [HttpGet("custom")]
    public IActionResult Custom() => Ok("Your endpoint");
}
```

---

## 🏗️ Architecture

```
┌───────────────────────────────────────────┐
│          Your Application                 │
│  (Controllers, Services, Business Logic)  │
└───────────────┬───────────────────────────┘
                │
┌───────────────▼───────────────────────────┐
│        DDAP Core Infrastructure           │
│  ✅ Entity Discovery                      │
│  ✅ Metadata Mapping                      │
│  ✅ Base Type Generation                  │
│  ✅ Auto-Reload Management                │
│  ✅ Lifecycle Hooks                       │
└───────────────┬───────────────────────────┘
                │
┌───────────────▼───────────────────────────┐
│       Your Configuration Choices          │
│  🎯 Database: SQL Server / MySQL / etc.  │
│  🎯 ORM: Dapper / Entity Framework        │
│  🎯 Serializer: System.Text.Json / etc.  │
│  🎯 APIs: REST / GraphQL / gRPC           │
└───────────────────────────────────────────┘
```

---

## 📦 Packages

| Package | Description | Status |
|---------|-------------|--------|
| **Server Packages** | | |
| `Ddap.Core` | Core abstractions and infrastructure | ✅ Stable |
| `Ddap.Data.Dapper` | Dapper provider (database-agnostic) | ✅ Stable |
| `Ddap.Data.EntityFramework` | Entity Framework Core provider | ✅ Stable |
| `Ddap.Rest` | REST API endpoints | ✅ Stable |
| `Ddap.GraphQL` | GraphQL API (HotChocolate) | ✅ Stable |
| `Ddap.Grpc` | gRPC services | ✅ Stable |
| `Ddap.Auth` | Authentication and authorization | ✅ Stable |
| `Ddap.Subscriptions` | Real-time subscriptions | ✅ Stable |
| `Ddap.Aspire` | .NET Aspire orchestration | ✅ Stable |
| `Ddap.Templates` | Project templates | ✅ Stable |
| `Ddap.CodeGen` | Source generators | ✅ Stable |
| **Client Packages** | | |
| `Ddap.Client.Core` | Core client abstractions | ✅ Stable |
| `Ddap.Client.Rest` | REST client | ✅ Stable |
| `Ddap.Client.GraphQL` | GraphQL client | ✅ Stable |
| `Ddap.Client.Grpc` | gRPC client | ✅ Stable |

---

## 📚 Documentation

- 🎯 **[Philosophy](https://schivei.github.io/ddap/philosophy)** - Developer in Control
- 📖 **[Getting Started](https://schivei.github.io/ddap/get-started)** - Step-by-step guide
- 🗄️ **[Database Providers](https://schivei.github.io/ddap/database-providers)** - Dapper vs EF
- 🌐 **[API Providers](https://schivei.github.io/ddap/api-providers)** - REST, GraphQL, gRPC
- 🔄 **[Auto-Reload](https://schivei.github.io/ddap/auto-reload)** - Schema refresh system
- 📦 **[Templates](https://schivei.github.io/ddap/templates)** - `dotnet new` guide
- 🏗️ **[Architecture](https://schivei.github.io/ddap/architecture)** - How it works
- 🔧 **[Advanced](https://schivei.github.io/ddap/advanced)** - Extensibility
- 🔍 **[Troubleshooting](https://schivei.github.io/ddap/troubleshooting)** - Common issues

---

## 🤝 Contributing

Contributions welcome! See [CONTRIBUTING.md](CONTRIBUTING.md)

---

## 📄 License

MIT License - see [LICENSE](LICENSE)

---

## ⭐ Star History

If DDAP helps you, please star the repo! 🌟

---

**Built with ❤️ by developers who believe in control, not constraints.**
