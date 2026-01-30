# Developer in Control: The DDAP Philosophy

## Introduction

**Developer in Control** is the foundational philosophy behind DDAP (Domain-Driven API Platform). At its core, this philosophy means that **you**, the developer, make all the critical decisions about your application—not the framework.

DDAP provides the infrastructure and patterns you need to build robust, scalable APIs, but it never forces technology choices upon you. You decide which JSON serializer to use, which database to connect to, how to configure your middleware, and how to structure your domain logic.

In an era where frameworks increasingly make decisions for you—often hiding those decisions behind layers of abstraction—DDAP takes the opposite approach: **explicit, transparent, and under your control**.

---

## The Problem with Opinionated Frameworks

Modern frameworks often claim to help you move faster by making decisions on your behalf. While this can accelerate initial development, it creates long-term problems:

### Hardcoded Serialization Libraries

Many frameworks hardwire specific serialization libraries:

```csharp
// Typical opinionated framework
public void ConfigureServices(IServiceCollection services)
{
    services.AddFrameworkApi(); // Uses Newtonsoft.Json internally
    // You're stuck with Newtonsoft.Json whether you like it or not
    // Changing it requires fighting the framework or forking it
}
```

**The Problem:** When `System.Text.Json` became the preferred serializer for performance, teams using these frameworks had to:
- Wait for the framework to update (if ever)
- Work around the framework's assumptions
- Accept degraded performance
- Fork and maintain their own version

### Database-Specific Packages

Opinionated frameworks often include database-specific dependencies:

```csharp
// Framework packages bring their own database opinions
Install-Package FrameworkX.Api          // Depends on Entity Framework
Install-Package FrameworkX.Repository   // Depends on Npgsql
Install-Package FrameworkX.Caching     // Depends on StackExchange.Redis
```

**The Problem:**
- You pay the DLL tax even if you use different tools
- Version conflicts arise between framework dependencies and your choices
- Switching databases means fighting framework expectations
- Your application grows heavier with unused dependencies

### Hidden Configurations

Opinionated frameworks make choices in ways you can't see or control:

```csharp
// What is this actually doing?
services.AddFrameworkDefaults();

// Behind the scenes (hidden from you):
// - Configures logging (but which provider?)
// - Sets up error handling (but how?)
// - Adds middleware (but what middleware and in what order?)
// - Configures serialization (but with what settings?)
// - Establishes conventions (but which ones?)
```

**The Problem:** When issues arise, you can't debug what you can't see. When requirements change, you can't modify what you don't control.

### Lock-In Patterns

Over time, these "helpful" abstractions create deep coupling:

```csharp
// Your code becomes dependent on framework abstractions
public class ProductService : FrameworkServiceBase<Product>
{
    // Using framework-specific base class
    // Inheriting framework-specific behavior
    // Locked into framework patterns
}
```

**The Problem:** Migrating away from the framework requires rewriting large portions of your codebase. You're locked in—not by choice, but by accumulated dependencies.

---

## The DDAP Way

DDAP takes a fundamentally different approach. It provides **infrastructure without opinions**.

### Infrastructure Only

DDAP generates the plumbing code you need but doesn't make technology choices:

```csharp
// DDAP generates this infrastructure
public abstract partial class ProductsApi : ApiBase
{
    // Abstract methods for you to implement
    protected abstract Task<IResult> HandleCreate(CreateProductCommand command);
    protected abstract Task<IResult> HandleGetById(Guid id);
}

// You implement it YOUR way
public partial class ProductsApi
{
    private readonly IProductRepository _repository;  // Your choice of repository
    private readonly IValidator<CreateProductCommand> _validator;  // Your choice of validation
    
    protected override async Task<IResult> HandleCreate(CreateProductCommand command)
    {
        // Your implementation
        // Your dependencies
        // Your control
    }
}
```

### You Choose Everything

Every technology decision is explicit and yours:

```csharp
// Program.cs - YOU configure everything
var builder = WebApplication.CreateBuilder(args);

// Choose your serializer
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    // System.Text.Json, Newtonsoft.Json, or anything else - your choice
});

// Choose your database access
builder.Services.AddScoped<IDbConnection>(_ => 
    new NpgsqlConnection(connectionString));  // PostgreSQL
    // Or SqlConnection, MySqlConnection, or any other - your choice

// Choose your GraphQL configuration
builder.Services
    .AddGraphQLServer()
    .AddQueryType<Query>()
    .ModifyRequestOptions(opt => opt.IncludeExceptionDetails = true)
    // Every option explicitly configured by you
```

### Explicit > Implicit

DDAP makes everything explicit. There's no magic:

