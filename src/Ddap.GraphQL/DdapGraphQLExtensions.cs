using Ddap.Core;
using HotChocolate.Execution.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Ddap.GraphQL;

/// <summary>
/// Provides extension methods for adding GraphQL support to DDAP.
/// </summary>
public static class DdapGraphQLExtensions
{
    /// <summary>
    /// Adds GraphQL support to the DDAP builder with optional configuration.
    /// This method configures GraphQL schema and services for exposing entities.
    /// Developers have full control over GraphQL configuration via the callback.
    /// </summary>
    /// <param name="builder">The DDAP builder.</param>
    /// <param name="configure">Optional action to configure the GraphQL request executor builder.</param>
    /// <returns>The DDAP builder for chaining.</returns>
    /// <example>
    /// <code>
    /// // Basic usage
    /// services.AddDdap(options => {
    ///     options.ConnectionString = "...";
    /// })
    /// .AddGraphQL();
    ///
    /// // Advanced usage with configuration
    /// services.AddDdap(options => { })
    /// .AddGraphQL(graphql =>
    /// {
    ///     graphql
    ///         .AddFiltering()
    ///         .AddSorting()
    ///         .AddProjections()
    ///         .ModifyRequestOptions(opt =>
    ///         {
    ///             opt.IncludeExceptionDetails = isDevelopment;
    ///             opt.ExecutionTimeout = TimeSpan.FromSeconds(30);
    ///         })
    ///         .AddInstrumentation();
    /// });
    /// </code>
    /// </example>
    public static IDdapBuilder AddGraphQL(
        this IDdapBuilder builder,
        Action<IRequestExecutorBuilder>? configure = null
    )
    {
        // Register GraphQL services with basic DDAP types
        var graphqlBuilder = builder
            .Services.AddGraphQLServer()
            .AddQueryType<Query>()
            .AddMutationType<Mutation>();

        // Allow developer to configure GraphQL as needed
        configure?.Invoke(graphqlBuilder);

        return builder;
    }
}
