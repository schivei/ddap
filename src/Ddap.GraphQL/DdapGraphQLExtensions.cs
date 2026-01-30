using Ddap.Core;
using Ddap.GraphQL.Scalars;
using HotChocolate.Execution.Configuration;
using HotChocolate.Types;
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
    ///         .AddExtendedTypes() // Add support for uint, ulong, DateOnly, TimeOnly, etc.
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

    /// <summary>
    /// Adds support for extended types including unsigned integers (uint, ulong, ushort, sbyte)
    /// and modern .NET date/time types (DateOnly, TimeOnly, TimeSpan, DateTimeOffset).
    /// Uses proper scalar types instead of unsafe type conversions.
    /// </summary>
    /// <param name="builder">The GraphQL request executor builder.</param>
    /// <returns>The GraphQL request executor builder for chaining.</returns>
    /// <example>
    /// <code>
    /// services.AddGraphQL(graphql =>
    /// {
    ///     graphql.AddExtendedTypes();
    /// });
    /// </code>
    /// </example>
    public static IRequestExecutorBuilder AddExtendedTypes(this IRequestExecutorBuilder builder)
    {
        // Add proper scalar types for unsigned integer types
        // These provide type-safe handling without dangerous unchecked conversions
        builder.AddType(new UIntType());
        builder.AddType(new ULongType());
        builder.AddType(new UShortType());
        builder.AddType(new SByteType());

        // Bind runtime types to their GraphQL scalar types
        builder.BindRuntimeType<uint, UIntType>();
        builder.BindRuntimeType<ulong, ULongType>();
        builder.BindRuntimeType<ushort, UShortType>();
        builder.BindRuntimeType<sbyte, SByteType>();

        // Add custom scalar types for DateOnly and TimeOnly
        builder.AddType(new DateOnlyType());
        builder.AddType(new TimeOnlyType());

        // Bind runtime types to their GraphQL scalar types
        builder.BindRuntimeType<DateOnly, DateOnlyType>();
        builder.BindRuntimeType<TimeOnly, TimeOnlyType>();

        // TimeSpan and DateTimeOffset have built-in support in HotChocolate
        // They will be automatically handled by the framework

        return builder;
    }
}
