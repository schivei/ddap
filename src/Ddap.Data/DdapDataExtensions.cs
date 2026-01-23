using Ddap.Core;
using Microsoft.Extensions.DependencyInjection;

namespace Ddap.Data;

/// <summary>
/// Provides extension methods for adding data providers to the DDAP builder.
/// </summary>
public static class DdapDataExtensions
{
    /// <summary>
    /// Adds the appropriate data provider based on the configured database provider.
    /// </summary>
    /// <param name="builder">The DDAP builder.</param>
    /// <returns>The DDAP builder for chaining.</returns>
    public static IDdapBuilder AddDataProvider(this IDdapBuilder builder)
    {
        var provider = builder.Options.Provider switch
        {
            DatabaseProvider.SQLServer => typeof(SqlServerDataProvider),
            DatabaseProvider.MySQL => typeof(MySqlDataProvider),
            DatabaseProvider.PostgreSQL => typeof(PostgreSqlDataProvider),
            _ => throw new NotSupportedException(
                $"Database provider '{builder.Options.Provider}' is not supported"
            ),
        };

        builder.Services.AddSingleton(typeof(IDataProvider), provider);
        builder.Services.AddHostedService<EntityLoaderHostedService>();

        return builder;
    }
}
