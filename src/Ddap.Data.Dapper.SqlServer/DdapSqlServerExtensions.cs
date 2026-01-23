using Ddap.Core;
using Microsoft.Extensions.DependencyInjection;

namespace Ddap.Data.Dapper.SqlServer;

/// <summary>
/// Provides extension methods for adding SQL Server Dapper data provider to DDAP.
/// </summary>
public static class DdapSqlServerExtensions
{
    /// <summary>
    /// Adds SQL Server data provider using Dapper.
    /// </summary>
    /// <param name="builder">The DDAP builder.</param>
    /// <returns>The DDAP builder for chaining.</returns>
    /// <example>
    /// <code>
    /// services.AddDdap(options =>
    /// {
    ///     options.ConnectionString = "Server=localhost;Database=MyDb;...";
    /// })
    /// .AddSqlServerDapper();
    /// </code>
    /// </example>
    public static IDdapBuilder AddSqlServerDapper(this IDdapBuilder builder)
    {
        builder.Services.AddSingleton<IDataProvider, SqlServerDataProvider>();
        builder.Services.AddHostedService<EntityLoaderHostedService>();

        return builder;
    }
}