```csharp
// Traditional framework - implicit behavior
[ApiController]
public class ProductsController : ControllerBase
{
    // What middleware is running?
    // What validation is happening?
    // What error handling is in place?
    // All hidden from you
}

// DDAP - explicit behavior
public partial class ProductsApi : ApiBase
{
    protected override async Task<IResult> HandleCreate(CreateProductCommand command)
    {
        // Explicit validation - you see it
        var validationResult = await _validator.ValidateAsync(command);
        if (!validationResult.IsValid)
            return Results.ValidationProblem(validationResult.ToDictionary());
        
        // Explicit error handling - you control it
        try
        {
            var product = await _repository.CreateAsync(command);
            return Results.Created($"/api/products/{product.Id}", product);
        }
        catch (DuplicateException ex)
        {
            return Results.Conflict(new { error = ex.Message });
        }
    }
}
```

---

## Real-World Scenarios

### Scenario 1: Serialization Choice

**With an Opinionated Framework:**
```csharp
// Framework forces Newtonsoft.Json
public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddFrameworkApi(); // Newtonsoft.Json baked in
    }
}

// Want to use System.Text.Json for performance? Too bad.
// Want custom serialization for specific endpoints? Fight the framework.
```

**With DDAP:**
```csharp
// You choose - System.Text.Json
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
    options.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
});

// OR Newtonsoft.Json if you prefer
builder.Services.AddControllers()
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
    });

// OR even different serializers for different endpoints
public partial class ProductsApi
{
    protected override Task<IResult> HandleGetById(Guid id)
    {
        var product = _repository.GetById(id);
        // Return with YOUR chosen serialization approach
        return Task.FromResult(Results.Json(product, _customSerializerOptions));
    }
}
```

### Scenario 2: Database Choice

**With an Opinionated Framework:**
```csharp
// Framework packages include database dependencies
// FrameworkX.Data.csproj includes:
<PackageReference Include="Microsoft.EntityFrameworkCore" Version="8.0.0" />
<PackageReference Include="Npgsql.EntityFrameworkCore.PostgreSQL" Version="8.0.0" />

// You must:
// 1. Accept Entity Framework even if you use Dapper
// 2. Accept PostgreSQL packages even if you use SQL Server
// 3. Deal with version conflicts
// 4. Ship unnecessary DLLs
```

**With DDAP:**
```csharp
// DDAP.Core has ZERO database dependencies
// You add exactly what you need:

// Using Dapper with SQL Server
Install-Package Dapper
Install-Package Microsoft.Data.SqlClient

// Using Dapper with PostgreSQL
Install-Package Dapper
Install-Package Npgsql

// Using Dapper with MySQL
Install-Package Dapper
Install-Package MySql.Data

// Using Entity Framework
Install-Package Microsoft.EntityFrameworkCore
Install-Package Microsoft.EntityFrameworkCore.SqlServer

// One Dapper package works with ANY database
public partial class ProductsApi
{
    private readonly IDbConnection _connection;
    
    protected override async Task<IResult> HandleGetAll()
    {
        // Same code works with SQL Server, PostgreSQL, MySQL, SQLite, Oracle...
        var products = await _connection.QueryAsync<Product>(
            "SELECT * FROM Products WHERE IsActive = @IsActive",
            new { IsActive = true });
        
        return Results.Ok(products);
    }
}
```

### Scenario 3: GraphQL Configuration

**With an Opinionated Framework:**
```csharp
// Framework pre-configures GraphQL
services.AddFrameworkGraphQL();

// Configuration is hidden
// Want to change error handling? Difficult.
// Want to add custom directives? Fight the framework.
// Want to modify query complexity? Hope it's exposed.
```

**With DDAP:**
```csharp
// You configure HotChocolate exactly as you want
builder.Services
    .AddGraphQLServer()
    .AddQueryType<Query>()
    .AddMutationType<Mutation>()
    .AddType<ProductType>()
    .AddFiltering()
    .AddSorting()
    .AddProjections()
    .ModifyRequestOptions(opt =>
    {
        opt.IncludeExceptionDetails = builder.Environment.IsDevelopment();
        opt.ExecutionTimeout = TimeSpan.FromSeconds(30);
    })
    .AddErrorFilter<CustomErrorFilter>()
    .AddDiagnosticEventListener<CustomDiagnostics>();

// Every option is explicit and under your control
```

---

## What DDAP Provides vs What You Control

| Aspect | DDAP Provides | You Control |
|--------|---------------|-------------|
| **API Structure** | Generated partial classes with abstract methods | Implementation of all business logic |
| **Routing** | Attribute-based route templates | Route patterns and versioning strategy |
| **Validation** | Hooks for validation | Choice of validation library (FluentValidation, DataAnnotations, custom) |
| **Serialization** | No opinion—uses ASP.NET defaults | Serializer choice and configuration |
| **Database Access** | No dependencies | ORM/micro-ORM choice and configuration |
| **Error Handling** | Basic exception handling pattern | Custom error responses and logging |
| **Authentication** | No opinion | Auth mechanism (JWT, OAuth, custom) |
| **GraphQL** | Generated query/mutation types | HotChocolate configuration and schema design |
| **Dependency Injection** | Uses ASP.NET DI | Service registrations and lifetimes |
| **Middleware** | None | All middleware configuration |
| **Caching** | No opinion | Caching strategy and implementation |
| **Logging** | No opinion | Logging provider and configuration |

