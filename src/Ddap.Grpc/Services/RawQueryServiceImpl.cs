using System.Text.Json;
using Ddap.Auth.Policies;
using Ddap.Core;
using Ddap.Grpc.RawQuery;
using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace Ddap.Grpc.Services;

/// <summary>
/// gRPC service implementation for executing raw SQL queries with policy-based security.
/// </summary>
public class RawQueryServiceImpl : RawQueryService.RawQueryServiceBase
{
    private readonly IRawQueryExecutor _executor;
    private readonly IRawQueryPolicy _policy;
    private readonly ILogger<RawQueryServiceImpl> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="RawQueryServiceImpl"/> class.
    /// </summary>
    /// <param name="executor">The raw query executor.</param>
    /// <param name="policy">The raw query policy.</param>
    /// <param name="logger">The logger.</param>
    public RawQueryServiceImpl(
        IRawQueryExecutor executor,
        IRawQueryPolicy policy,
        ILogger<RawQueryServiceImpl> logger
    )
    {
        _executor = executor ?? throw new ArgumentNullException(nameof(executor));
        _policy = policy ?? throw new ArgumentNullException(nameof(policy));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc />
    public override async Task<ScalarResult> ExecuteScalar(
        RawQueryRequest request,
        ServerCallContext context
    )
    {
        _logger.LogInformation("Raw query scalar execution requested: {Query}", request.Query);

        try
        {
            // Validate and authorize
            await ValidateAndAuthorizeQuery(request.Query, context);

            // Parse parameters
            var parameters = ParseParameters(request.ParametersJson);

            // Execute query
            var result = await _executor.ExecuteScalarAsync<object>(
                request.Query,
                parameters,
                context.CancellationToken
            );

            _logger.LogInformation("Raw query scalar executed successfully");

            // Serialize result
            var typeName = result?.GetType().FullName ?? "System.Object";
            return new ScalarResult
            {
                Value = Google.Protobuf.ByteString.CopyFrom(
                    RawQueryBinarySerializer.SerializeScalar(result)
                ),
                TypeName = typeName,
                IsNull = result == null,
            };
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Unauthorized raw query attempt: {Query}", request.Query);
            throw new RpcException(new Status(StatusCode.PermissionDenied, ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing raw query scalar: {Query}", request.Query);
            throw new RpcException(new Status(StatusCode.Internal, ex.Message));
        }
    }

    /// <inheritdoc />
    public override async Task<SingleResult> ExecuteSingle(
        RawQueryRequest request,
        ServerCallContext context
    )
    {
        _logger.LogInformation("Raw query single execution requested: {Query}", request.Query);

        try
        {
            // Validate and authorize
            await ValidateAndAuthorizeQuery(request.Query, context);

            // Parse parameters
            var parameters = ParseParameters(request.ParametersJson);

            // Execute query
            var result = await _executor.ExecuteSingleAsync(
                request.Query,
                parameters,
                context.CancellationToken
            );

            _logger.LogInformation("Raw query single executed successfully");

            if (result == null)
            {
                return new SingleResult { IsEmpty = true };
            }

            // Extract column info and serialize
            var columnInfo = RawQueryBinarySerializer.ExtractColumnInfo(result);
            return new SingleResult
            {
                RowData = Google.Protobuf.ByteString.CopyFrom(
                    RawQueryBinarySerializer.SerializeSingleRow(result)
                ),
                ColumnNames = { columnInfo.names },
                ColumnTypes = { columnInfo.types },
                IsEmpty = false,
            };
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Unauthorized raw query attempt: {Query}", request.Query);
            throw new RpcException(new Status(StatusCode.PermissionDenied, ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing raw query single: {Query}", request.Query);
            throw new RpcException(new Status(StatusCode.Internal, ex.Message));
        }
    }

    /// <inheritdoc />
    public override async Task<MultipleResult> ExecuteMultiple(
        RawQueryRequest request,
        ServerCallContext context
    )
    {
        _logger.LogInformation("Raw query multiple execution requested: {Query}", request.Query);

        try
        {
            // Validate and authorize
            await ValidateAndAuthorizeQuery(request.Query, context);

            // Parse parameters
            var parameters = ParseParameters(request.ParametersJson);

            // Execute query
            var results = await _executor.ExecuteMultipleAsync(
                request.Query,
                parameters,
                context.CancellationToken
            );

            _logger.LogInformation("Raw query multiple executed successfully");

            var resultList = results.ToList();
            if (!resultList.Any())
            {
                return new MultipleResult { RowCount = 0 };
            }

            // Extract column info from first row
            var columnInfo = RawQueryBinarySerializer.ExtractColumnInfo(resultList[0]);

            return new MultipleResult
            {
                RowsData = Google.Protobuf.ByteString.CopyFrom(
                    RawQueryBinarySerializer.SerializeMultipleRows(resultList)
                ),
                ColumnNames = { columnInfo.names },
                ColumnTypes = { columnInfo.types },
                RowCount = resultList.Count,
            };
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Unauthorized raw query attempt: {Query}", request.Query);
            throw new RpcException(new Status(StatusCode.PermissionDenied, ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing raw query multiple: {Query}", request.Query);
            throw new RpcException(new Status(StatusCode.Internal, ex.Message));
        }
    }

    /// <inheritdoc />
    public override async Task<VoidResult> ExecuteNonQuery(
        RawQueryRequest request,
        ServerCallContext context
    )
    {
        _logger.LogInformation("Raw query non-query execution requested: {Query}", request.Query);

        try
        {
            // Validate and authorize
            await ValidateAndAuthorizeQuery(request.Query, context);

            // Parse parameters
            var parameters = ParseParameters(request.ParametersJson);

            // Execute query
            var rowsAffected = await _executor.ExecuteNonQueryAsync(
                request.Query,
                parameters,
                context.CancellationToken
            );

            _logger.LogInformation(
                "Raw query non-query executed successfully. Rows affected: {RowsAffected}",
                rowsAffected
            );

            return new VoidResult { RowsAffected = rowsAffected };
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Unauthorized raw query attempt: {Query}", request.Query);
            throw new RpcException(new Status(StatusCode.PermissionDenied, ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing raw query non-query: {Query}", request.Query);
            throw new RpcException(new Status(StatusCode.Internal, ex.Message));
        }
    }

    private async Task ValidateAndAuthorizeQuery(string query, ServerCallContext context)
    {
        // Check for potential SQL injection
        if (QueryAnalyzer.HasPotentialInjection(query))
        {
            _logger.LogWarning("Potential SQL injection detected: {Query}", query);
            throw new UnauthorizedAccessException("Query contains potential security risks");
        }

        // Determine query type and extract metadata
        var queryType = QueryAnalyzer.DetermineQueryType(query);
        var tableName = QueryAnalyzer.ExtractTableName(query);

        // Get user info from context
        var userId = context.GetHttpContext()?.User?.Identity?.Name;
        var userRoles = context
            .GetHttpContext()
            ?.User?.Claims.Where(c => c.Type == "role")
            .Select(c => c.Value);

        // Build policy context
        var policyContext = new RawQueryContext
        {
            Query = query,
            QueryType = queryType,
            TableName = tableName,
            UserId = userId,
            UserRoles = userRoles,
        };

        // Check policy
        var allowed = await _policy.CanExecuteQueryAsync(policyContext);
        if (!allowed)
        {
            _logger.LogWarning(
                "Query denied by policy. User: {UserId}, QueryType: {QueryType}, Table: {Table}",
                userId,
                queryType,
                tableName
            );
            throw new UnauthorizedAccessException("Query not allowed by security policy");
        }

        _logger.LogInformation(
            "Query authorized. User: {UserId}, QueryType: {QueryType}, Table: {Table}",
            userId,
            queryType,
            tableName
        );
    }

    private static object? ParseParameters(string? parametersJson)
    {
        if (string.IsNullOrWhiteSpace(parametersJson))
            return null;

        try
        {
            return JsonSerializer.Deserialize<Dictionary<string, object>>(parametersJson);
        }
        catch (JsonException ex)
        {
            throw new RpcException(
                new Status(
                    StatusCode.InvalidArgument,
                    $"Failed to deserialize parameters JSON: {ex.Message}"
                )
            );
        }
    }
}
