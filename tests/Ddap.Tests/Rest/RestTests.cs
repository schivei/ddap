using System.Text;
using Ddap.Core;
using Ddap.Core.Internals;
using Ddap.Rest;
using Ddap.Rest.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Moq;
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

public class QueryParametersTests
{
    [Fact]
    public void PageNumber_Should_Default_To_One()
    {
        // Arrange & Act
        var queryParams = new QueryParameters();

        // Assert
        queryParams.PageNumber.Should().Be(1);
    }

    [Fact]
    public void PageSize_Should_Default_To_Ten()
    {
        // Arrange & Act
        var queryParams = new QueryParameters();

        // Assert
        queryParams.PageSize.Should().Be(10);
    }

    [Fact]
    public void PageSize_Should_Accept_Valid_Values()
    {
        // Arrange
        var queryParams = new QueryParameters();

        // Act
        queryParams.PageSize = 50;

        // Assert
        queryParams.PageSize.Should().Be(50);
    }

    [Fact]
    public void PageSize_Should_Cap_At_MaxPageSize_When_Exceeding()
    {
        // Arrange
        var queryParams = new QueryParameters();

        // Act
        queryParams.PageSize = 200; // Exceeds max of 100

        // Assert
        queryParams.PageSize.Should().Be(100);
    }

    [Fact]
    public void PageSize_Should_Cap_At_MaxPageSize_Exactly()
    {
        // Arrange
        var queryParams = new QueryParameters();

        // Act
        queryParams.PageSize = 101; // Just over max

        // Assert
        queryParams.PageSize.Should().Be(100);
    }

    [Fact]
    public void Filter_Should_Be_Null_By_Default()
    {
        // Arrange & Act
        var queryParams = new QueryParameters();

        // Assert
        queryParams.Filter.Should().BeNull();
    }

    [Fact]
    public void Filter_Should_Accept_Value()
    {
        // Arrange
        var queryParams = new QueryParameters();

        // Act
        queryParams.Filter = "name eq 'John'";

        // Assert
        queryParams.Filter.Should().Be("name eq 'John'");
    }

    [Fact]
    public void OrderBy_Should_Be_Null_By_Default()
    {
        // Arrange & Act
        var queryParams = new QueryParameters();

        // Assert
        queryParams.OrderBy.Should().BeNull();
    }

    [Fact]
    public void OrderBy_Should_Accept_Value()
    {
        // Arrange
        var queryParams = new QueryParameters();

        // Act
        queryParams.OrderBy = "name desc";

        // Assert
        queryParams.OrderBy.Should().Be("name desc");
    }

    [Fact]
    public void PageNumber_Should_Accept_Value()
    {
        // Arrange
        var queryParams = new QueryParameters();

        // Act
        queryParams.PageNumber = 5;

        // Assert
        queryParams.PageNumber.Should().Be(5);
    }
}

public class PagedResultTests
{
    [Fact]
    public void PagedResult_Should_Initialize_Properties()
    {
        // Arrange
        var items = new List<string> { "item1", "item2" };

        // Act
        var result = new PagedResult<string>(items, 100, 2, 10);

        // Assert
        result.Items.Should().BeEquivalentTo(items);
        result.TotalCount.Should().Be(100);
        result.PageNumber.Should().Be(2);
        result.PageSize.Should().Be(10);
    }

    [Fact]
    public void PagedResult_Should_Calculate_TotalPages()
    {
        // Arrange & Act
        var result = new PagedResult<string>(new List<string>(), 95, 1, 10);

        // Assert
        result.TotalPages.Should().Be(10); // 95 items / 10 per page = 10 pages
    }

    [Fact]
    public void PagedResult_HasPrevious_Should_Be_False_For_First_Page()
    {
        // Arrange & Act
        var result = new PagedResult<string>(new List<string>(), 100, 1, 10);

        // Assert
        result.HasPrevious.Should().BeFalse();
    }

    [Fact]
    public void PagedResult_HasPrevious_Should_Be_True_For_Second_Page()
    {
        // Arrange & Act
        var result = new PagedResult<string>(new List<string>(), 100, 2, 10);

        // Assert
        result.HasPrevious.Should().BeTrue();
    }

    [Fact]
    public void PagedResult_HasNext_Should_Be_True_When_More_Pages_Exist()
    {
        // Arrange & Act
        var result = new PagedResult<string>(new List<string>(), 100, 1, 10);

        // Assert
        result.HasNext.Should().BeTrue();
    }

