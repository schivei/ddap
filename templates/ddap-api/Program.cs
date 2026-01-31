using Ddap.Core;
#if (UseDapper)
using Ddap.Data.Dapper;
#endif
#if (UseEntityFramework)
using Ddap.Data.EntityFramework;
#endif
#if (IncludeRest)
using Ddap.Rest;
#endif
#if (IncludeGraphQL)
using Ddap.GraphQL;
#endif
#if (IncludeGrpc)
using Ddap.Grpc;
#endif
#if (include-auth)
using Ddap.Auth;
#endif
#if (include-subscriptions)
using Ddap.Subscriptions;
#endif

var builder = WebApplication.CreateBuilder(args);

#if (use-aspire)
// Add service defaults & Aspire client integrations
builder.AddServiceDefaults();

#endif
// Get connection string from configuration
// IMPORTANT: Configure connection string in appsettings.json or user secrets
// Never hardcode connection strings with credentials in code
// Connection string must be configured in appsettings.json or user secrets
// NEVER hardcode connection strings with credentials in source code
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException(
        "Connection string 'DefaultConnection' not found in configuration. " +
        "Add it to appsettings.json under 'ConnectionStrings' section or use User Secrets for development.");

// Configure DDAP
var ddapBuilder = builder.Services.AddDdap(options =>
{
    options.ConnectionString = connectionString;
});

// Add database provider
#if (UseDapper)
ddapBuilder.AddDapper();
#endif
#if (UseEntityFramework && UseSqlServer)
ddapBuilder.AddEntityFramework<Microsoft.EntityFrameworkCore.SqlServer.Infrastructure.SqlServerDbContextOptionsBuilder>();
#endif
#if (UseEntityFramework && UseMySQL)
// Using official Oracle MySQL provider (MySql.EntityFrameworkCore)
// Alternative: Community Pomelo provider (see README.md for instructions)
ddapBuilder.AddEntityFramework<MySql.EntityFrameworkCore.Infrastructure.MySQLDbContextOptionsBuilder>();
#endif
#if (UseEntityFramework && UsePostgreSQL)
ddapBuilder.AddEntityFramework<Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure.NpgsqlDbContextOptionsBuilder>();
#endif
#if (UseEntityFramework && UseSQLite)
ddapBuilder.AddEntityFramework<Microsoft.EntityFrameworkCore.Sqlite.Infrastructure.SqliteDbContextOptionsBuilder>();
#endif

// Add API providers
#if (IncludeRest)
ddapBuilder.AddRest();
#endif
#if (IncludeGraphQL)
ddapBuilder.AddGraphQL();
#endif
#if (IncludeGrpc)
ddapBuilder.AddGrpc();
#endif

#if (include-auth)
// Add authentication
// JWT configuration should be in appsettings.json or user secrets
var jwtIssuer = builder.Configuration["Jwt:Issuer"] 
    ?? throw new InvalidOperationException(
        "JWT Issuer is not configured. Add 'Jwt:Issuer' to appsettings.json or user secrets.");

var jwtAudience = builder.Configuration["Jwt:Audience"] 
    ?? throw new InvalidOperationException(
        "JWT Audience is not configured. Add 'Jwt:Audience' to appsettings.json or user secrets.");

var jwtSecretKey = builder.Configuration["Jwt:SecretKey"] 
    ?? throw new InvalidOperationException(
        "JWT SecretKey is not configured. Add 'Jwt:SecretKey' to appsettings.json or user secrets. " +
        "NEVER hardcode secrets in code!");

ddapBuilder.AddDdapAuthentication(jwtIssuer, jwtAudience, jwtSecretKey);
#endif

#if (include-subscriptions)
// Add subscriptions
ddapBuilder.AddDdapSubscriptions();
#endif

var app = builder.Build();

#if (use-aspire)
// Configure Aspire defaults
app.MapDefaultEndpoints();

#endif
// Configure middleware
app.UseRouting();

#if (include-auth)
app.UseAuthentication();
app.UseAuthorization();
#endif

#if (IncludeRest)
// Map REST controllers
app.MapControllers();
#endif

#if (IncludeGraphQL)
// Map GraphQL endpoint
app.MapGraphQL("/graphql");
#endif

app.MapGet("/", () => 
{
    // Endpoint paths - configure in appsettings.json if customization needed
    const string RestApiPath = "/api/entity";
    const string GraphQLPath = "/graphql";
    const string AuthLoginPath = "/auth/login";
    const string AuthTokenPath = "/auth/token";
    
    var endpoints = new List<string>();
#if (IncludeRest)
    endpoints.Add($"REST API: {RestApiPath}");
#endif
#if (IncludeGraphQL)
    endpoints.Add($"GraphQL: {GraphQLPath}");
#endif
#if (IncludeGrpc)
    endpoints.Add("gRPC services available");
#endif
#if (include-auth)
    endpoints.Add($"Authentication: {AuthLoginPath}, {AuthTokenPath}");
#endif
    
    return new 
    {
        Message = "DDAP API",
        Endpoints = endpoints
    };
});

app.Run();
