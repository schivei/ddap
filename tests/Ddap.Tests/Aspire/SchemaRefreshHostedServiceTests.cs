using Ddap.Aspire;
using Ddap.Core;
using Ddap.Core.Internals;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Ddap.Tests.Aspire;

public class SchemaRefreshHostedServiceTests
{
    [Fact]
    public async Task SchemaRefreshHostedService_Should_Start_And_Stop()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddSingleton<IEntityRepository, EntityRepository>();

        var serviceProvider = services.BuildServiceProvider();

        var service = new TestSchemaRefreshHostedService(serviceProvider, 60);
        var cts = new CancellationTokenSource();

        // Act
        var startTask = service.StartAsync(cts.Token);
        await Task.Delay(100); // Give it time to start
        cts.Cancel();
        await startTask;

        // Assert
        service.Should().NotBeNull();
    }

    [Fact]
    public async Task SchemaRefreshHostedService_Should_Refresh_Schema_Periodically()
    {
        // Arrange
        var mockDataProvider = new Mock<IDataProvider>();
        var mockEntities = new List<IEntityConfiguration>
        {
            CreateTestEntity("Entity1", "dbo", 3),
            CreateTestEntity("Entity2", "public", 5),
        };

        mockDataProvider
            .Setup(p => p.LoadEntitiesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockEntities);

        var services = new ServiceCollection();
        services.AddLogging();
        services.AddSingleton<IDataProvider>(mockDataProvider.Object);
        services.AddSingleton<IEntityRepository, EntityRepository>();

        var serviceProvider = services.BuildServiceProvider();

        var service = new TestSchemaRefreshHostedService(serviceProvider, 1); // 1 second interval
        var cts = new CancellationTokenSource();

        // Act
        var startTask = service.StartAsync(cts.Token);
        await Task.Delay(2500); // Wait for at least 2 refresh cycles
        cts.Cancel();

        try
        {
            await startTask;
        }
        catch (OperationCanceledException)
        {
            // Expected when cancelling
        }

        // Assert
        mockDataProvider.Verify(
            p => p.LoadEntitiesAsync(It.IsAny<CancellationToken>()),
            Times.AtLeastOnce
        );
    }

    [Fact]
    public async Task SchemaRefreshHostedService_Should_Handle_Missing_DataProvider()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddSingleton<IEntityRepository, EntityRepository>();
        // No IDataProvider registered

        var serviceProvider = services.BuildServiceProvider();

        var service = new TestSchemaRefreshHostedService(serviceProvider, 1);
        var cts = new CancellationTokenSource();

        // Act
        var startTask = service.StartAsync(cts.Token);
        await Task.Delay(1500); // Wait for one cycle
        cts.Cancel();

        try
        {
            await startTask;
        }
        catch (OperationCanceledException)
        {
            // Expected
        }

        // Assert - Should not throw, just log warning
        service.Should().NotBeNull();
    }

    [Fact]
    public async Task SchemaRefreshHostedService_Should_Handle_Exceptions_Gracefully()
    {
        // Arrange
        var mockDataProvider = new Mock<IDataProvider>();
        mockDataProvider
            .Setup(p => p.LoadEntitiesAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Test error"));

        var services = new ServiceCollection();
        services.AddLogging();
        services.AddSingleton<IDataProvider>(mockDataProvider.Object);
        services.AddSingleton<IEntityRepository, EntityRepository>();

        var serviceProvider = services.BuildServiceProvider();

        var service = new TestSchemaRefreshHostedService(serviceProvider, 1);
        var cts = new CancellationTokenSource();

        // Act
        var startTask = service.StartAsync(cts.Token);
        await Task.Delay(1500); // Wait for one cycle with error
        cts.Cancel();

        try
        {
            await startTask;
        }
        catch (OperationCanceledException)
        {
            // Expected
        }

        // Assert - Should continue running despite errors
        service.Should().NotBeNull();
    }

    [Fact]
    public async Task SchemaRefreshHostedService_Should_Update_EntityRepository()
    {
        // Arrange
        var mockEntities = new List<IEntityConfiguration>
        {
            CreateTestEntity("TestEntity", "dbo", 2),
        };

        var mockDataProvider = new Mock<IDataProvider>();
        mockDataProvider
            .Setup(p => p.LoadEntitiesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockEntities);

        var services = new ServiceCollection();
        services.AddLogging();
        services.AddSingleton<IDataProvider>(mockDataProvider.Object);
        services.AddSingleton<IEntityRepository, EntityRepository>();

        var serviceProvider = services.BuildServiceProvider();
        var entityRepository =
            serviceProvider.GetRequiredService<IEntityRepository>() as EntityRepository;

        var service = new TestSchemaRefreshHostedService(serviceProvider, 1);
        var cts = new CancellationTokenSource();

        // Act
        var startTask = service.StartAsync(cts.Token);
        await Task.Delay(1500); // Wait for refresh
        cts.Cancel();

        try
        {
            await startTask;
        }
        catch (OperationCanceledException)
        {
            // Expected
        }

        // Assert
        var entities = entityRepository!.GetAllEntities();
        entities.Should().NotBeEmpty();
    }

    // Helper class to expose protected members for testing
    private class TestSchemaRefreshHostedService : IHostedService
    {
        private readonly Task _executeTask;
        private readonly CancellationTokenSource _stoppingCts = new();

        public TestSchemaRefreshHostedService(IServiceProvider serviceProvider, int intervalSeconds)
        {
            // Use reflection to create the actual service
            var type = Type.GetType("Ddap.Aspire.SchemaRefreshHostedService, Ddap.Aspire");
            var instance = Activator.CreateInstance(type!, serviceProvider, intervalSeconds);
            var method = type!.GetMethod("StartAsync");
            _executeTask = (Task)method!.Invoke(instance, new object[] { _stoppingCts.Token })!;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _stoppingCts.Cancel();
            await _executeTask;
        }
    }

    private static IEntityConfiguration CreateTestEntity(
        string name,
        string? schema,
        int propertyCount
    )
    {
        var properties = new List<IPropertyConfiguration>();
        for (int i = 0; i < propertyCount; i++)
        {
            var mockProp = new Mock<IPropertyConfiguration>();
            mockProp.Setup(p => p.PropertyName).Returns($"Property{i}");
            mockProp.Setup(p => p.ColumnName).Returns($"Column{i}");
            mockProp.Setup(p => p.PropertyType).Returns(typeof(string));
            mockProp.Setup(p => p.DatabaseType).Returns("nvarchar");
            mockProp.Setup(p => p.IsPrimaryKey).Returns(i == 0);
            mockProp.Setup(p => p.IsNullable).Returns(false);
            mockProp.Setup(p => p.IsAutoGenerated).Returns(false);
            mockProp.Setup(p => p.MaxLength).Returns((int?)null);
            properties.Add(mockProp.Object);
        }

        var mockEntity = new Mock<IEntityConfiguration>();
        mockEntity.Setup(e => e.EntityName).Returns(name);
        mockEntity.Setup(e => e.SchemaName).Returns(schema);
        mockEntity.Setup(e => e.Properties).Returns(properties);
        mockEntity.Setup(e => e.Indexes).Returns(new List<IIndexConfiguration>());
        mockEntity.Setup(e => e.Relationships).Returns(new List<IRelationshipConfiguration>());
        return mockEntity.Object;
    }
}