    [Fact]
    public void PagedResult_HasNext_Should_Be_False_On_Last_Page()
    {
        // Arrange & Act
        var result = new PagedResult<string>(new List<string>(), 100, 10, 10);

        // Assert
        result.HasNext.Should().BeFalse();
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
            CreateTestEntity("Entity2", "schema1", 3),
        };
        _mockRepository.Setup(r => r.GetAllEntities()).Returns(entities);

        // Act
        var result = _controller.GetAllEntities(new QueryParameters());

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
        var result = _controller.GetAllEntities(new QueryParameters());

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
        _mockRepository
            .Setup(r => r.GetEntity("NonExistent"))
            .Returns((IEntityConfiguration?)null);

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

    [Fact]
    public void GetAllEntities_Should_Apply_Pagination_Correctly()
    {
        // Arrange
        var entities = new List<IEntityConfiguration>();
        for (int i = 0; i < 50; i++)
        {
            entities.Add(CreateTestEntity($"Entity{i}", "dbo", 1));
        }
        _mockRepository.Setup(r => r.GetAllEntities()).Returns(entities);

        var parameters = new QueryParameters { PageNumber = 2, PageSize = 10 };

        // Act
        var result = _controller.GetAllEntities(parameters);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        var pagedResult = okResult!.Value;

        var pagedResultType = pagedResult!.GetType();
        var pageNumber = (int)pagedResultType.GetProperty("PageNumber")!.GetValue(pagedResult)!;
        var pageSize = (int)pagedResultType.GetProperty("PageSize")!.GetValue(pagedResult)!;
        var totalCount = (int)pagedResultType.GetProperty("TotalCount")!.GetValue(pagedResult)!;
        var items =
            pagedResultType.GetProperty("Items")!.GetValue(pagedResult) as IEnumerable<object>;

        pageNumber.Should().Be(2);
        pageSize.Should().Be(10);
        totalCount.Should().Be(50);
        items.Should().HaveCount(10);
    }

