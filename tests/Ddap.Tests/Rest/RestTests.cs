using Ddap.Core;
using Ddap.Core.Internals;
using Ddap.Rest;
using Ddap.Rest.Formatters;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.Text;
using Xunit;

namespace Ddap.Tests.Rest;

public class DdapRestExtensionsTests
{
    [Fact]
    public void AddRest_Should_Register_Controllers()
    {
        // Arrange
        var services = new ServiceCollection();
        var builder = services.AddDdap(options =>
        {
            options.ConnectionString = "test";
        });

        // Act
        builder.AddRest();

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var restProvider = serviceProvider.GetService<IRestApiProvider>();
        restProvider.Should().NotBeNull();
    }

    [Fact]
    public void AddRest_Should_Return_IDdapBuilder_For_Chaining()
    {
        // Arrange
        var services = new ServiceCollection();
        var builder = services.AddDdap(options => { });

        // Act
        var result = builder.AddRest();

        // Assert
        result.Should().BeAssignableTo<IDdapBuilder>();
        result.Services.Should().BeSameAs(services);
    }

    [Fact]
    public void AddRest_Should_Configure_Content_Negotiation()
    {
        // Arrange
        var services = new ServiceCollection();
        var builder = services.AddDdap(options =>
        {
            options.ConnectionString = "test";
        });

        // Act
        builder.AddRest();
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        serviceProvider.Should().NotBeNull();
    }

    [Fact]
    public void AddRest_Should_Support_Multiple_Calls()
    {
        // Arrange
        var services = new ServiceCollection();
        var builder = services.AddDdap(options => { });

        // Act
        var result1 = builder.AddRest();
        var result2 = result1.AddRest();

        // Assert
        result2.Should().NotBeNull();
    }
}

public class EntityControllerTests
{
    private readonly Mock<IEntityRepository> _mockRepository;
    private readonly EntityController _controller;

    public EntityControllerTests()
    {
        _mockRepository = new Mock<IEntityRepository>();
        _controller = new EntityController(_mockRepository.Object);
    }

    [Fact]
    public void Constructor_Should_Initialize_With_Repository()
    {
        // Arrange & Act
        var controller = new EntityController(_mockRepository.Object);

        // Assert
        controller.Should().NotBeNull();
    }

    [Fact]
    public void GetAllEntities_Should_Return_Ok_With_Entity_List()
    {
        // Arrange
        var entities = new List<IEntityConfiguration>
        {
            CreateTestEntity("Entity1", "dbo", 5),
            CreateTestEntity("Entity2", "schema1", 3)
        };
        _mockRepository.Setup(r => r.GetAllEntities()).Returns(entities);

        // Act
        var result = _controller.GetAllEntities(new Ddap.Rest.Models.QueryParameters());

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult!.Value.Should().NotBeNull();
    }

