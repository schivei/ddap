using Ddap.Core;
using Ddap.Core.Internals;
using Ddap.Grpc;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace Ddap.Tests.Grpc;

public class DdapGrpcExtensionsTests
{
    [Fact]
    public void AddGrpc_Should_Register_Grpc_Services()
    {
        // Arrange
        var services = new ServiceCollection();
        var builder = services.AddDdap(options =>
        {
            options.ConnectionString = "test";
        });

        // Act
        builder.AddGrpc();

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var grpcProvider = serviceProvider.GetService<IGrpcServiceProvider>();
        grpcProvider.Should().NotBeNull();
    }

    [Fact]
    public void AddGrpc_Should_Return_IDdapBuilder_For_Chaining()
    {
        // Arrange
        var services = new ServiceCollection();
        var builder = services.AddDdap(options => { });

        // Act
        var result = builder.AddGrpc();

        // Assert
        result.Should().BeAssignableTo<IDdapBuilder>();
        result.Services.Should().BeSameAs(services);
    }

    [Fact]
    public void AddGrpc_Should_Support_Multiple_Calls()
    {
        // Arrange
        var services = new ServiceCollection();
        var builder = services.AddDdap(options => { });

        // Act
        var result1 = builder.AddGrpc();
        var result2 = result1.AddGrpc();

        // Assert
        result2.Should().NotBeNull();
    }

    [Fact]
    public void AddGrpc_Should_Register_IGrpcServiceProvider()
    {
        // Arrange
        var services = new ServiceCollection();
        var builder = services.AddDdap(options =>
        {
            options.ConnectionString = "test";
        });

        // Act
        builder.AddGrpc();

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var grpcProvider = serviceProvider.GetService<IGrpcServiceProvider>();
        grpcProvider.Should().NotBeNull();
        grpcProvider.Should().BeAssignableTo<IGrpcServiceProvider>();
    }

    [Fact]
    public void AddGrpc_Should_Add_Grpc_To_Services()
    {
        // Arrange
        var services = new ServiceCollection();
        var builder = services.AddDdap(options => { });

        // Act
        builder.AddGrpc();

        // Assert
        services.Should().NotBeEmpty();
    }
}

public class EntityServiceTests
{
    private readonly Mock<IEntityRepository> _mockRepository;
    private readonly EntityService _service;

    public EntityServiceTests()
    {
        _mockRepository = new Mock<IEntityRepository>();
        _service = new EntityService(_mockRepository.Object);
    }

    [Fact]
    public void Constructor_Should_Initialize_With_Repository()
    {
        // Arrange & Act
        var service = new EntityService(_mockRepository.Object);

        // Assert
        service.Should().NotBeNull();
    }

    [Fact]
    public void GetEntities_Should_Return_Entity_List_Response()
    {
        // Arrange
        var entities = new List<IEntityConfiguration>
        {
            CreateTestEntity("Entity1", "dbo"),
            CreateTestEntity("Entity2", "schema1"),
            CreateTestEntity("Entity3", null)
        };
        _mockRepository.Setup(r => r.GetAllEntities()).Returns(entities);

        // Act
        var result = _service.GetEntities();

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<EntityListResponse>();
        result.Entities.Should().HaveCount(3);
        result.Entities.Should().Contain("Entity1");
        result.Entities.Should().Contain("Entity2");
        result.Entities.Should().Contain("Entity3");
    }

