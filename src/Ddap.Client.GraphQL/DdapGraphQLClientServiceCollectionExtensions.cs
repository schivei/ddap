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
        services.AddDdapClientCore(configureOptions);

        services
            .AddHttpClient<DdapGraphQLClient>(
                (sp, client) =>
                {
                    var options = sp.GetRequiredService<DdapClientOptions>();
                    client.BaseAddress = new Uri(options.BaseUrl);
                    client.Timeout = options.Timeout;
                }
            )
            .AddPolicyHandler(
                (sp, _) =>
                {
                    var options = sp.GetRequiredService<DdapClientOptions>();
                    return ResiliencePolicyProvider.GetRetryPolicy(options);
                }
            );

        return services;
    }
}
