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
        services.AddDdapClientCore(configureOptions);

        services
            .AddHttpClient<DdapRestClient>(
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
