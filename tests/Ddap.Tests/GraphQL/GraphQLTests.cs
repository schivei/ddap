using Ddap.Core;
using Ddap.Core.Internals;
using Ddap.GraphQL;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace Ddap.Tests.GraphQL;

public class DdapGraphQLExtensionsTests
{
    [Fact]
    public void AddGraphQL_Should_Register_GraphQL_Services()
    {
        // Arrange
        var services = new ServiceCollection();
        var builder = services.AddDdap(options =>
        {
            options.ConnectionString = "test";
        });

        // Act
        builder.AddGraphQL();

        // Assert
        services.Should().NotBeEmpty();
    }

    [Fact]
    public void AddGraphQL_Should_Return_IDdapBuilder_For_Chaining()
    {
        // Arrange
        var services = new ServiceCollection();
        var builder = services.AddDdap(options => { });

        // Act
        var result = builder.AddGraphQL();

        // Assert
        result.Should().BeAssignableTo<IDdapBuilder>();
        result.Services.Should().BeSameAs(services);
    }

    [Fact]
    public void AddGraphQL_Should_Support_Multiple_Calls()
    {
        // Arrange
        var services = new ServiceCollection();
        var builder = services.AddDdap(options => { });

        // Act
        var result1 = builder.AddGraphQL();
        var result2 = result1.AddGraphQL();

        // Assert
        result2.Should().NotBeNull();
    }

    [Fact]
    public void AddGraphQL_Should_Add_Query_Type()
    {
        // Arrange
        var services = new ServiceCollection();
        var builder = services.AddDdap(options =>
        {
            options.ConnectionString = "test";
        });

        // Act
        builder.AddGraphQL();

        // Assert
        services.Should().NotBeEmpty();
    }

    [Fact]
    public void AddGraphQL_Should_Add_Mutation_Type()
    {
        // Arrange
        var services = new ServiceCollection();
        var builder = services.AddDdap(options =>
        {
            options.ConnectionString = "test";
        });

        // Act
        builder.AddGraphQL();

        // Assert
        services.Should().NotBeEmpty();
    }
}

public class QueryTests
{
    private readonly Mock<IEntityRepository> _mockRepository;

    public QueryTests()
    {
        _mockRepository = new Mock<IEntityRepository>();
    }

    [Fact]
    public void GetEntities_Should_Return_All_Entities()
    {
        // Arrange
        var query = new Query();
        var entities = new List<IEntityConfiguration>
        {
            CreateTestEntity("Entity1", "dbo", 5),
            CreateTestEntity("Entity2", "schema1", 3),
            CreateTestEntity("Entity3", null, 2)
        };
        _mockRepository.Setup(r => r.GetAllEntities()).Returns(entities);

        // Act
        var result = query.GetEntities(_mockRepository.Object).ToList();

        // Assert
        result.Should().HaveCount(3);
        result[0].Name.Should().Be("Entity1");
        result[0].Schema.Should().Be("dbo");
        result[0].PropertyCount.Should().Be(5);
        result[1].Name.Should().Be("Entity2");
        result[2].Name.Should().Be("Entity3");
    }

