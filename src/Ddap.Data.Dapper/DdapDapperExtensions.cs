using System.Data;
using Ddap.Core;
using Microsoft.Extensions.DependencyInjection;

namespace Ddap.Data.Dapper;

/// <summary>
/// Provides extension methods for adding Dapper data provider to DDAP.
/// </summary>
public static class DdapDapperExtensions
{
    /// <summary>
    /// Adds Dapper data provider to DDAP.
    /// Works with ANY database that provides an IDbConnection implementation.
    /// </summary>
    /// <param name="builder">The DDAP builder.</param>
    /// <param name="connectionFactory">Factory function that creates database connections.</param>
    /// <param name="configure">Optional action to configure Dapper provider options.</param>
    /// <returns>The DDAP builder for chaining.</returns>
    /// <example>
    /// <code>
    /// // SQL Server
    /// .AddDapper(() => new SqlConnection(connectionString))
    ///
    /// // MySQL
    /// .AddDapper(() => new MySqlConnection(connectionString))
    ///
    /// // PostgreSQL
    /// .AddDapper(() => new NpgsqlConnection(connectionString))
    ///
    /// // SQLite
    /// .AddDapper(() => new SqliteConnection(connectionString))
    ///
    /// // Any other IDbConnection
    /// .AddDapper(() => new MyCustomConnection(connectionString))
    /// </code>
    /// </example>
    public static IDdapBuilder AddDapper(
        this IDdapBuilder builder,
        Func<IDbConnection> connectionFactory,
        Action<DapperProviderOptions>? configure = null
    )
    {
        var options = new DapperProviderOptions { ConnectionFactory = connectionFactory };
        configure?.Invoke(options);

        builder.Services.AddSingleton(options);
        builder.Services.AddSingleton<IDataProvider, DapperDataProvider>();

        // Add hosted service for entity loading if configured
        if (builder.Options.LoadOnStartup)
        {
            builder.Services.AddHostedService<EntityLoaderHostedService>();
        }

        return builder;
    }

    /// <summary>
    /// Adds Dapper data provider for SQL Server.
    /// Convenience method with SQL Server-specific index and foreign key queries.
    /// </summary>
    /// <param name="builder">The DDAP builder.</param>
    /// <param name="connectionFactory">Factory function that creates SQL Server connections.</param>
    /// <returns>The DDAP builder for chaining.</returns>
    public static IDdapBuilder AddDapperSqlServer(
        this IDdapBuilder builder,
        Func<IDbConnection> connectionFactory
    )
    {
        return builder.AddDapper(
            connectionFactory,
            options =>
            {
                options.ProviderName = "SQLServer";
                options.CustomIndexQuery =
                    @"
                    SELECT 
                        i.name as IndexName,
                        i.is_unique as IsUnique,
                        i.type_desc = 'CLUSTERED' as IsClustered,
                        STRING_AGG(c.name, ',') WITHIN GROUP (ORDER BY ic.key_ordinal) as Columns
                    FROM sys.indexes i
                    INNER JOIN sys.index_columns ic ON i.object_id = ic.object_id AND i.index_id = ic.index_id
                    INNER JOIN sys.columns c ON ic.object_id = c.object_id AND ic.column_id = c.column_id
                    INNER JOIN sys.tables t ON i.object_id = t.object_id
                    INNER JOIN sys.schemas s ON t.schema_id = s.schema_id
                    WHERE s.name = @Schema AND t.name = @TableName AND i.is_primary_key = 0
                    GROUP BY i.name, i.is_unique, i.type_desc";

                options.CustomForeignKeyQuery =
                    @"
                    SELECT 
                        fk.name as ForeignKeyName,
                        OBJECT_NAME(fk.referenced_object_id) as ReferencedTable,
                        STRING_AGG(c.name, ',') WITHIN GROUP (ORDER BY fkc.constraint_column_id) as ForeignKeyColumns,
                        STRING_AGG(rc.name, ',') WITHIN GROUP (ORDER BY fkc.constraint_column_id) as ReferencedColumns,
                        CAST(0 as BIT) as IsRequired
                    FROM sys.foreign_keys fk
                    INNER JOIN sys.foreign_key_columns fkc ON fk.object_id = fkc.constraint_object_id
                    INNER JOIN sys.columns c ON fkc.parent_object_id = c.object_id AND fkc.parent_column_id = c.column_id
                    INNER JOIN sys.columns rc ON fkc.referenced_object_id = rc.object_id AND fkc.referenced_column_id = rc.column_id
                    INNER JOIN sys.tables t ON fk.parent_object_id = t.object_id
                    INNER JOIN sys.schemas s ON t.schema_id = s.schema_id
                    WHERE s.name = @Schema AND t.name = @TableName
                    GROUP BY fk.name, fk.referenced_object_id";
            }
        );
    }

