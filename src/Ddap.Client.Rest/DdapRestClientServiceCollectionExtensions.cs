using Ddap.Client.Core;
using Microsoft.Extensions.DependencyInjection;

namespace Ddap.Client.Rest;

/// <summary>
/// Extension methods for registering REST client services
/// </summary>
public static class DdapRestClientServiceCollectionExtensions
{
    /// <summary>
    /// Adds DDAP REST client to the service collection
    /// </summary>
    public static IServiceCollection AddDdapRestClient(
        this IServiceCollection services,
        Action<DdapClientOptions> configureOptions
    )
    {
        var options = new DdapClientOptions();
        configureOptions(options);

        services.AddDdapClientCore(configureOptions);
        services
            .AddHttpClient<DdapRestClient>()
            .AddPolicyHandler(ResiliencePolicyProvider.GetRetryPolicy(options));

        return services;
    }
}