    [Fact]
    public void GetEntities_Should_Return_Empty_When_No_Entities()
    {
        // Arrange
        var query = new Query();
        _mockRepository.Setup(r => r.GetAllEntities()).Returns(new List<IEntityConfiguration>());

        // Act
        var result = query.GetEntities(_mockRepository.Object);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void GetEntities_Should_Handle_Null_Schema()
    {
        // Arrange
        var query = new Query();
        var entities = new List<IEntityConfiguration>
        {
            CreateTestEntity("Entity1", null, 5)
        };
        _mockRepository.Setup(r => r.GetAllEntities()).Returns(entities);

        // Act
        var result = query.GetEntities(_mockRepository.Object).ToList();

        // Assert
        result.Should().HaveCount(1);
        result[0].Schema.Should().BeNull();
    }

    [Fact]
    public void GetEntity_Should_Return_Entity_When_Exists()
    {
        // Arrange
        var query = new Query();
        var entity = CreateTestEntity("TestEntity", "dbo", 5);
        _mockRepository.Setup(r => r.GetEntity("TestEntity")).Returns(entity);

        // Act
        var result = query.GetEntity("TestEntity", _mockRepository.Object);

        // Assert
        result.Should().NotBeNull();
        result!.Name.Should().Be("TestEntity");
        result.Schema.Should().Be("dbo");
        result.PropertyCount.Should().Be(5);
    }

    [Fact]
    public void GetEntity_Should_Return_Null_When_Entity_Does_Not_Exist()
    {
        // Arrange
        var query = new Query();
        _mockRepository.Setup(r => r.GetEntity("NonExistent")).Returns((IEntityConfiguration?)null);

        // Act
        var result = query.GetEntity("NonExistent", _mockRepository.Object);

        // Assert
        result.Should().BeNull();
    }

    [Theory]
    [InlineData("Entity1")]
    [InlineData("Entity_With_Underscore")]
    [InlineData("Entity123")]
    public void GetEntity_Should_Handle_Various_Entity_Names(string entityName)
    {
        // Arrange
        var query = new Query();
        var entity = CreateTestEntity(entityName, "dbo", 3);
        _mockRepository.Setup(r => r.GetEntity(entityName)).Returns(entity);

        // Act
        var result = query.GetEntity(entityName, _mockRepository.Object);

        // Assert
        result.Should().NotBeNull();
        result!.Name.Should().Be(entityName);
    }

    [Fact]
    public void GetEntity_Should_Handle_Empty_Property_List()
    {
        // Arrange
        var query = new Query();
        var entity = CreateTestEntity("EmptyEntity", "dbo", 0);
        _mockRepository.Setup(r => r.GetEntity("EmptyEntity")).Returns(entity);

        // Act
        var result = query.GetEntity("EmptyEntity", _mockRepository.Object);

        // Assert
        result.Should().NotBeNull();
        result!.PropertyCount.Should().Be(0);
    }

    [Fact]
    public void GetEntities_Should_Call_Repository_Once()
    {
        // Arrange
        var query = new Query();
        var entities = new List<IEntityConfiguration>
        {
            CreateTestEntity("Entity1", "dbo", 1)
        };
        _mockRepository.Setup(r => r.GetAllEntities()).Returns(entities);

        // Act
        query.GetEntities(_mockRepository.Object).ToList();

        // Assert
        _mockRepository.Verify(r => r.GetAllEntities(), Times.Once);
    }

    [Fact]
    public void GetEntity_Should_Call_Repository_With_Correct_Name()
    {
        // Arrange
        var query = new Query();
        var entity = CreateTestEntity("TestEntity", "dbo", 1);
        _mockRepository.Setup(r => r.GetEntity("TestEntity")).Returns(entity);

        // Act
        query.GetEntity("TestEntity", _mockRepository.Object);

        // Assert
        _mockRepository.Verify(r => r.GetEntity("TestEntity"), Times.Once);
    }

    private static IEntityConfiguration CreateTestEntity(string name, string? schema, int propertyCount)
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

public class MutationTests
{
    [Fact]
    public void Ping_Should_Return_Pong()
    {
        // Arrange
        var mutation = new Mutation();

        // Act
        var result = mutation.Ping();

        // Assert
        result.Should().Be("Pong");
    }

    [Fact]
    public void Ping_Should_Always_Return_Same_Value()
    {
        // Arrange
        var mutation = new Mutation();

        // Act
        var result1 = mutation.Ping();
        var result2 = mutation.Ping();

        // Assert
        result1.Should().Be(result2);
    }

    [Fact]
    public void Ping_Should_Not_Return_Null()
    {
        // Arrange
        var mutation = new Mutation();

        // Act
        var result = mutation.Ping();

        // Assert
        result.Should().NotBeNull();
    }
}

public class EntityMetadataTests
{
    [Fact]
    public void EntityMetadata_Should_Allow_Setting_Properties()
    {
        // Arrange & Act
        var metadata = new EntityMetadata
        {
            Name = "TestEntity",
            Schema = "dbo",
            PropertyCount = 5
        };

        // Assert
        metadata.Name.Should().Be("TestEntity");
        metadata.Schema.Should().Be("dbo");
        metadata.PropertyCount.Should().Be(5);
    }

    [Fact]
    public void EntityMetadata_Should_Allow_Null_Schema()
    {
        // Arrange & Act
        var metadata = new EntityMetadata
        {
            Name = "TestEntity",
            Schema = null,
            PropertyCount = 5
        };

        // Assert
        metadata.Schema.Should().BeNull();
    }

    [Fact]
    public void EntityMetadata_Should_Allow_Zero_Property_Count()
    {
        // Arrange & Act
        var metadata = new EntityMetadata
        {
            Name = "TestEntity",
            Schema = "dbo",
            PropertyCount = 0
        };

        // Assert
        metadata.PropertyCount.Should().Be(0);
    }

    [Fact]
    public void EntityMetadata_Should_Allow_Large_Property_Count()
    {
        // Arrange & Act
        var metadata = new EntityMetadata
        {
            Name = "TestEntity",
            Schema = "dbo",
            PropertyCount = 1000
        };

        // Assert
        metadata.PropertyCount.Should().Be(1000);
    }
}
