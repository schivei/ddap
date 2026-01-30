using Ddap.Client.Core;
using Microsoft.Extensions.DependencyInjection;

namespace Ddap.Client.GraphQL;

/// <summary>
/// Extension methods for registering GraphQL client services
/// </summary>
public static class DdapGraphQLClientServiceCollectionExtensions
{
    /// <summary>
    /// Adds DDAP GraphQL client to the service collection
    /// </summary>
    public static IServiceCollection AddDdapGraphQLClient(
        this IServiceCollection services,
        Action<DdapClientOptions> configureOptions
    )
    {
        var options = new DdapClientOptions();
        configureOptions(options);

        services.AddDdapClientCore(configureOptions);
        services
            .AddHttpClient<DdapGraphQLClient>()
            .AddPolicyHandler(ResiliencePolicyProvider.GetRetryPolicy(options));

        return services;
    }
}
