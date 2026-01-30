using System.Text;
using System.Text.Json;
using Ddap.Client.Core;

namespace Ddap.Client.GraphQL;

/// <summary>
/// GraphQL client for DDAP
/// </summary>
public class DdapGraphQLClient : IDdapClient
{
    private readonly HttpClient _httpClient;
    private readonly DdapClientOptions _options;
    private readonly JsonSerializerOptions _jsonOptions;

    public DdapGraphQLClient(HttpClient httpClient, DdapClientOptions options)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _options = options ?? throw new ArgumentNullException(nameof(options));

        _httpClient.BaseAddress = new Uri(_options.BaseUrl);
        _httpClient.Timeout = _options.Timeout;

        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };
    }

    public string BaseUrl => _options.BaseUrl;

    public bool IsConnected { get; private set; }

    public async Task<bool> TestConnectionAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.GetAsync("/", cancellationToken);
            IsConnected = response.IsSuccessStatusCode;
            return IsConnected;
        }
        catch
        {
            IsConnected = false;
            return false;
        }
    }

    /// <summary>
    /// Executes a GraphQL query
    /// </summary>
    public async Task<GraphQLResponse<TData>> QueryAsync<TData>(
        string query,
        object? variables = null,
        CancellationToken cancellationToken = default
    )
    {
        var request = new GraphQLRequest { Query = query, Variables = variables };

        var json = JsonSerializer.Serialize(request, _jsonOptions);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync("/graphql", content, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw new DdapApiException(
                $"GraphQL query failed: {response.ReasonPhrase}",
                (int)response.StatusCode
            );
        }

        var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
        return JsonSerializer.Deserialize<GraphQLResponse<TData>>(responseContent, _jsonOptions)
            ?? throw new DdapApiException("Failed to deserialize GraphQL response");
    }

    /// <summary>
    /// Executes a GraphQL mutation
    /// </summary>
    public async Task<GraphQLResponse<TData>> MutationAsync<TData>(
        string mutation,
        object? variables = null,
        CancellationToken cancellationToken = default
    )
    {
        return await QueryAsync<TData>(mutation, variables, cancellationToken);
    }
}

/// <summary>
/// Represents a GraphQL request
/// </summary>
public class GraphQLRequest
{
    public string Query { get; set; } = string.Empty;
    public object? Variables { get; set; }
}

/// <summary>
/// Represents a GraphQL response
/// </summary>
public class GraphQLResponse<TData>
{
    public TData? Data { get; set; }
    public GraphQLError[]? Errors { get; set; }
}

/// <summary>
/// Represents a GraphQL error
/// </summary>
public class GraphQLError
{
    public string Message { get; set; } = string.Empty;
    public GraphQLLocation[]? Locations { get; set; }
    public string[]? Path { get; set; }
}

/// <summary>
/// Represents a GraphQL error location
/// </summary>
public class GraphQLLocation
{
    public int Line { get; set; }
    public int Column { get; set; }
}
