using System.Data;
using Dapper;
using Ddap.Core;
using Ddap.Core.Internals;

namespace Ddap.Data.Dapper;

/// <summary>
/// Generic Dapper data provider that works with any IDbConnection.
/// Supports SQL Server, MySQL, PostgreSQL, SQLite, and any other database with IDbConnection.
/// </summary>
public class DapperDataProvider : IDataProvider
{
    private readonly DdapOptions _options;
    private readonly DapperProviderOptions _dapperOptions;

    /// <summary>
    /// Initializes a new instance of the <see cref="DapperDataProvider"/> class.
    /// </summary>
    /// <param name="options">The DDAP configuration options.</param>
    /// <param name="dapperOptions">The Dapper provider options.</param>
    public DapperDataProvider(DdapOptions options, DapperProviderOptions dapperOptions)
    {
        _options = options;
        _dapperOptions = dapperOptions;
    }

    /// <inheritdoc/>
    public string ProviderName => _dapperOptions.ProviderName;

    /// <inheritdoc/>
    public async Task<IReadOnlyList<IEntityConfiguration>> LoadEntitiesAsync(
        CancellationToken cancellationToken = default
    )
    {
        using var connection = _dapperOptions.ConnectionFactory();

        // Open connection - handle both sync and async scenarios
        if (connection.State != ConnectionState.Open)
        {
            if (connection is System.Data.Common.DbConnection dbConnection)
            {
                await dbConnection.OpenAsync(cancellationToken);
            }
            else
            {
                connection.Open();
            }
        }

        var tables = await LoadTablesAsync(connection);
        var entities = new List<IEntityConfiguration>();

        foreach (var (schema, tableName) in tables)
        {
            // Apply table filters
            if (_options.IncludeTables?.Count > 0 && !_options.IncludeTables.Contains(tableName))
                continue;

            if (_options.ExcludeTables?.Contains(tableName) == true)
                continue;

            if (_options.TableFilter != null && !_options.TableFilter(tableName))
                continue;

            var properties = await LoadPropertiesAsync(connection, schema, tableName);
            var indexes = await LoadIndexesAsync(connection, schema, tableName);
            var relationships = await LoadRelationshipsAsync(connection, schema, tableName);

            entities.Add(
                new EntityConfiguration
                {
                    EntityName = tableName,
                    SchemaName = schema,
                    Properties = properties,
                    Indexes = indexes,
                    Relationships = relationships,
                }
            );
        }

        return entities;
    }

    private async Task<List<(string Schema, string TableName)>> LoadTablesAsync(
        IDbConnection connection
    )
    {
        var query =
            _dapperOptions.CustomTableQuery
            ?? @"
            SELECT 
                TABLE_SCHEMA as Schema,
                TABLE_NAME as TableName
            FROM INFORMATION_SCHEMA.TABLES
            WHERE TABLE_TYPE = 'BASE TABLE'";

        // Apply schema filters using parameterization to prevent SQL injection
        var parameters = new DynamicParameters();

        if (_options.IncludeSchemas?.Count > 0)
        {
            query += " AND TABLE_SCHEMA IN @IncludeSchemas";
            parameters.Add("IncludeSchemas", _options.IncludeSchemas);
        }

        if (_options.ExcludeSchemas?.Count > 0)
        {
            query += " AND TABLE_SCHEMA NOT IN @ExcludeSchemas";
            parameters.Add("ExcludeSchemas", _options.ExcludeSchemas);
        }

        var result = await connection.QueryAsync<(string, string)>(query, parameters);
        return result.ToList();
    }

    private async Task<IReadOnlyList<IPropertyConfiguration>> LoadPropertiesAsync(
        IDbConnection connection,
        string schema,
        string tableName
    )
    {
        var query =
            _dapperOptions.CustomColumnQuery
            ?? @"
            SELECT 
                c.COLUMN_NAME as ColumnName,
                c.DATA_TYPE as DataType,
                c.IS_NULLABLE as IsNullable,
                c.CHARACTER_MAXIMUM_LENGTH as MaxLength,
                c.COLUMN_DEFAULT as ColumnDefault,
                CASE WHEN pk.COLUMN_NAME IS NOT NULL THEN 1 ELSE 0 END as IsPrimaryKey
            FROM INFORMATION_SCHEMA.COLUMNS c
            LEFT JOIN (
                SELECT ku.TABLE_SCHEMA, ku.TABLE_NAME, ku.COLUMN_NAME
                FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS tc
                JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE ku
                    ON tc.CONSTRAINT_TYPE = 'PRIMARY KEY' 
                    AND tc.CONSTRAINT_NAME = ku.CONSTRAINT_NAME
                    AND tc.TABLE_SCHEMA = ku.TABLE_SCHEMA
                    AND tc.TABLE_NAME = ku.TABLE_NAME
            ) pk ON c.TABLE_SCHEMA = pk.TABLE_SCHEMA 
                AND c.TABLE_NAME = pk.TABLE_NAME 
                AND c.COLUMN_NAME = pk.COLUMN_NAME
            WHERE c.TABLE_SCHEMA = @Schema AND c.TABLE_NAME = @TableName
            ORDER BY c.ORDINAL_POSITION";

        var columns = await connection.QueryAsync(
            query,
            new { Schema = schema, TableName = tableName }
        );

        return columns
            .Select(col =>
            {
                var isAutoGenerated =
                    col.ColumnDefault != null
                    && (
                        col.ColumnDefault.ToString()?.Contains("nextval") == true
                        || col.ColumnDefault.ToString()?.Contains("IDENTITY") == true
                        || col.ColumnDefault.ToString()?.Contains("AUTO_INCREMENT") == true
                    );

                return new PropertyConfiguration
                    {
                        PropertyName = col.ColumnName,
                        ColumnName = col.ColumnName,
                        PropertyType = MapSqlTypeToClrType(col.DataType, col.IsNullable == "YES"),
                        DatabaseType = col.DataType,
                        IsPrimaryKey = col.IsPrimaryKey == 1,
                        IsNullable = col.IsNullable == "YES",
                        IsAutoGenerated = isAutoGenerated,
                        MaxLength = col.MaxLength,
                    } as IPropertyConfiguration;
            })
            .ToList();
    }

