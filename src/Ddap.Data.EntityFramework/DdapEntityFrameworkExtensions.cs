using Ddap.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Ddap.Data.EntityFramework;

/// <summary>
/// Provides extension methods for adding Entity Framework data provider to DDAP.
/// </summary>
public static class DdapEntityFrameworkExtensions
{
    /// <summary>
    /// Adds Entity Framework data provider to DDAP.
    /// The DbContext must be registered separately using AddPooledDbContextFactory or AddDbContextFactory.
    /// </summary>
    /// <typeparam name="TContext">The DbContext type.</typeparam>
    /// <param name="builder">The DDAP builder.</param>
    /// <param name="configure">Optional action to configure EntityFramework provider options.</param>
    /// <returns>The DDAP builder for chaining.</returns>
    /// <example>
    /// <code>
    /// // First, register the DbContext factory (developer has full control)
    /// services.AddPooledDbContextFactory&lt;MyDbContext&gt;(options =>
    /// {
    ///     options.UseMySql(connectionString, mysql =>
    ///     {
    ///         mysql.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
    ///     });
    /// });
    ///
    /// // Then add DDAP with EntityFramework provider
    /// services.AddDdap(options => { })
    ///     .AddEntityFramework&lt;MyDbContext&gt;(ef =>
    ///     {
    ///         // Optional: Filter entities
    ///         ef.EntityFilter = type => !type.Name.StartsWith("Audit");
    ///     });
    /// </code>
    /// </example>
    public static IDdapBuilder AddEntityFramework<TContext>(
        this IDdapBuilder builder,
        Action<EntityFrameworkProviderOptions>? configure = null
    )
        where TContext : DbContext
    {
        var options = new EntityFrameworkProviderOptions();
        configure?.Invoke(options);

        builder.Services.AddSingleton(options);
        builder.Services.AddSingleton<IDataProvider, EntityFrameworkDataProvider<TContext>>();

        // Add hosted service for entity loading if configured
        if (builder.Options.LoadOnStartup)
        {
            builder.Services.AddHostedService<EntityLoaderHostedService<TContext>>();
        }

        return builder;
    }
}