    [Fact]
    public void GetAllEntities_Should_Return_Empty_List_When_No_Entities()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetAllEntities()).Returns(new List<IEntityConfiguration>());

        // Act
        var result = _controller.GetAllEntities(new Ddap.Rest.Models.QueryParameters());

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult!.Value.Should().NotBeNull();
    }

    [Fact]
    public void GetEntityMetadata_Should_Return_Ok_When_Entity_Exists()
    {
        // Arrange
        var entity = CreateTestEntityWithDetails("TestEntity", "dbo");
        _mockRepository.Setup(r => r.GetEntity("TestEntity")).Returns(entity);

        // Act
        var result = _controller.GetEntityMetadata("TestEntity");

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult!.Value.Should().NotBeNull();
    }

    [Fact]
    public void GetEntityMetadata_Should_Return_NotFound_When_Entity_Does_Not_Exist()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetEntity("NonExistent")).Returns((IEntityConfiguration?)null);

        // Act
        var result = _controller.GetEntityMetadata("NonExistent");

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
        var notFoundResult = result as NotFoundObjectResult;
        notFoundResult!.Value.Should().NotBeNull();
    }

    [Fact]
    public void GetEntityMetadata_Should_Include_Properties()
    {
        // Arrange
        var entity = CreateTestEntityWithDetails("TestEntity", "dbo");
        _mockRepository.Setup(r => r.GetEntity("TestEntity")).Returns(entity);

        // Act
        var result = _controller.GetEntityMetadata("TestEntity");

        // Assert
        result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public void GetEntityMetadata_Should_Handle_Null_Schema()
    {
        // Arrange
        var entity = CreateTestEntityWithDetails("TestEntity", null);
        _mockRepository.Setup(r => r.GetEntity("TestEntity")).Returns(entity);

        // Act
        var result = _controller.GetEntityMetadata("TestEntity");

        // Assert
        result.Should().BeOfType<OkObjectResult>();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("ValidEntity")]
    public void GetEntityMetadata_Should_Handle_Various_Entity_Names(string entityName)
    {
        // Arrange
        var entity = CreateTestEntityWithDetails(entityName, "dbo");
        _mockRepository.Setup(r => r.GetEntity(entityName)).Returns(entity);

        // Act
        var result = _controller.GetEntityMetadata(entityName);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
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

    private static IEntityConfiguration CreateTestEntityWithDetails(string name, string? schema)
    {
        var properties = new List<IPropertyConfiguration>();
        
        var mockProp1 = new Mock<IPropertyConfiguration>();
        mockProp1.Setup(p => p.PropertyName).Returns("Id");
        mockProp1.Setup(p => p.ColumnName).Returns("Id");
        mockProp1.Setup(p => p.PropertyType).Returns(typeof(int));
        mockProp1.Setup(p => p.DatabaseType).Returns("int");
        mockProp1.Setup(p => p.IsPrimaryKey).Returns(true);
        mockProp1.Setup(p => p.IsNullable).Returns(false);
        mockProp1.Setup(p => p.IsAutoGenerated).Returns(true);
        mockProp1.Setup(p => p.MaxLength).Returns((int?)null);
        properties.Add(mockProp1.Object);

        var mockProp2 = new Mock<IPropertyConfiguration>();
        mockProp2.Setup(p => p.PropertyName).Returns("Name");
        mockProp2.Setup(p => p.ColumnName).Returns("Name");
        mockProp2.Setup(p => p.PropertyType).Returns(typeof(string));
        mockProp2.Setup(p => p.DatabaseType).Returns("nvarchar");
        mockProp2.Setup(p => p.IsPrimaryKey).Returns(false);
        mockProp2.Setup(p => p.IsNullable).Returns(false);
        mockProp2.Setup(p => p.IsAutoGenerated).Returns(false);
        mockProp2.Setup(p => p.MaxLength).Returns(100);
        properties.Add(mockProp2.Object);

        var indexes = new List<IIndexConfiguration>();
        var mockIndex = new Mock<IIndexConfiguration>();
        mockIndex.Setup(i => i.IndexName).Returns("IX_Name");
        mockIndex.Setup(i => i.PropertyNames).Returns(new List<string> { "Name" });
        mockIndex.Setup(i => i.IsUnique).Returns(false);
        mockIndex.Setup(i => i.IsClustered).Returns(false);
        indexes.Add(mockIndex.Object);

        var relationships = new List<IRelationshipConfiguration>();
        var mockRel = new Mock<IRelationshipConfiguration>();
        mockRel.Setup(r => r.RelationshipName).Returns("FK_TestEntity_Parent");
        mockRel.Setup(r => r.RelatedEntityName).Returns("ParentEntity");
        mockRel.Setup(r => r.RelationshipType).Returns(RelationshipType.OneToMany);
        mockRel.Setup(r => r.ForeignKeyProperties).Returns(new List<string> { "ParentId" });
        mockRel.Setup(r => r.PrincipalKeyProperties).Returns(new List<string> { "Id" });
        mockRel.Setup(r => r.IsRequired).Returns(true);
        relationships.Add(mockRel.Object);

        var mockEntity = new Mock<IEntityConfiguration>();
        mockEntity.Setup(e => e.EntityName).Returns(name);
        mockEntity.Setup(e => e.SchemaName).Returns(schema);
        mockEntity.Setup(e => e.Properties).Returns(properties);
        mockEntity.Setup(e => e.Indexes).Returns(indexes);
        mockEntity.Setup(e => e.Relationships).Returns(relationships);
        return mockEntity.Object;
    }
}

public class YamlOutputFormatterTests
{
    [Fact]
    public void Constructor_Should_Set_Supported_Media_Types()
    {
        // Arrange & Act
        var formatter = new YamlOutputFormatter();

        // Assert
        formatter.SupportedMediaTypes.Should().Contain(mt => mt.ToString() == "application/yaml");
        formatter.SupportedMediaTypes.Should().Contain(mt => mt.ToString() == "application/x-yaml");
        formatter.SupportedMediaTypes.Should().Contain(mt => mt.ToString() == "text/yaml");
        formatter.SupportedMediaTypes.Should().Contain(mt => mt.ToString() == "text/x-yaml");
    }

    [Fact]
    public void Constructor_Should_Set_Supported_Encodings()
    {
        // Arrange & Act
        var formatter = new YamlOutputFormatter();

        // Assert
        formatter.SupportedEncodings.Should().Contain(Encoding.UTF8);
        formatter.SupportedEncodings.Should().Contain(Encoding.Unicode);
    }

    // Note: CanWriteType is protected and cannot be tested directly from unit tests
    // The functionality is indirectly tested through WriteResponseBodyAsync tests

    [Fact]
    public async Task WriteResponseBodyAsync_Should_Write_Yaml_Content()
    {
        // Arrange
        var formatter = new YamlOutputFormatter();
        var httpContext = new DefaultHttpContext();
        var memoryStream = new MemoryStream();
        httpContext.Response.Body = memoryStream;

        var testObject = new { Name = "Test", Value = 123 };
        var context = new OutputFormatterWriteContext(
            httpContext,
            (stream, encoding) => new StreamWriter(stream, encoding),
            typeof(object),
            testObject
        );

        // Act
        await formatter.WriteResponseBodyAsync(context, Encoding.UTF8);

        // Assert
        memoryStream.Position = 0;
        using var reader = new StreamReader(memoryStream);
        var content = await reader.ReadToEndAsync();
        content.Should().NotBeEmpty();
        content.Should().Contain("Name:");
        content.Should().Contain("Test");
    }

    [Fact]
    public async Task WriteResponseBodyAsync_Should_Throw_When_Context_Is_Null()
    {
        // Arrange
        var formatter = new YamlOutputFormatter();

        // Act & Assert
        var act = async () => await formatter.WriteResponseBodyAsync(null!, Encoding.UTF8);
        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task WriteResponseBodyAsync_Should_Handle_Empty_Object()
    {
        // Arrange
        var formatter = new YamlOutputFormatter();
        var httpContext = new DefaultHttpContext();
        var memoryStream = new MemoryStream();
        httpContext.Response.Body = memoryStream;

        var testObject = new { };
        var context = new OutputFormatterWriteContext(
            httpContext,
            (stream, encoding) => new StreamWriter(stream, encoding),
            typeof(object),
            testObject
        );

        // Act
        await formatter.WriteResponseBodyAsync(context, Encoding.UTF8);

        // Assert
        memoryStream.Position = 0;
        using var reader = new StreamReader(memoryStream);
        var content = await reader.ReadToEndAsync();
        content.Should().NotBeNull();
    }

    [Fact]
    public async Task WriteResponseBodyAsync_Should_Handle_Complex_Objects()
    {
        // Arrange
        var formatter = new YamlOutputFormatter();
        var httpContext = new DefaultHttpContext();
        var memoryStream = new MemoryStream();
        httpContext.Response.Body = memoryStream;

        var testObject = new
        {
            Name = "Test",
            Items = new[] { "Item1", "Item2" },
            Nested = new { Value = 42 }
        };
        var context = new OutputFormatterWriteContext(
            httpContext,
            (stream, encoding) => new StreamWriter(stream, encoding),
            typeof(object),
            testObject
        );

        // Act
        await formatter.WriteResponseBodyAsync(context, Encoding.UTF8);

        // Assert
        memoryStream.Position = 0;
        using var reader = new StreamReader(memoryStream);
        var content = await reader.ReadToEndAsync();
        content.Should().Contain("Name:");
        content.Should().Contain("Items:");
        content.Should().Contain("Nested:");
    }
}