    private async Task<IReadOnlyList<IIndexConfiguration>> LoadIndexesAsync(
        IDbConnection connection,
        string schema,
        string tableName
    )
    {
        if (_dapperOptions.CustomIndexQuery == null)
        {
            // Return empty list if no custom query provided
            // Index information is database-specific and optional
            return new List<IIndexConfiguration>();
        }

        try
        {
            var indexes = await connection.QueryAsync(
                _dapperOptions.CustomIndexQuery,
                new { Schema = schema, TableName = tableName }
            );

            return indexes
                .Select(idx =>
                    new IndexConfiguration
                    {
                        IndexName = idx.IndexName,
                        PropertyNames = ((string)idx.Columns).Split(',').ToList(),
                        IsUnique = idx.IsUnique,
                        IsClustered = idx.IsClustered ?? false,
                    } as IIndexConfiguration
                )
                .ToList();
        }
        catch
        {
            // If index query fails, return empty list (indexes are optional)
            return new List<IIndexConfiguration>();
        }
    }

    private async Task<IReadOnlyList<IRelationshipConfiguration>> LoadRelationshipsAsync(
        IDbConnection connection,
        string schema,
        string tableName
    )
    {
        if (_dapperOptions.CustomForeignKeyQuery == null)
        {
            // Return empty list if no custom query provided
            // Relationship information is database-specific and optional
            return new List<IRelationshipConfiguration>();
        }

        try
        {
            var relationships = await connection.QueryAsync(
                _dapperOptions.CustomForeignKeyQuery,
                new { Schema = schema, TableName = tableName }
            );

            return relationships
                .Select(rel =>
                    new RelationshipConfiguration
                    {
                        RelationshipName = rel.ForeignKeyName,
                        RelatedEntityName = rel.ReferencedTable,
                        RelationshipType = RelationshipType.OneToMany,
                        ForeignKeyProperties = ((string)rel.ForeignKeyColumns).Split(',').ToList(),
                        PrincipalKeyProperties = ((string)rel.ReferencedColumns)
                            .Split(',')
                            .ToList(),
                        IsRequired = rel.IsRequired ?? false,
                    } as IRelationshipConfiguration
                )
                .ToList();
        }
        catch
        {
            // If foreign key query fails, return empty list (relationships are optional)
            return new List<IRelationshipConfiguration>();
        }
    }

    private static Type MapSqlTypeToClrType(string sqlType, bool isNullable)
    {
        var sqlTypeLower = sqlType.ToLower();

        var baseType = sqlTypeLower switch
        {
            // Unsigned integer types (MySQL)
            var t
                when t.Contains("int")
                    && t.Contains("unsigned")
                    && !t.Contains("big")
                    && !t.Contains("small")
                    && !t.Contains("tiny") => typeof(uint),
            var t when t.Contains("bigint") && t.Contains("unsigned") => typeof(ulong),
            var t when t.Contains("smallint") && t.Contains("unsigned") => typeof(ushort),
            var t when t.Contains("tinyint") && t.Contains("unsigned") => typeof(byte),

            // Signed integer types
            "int" or "integer" or "int4" => typeof(int),
            "bigint" or "int8" or "long" => typeof(long),
            "smallint" or "int2" => typeof(short),
            // Note: SQL Server tinyint is unsigned (byte), MySQL tinyint signed is sbyte
            // This mapping assumes byte for compatibility. Use unsigned detection for MySQL.
            "tinyint" => typeof(byte),

            // Boolean
            "bit" or "bool" or "boolean" => typeof(bool),

            // Decimal types
            "decimal" or "numeric" or "money" or "smallmoney" or "number" => typeof(decimal),

            // Floating point
            "float" or "float8" or "double" or "double precision" => typeof(double),
            "real" or "float4" => typeof(float),

            // Date/Time types
            "datetime" or "datetime2" or "smalldatetime" or "timestamp" or "timestamptz" =>
                typeof(DateTime),
            "date" => typeof(DateOnly), // Map date to DateOnly
            "time" => typeof(TimeOnly), // Map time to TimeOnly (breaking change: was TimeSpan)
            "interval" => typeof(TimeSpan), // PostgreSQL interval type
            "datetimeoffset" => typeof(DateTimeOffset),

            // GUID
            "uniqueidentifier" or "uuid" => typeof(Guid),

            // Binary
            "binary" or "varbinary" or "image" or "bytea" or "blob" => typeof(byte[]),

            // String (default)
            _ => typeof(string),
        };

        if (isNullable && baseType.IsValueType)
        {
            return typeof(Nullable<>).MakeGenericType(baseType);
        }

        return baseType;
    }
}
