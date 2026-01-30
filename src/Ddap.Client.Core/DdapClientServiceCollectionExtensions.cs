using Microsoft.Extensions.DependencyInjection;

namespace Ddap.Client.Core;

/// <summary>
/// Extension methods for registering DDAP client services
/// </summary>
public static class DdapClientServiceCollectionExtensions
{
    /// <summary>
    /// Adds DDAP client core services to the service collection
    /// </summary>
    public static IServiceCollection AddDdapClientCore(
        this IServiceCollection services,
        Action<DdapClientOptions>? configureOptions = null
    )
    {
        var options = new DdapClientOptions();
        configureOptions?.Invoke(options);

        services.AddSingleton(options);
        services.AddHttpClient();

        return services;
    }
}
