using Ddap.Core;
using Ddap.Data.Dapper.SqlServer;
using Ddap.GraphQL;
using Ddap.Grpc;
using Ddap.Rest;

var builder = WebApplication.CreateBuilder(args);

// Configure DDAP with multiple providers
builder
    .Services.AddDdap(options =>
    {
        // Example configuration for SQL Server
        // In production, use configuration from appsettings.json
        options.ConnectionString =
            builder.Configuration.GetConnectionString("DefaultConnection")
            ?? "Server=localhost;Database=SampleDb;Trusted_Connection=True;";
    })
    .AddSqlServerDapper() // Add SQL Server provider with Dapper
    .AddRest() // Add REST API support with JSON/XML/YAML
    .AddGrpc() // Add gRPC support
    .AddGraphQL(); // Add GraphQL support

var app = builder.Build();

// Configure middleware
app.UseRouting();

// Map REST controllers
app.MapControllers();

// Map GraphQL endpoint
app.MapGraphQL("/graphql");

app.MapGet(
    "/",
    () => "DDAP Example API - Use /api/entity to see loaded entities, /graphql for GraphQL"
);

app.Run();
