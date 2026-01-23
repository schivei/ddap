using Ddap.Core;
using Microsoft.Extensions.DependencyInjection;

namespace Ddap.Grpc;

/// <summary>
/// Provides extension methods for adding gRPC support to DDAP.
/// </summary>
public static class DdapGrpcExtensions
{
    /// <summary>
    /// Adds gRPC support to the DDAP builder.
    /// This method configures gRPC services for exposing entities.
    /// </summary>
    /// <param name="builder">The DDAP builder.</param>
    /// <returns>The DDAP builder for chaining.</returns>
    /// <example>
    /// <code>
    /// services.AddDdap(options => {
    ///     options.ConnectionString = "...";
    /// })
    /// .AddGrpc();
    /// </code>
    /// </example>
    public static IDdapBuilder AddGrpc(this IDdapBuilder builder)
    {
        // Register gRPC services
        builder.Services.AddGrpc();
        builder.Services.AddSingleton<IGrpcServiceProvider, GrpcServiceProvider>();

        return builder;
    }
}