    [Fact]
    public void GetEntities_Should_Return_Empty_List_When_No_Entities()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetAllEntities()).Returns(new List<IEntityConfiguration>());

        // Act
        var result = _service.GetEntities();

        // Assert
        result.Should().NotBeNull();
        result.Entities.Should().BeEmpty();
    }

    [Fact]
    public void GetEntities_Should_Call_Repository_Once()
    {
        // Arrange
        var entities = new List<IEntityConfiguration>
        {
            CreateTestEntity("Entity1", "dbo")
        };
        _mockRepository.Setup(r => r.GetAllEntities()).Returns(entities);

        // Act
        _service.GetEntities();

        // Assert
        _mockRepository.Verify(r => r.GetAllEntities(), Times.Once);
    }

    [Fact]
    public void GetEntities_Should_Return_Only_Entity_Names()
    {
        // Arrange
        var entities = new List<IEntityConfiguration>
        {
            CreateTestEntity("Entity1", "dbo"),
            CreateTestEntity("Entity2", "schema1")
        };
        _mockRepository.Setup(r => r.GetAllEntities()).Returns(entities);

        // Act
        var result = _service.GetEntities();

        // Assert
        result.Entities.Should().OnlyContain(name => !string.IsNullOrEmpty(name));
        result.Entities.Should().BeInAscendingOrder();
    }

    [Theory]
    [InlineData("Entity1")]
    [InlineData("Entity_With_Underscore")]
    [InlineData("Entity123")]
    [InlineData("entity-with-dash")]
    public void GetEntities_Should_Handle_Various_Entity_Names(string entityName)
    {
        // Arrange
        var entities = new List<IEntityConfiguration>
        {
            CreateTestEntity(entityName, "dbo")
        };
        _mockRepository.Setup(r => r.GetAllEntities()).Returns(entities);

        // Act
        var result = _service.GetEntities();

        // Assert
        result.Entities.Should().Contain(entityName);
    }

    [Fact]
    public void GetEntities_Should_Handle_Large_Number_Of_Entities()
    {
        // Arrange
        var entities = Enumerable.Range(1, 1000)
            .Select(i => CreateTestEntity($"Entity{i}", "dbo"))
            .ToList();
        _mockRepository.Setup(r => r.GetAllEntities()).Returns(entities);

        // Act
        var result = _service.GetEntities();

        // Assert
        result.Entities.Should().HaveCount(1000);
    }

    [Fact]
    public void GetEntities_Should_Handle_Entities_With_Null_Schema()
    {
        // Arrange
        var entities = new List<IEntityConfiguration>
        {
            CreateTestEntity("Entity1", null),
            CreateTestEntity("Entity2", "dbo")
        };
        _mockRepository.Setup(r => r.GetAllEntities()).Returns(entities);

        // Act
        var result = _service.GetEntities();

        // Assert
        result.Entities.Should().HaveCount(2);
    }

    private static IEntityConfiguration CreateTestEntity(string name, string? schema)
    {
        var mockProp = new Mock<IPropertyConfiguration>();
        mockProp.Setup(p => p.PropertyName).Returns("Id");
        mockProp.Setup(p => p.ColumnName).Returns("Id");
        mockProp.Setup(p => p.PropertyType).Returns(typeof(int));
        mockProp.Setup(p => p.DatabaseType).Returns("int");
        mockProp.Setup(p => p.IsPrimaryKey).Returns(true);
        mockProp.Setup(p => p.IsNullable).Returns(false);
        mockProp.Setup(p => p.IsAutoGenerated).Returns(true);
        mockProp.Setup(p => p.MaxLength).Returns((int?)null);

        var mockEntity = new Mock<IEntityConfiguration>();
        mockEntity.Setup(e => e.EntityName).Returns(name);
        mockEntity.Setup(e => e.SchemaName).Returns(schema);
        mockEntity.Setup(e => e.Properties).Returns(new List<IPropertyConfiguration> { mockProp.Object });
        mockEntity.Setup(e => e.Indexes).Returns(new List<IIndexConfiguration>());
        mockEntity.Setup(e => e.Relationships).Returns(new List<IRelationshipConfiguration>());
        return mockEntity.Object;
    }
}

public class EntityListResponseTests
{
    [Fact]
    public void EntityListResponse_Should_Initialize_With_Empty_List()
    {
        // Arrange & Act
        var response = new EntityListResponse();

        // Assert
        response.Entities.Should().NotBeNull();
        response.Entities.Should().BeEmpty();
    }

    [Fact]
    public void EntityListResponse_Should_Allow_Setting_Entities()
    {
        // Arrange
        var response = new EntityListResponse();
        var entities = new List<string> { "Entity1", "Entity2", "Entity3" };

        // Act
        response.Entities = entities;

        // Assert
        response.Entities.Should().HaveCount(3);
        response.Entities.Should().Contain("Entity1");
        response.Entities.Should().Contain("Entity2");
        response.Entities.Should().Contain("Entity3");
    }

    [Fact]
    public void EntityListResponse_Should_Allow_Adding_Entities()
    {
        // Arrange
        var response = new EntityListResponse();

        // Act
        response.Entities.Add("Entity1");
        response.Entities.Add("Entity2");

        // Assert
        response.Entities.Should().HaveCount(2);
    }

    [Fact]
    public void EntityListResponse_Should_Handle_Null_Entity_Names()
    {
        // Arrange
        var response = new EntityListResponse();

        // Act
        response.Entities.Add(null!);

        // Assert
        response.Entities.Should().HaveCount(1);
        response.Entities[0].Should().BeNull();
    }

    [Fact]
    public void EntityListResponse_Should_Allow_Clearing_Entities()
    {
        // Arrange
        var response = new EntityListResponse
        {
            Entities = new List<string> { "Entity1", "Entity2" }
        };

        // Act
        response.Entities.Clear();

        // Assert
        response.Entities.Should().BeEmpty();
    }
}