    /// <summary>
    /// Adds Dapper data provider for MySQL.
    /// Convenience method with MySQL-specific index and foreign key queries.
    /// </summary>
    /// <param name="builder">The DDAP builder.</param>
    /// <param name="connectionFactory">Factory function that creates MySQL connections.</param>
    /// <returns>The DDAP builder for chaining.</returns>
    public static IDdapBuilder AddDapperMySql(
        this IDdapBuilder builder,
        Func<IDbConnection> connectionFactory
    )
    {
        return builder.AddDapper(
            connectionFactory,
            options =>
            {
                options.ProviderName = "MySQL";
                options.CustomIndexQuery =
                    @"
                    SELECT 
                        INDEX_NAME as IndexName,
                        (NON_UNIQUE = 0) as IsUnique,
                        (INDEX_TYPE = 'BTREE') as IsClustered,
                        GROUP_CONCAT(COLUMN_NAME ORDER BY SEQ_IN_INDEX) as Columns
                    FROM INFORMATION_SCHEMA.STATISTICS
                    WHERE TABLE_SCHEMA = @Schema AND TABLE_NAME = @TableName
                    AND INDEX_NAME != 'PRIMARY'
                    GROUP BY INDEX_NAME, NON_UNIQUE, INDEX_TYPE";

                options.CustomForeignKeyQuery =
                    @"
                    SELECT 
                        CONSTRAINT_NAME as ForeignKeyName,
                        REFERENCED_TABLE_NAME as ReferencedTable,
                        GROUP_CONCAT(COLUMN_NAME ORDER BY ORDINAL_POSITION) as ForeignKeyColumns,
                        GROUP_CONCAT(REFERENCED_COLUMN_NAME ORDER BY ORDINAL_POSITION) as ReferencedColumns,
                        0 as IsRequired
                    FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE
                    WHERE TABLE_SCHEMA = @Schema AND TABLE_NAME = @TableName
                    AND REFERENCED_TABLE_NAME IS NOT NULL
                    GROUP BY CONSTRAINT_NAME, REFERENCED_TABLE_NAME";
            }
        );
    }

    /// <summary>
    /// Adds Dapper data provider for PostgreSQL.
    /// Convenience method with PostgreSQL-specific index and foreign key queries.
    /// </summary>
    /// <param name="builder">The DDAP builder.</param>
    /// <param name="connectionFactory">Factory function that creates PostgreSQL connections.</param>
    /// <returns>The DDAP builder for chaining.</returns>
    public static IDdapBuilder AddDapperPostgreSql(
        this IDdapBuilder builder,
        Func<IDbConnection> connectionFactory
    )
    {
        return builder.AddDapper(
            connectionFactory,
            options =>
            {
                options.ProviderName = "PostgreSQL";
                options.CustomIndexQuery =
                    @"
                    SELECT 
                        i.relname as IndexName,
                        ix.indisunique as IsUnique,
                        false as IsClustered,
                        string_agg(a.attname, ',' ORDER BY array_position(ix.indkey, a.attnum)) as Columns
                    FROM pg_class t
                    JOIN pg_namespace n ON n.oid = t.relnamespace
                    JOIN pg_index ix ON t.oid = ix.indrelid
                    JOIN pg_class i ON i.oid = ix.indexrelid
                    JOIN pg_attribute a ON a.attrelid = t.oid AND a.attnum = ANY(ix.indkey)
                    WHERE n.nspname = @Schema AND t.relname = @TableName
                    AND NOT ix.indisprimary
                    GROUP BY i.relname, ix.indisunique";

                options.CustomForeignKeyQuery =
                    @"
                    SELECT 
                        tc.constraint_name as ForeignKeyName,
                        ccu.table_name as ReferencedTable,
                        string_agg(kcu.column_name, ',' ORDER BY kcu.ordinal_position) as ForeignKeyColumns,
                        string_agg(ccu.column_name, ',' ORDER BY kcu.ordinal_position) as ReferencedColumns,
                        false as IsRequired
                    FROM information_schema.table_constraints tc
                    JOIN information_schema.key_column_usage kcu ON tc.constraint_name = kcu.constraint_name
                    JOIN information_schema.constraint_column_usage ccu ON ccu.constraint_name = tc.constraint_name
                    WHERE tc.constraint_type = 'FOREIGN KEY'
                    AND tc.table_schema = @Schema AND tc.table_name = @TableName
                    GROUP BY tc.constraint_name, ccu.table_name";
            }
        );
    }
}
