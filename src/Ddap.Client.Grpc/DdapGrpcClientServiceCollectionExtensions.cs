using Ddap.Client.Core;
using Microsoft.Extensions.DependencyInjection;

namespace Ddap.Client.Grpc;

/// <summary>
/// Extension methods for registering gRPC client services
/// </summary>
public static class DdapGrpcClientServiceCollectionExtensions
{
    /// <summary>
    /// Adds DDAP gRPC client to the service collection
    /// </summary>
    public static IServiceCollection AddDdapGrpcClient(
        this IServiceCollection services,
        Action<DdapClientOptions> configureOptions
    )
    {
        var options = new DdapClientOptions();
        configureOptions(options);

        services.AddDdapClientCore(configureOptions);
        services.AddSingleton<DdapGrpcClient>();

        return services;
    }
}
