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
    /// This method registers controllers for exposing entities via REST.
    /// Developers configure serialization, formatters, and other options externally.
    /// </summary>
    /// <param name="builder">The DDAP builder.</param>
    /// <returns>The DDAP builder for chaining.</returns>
    /// <example>
    /// <code>
    /// // Basic usage - just registers controllers
    /// services.AddDdap(options => { })
    ///     .AddRest();
    ///
    /// // Developer configures serialization externally
    /// services.AddControllers()
    ///     .AddJsonOptions(options => { /* configure System.Text.Json */ })
    ///     .AddXmlSerializerFormatters()
    ///     .AddNewtonsoftJson(options => { /* configure Newtonsoft.Json */ });
    /// </code>
    /// </example>
    public static IDdapBuilder AddRest(this IDdapBuilder builder)
    {
        // Simply register controllers - developer configures serialization externally
        builder.Services.AddControllers();

        // Register REST API provider for DDAP-specific features
        builder.Services.AddSingleton<IRestApiProvider, RestApiProvider>();

        return builder;
    }
}
