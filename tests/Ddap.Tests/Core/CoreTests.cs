using Ddap.Core;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace Ddap.Tests.Core;

public class DdapOptionsTests
{
    [Fact]
    public void DdapOptions_Should_Have_Default_Values()
    {
        // Arrange & Act
        var options = new DdapOptions();

        // Assert
        options.ConnectionString.Should().BeEmpty();
        options.LoadOnStartup.Should().BeTrue();
        options.Provider.Should().Be(DatabaseProvider.SQLServer);
    }

    [Fact]
    public void DdapOptions_Should_Allow_Setting_ConnectionString()
    {
        // Arrange
        var options = new DdapOptions();
        var connectionString = "Server=localhost;Database=Test;";

        // Act
        options.ConnectionString = connectionString;

        // Assert
        options.ConnectionString.Should().Be(connectionString);
    }

    [Theory]
    [InlineData(DatabaseProvider.SQLServer)]
    [InlineData(DatabaseProvider.MySQL)]
    [InlineData(DatabaseProvider.PostgreSQL)]
    public void DdapOptions_Should_Support_All_Database_Providers(DatabaseProvider provider)
    {
        // Arrange
        var options = new DdapOptions();

        // Act
        options.Provider = provider;

        // Assert
        options.Provider.Should().Be(provider);
    }
}

public class DdapServiceCollectionExtensionsTests
{
    [Fact]
    public void AddDdap_Should_Register_Services()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        var builder = services.AddDdap(options =>
        {
            options.ConnectionString = "test";
            options.Provider = DatabaseProvider.SQLServer;
        });

        // Assert
        builder.Should().NotBeNull();
        builder.Services.Should().BeSameAs(services);
        
        var serviceProvider = services.BuildServiceProvider();
        var entityRepository = serviceProvider.GetService<IEntityRepository>();
        entityRepository.Should().NotBeNull();
    }

    [Fact]
    public void AddDdap_Should_Return_IDdapBuilder_For_Chaining()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        var builder = services.AddDdap(options => { });

        // Assert
        builder.Should().BeAssignableTo<IDdapBuilder>();
        builder.Services.Should().BeSameAs(services);
    }
}

public class EntityRepositoryTests
{
    [Fact]
    public void EntityRepository_Should_Start_Empty()
    {
        // Arrange & Act
        var repository = new EntityRepository();

        // Assert
        repository.GetAllEntities().Should().BeEmpty();
    }

    [Fact]
    public void EntityRepository_Should_Add_Entity()
    {
        // Arrange
        var repository = new EntityRepository();
        var entity = CreateTestEntity("TestEntity");

        // Act
        repository.AddOrUpdateEntity(entity);

        // Assert
        repository.GetAllEntities().Should().HaveCount(1);
        repository.GetEntity("TestEntity").Should().NotBeNull();
    }

    [Fact]
    public void EntityRepository_Should_Update_Existing_Entity()
    {
        // Arrange
        var repository = new EntityRepository();
        var entity1 = CreateTestEntity("TestEntity");
        var entity2 = CreateTestEntity("TestEntity", "UpdatedSchema");

        // Act
        repository.AddOrUpdateEntity(entity1);
        repository.AddOrUpdateEntity(entity2);

        // Assert
        repository.GetAllEntities().Should().HaveCount(1);
        repository.GetEntity("TestEntity")!.SchemaName.Should().Be("UpdatedSchema");
    }

    [Fact]
    public void EntityRepository_Should_Clear_All_Entities()
    {
        // Arrange
        var repository = new EntityRepository();
        repository.AddOrUpdateEntity(CreateTestEntity("Entity1"));
        repository.AddOrUpdateEntity(CreateTestEntity("Entity2"));

        // Act
        repository.Clear();

        // Assert
        repository.GetAllEntities().Should().BeEmpty();
    }

    [Fact]
    public void EntityRepository_Should_Return_Null_For_NonExistent_Entity()
    {
        // Arrange
        var repository = new EntityRepository();

        // Act
        var entity = repository.GetEntity("NonExistent");

        // Assert
        entity.Should().BeNull();
    }

    private static IEntityConfiguration CreateTestEntity(string name, string? schema = "dbo")
    {
        var mockEntity = new Mock<IEntityConfiguration>();
        mockEntity.Setup(e => e.EntityName).Returns(name);
        mockEntity.Setup(e => e.SchemaName).Returns(schema);
        mockEntity.Setup(e => e.Properties).Returns(new List<IPropertyConfiguration>());
        mockEntity.Setup(e => e.Indexes).Returns(new List<IIndexConfiguration>());
        mockEntity.Setup(e => e.Relationships).Returns(new List<IRelationshipConfiguration>());
        return mockEntity.Object;
    }
}
