using System.Text;
using System.Text.Json;
using Ddap.Client.Core;

namespace Ddap.Client.Rest;

/// <summary>
/// REST API client for DDAP
/// </summary>
public class DdapRestClient : IDdapClient
{
    private readonly HttpClient _httpClient;
    private readonly DdapClientOptions _options;
    private readonly JsonSerializerOptions _jsonOptions;

    public DdapRestClient(HttpClient httpClient, DdapClientOptions options)
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
    /// Gets entities from the specified endpoint
    /// </summary>
    public async Task<TEntity[]> GetAsync<TEntity>(
        string endpoint,
        CancellationToken cancellationToken = default
    )
    {
        var response = await _httpClient.GetAsync(endpoint, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw new DdapApiException(
                $"Failed to get entities: {response.ReasonPhrase}",
                (int)response.StatusCode
            );
        }

        var content = await response.Content.ReadAsStringAsync(cancellationToken);
        return JsonSerializer.Deserialize<TEntity[]>(content, _jsonOptions)
            ?? Array.Empty<TEntity>();
    }

    /// <summary>
    /// Gets a single entity by ID
    /// </summary>
    public async Task<TEntity?> GetByIdAsync<TEntity>(
        string endpoint,
        object id,
        CancellationToken cancellationToken = default
    )
    {
        var response = await _httpClient.GetAsync($"{endpoint}/{id}", cancellationToken);

        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return default;
        }

        if (!response.IsSuccessStatusCode)
        {
            throw new DdapApiException(
                $"Failed to get entity: {response.ReasonPhrase}",
                (int)response.StatusCode
            );
        }

        var content = await response.Content.ReadAsStringAsync(cancellationToken);
        return JsonSerializer.Deserialize<TEntity>(content, _jsonOptions);
    }

    /// <summary>
    /// Creates a new entity
    /// </summary>
    public async Task<TEntity> CreateAsync<TEntity>(
        string endpoint,
        TEntity entity,
        CancellationToken cancellationToken = default
    )
    {
        var json = JsonSerializer.Serialize(entity, _jsonOptions);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync(endpoint, content, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw new DdapApiException(
                $"Failed to create entity: {response.ReasonPhrase}",
                (int)response.StatusCode
            );
        }

        var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
        var result = JsonSerializer.Deserialize<TEntity>(responseContent, _jsonOptions);

        if (result == null)
        {
            throw new DdapApiException("Failed to deserialize created entity");
        }

        return result;
    }

    /// <summary>
    /// Updates an existing entity
    /// </summary>
    public async Task<TEntity> UpdateAsync<TEntity>(
        string endpoint,
        object id,
        TEntity entity,
        CancellationToken cancellationToken = default
    )
    {
        var json = JsonSerializer.Serialize(entity, _jsonOptions);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PutAsync($"{endpoint}/{id}", content, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw new DdapApiException(
                $"Failed to update entity: {response.ReasonPhrase}",
                (int)response.StatusCode
            );
        }

        var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
        var result = JsonSerializer.Deserialize<TEntity>(responseContent, _jsonOptions);

        if (result == null)
        {
            throw new DdapApiException("Failed to deserialize updated entity");
        }

        return result;
    }

    /// <summary>
    /// Deletes an entity
    /// </summary>
    public async Task<bool> DeleteAsync(
        string endpoint,
        object id,
        CancellationToken cancellationToken = default
    )
    {
        var response = await _httpClient.DeleteAsync($"{endpoint}/{id}", cancellationToken);

        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return false;
        }

        if (!response.IsSuccessStatusCode)
        {
            throw new DdapApiException(
                $"Failed to delete entity: {response.ReasonPhrase}",
                (int)response.StatusCode
            );
        }

        return true;
    }
}
