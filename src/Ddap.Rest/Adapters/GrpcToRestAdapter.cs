using Ddap.Core;
using Grpc.Core;
using System.Reflection;

namespace Ddap.Rest.Adapters;

/// <summary>
/// Converts gRPC service calls to REST API endpoints.
/// Enables REST endpoints to be derived from existing gRPC service implementations.
/// </summary>
/// <example>
/// <code>
/// // Register with gRPC integration:
/// services.AddDdap(options => { })
///     .AddGrpc()
///     .AddRest()
///     .WithGrpcIntegration();
/// 
/// // Now REST endpoints will automatically use gRPC service implementations
/// </code>
/// </example>
public class GrpcToRestAdapter
{
    /// <summary>
    /// Converts a gRPC service method call to a REST-compatible result.
    /// </summary>
    /// <typeparam name="TRequest">The gRPC request type.</typeparam>
    /// <typeparam name="TResponse">The gRPC response type.</typeparam>
    /// <param name="serviceInstance">The gRPC service instance.</param>
    /// <param name="methodName">The name of the method to call.</param>
    /// <param name="request">The request object.</param>
    /// <returns>A task representing the asynchronous operation with the response.</returns>
    public async Task<TResponse> InvokeGrpcMethodAsync<TRequest, TResponse>(
        object serviceInstance,
        string methodName,
        TRequest request)
        where TRequest : class
        where TResponse : class
    {
        var serviceType = serviceInstance.GetType();
        var method = serviceType.GetMethod(methodName, BindingFlags.Public | BindingFlags.Instance);

        if (method == null)
        {
            throw new InvalidOperationException($"Method '{methodName}' not found on service '{serviceType.Name}'");
        }

        // Check if method has ServerCallContext parameter
        var parameters = method.GetParameters();
        object? result;

        if (parameters.Length == 2 && parameters[1].ParameterType == typeof(ServerCallContext))
        {
            // Create a dummy ServerCallContext (for non-streaming calls)
            result = method.Invoke(serviceInstance, new object[] { request, null! });
        }
        else
        {
            result = method.Invoke(serviceInstance, new object[] { request });
        }

        if (result is Task<TResponse> taskResult)
        {
            return await taskResult;
        }

        if (result is TResponse directResult)
        {
            return directResult;
        }

        throw new InvalidOperationException($"Method '{methodName}' did not return expected type");
    }

    /// <summary>
    /// Maps a REST HTTP method to a corresponding gRPC service method name.
    /// </summary>
    /// <param name="httpMethod">The HTTP method (GET, POST, PUT, DELETE).</param>
    /// <param name="entityName">The entity name.</param>
    /// <param name="hasId">Whether the request includes an ID parameter.</param>
    /// <returns>The gRPC method name.</returns>
    public string MapRestToGrpcMethod(string httpMethod, string entityName, bool hasId)
    {
        return httpMethod.ToUpper() switch
        {
            "GET" when hasId => "Get",
            "GET" when !hasId => "List",
            "POST" => "Create",
            "PUT" => "Update",
            "DELETE" => "Delete",
            _ => throw new NotSupportedException($"HTTP method '{httpMethod}' is not supported")
        };
    }

    /// <summary>
    /// Creates a gRPC request object from REST parameters.
    /// </summary>
    /// <typeparam name="TRequest">The gRPC request type.</typeparam>
    /// <param name="id">The optional entity ID.</param>
    /// <param name="data">The optional entity data.</param>
    /// <param name="pageNumber">The optional page number.</param>
    /// <param name="pageSize">The optional page size.</param>
    /// <returns>The constructed request object.</returns>
    public TRequest CreateGrpcRequest<TRequest>(
        string? id = null,
        object? data = null,
        int? pageNumber = null,
        int? pageSize = null)
        where TRequest : class, new()
    {
        var request = new TRequest();
        var requestType = typeof(TRequest);

        if (id != null)
        {
            var idProperty = requestType.GetProperty("Id", BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            idProperty?.SetValue(request, id);
        }

        if (data != null)
        {
            var entityProperty = requestType.GetProperty("Entity", BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            entityProperty?.SetValue(request, data);
        }

        if (pageNumber.HasValue)
        {
            var pageNumberProperty = requestType.GetProperty("PageNumber", BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            pageNumberProperty?.SetValue(request, pageNumber.Value);
        }

        if (pageSize.HasValue)
        {
            var pageSizeProperty = requestType.GetProperty("PageSize", BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            pageSizeProperty?.SetValue(request, pageSize.Value);
        }

        return request;
    }
}
