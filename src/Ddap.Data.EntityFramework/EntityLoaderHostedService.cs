using Ddap.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Ddap.Data.EntityFramework;

/// <summary>
/// Hosted service that loads entity configurations at startup.
/// </summary>
/// <typeparam name="TContext">The DbContext type.</typeparam>
public class EntityLoaderHostedService<TContext> : IHostedService
    where TContext : DbContext
{
    private readonly IDataProvider _dataProvider;
    private readonly IEntityRepository _entityRepository;
    private readonly ILogger<EntityLoaderHostedService<TContext>> _logger;
    private readonly DdapOptions _options;
    private readonly IServiceProvider _serviceProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="EntityLoaderHostedService{TContext}"/> class.
    /// </summary>
    /// <param name="dataProvider">The data provider.</param>
    /// <param name="entityRepository">The entity repository.</param>
    /// <param name="logger">The logger.</param>
    /// <param name="options">The DDAP options.</param>
    /// <param name="serviceProvider">The service provider for callbacks.</param>
    public EntityLoaderHostedService(
        IDataProvider dataProvider,
        IEntityRepository entityRepository,
        ILogger<EntityLoaderHostedService<TContext>> logger,
        DdapOptions options,
        IServiceProvider serviceProvider
    )
    {
        _dataProvider = dataProvider;
        _entityRepository = entityRepository;
        _logger = logger;
        _options = options;
        _serviceProvider = serviceProvider;
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
                _entityRepository.AddOrUpdateEntity(entity);
            }

            _logger.LogInformation("Loaded {Count} entity configurations.", entities.Count);

            // Invoke OnStartup callback if provided
            if (_options.OnStartupAsync != null)
            {
                _logger.LogDebug("Invoking OnStartup callback...");
                await _options.OnStartupAsync(_serviceProvider);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load entity configurations.");

            // Invoke OnError callback if provided
            if (_options.OnErrorAsync != null)
            {
                await _options.OnErrorAsync(_serviceProvider, ex);
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
