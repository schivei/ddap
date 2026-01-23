using Ddap.Core;
using Microsoft.Extensions.DependencyInjection;

namespace Ddap.GraphQL;

/// <summary>
/// Provides extension methods for adding GraphQL support to DDAP.
/// </summary>
public static class DdapGraphQLExtensions
{
    /// <summary>
    /// Adds GraphQL support to the DDAP builder.
    /// This method configures GraphQL schema and services for exposing entities.
    /// </summary>
    /// <param name="builder">The DDAP builder.</param>
    /// <returns>The DDAP builder for chaining.</returns>
    /// <example>
    /// <code>
    /// services.AddDdap(options => {
    ///     options.ConnectionString = "...";
    /// })
    /// .AddGraphQL();
    /// </code>
    /// </example>
    public static IDdapBuilder AddGraphQL(this IDdapBuilder builder)
    {
        // Register GraphQL services
        builder.Services.AddGraphQLServer().AddQueryType<Query>().AddMutationType<Mutation>();

        return builder;
    }
}
