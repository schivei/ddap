using Ddap.Core;
using Microsoft.Extensions.DependencyInjection;

namespace Ddap.Data.Dapper.PostgreSQL;

/// <summary>
/// Provides extension methods for adding PostgreSQL Dapper data provider to DDAP.
/// </summary>
public static class DdapPostgreSqlExtensions
{
    /// <summary>
    /// Adds PostgreSQL data provider using Dapper.
    /// </summary>
    /// <param name="builder">The DDAP builder.</param>
    /// <returns>The DDAP builder for chaining.</returns>
    /// <example>
    /// <code>
    /// services.AddDdap(options =>
    /// {
    ///     options.ConnectionString = "Host=localhost;Database=MyDb;Username=postgres;Password=...";
    /// })
    /// .AddPostgreSqlDapper();
    /// </code>
    /// </example>
    public static IDdapBuilder AddPostgreSqlDapper(this IDdapBuilder builder)
    {
        builder.Services.AddSingleton<IDataProvider, PostgreSqlDataProvider>();
        builder.Services.AddHostedService<EntityLoaderHostedService>();

        return builder;
    }
}
