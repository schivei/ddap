using Ddap.Core;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Ddap.Data.Dapper.SqlServer;

/// <summary>
/// Hosted service that loads entity configurations from SQL Server on application startup.
/// </summary>
internal class EntityLoaderHostedService : IHostedService
{
    private readonly IDataProvider _dataProvider;
    private readonly EntityRepository _entityRepository;
    private readonly DdapOptions _options;
    private readonly ILogger<EntityLoaderHostedService> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="EntityLoaderHostedService"/> class.
    /// </summary>
    /// <param name="dataProvider">The data provider for loading entities.</param>
    /// <param name="entityRepository">The entity repository to store loaded entities.</param>
    /// <param name="options">The DDAP configuration options.</param>
    /// <param name="logger">The logger instance.</param>
    public EntityLoaderHostedService(
        IDataProvider dataProvider,
        IEntityRepository entityRepository,
        DdapOptions options,
        ILogger<EntityLoaderHostedService> logger
    )
    {
        _dataProvider = dataProvider;
        _entityRepository = (EntityRepository)entityRepository;
        _options = options;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        if (!_options.LoadOnStartup)
        {
            _logger.LogInformation("Entity loading is disabled. Skipping startup load.");
            return;
        }

        _logger.LogInformation(
            "Loading entities from database using provider: {ProviderName}",
            _dataProvider.ProviderName
        );

        try
        {
            var entities = await _dataProvider.LoadEntitiesAsync(cancellationToken);

            _entityRepository.Clear();

            foreach (var entity in entities)
            {
                _entityRepository.AddOrUpdateEntity(entity);
                _logger.LogDebug("Loaded entity: {EntityName}", entity.EntityName);
            }

            _logger.LogInformation(
                "Successfully loaded {Count} entities from database",
                entities.Count
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to load entities from database using provider: {ProviderName}",
                _dataProvider.ProviderName
            );
            throw;
        }
    }

    /// <inheritdoc/>
    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Entity loader hosted service is stopping");
        return Task.CompletedTask;
    }
}
