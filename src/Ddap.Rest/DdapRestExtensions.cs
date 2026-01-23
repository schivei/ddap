using Ddap.Core;
using Microsoft.Extensions.DependencyInjection;

namespace Ddap.Rest;

/// <summary>
/// Provides extension methods for adding REST API support to DDAP.
/// </summary>
public static class DdapRestExtensions
{
    /// <summary>
    /// Adds REST API support to the DDAP builder.
    /// This method configures controllers and endpoints for exposing entities via REST.
    /// </summary>
    /// <param name="builder">The DDAP builder.</param>
    /// <returns>The DDAP builder for chaining.</returns>
    /// <example>
    /// <code>
    /// services.AddDdap(options => {
    ///     options.ConnectionString = "...";
    /// })
    /// .AddRest();
    /// </code>
    /// </example>
    public static IDdapBuilder AddRest(this IDdapBuilder builder)
    {
        // Register REST-specific services
        builder.Services.AddControllers();
        builder.Services.AddSingleton<IRestApiProvider, RestApiProvider>();

        return builder;
    }
}