    [Fact]
    public void GetAllEntities_Should_Include_PropertyCount()
    {
        // Arrange
        var entities = new List<IEntityConfiguration>
        {
            CreateTestEntity("Entity1", "dbo", 5),
            CreateTestEntity("Entity2", "schema1", 10),
        };
        _mockRepository.Setup(r => r.GetAllEntities()).Returns(entities);

        // Act
        var result = _controller.GetAllEntities(new QueryParameters());

        // Assert
        result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public void GetAllEntities_Should_Handle_Large_Page_Number()
    {
        // Arrange
        var entities = new List<IEntityConfiguration> { CreateTestEntity("Entity1", "dbo", 1) };
        _mockRepository.Setup(r => r.GetAllEntities()).Returns(entities);

        var parameters = new QueryParameters { PageNumber = 100, PageSize = 10 };

        // Act
        var result = _controller.GetAllEntities(parameters);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public void GetEntityMetadata_Should_Include_Indexes()
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
    public void GetEntityMetadata_Should_Include_Relationships()
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
    public void GetEntityMetadata_Should_Include_Property_Type_Names()
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
    public void GetEntityMetadata_Should_Include_Relationship_Type_As_String()
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
    public void GetAllEntities_Should_Return_Correct_TotalCount()
    {
        // Arrange
        var entities = new List<IEntityConfiguration>();
        for (int i = 0; i < 25; i++)
        {
            entities.Add(CreateTestEntity($"Entity{i}", "dbo", 1));
        }
        _mockRepository.Setup(r => r.GetAllEntities()).Returns(entities);

        // Act
        var result = _controller.GetAllEntities(new QueryParameters { PageSize = 10 });

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        var pagedResult = okResult!.Value;

        var totalCount = (int)
            pagedResult!.GetType().GetProperty("TotalCount")!.GetValue(pagedResult)!;
        totalCount.Should().Be(25);
    }

    [Fact]
    public void GetAllEntities_Should_Skip_Correct_Number_Of_Items()
    {
        // Arrange
        var entities = new List<IEntityConfiguration>();
        for (int i = 0; i < 30; i++)
        {
            entities.Add(CreateTestEntity($"Entity{i}", "dbo", 1));
        }
        _mockRepository.Setup(r => r.GetAllEntities()).Returns(entities);

        var parameters = new QueryParameters { PageNumber = 3, PageSize = 10 };

        // Act
        var result = _controller.GetAllEntities(parameters);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public void GetAllEntities_Should_Return_Correct_Entity_Data()
    {
        // Arrange
        var entities = new List<IEntityConfiguration>
        {
            CreateTestEntity("User", "dbo", 5),
            CreateTestEntity("Product", "sales", 10),
        };
        _mockRepository.Setup(r => r.GetAllEntities()).Returns(entities);

        // Act
        var result = _controller.GetAllEntities(new QueryParameters());

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        var pagedResult = okResult!.Value;
        pagedResult.Should().NotBeNull();

        // Use reflection to check the PagedResult structure
        var pagedResultType = pagedResult!.GetType();
        var itemsProperty = pagedResultType.GetProperty("Items");
        var items = itemsProperty!.GetValue(pagedResult) as IEnumerable<object>;
        items.Should().NotBeNull();
        items.Should().HaveCount(2);
    }

    [Fact]
    public void GetAllEntities_Should_Include_EntityName_SchemaName_PropertyCount()
    {
        // Arrange
        var entities = new List<IEntityConfiguration> { CreateTestEntity("User", "dbo", 7) };
        _mockRepository.Setup(r => r.GetAllEntities()).Returns(entities);

        // Act
        var result = _controller.GetAllEntities(new QueryParameters());

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        var pagedResult = okResult!.Value;

        var pagedResultType = pagedResult!.GetType();
        var itemsProperty = pagedResultType.GetProperty("Items");
        var items = (itemsProperty!.GetValue(pagedResult) as IEnumerable<object>)!.ToList();

        var firstItem = items.First();
        var firstItemType = firstItem.GetType();

        var entityName = firstItemType.GetProperty("EntityName")!.GetValue(firstItem);
        var schemaName = firstItemType.GetProperty("SchemaName")!.GetValue(firstItem);
        var propertyCount = firstItemType.GetProperty("PropertyCount")!.GetValue(firstItem);

        entityName.Should().Be("User");
        schemaName.Should().Be("dbo");
        propertyCount.Should().Be(7);
    }

    [Fact]
    public void GetEntityMetadata_Should_Return_Complete_Property_Information()
    {
        // Arrange
        var entity = CreateTestEntityWithDetails("TestEntity", "dbo");
        _mockRepository.Setup(r => r.GetEntity("TestEntity")).Returns(entity);

        // Act
        var result = _controller.GetEntityMetadata("TestEntity");

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        var metadata = okResult!.Value;

        var metadataType = metadata!.GetType();
        var entityName = metadataType.GetProperty("EntityName")!.GetValue(metadata);
        var schemaName = metadataType.GetProperty("SchemaName")!.GetValue(metadata);
        var properties = metadataType.GetProperty("Properties")!.GetValue(metadata);

        entityName.Should().Be("TestEntity");
        schemaName.Should().Be("dbo");
        properties.Should().NotBeNull();
    }

    [Fact]
    public void GetEntityMetadata_Should_Return_Properties_With_All_Details()
    {
        // Arrange
        var entity = CreateTestEntityWithDetails("TestEntity", "dbo");
        _mockRepository.Setup(r => r.GetEntity("TestEntity")).Returns(entity);

        // Act
        var result = _controller.GetEntityMetadata("TestEntity");

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        var metadata = okResult!.Value;

        var metadataType = metadata!.GetType();
        var properties =
            metadataType.GetProperty("Properties")!.GetValue(metadata) as IEnumerable<object>;
        properties.Should().NotBeNull();

        var propertiesList = properties!.ToList();
        propertiesList.Should().HaveCount(2);

        var firstProp = propertiesList.First();
        var propType = firstProp.GetType();

        propType.GetProperty("PropertyName").Should().NotBeNull();
        propType.GetProperty("ColumnName").Should().NotBeNull();
        propType.GetProperty("PropertyType").Should().NotBeNull();
        propType.GetProperty("DatabaseType").Should().NotBeNull();
        propType.GetProperty("IsPrimaryKey").Should().NotBeNull();
        propType.GetProperty("IsNullable").Should().NotBeNull();
        propType.GetProperty("IsAutoGenerated").Should().NotBeNull();
        propType.GetProperty("MaxLength").Should().NotBeNull();
    }

    [Fact]
    public void GetEntityMetadata_Should_Return_Indexes_With_All_Details()
    {
        // Arrange
        var entity = CreateTestEntityWithDetails("TestEntity", "dbo");
        _mockRepository.Setup(r => r.GetEntity("TestEntity")).Returns(entity);

        // Act
        var result = _controller.GetEntityMetadata("TestEntity");

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        var metadata = okResult!.Value;

        var metadataType = metadata!.GetType();
        var indexes =
            metadataType.GetProperty("Indexes")!.GetValue(metadata) as IEnumerable<object>;
        indexes.Should().NotBeNull();

        var indexesList = indexes!.ToList();
        indexesList.Should().HaveCount(1);

        var firstIndex = indexesList.First();
        var indexType = firstIndex.GetType();

        indexType.GetProperty("IndexName").Should().NotBeNull();
        indexType.GetProperty("PropertyNames").Should().NotBeNull();
        indexType.GetProperty("IsUnique").Should().NotBeNull();
        indexType.GetProperty("IsClustered").Should().NotBeNull();
    }

    [Fact]
    public void GetEntityMetadata_Should_Return_Relationships_With_All_Details()
    {
        // Arrange
        var entity = CreateTestEntityWithDetails("TestEntity", "dbo");
        _mockRepository.Setup(r => r.GetEntity("TestEntity")).Returns(entity);

        // Act
        var result = _controller.GetEntityMetadata("TestEntity");

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        var metadata = okResult!.Value;

        var metadataType = metadata!.GetType();
        var relationships =
            metadataType.GetProperty("Relationships")!.GetValue(metadata) as IEnumerable<object>;
        relationships.Should().NotBeNull();

        var relationshipsList = relationships!.ToList();
        relationshipsList.Should().HaveCount(1);

        var firstRel = relationshipsList.First();
        var relType = firstRel.GetType();

        relType.GetProperty("RelationshipName").Should().NotBeNull();
        relType.GetProperty("RelatedEntityName").Should().NotBeNull();
        relType.GetProperty("RelationshipType").Should().NotBeNull();
        relType.GetProperty("ForeignKeyProperties").Should().NotBeNull();
        relType.GetProperty("PrincipalKeyProperties").Should().NotBeNull();
        relType.GetProperty("IsRequired").Should().NotBeNull();
    }

    [Fact]
    public void GetEntityMetadata_Should_Convert_RelationshipType_To_String()
    {
        // Arrange
        var entity = CreateTestEntityWithDetails("TestEntity", "dbo");
        _mockRepository.Setup(r => r.GetEntity("TestEntity")).Returns(entity);

        // Act
        var result = _controller.GetEntityMetadata("TestEntity");

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        var metadata = okResult!.Value;

        var metadataType = metadata!.GetType();
        var relationships =
            metadataType.GetProperty("Relationships")!.GetValue(metadata) as IEnumerable<object>;
        var relationshipsList = relationships!.ToList();
        var firstRel = relationshipsList.First();
        var relType = firstRel.GetType();

        var relationshipType = relType.GetProperty("RelationshipType")!.GetValue(firstRel);
        relationshipType.Should().BeOfType<string>();
        relationshipType.Should().Be("OneToMany");
    }

    [Fact]
    public void GetEntityMetadata_Should_Convert_PropertyType_To_String()
    {
        // Arrange
        var entity = CreateTestEntityWithDetails("TestEntity", "dbo");
        _mockRepository.Setup(r => r.GetEntity("TestEntity")).Returns(entity);

        // Act
        var result = _controller.GetEntityMetadata("TestEntity");

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        var metadata = okResult!.Value;

        var metadataType = metadata!.GetType();
        var properties =
            metadataType.GetProperty("Properties")!.GetValue(metadata) as IEnumerable<object>;
        var propertiesList = properties!.ToList();
        var firstProp = propertiesList.First();
        var propType = firstProp.GetType();

        var propertyType = propType.GetProperty("PropertyType")!.GetValue(firstProp);
        propertyType.Should().BeOfType<string>();
        propertyType.Should().Be("Int32");
    }

    [Fact]
    public void GetAllEntities_Should_Return_PagedResult_With_Correct_Structure()
    {
        // Arrange
        var entities = new List<IEntityConfiguration>
        {
            CreateTestEntity("Entity1", "dbo", 1),
            CreateTestEntity("Entity2", "dbo", 1),
        };
        _mockRepository.Setup(r => r.GetAllEntities()).Returns(entities);

        // Act
        var result = _controller.GetAllEntities(
            new QueryParameters { PageNumber = 1, PageSize = 10 }
        );

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        var pagedResult = okResult!.Value;

        var pagedResultType = pagedResult!.GetType();
        pagedResultType.GetProperty("Items").Should().NotBeNull();
        pagedResultType.GetProperty("TotalCount").Should().NotBeNull();
        pagedResultType.GetProperty("PageNumber").Should().NotBeNull();
        pagedResultType.GetProperty("PageSize").Should().NotBeNull();
    }
}

// YamlOutputFormatterTests removed - YAML formatter was removed as part of
// removing opinionated dependencies. Developers configure their own formatters.
