# DDAP Authentication & Authorization Example

This example demonstrates how to use DDAP with authentication and authorization using the `Ddap.Auth` package.

## Features Demonstrated

- JWT Bearer authentication
- Role-based authorization
- Entity-level security policies
- Custom authorization handlers
- Integration with ASP.NET Core Identity

## Prerequisites

- .NET 10 SDK
- SQL Server (or another supported database)

## Installation

```bash
dotnet add package Ddap.Core
dotnet add package Ddap.Auth
dotnet add package Ddap.Data.Dapper
dotnet add package Microsoft.Data.SqlClient
dotnet add package Ddap.Rest
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer
```

## Basic Configuration

### Program.cs

```csharp
using Ddap.Core;
using Ddap.Auth;
using Ddap.Data.Dapper;
using Ddap.Rest;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Data.SqlClient;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Configure JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });

builder.Services.AddAuthorization();

// Configure DDAP with authentication
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services
    .AddDdap(options =>
    {
        options.ConnectionString = connectionString;
    })
    .AddDapper(() => new SqlConnection(connectionString))
    .AddAuth(authOptions =>
    {
        // Enable authentication for all entities by default
        authOptions.RequireAuthenticationByDefault = true;
        
        // Configure entity-level policies
        authOptions.AddEntityPolicy("Users", policy =>
        {
            policy.RequireRole("Admin");
        });
        
        authOptions.AddEntityPolicy("Products", policy =>
        {
            policy.RequireAuthenticatedUser();
        });
    })
    .AddRest();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();
app.UseRouting();
app.MapControllers();

app.Run();
```

## Configuration (appsettings.json)

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=MyDb;Integrated Security=true;"
  },
  "Jwt": {
    "Key": "YourSuperSecretKeyHere_MinimumLength32Characters",
    "Issuer": "https://your-app.com",
    "Audience": "https://your-app.com"
  }
}
```

## Entity-Level Authorization Examples

### Read-Only Access

```csharp
authOptions.AddEntityPolicy("PublicData", policy =>
{
    policy.AllowAnonymous()
          .AllowRead()
          .DenyWrite();
});
```

### Admin-Only Entity

```csharp
authOptions.AddEntityPolicy("SensitiveData", policy =>
{
    policy.RequireRole("Admin", "SuperAdmin");
});
```

## Testing the API

### Get JWT Token

```bash
# Get token (implement login endpoint first)
curl -X POST http://localhost:5000/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"username":"admin","password":"password"}'

# Access protected endpoint
curl http://localhost:5000/api/entity/Users \
  -H "Authorization: Bearer YOUR_TOKEN_HERE"
```

## Learn More

- [ASP.NET Core Authentication](https://docs.microsoft.com/aspnet/core/security/authentication/)
- [DDAP Documentation](../../docs/get-started.md)
