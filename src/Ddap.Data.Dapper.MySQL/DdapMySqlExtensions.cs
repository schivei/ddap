using Ddap.Core;
using Microsoft.Extensions.DependencyInjection;

namespace Ddap.Data.Dapper.MySQL;

/// <summary>
/// Provides extension methods for adding MySQL Dapper data provider to DDAP.
/// </summary>
public static class DdapMySqlExtensions
{
    /// <summary>
    /// Adds MySQL data provider using Dapper.
    /// </summary>
    /// <param name="builder">The DDAP builder.</param>
    /// <returns>The DDAP builder for chaining.</returns>
    /// <example>
    /// <code>
    /// services.AddDdap(options =>
    /// {
    ///     options.ConnectionString = "Server=localhost;Database=MyDb;User=root;Password=...";
    /// })
    /// .AddMySqlDapper();
    /// </code>
    /// </example>
    public static IDdapBuilder AddMySqlDapper(this IDdapBuilder builder)
    {
        builder.Services.AddSingleton<IDataProvider, MySqlDataProvider>();
        builder.Services.AddHostedService<EntityLoaderHostedService>();

        return builder;
    }
}