---

## Design Principles

### No Hidden Magic

Every piece of generated code is visible in your project. You can read it, understand it, and modify it if needed.

```csharp
// Generated code is in YOUR project
// src/Generated/ProductsApi.g.cs
public abstract partial class ProductsApi : ApiBase
{
    // You can see exactly what's generated
    // No hidden base classes in framework DLLs
}
```

### Explicit Dependencies

DDAP Core has minimal dependencies—only what's essential for ASP.NET Core integration:

```xml
<!-- DDAP.Core.csproj -->
<ItemGroup>
    <PackageReference Include="Microsoft.AspNetCore.Http.Abstractions" Version="8.0.0" />
    <PackageReference Include="Microsoft.Extensions.DependencyInjection.Abstractions" Version="8.0.0" />
</ItemGroup>

<!-- That's it. No database. No serialization. No GraphQL. -->
```

### Partial Classes for Extension

Generated code uses partial classes so you can extend without modification:

```csharp
// Generated (do not modify)
public abstract partial class ProductsApi : ApiBase
{
    [HttpPost]
    [Route("products")]
    protected abstract Task<IResult> HandleCreate(CreateProductCommand command);
}

// Your implementation (extend freely)
public partial class ProductsApi
{
    private readonly ILogger<ProductsApi> _logger;
    
    // Add your own fields and methods
    private async Task<bool> IsSkuUnique(string sku)
    {
        return await _repository.IsSkuUniqueAsync(sku);
    }
    
    protected override async Task<IResult> HandleCreate(CreateProductCommand command)
    {
        if (!await IsSkuUnique(command.Sku))
            return Results.Conflict("SKU already exists");
        
        // Your implementation
    }
}
```

### Convention over Configuration (But Configurable)

DDAP provides sensible defaults but lets you override anything:

```csharp
// Convention: Routes follow REST patterns
[HttpGet]
[Route("products/{id:guid}")]  // Convention

// But you can override
[HttpGet]
[Route("api/v2/products/{id:guid}")]  // Your custom route

// Convention: Commands map to POST, Queries to GET
// But you control the actual implementation completely
```

---

## When to Use DDAP

DDAP is ideal when you:

1. **Value Long-Term Maintainability** over rapid prototyping
2. **Need Full Control** over technology choices
3. **Have Specific Requirements** that generic frameworks don't accommodate
4. **Want to Avoid Lock-In** to framework opinions
5. **Prefer Explicit Code** over "magical" abstractions
6. **Need to Support Multiple Databases** with minimal code changes
7. **Have Team Members** who understand your domain better than framework authors do
8. **Require Performance Optimization** and want to choose your own tools
9. **Plan to Migrate** gradually from another framework without rewriting everything
10. **Believe in Code Generation** as documentation rather than abstraction

---

## When NOT to Use DDAP

Be honest with yourself. DDAP might not be the best choice if you:

1. **Need to Move Extremely Fast** on a prototype or MVP
   - Opinionated frameworks can accelerate initial development
   - DDAP requires you to make decisions that frameworks make for you

2. **Have a Team Unfamiliar with Domain-Driven Design**
   - DDAP works best when your team understands DDD concepts
   - The learning curve might slow down teams new to these patterns

3. **Want Everything Decided for You**
   - If you prefer frameworks that make all the choices, DDAP will feel bare-bones
   - You'll need to configure things that other frameworks handle automatically

4. **Need Rich Framework Ecosystems**
   - Mature frameworks have extensive plugin ecosystems
   - DDAP is newer and has a smaller community (though growing)

5. **Prefer Convention-Heavy Approaches**
   - Rails-style "magic" might be more your style
   - DDAP's explicitness requires more upfront thought

6. **Are Building Throwaway Projects**
   - If the project won't be maintained long-term, framework lock-in doesn't matter
   - DDAP's benefits shine in long-lived, maintained applications

---

## Conclusion

**Developer in Control** isn't just a tagline—it's a commitment. DDAP commits to:

- Never forcing technology choices on you
- Keeping dependencies minimal and explicit
- Generating code you can see, understand, and modify
- Staying out of your way while providing valuable infrastructure
- Treating you as an expert who makes informed decisions

When you choose DDAP, you choose control, transparency, and long-term maintainability over short-term convenience. You choose to be the architect of your application, not a consumer of someone else's opinions.

Welcome to development where **you're in control**.
