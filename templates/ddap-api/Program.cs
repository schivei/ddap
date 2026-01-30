using Ddap.Core;
#if (UseDapper && UseSqlServer)
using Ddap.Data.Dapper.SqlServer;
#endif
#if (UseDapper && UseMySQL)
using Ddap.Data.Dapper.MySQL;
#endif
#if (UseDapper && UsePostgreSQL)
using Ddap.Data.Dapper.PostgreSQL;
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
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
#if (UseSqlServer)
    ?? "Server=localhost;Database=DdapDb;Integrated Security=true;TrustServerCertificate=true;";
#endif
#if (UseMySQL)
    ?? "Server=localhost;Database=DdapDb;User=root;Password=secret;";
#endif
#if (UsePostgreSQL)
    ?? "Host=localhost;Database=DdapDb;Username=postgres;Password=secret;";
#endif
#if (UseSQLite)
    ?? "Data Source=ddap.db";
#endif

// Configure DDAP
var ddapBuilder = builder.Services.AddDdap(options =>
{
    options.ConnectionString = connectionString;
});

// Add database provider
#if (UseDapper && UseSqlServer)
ddapBuilder.AddSqlServerDapper();
#endif
#if (UseDapper && UseMySQL)
ddapBuilder.AddMySqlDapper();
#endif
#if (UseDapper && UsePostgreSQL)
ddapBuilder.AddPostgreSqlDapper();
#endif
#if (UseDapper && UseSQLite)
ddapBuilder.AddDapper();
#endif
#if (UseEntityFramework && UseSqlServer)
ddapBuilder.AddEntityFramework<Microsoft.EntityFrameworkCore.SqlServer.Infrastructure.SqlServerDbContextOptionsBuilder>();
#endif
#if (UseEntityFramework && UseMySQL)
ddapBuilder.AddEntityFramework<Pomelo.EntityFrameworkCore.MySql.Infrastructure.MySqlDbContextOptionsBuilder>();
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
ddapBuilder.AddDdapAuthentication(
    builder.Configuration["Jwt:Issuer"] ?? "DdapApi",
    builder.Configuration["Jwt:Audience"] ?? "DdapApi",
    builder.Configuration["Jwt:SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey is not configured. Use user secrets or appsettings.json.")
);
#endif

#if (include-subscriptions)
// Add subscriptions
ddapBuilder.AddSubscriptions();
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
    var endpoints = new List<string>();
#if (IncludeRest)
    endpoints.Add("REST API: /api/entity");
#endif
#if (IncludeGraphQL)
    endpoints.Add("GraphQL: /graphql");
#endif
#if (IncludeGrpc)
    endpoints.Add("gRPC services available");
#endif
#if (include-auth)
    endpoints.Add("Authentication: /auth/login, /auth/token");
#endif
    
    return new 
    {
        Message = "DDAP API",
        Endpoints = endpoints
    };
});

app.Run();
