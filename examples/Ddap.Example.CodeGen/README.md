# DDAP Source Generator (CodeGen) Example

This example demonstrates how to use DDAP's source generator (`Ddap.CodeGen`) to generate strongly-typed entity classes at compile-time.

## Features Demonstrated

- Compile-time code generation
- Strongly-typed entity classes
- IntelliSense support
- Type-safe property access
- Reduced runtime overhead

## Prerequisites

- .NET 10 SDK
- SQL Server (or another supported database)

## Installation

```bash
dotnet add package Ddap.Core
dotnet add package Ddap.CodeGen
dotnet add package Ddap.Data.Dapper.SqlServer
dotnet add package Ddap.Rest
```

## Basic Configuration

### Enable Source Generator

Add to your `.csproj` file:

```xml
<Project Sdk="Microsoft.NET.Sdk.Web">
  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <EmitCompilerGeneratedFiles>true</EmitCompilerGeneratedFiles>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Ddap.Core" Version="*" />
    <PackageReference Include="Ddap.CodeGen" Version="*" />
    <PackageReference Include="Ddap.Data.Dapper.SqlServer" Version="*" />
    <PackageReference Include="Ddap.Rest" Version="*" />
  </ItemGroup>
</Project>
```

### Configure Connection String

Add to `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=MyDb;Integrated Security=true;"
  },
  "Ddap": {
    "CodeGen": {
      "ConnectionString": "Server=localhost;Database=MyDb;Integrated Security=true;",
      "Provider": "SqlServer",
      "Namespace": "MyApp.Entities",
      "OutputPath": "Generated"
    }
  }
}
```

### Mark Entities for Generation

Use attributes to specify which entities to generate:

```csharp
using Ddap.CodeGen.Attributes;

[GenerateEntity("Products")]
public partial class Product
{
    // Properties will be generated based on database schema
}

[GenerateEntity("Orders")]
public partial class Order
{
    // Source generator will add properties at compile-time
}

[GenerateEntity("Customers")]
public partial class Customer
{
}
```

## Generated Code Example

Given a `Products` table:

```sql
CREATE TABLE Products (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(100) NOT NULL,
    Description NVARCHAR(500),
    Price DECIMAL(18,2) NOT NULL,
    Stock INT NOT NULL,
    CategoryId INT,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE()
);
```

The source generator creates:

```csharp
// Generated/Product.g.cs
namespace MyApp.Entities
{
    public partial class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public int? CategoryId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
```

## Using Generated Entities

### Program.cs

```csharp
using Ddap.Core;
using Ddap.Data.Dapper;
using Ddap.Rest;
using Microsoft.Data.SqlClient;
using MyApp.Entities;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services
    .AddDdap(options =>
    {
        options.ConnectionString = connectionString;
    })
    .AddDapper(() => new SqlConnection(connectionString))
    .AddRest();

var app = builder.Build();

app.UseRouting();
app.MapControllers();

app.Run();
```

### Using Strongly-Typed Entities

```csharp
using Microsoft.AspNetCore.Mvc;
using MyApp.Entities;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IEntityRepository<Product> _repository;
    
    public ProductsController(IEntityRepository<Product> repository)
    {
        _repository = repository;
    }
    
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Product>>> GetAll()
    {
        var products = await _repository.GetAllAsync();
        return Ok(products);
    }
    
    [HttpGet("{id}")]
    public async Task<ActionResult<Product>> GetById(int id)
    {
        var product = await _repository.GetByIdAsync(id);
        if (product == null)
            return NotFound();
        return Ok(product);
    }
    
    [HttpPost]
    public async Task<ActionResult<Product>> Create(Product product)
    {
        // IntelliSense works with generated properties!
        if (string.IsNullOrEmpty(product.Name))
            return BadRequest("Name is required");
        
        if (product.Price < 0)
            return BadRequest("Price must be positive");
        
        await _repository.CreateAsync(product);
        return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
    }
    
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, Product product)
    {
        product.Id = id;
        await _repository.UpdateAsync(product);
        return NoContent();
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _repository.DeleteAsync(id);
        return NoContent();
    }
}
```

## Advanced Features

### Generate With Relationships

```csharp
[GenerateEntity("Orders")]
[IncludeRelationships]
public partial class Order
{
    // Source generator adds:
    // - public Customer? Customer { get; set; }
    // - public ICollection<OrderItem> OrderItems { get; set; }
}
```

### Custom Property Attributes

```csharp
[GenerateEntity("Products")]
public partial class Product
{
    // Add custom attributes to generated properties
    [Required]
    [MaxLength(100)]
    partial void OnNamePropertyGenerated(PropertyInfo property);
    
    [Range(0, double.MaxValue)]
    partial void OnPricePropertyGenerated(PropertyInfo property);
}
```

### Generate DTOs

```csharp
[GenerateEntity("Products")]
[GenerateDto(DtoType.Create | DtoType.Update | DtoType.Read)]
public partial class Product
{
    // Generates:
    // - ProductCreateDto
    // - ProductUpdateDto
    // - ProductReadDto
}
```

## Benefits

1. **Type Safety**: Compile-time checking of property names and types
2. **IntelliSense**: Full IDE support with auto-completion
3. **Performance**: No reflection needed at runtime
4. **Refactoring**: Easy to rename properties across the codebase
5. **Documentation**: Self-documenting code with generated XML docs

## Viewing Generated Code

Generated files are in:
```
obj/Debug/net10.0/generated/Ddap.CodeGen/
```

Or if `EmitCompilerGeneratedFiles` is enabled:
```
obj/Debug/net10.0/Generated/
```

## Troubleshooting

### Generated Code Not Appearing

1. Clean and rebuild:
   ```bash
   dotnet clean
   dotnet build
   ```

2. Check build output for errors

3. Verify connection string is correct

### IntelliSense Not Working

1. Restart IDE
2. Delete `obj/` and `bin/` folders
3. Rebuild project

### Multiple Schemas

```csharp
[GenerateEntity("dbo.Products")]
public partial class Product { }

[GenerateEntity("sales.Orders")]
public partial class Order { }
```

## Configuration Options

```json
{
  "Ddap": {
    "CodeGen": {
      "ConnectionString": "...",
      "Provider": "SqlServer",
      "Namespace": "MyApp.Entities",
      "OutputPath": "Generated",
      "GenerateRelationships": true,
      "GenerateNavigationProperties": true,
      "UseNullableReferenceTypes": true,
      "IncludeXmlDocumentation": true
    }
  }
}
```

## Learn More

- [Source Generators in C#](https://docs.microsoft.com/dotnet/csharp/roslyn-sdk/source-generators-overview)
- [DDAP Documentation](../../docs/get-started.md)
