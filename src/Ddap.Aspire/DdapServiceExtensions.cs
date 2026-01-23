using Ddap.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Ddap.Aspire;

/// <summary>
/// Provides extension methods for configuring DDAP in Aspire service projects.
/// </summary>
public static class DdapServiceExtensions
{
    /// <summary>
    /// Adds DDAP services configured for Aspire with automatic database connection.
    /// This method reads the connection string from Aspire service bindings.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The configuration.</param>
    /// <returns>A DDAP builder for chaining.</returns>
    /// <example>
    /// <code>
    /// var builder = WebApplication.CreateBuilder(args);
    /// 
    /// builder.AddServiceDefaults();
    /// 
    /// builder.Services.AddDdapForAspire(builder.Configuration)
    ///        .AddSqlServerDapper()
    ///        .AddRest()
    ///        .AddGraphQL();
    /// 
    /// var app = builder.Build();
    /// app.Run();
    /// </code>
    /// </example>
    public static IDdapBuilder AddDdapForAspire(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        var connectionString =
            configuration.GetConnectionString("database")
            ?? configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException(
                "No database connection string found. Ensure the database is referenced in the AppHost."
            );

        return services.AddDdap(options =>
        {
            options.ConnectionString = connectionString;
            options.LoadOnStartup = true;
        });
    }

    /// <summary>
    /// Adds DDAP services with auto-refresh for agile development.
    /// The schema will automatically reload when changes are detected.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The configuration.</param>
    /// <param name="refreshIntervalSeconds">Refresh interval in seconds.</param>
    /// <returns>A DDAP builder for chaining.</returns>
    public static IDdapBuilder AddDdapForAspireWithAutoRefresh(
        this IServiceCollection services,
        IConfiguration configuration,
        int refreshIntervalSeconds = 30
    )
    {
        var builder = services.AddDdapForAspire(configuration);

        // Add auto-refresh hosted service
        services.AddHostedService(sp => new SchemaRefreshHostedService(
            sp,
            refreshIntervalSeconds
        ));

        return builder;
    }
}
