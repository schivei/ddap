using Ddap.Core;
using Ddap.Rest.Formatters;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace Ddap.Rest;

/// <summary>
/// Provides extension methods for adding REST API support to DDAP.
/// </summary>
public static class DdapRestExtensions
{
    /// <summary>
    /// Adds REST API support to the DDAP builder.
    /// This method configures controllers and endpoints for exposing entities via REST.
    /// Supports content negotiation for JSON (default, using Newtonsoft.Json), XML, and YAML formats.
    /// </summary>
    /// <param name="builder">The DDAP builder.</param>
    /// <returns>The DDAP builder for chaining.</returns>
    /// <example>
    /// <code>
    /// services.AddDdap(options => {
    ///     options.ConnectionString = "...";
    /// })
    /// .AddRest();
    /// 
    /// // Client can request different formats:
    /// // Accept: application/json (default)
    /// // Accept: application/xml
    /// // Accept: application/yaml
    /// </code>
    /// </example>
    public static IDdapBuilder AddRest(this IDdapBuilder builder)
    {
        // Register REST-specific services with content negotiation support
        builder
            .Services.AddControllers(options =>
            {
                // Add YAML output formatter
                options.OutputFormatters.Add(new YamlOutputFormatter());

                // Respect Accept header for content negotiation
                options.RespectBrowserAcceptHeader = true;
                options.ReturnHttpNotAcceptable = true;
            })
            // Use Newtonsoft.Json for JSON serialization (instead of System.Text.Json)
            .AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                options.SerializerSettings.Formatting = Formatting.Indented;
                options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                options.SerializerSettings.Converters.Add(new StringEnumConverter());
            })
            // Add XML formatters
            .AddXmlSerializerFormatters()
            .AddXmlDataContractSerializerFormatters();

        builder.Services.AddSingleton<IRestApiProvider, RestApiProvider>();

        return builder;
    }

    /// <summary>
    /// Enables gRPC-to-REST integration for the DDAP builder.
    /// This allows REST endpoints to automatically use gRPC service implementations.
    /// </summary>
    /// <param name="builder">The DDAP builder.</param>
    /// <returns>The DDAP builder for chaining.</returns>
    /// <example>
    /// <code>
    /// services.AddDdap(options => { })
    ///     .AddGrpc()
    ///     .AddRest()
    ///     .WithGrpcIntegration();
    /// 
    /// // REST endpoints will now derive from gRPC services
    /// </code>
    /// </example>
    public static IDdapBuilder WithGrpcIntegration(this IDdapBuilder builder)
    {
        builder.Services.AddSingleton<Adapters.GrpcToRestAdapter>();
        return builder;
    }
}
