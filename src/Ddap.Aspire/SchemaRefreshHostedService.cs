using Ddap.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Ddap.Aspire;

/// <summary>
/// Hosted service that automatically refreshes the database schema
/// for agile development scenarios.
/// </summary>
internal class SchemaRefreshHostedService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly int _intervalSeconds;
    private readonly ILogger<SchemaRefreshHostedService> _logger;

    public SchemaRefreshHostedService(IServiceProvider serviceProvider, int intervalSeconds)
    {
        _serviceProvider = serviceProvider;
        _intervalSeconds = intervalSeconds;
        _logger = serviceProvider.GetRequiredService<ILogger<SchemaRefreshHostedService>>();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation(
            "Schema auto-refresh started with interval of {Interval} seconds",
            _intervalSeconds
        );

        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(TimeSpan.FromSeconds(_intervalSeconds), stoppingToken);

            try
            {
                using var scope = _serviceProvider.CreateScope();
                var dataProvider = scope.ServiceProvider.GetService<IDataProvider>();
                var entityRepository =
                    scope.ServiceProvider.GetRequiredService<IEntityRepository>();

                if (dataProvider == null)
                {
                    _logger.LogWarning("No data provider registered. Schema refresh skipped.");
                    continue;
                }

                _logger.LogDebug("Refreshing database schema...");

                var entities = await dataProvider.LoadEntitiesAsync(stoppingToken);

                // Clear and reload
                if (entityRepository is EntityRepository repo)
                {
                    repo.Clear();
                    foreach (var entity in entities)
                    {
                        repo.AddOrUpdateEntity(entity);
                    }
                }

                _logger.LogInformation(
                    "Schema refreshed successfully. {Count} entities loaded.",
                    entities.Count
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error refreshing database schema");
            }
        }

        _logger.LogInformation("Schema auto-refresh stopped");
    }
}
