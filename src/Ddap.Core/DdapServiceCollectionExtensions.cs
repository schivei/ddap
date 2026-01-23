using Microsoft.Extensions.DependencyInjection;

namespace Ddap.Core;

/// <summary>
/// Provides extension methods for configuring DDAP services.
/// </summary>
public static class DdapServiceCollectionExtensions
{
    /// <summary>
    /// Adds DDAP core services to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configureOptions">An action to configure DDAP options.</param>
    /// <returns>A <see cref="IDdapBuilder"/> for chaining additional provider configurations.</returns>
    /// <example>
    /// <code>
    /// services.AddDdap(options =>
    /// {
    ///     options.ConnectionString = "Server=localhost;Database=MyDb;";
    ///     options.Provider = DatabaseProvider.SQLServer;
    /// })
    /// .AddRest()
    /// .AddGrpc()
    /// .AddGraphQL();
    /// </code>
    /// </example>
    public static IDdapBuilder AddDdap(
        this IServiceCollection services,
        Action<DdapOptions> configureOptions
    )
    {
        var options = new DdapOptions();
        configureOptions(options);

        services.AddSingleton(options);
        services.AddSingleton<IEntityRepository, EntityRepository>();

        return new DdapBuilder(services, options);
    }
}
