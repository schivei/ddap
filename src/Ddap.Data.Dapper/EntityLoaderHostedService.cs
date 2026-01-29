using Ddap.Core;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Ddap.Data.Dapper;

/// <summary>
/// Hosted service that loads entity configurations at startup using Dapper.
/// </summary>
public class EntityLoaderHostedService : IHostedService
{
    private readonly IDataProvider _dataProvider;
    private readonly IEntityRepository _entityRepository;
    private readonly ILogger<EntityLoaderHostedService> _logger;
    private readonly DdapOptions _options;

    /// <summary>
    /// Initializes a new instance of the <see cref="EntityLoaderHostedService"/> class.
    /// </summary>
    /// <param name="dataProvider">The data provider.</param>
    /// <param name="entityRepository">The entity repository.</param>
    /// <param name="logger">The logger.</param>
    /// <param name="options">The DDAP options.</param>
    public EntityLoaderHostedService(
        IDataProvider dataProvider,
        IEntityRepository entityRepository,
        ILogger<EntityLoaderHostedService> logger,
        DdapOptions options
    )
    {
        _dataProvider = dataProvider;
        _entityRepository = entityRepository;
        _logger = logger;
        _options = options;
    }

    /// <inheritdoc/>
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation(
                "Loading entity configurations from {Provider}...",
                _dataProvider.ProviderName
            );

            var entities = await _dataProvider.LoadEntitiesAsync(cancellationToken);

            foreach (var entity in entities)
            {
                if (_entityRepository is EntityRepository repository)
                {
                    repository.AddOrUpdateEntity(entity);
                }
            }

            _logger.LogInformation("Loaded {Count} entity configurations.", entities.Count);

            // Invoke OnStartup callback if provided
            if (_options.OnStartupAsync != null)
            {
                _logger.LogDebug("Invoking OnStartup callback...");
                var serviceProvider = (_entityRepository as dynamic)?.ServiceProvider;
                if (serviceProvider != null)
                {
                    await _options.OnStartupAsync(serviceProvider);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load entity configurations.");

            // Invoke OnError callback if provided
            if (_options.OnErrorAsync != null)
            {
                var serviceProvider = (_entityRepository as dynamic)?.ServiceProvider;
                if (serviceProvider != null)
                {
                    await _options.OnErrorAsync(serviceProvider, ex);
                }
            }

            throw;
        }
    }

    /// <inheritdoc/>
    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Entity loader service stopped.");
        return Task.CompletedTask;
    }
}
