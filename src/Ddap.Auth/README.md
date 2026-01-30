# Ddap.Auth

Authentication and authorization support for DDAP with JWT Bearer token integration and ASP.NET Core authorization policies.

## Installation

```bash
dotnet add package Ddap.Auth
```

## What's Included

This package provides authentication and authorization features for DDAP:

- **JWT Bearer Authentication** - Token-based authentication support
- **Authorization Policies** - Policy-based access control
- **ASP.NET Core Integration** - Seamless integration with ASP.NET Core auth
- **Security Middleware** - Authentication/authorization middleware

## Quick Start

```csharp
using Ddap.Auth;

var builder = WebApplication.CreateBuilder(args);

// Add DDAP with authentication
builder.Services.AddDdap(options =>
{
    options.ConnectionString = builder.Configuration.GetConnectionString("DefaultConnection");
})
.AddDdapAuth(authOptions =>
{
    authOptions.RequireAuthentication = true;
    authOptions.DefaultPolicy = "ApiUser";
});

// Configure JWT Bearer
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = "https://your-auth-server.com";
        options.Audience = "ddap-api";
    });

var app = builder.Build();
app.UseAuthentication();
app.UseAuthorization();
app.Run();
```

## Documentation

Full documentation: **https://schivei.github.io/ddap**

## Related Packages

- `Ddap.Core` - Core abstractions and infrastructure
- `Ddap.Rest` - REST APIs with auth support
- `Ddap.GraphQL` - GraphQL APIs with auth support
- `Ddap.Grpc` - gRPC services with auth support

## License

MIT - see [LICENSE](../../LICENSE)
