using Aspire.Hosting;
using Aspire.Hosting.ApplicationModel;

namespace Ddap.Aspire;

/// <summary>
/// Provides extension methods for adding DDAP to .NET Aspire applications.
/// Enables agile development with automatic API generation from database schemas.
/// </summary>
public static class DdapAspireExtensions
{
    /// <summary>
    /// Adds a DDAP API service to the Aspire application with automatic database discovery.
    /// This creates a dynamic API that automatically generates REST, gRPC, and GraphQL endpoints
    /// from the connected database schema.
    /// </summary>
    /// <param name="builder">The distributed application builder.</param>
    /// <param name="name">The name of the DDAP service.</param>
    /// <returns>A resource builder for the DDAP service.</returns>
    /// <example>
    /// <code>
    /// var builder = DistributedApplication.CreateBuilder(args);
    /// 
    /// var db = builder.AddSqlServer("sql")
    ///                .AddDatabase("mydb");
    /// 
    /// builder.AddDdapApi("api")
    ///        .WithReference(db)
    ///        .WithRestApi()
    ///        .WithGraphQL();
    /// 
    /// builder.Build().Run();
    /// </code>
    /// </example>
    public static IResourceBuilder<DdapResource> AddDdapApi(
        this IDistributedApplicationBuilder builder,
        string name
    )
    {
        var resource = new DdapResource(name);
        return builder.AddResource(resource).WithManifestPublishingCallback(context =>
        {
            context.Writer.WriteString("type", "project.v0");
        });
    }

    /// <summary>
    /// Configures the DDAP service to expose REST API endpoints.
    /// Supports JSON (Newtonsoft.Json), XML, and YAML content negotiation.
    /// </summary>
    /// <param name="builder">The DDAP resource builder.</param>
    /// <param name="port">Optional port number for the REST API (default: auto-assigned).</param>
    /// <returns>The resource builder for chaining.</returns>
    public static IResourceBuilder<DdapResource> WithRestApi(
        this IResourceBuilder<DdapResource> builder,
        int? port = null
    )
    {
        builder.Resource.EnableRest = true;
        if (port.HasValue)
        {
            builder.WithHttpEndpoint(port.Value, name: "rest");
        }
        else
        {
            builder.WithHttpEndpoint(name: "rest");
        }
        return builder;
    }

    /// <summary>
    /// Configures the DDAP service to expose GraphQL endpoints.
    /// </summary>
    /// <param name="builder">The DDAP resource builder.</param>
    /// <param name="path">The path for the GraphQL endpoint (default: "/graphql").</param>
    /// <returns>The resource builder for chaining.</returns>
    public static IResourceBuilder<DdapResource> WithGraphQL(
        this IResourceBuilder<DdapResource> builder,
        string path = "/graphql"
    )
    {
        builder.Resource.EnableGraphQL = true;
        builder.Resource.GraphQLPath = path;
        return builder;
    }

    /// <summary>
    /// Configures the DDAP service to expose gRPC endpoints.
    /// </summary>
    /// <param name="builder">The DDAP resource builder.</param>
    /// <param name="port">Optional port number for gRPC (default: auto-assigned).</param>
    /// <returns>The resource builder for chaining.</returns>
    public static IResourceBuilder<DdapResource> WithGrpc(
        this IResourceBuilder<DdapResource> builder,
        int? port = null
    )
    {
        builder.Resource.EnableGrpc = true;
        if (port.HasValue)
        {
            builder.WithHttpEndpoint(port.Value, name: "grpc");
        }
        else
        {
            builder.WithHttpEndpoint(name: "grpc");
        }
        return builder;
    }

    /// <summary>
    /// Configures automatic schema refresh for agile development.
    /// When enabled, the DDAP service will automatically detect database schema changes.
    /// </summary>
    /// <param name="builder">The DDAP resource builder.</param>
    /// <param name="intervalSeconds">Refresh interval in seconds (default: 30).</param>
    /// <returns>The resource builder for chaining.</returns>
    public static IResourceBuilder<DdapResource> WithAutoRefresh(
        this IResourceBuilder<DdapResource> builder,
        int intervalSeconds = 30
    )
    {
        builder.Resource.AutoRefresh = true;
        builder.Resource.RefreshIntervalSeconds = intervalSeconds;
        return builder;
    }
}
