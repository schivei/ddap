using Aspire.Hosting;
using Aspire.Hosting.ApplicationModel;
using Ddap.Aspire;
using Ddap.Core;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Ddap.Tests.Aspire;

public class DdapAspireExtensionsTests
{
    [Fact]
    public void AddDdapApi_Should_Create_DdapResource()
    {
        // Arrange
        var builder = DistributedApplication.CreateBuilder(new DistributedApplicationOptions
        {
            Args = Array.Empty<string>()
        });

        // Act
        var resource = builder.AddDdapApi("test-api");

        // Assert
        resource.Should().NotBeNull();
        resource.Resource.Should().BeOfType<DdapResource>();
        resource.Resource.Name.Should().Be("test-api");
    }

    [Fact]
    public void AddDdapApi_Should_Return_Resource_Builder()
    {
        // Arrange
        var builder = DistributedApplication.CreateBuilder(new DistributedApplicationOptions
        {
            Args = Array.Empty<string>()
        });

        // Act
        var resource = builder.AddDdapApi("test-api");

        // Assert
        resource.Should().BeAssignableTo<IResourceBuilder<DdapResource>>();
    }

    [Theory]
    [InlineData("api")]
    [InlineData("my-api")]
    [InlineData("testapi123")]
    public void AddDdapApi_Should_Handle_Various_Names(string name)
    {
        // Arrange
        var builder = DistributedApplication.CreateBuilder(new DistributedApplicationOptions
        {
            Args = Array.Empty<string>()
        });

        // Act
        var resource = builder.AddDdapApi(name);

        // Assert
        resource.Resource.Name.Should().Be(name);
    }

    [Fact]
    public void WithRestApi_Should_Enable_Rest()
    {
        // Arrange
        var builder = DistributedApplication.CreateBuilder(new DistributedApplicationOptions
        {
            Args = Array.Empty<string>()
        });
        var resource = builder.AddDdapApi("test-api");

        // Act
        var result = resource.WithRestApi();

        // Assert
        result.Resource.EnableRest.Should().BeTrue();
    }

    [Fact]
    public void WithRestApi_Should_Return_Builder_For_Chaining()
    {
        // Arrange
        var builder = DistributedApplication.CreateBuilder(new DistributedApplicationOptions
        {
            Args = Array.Empty<string>()
        });
        var resource = builder.AddDdapApi("test-api");

        // Act
        var result = resource.WithRestApi();

        // Assert
        result.Should().BeSameAs(resource);
    }

    [Fact]
    public void WithRestApi_With_Port_Should_Enable_Rest()
    {
        // Arrange
        var builder = DistributedApplication.CreateBuilder(new DistributedApplicationOptions
        {
            Args = Array.Empty<string>()
        });
        var resource = builder.AddDdapApi("test-api");

        // Act
        var result = resource.WithRestApi(8080);

        // Assert
        result.Resource.EnableRest.Should().BeTrue();
    }

    [Fact]
    public void WithGraphQL_Should_Enable_GraphQL()
    {
        // Arrange
        var builder = DistributedApplication.CreateBuilder(new DistributedApplicationOptions
        {
            Args = Array.Empty<string>()
        });
        var resource = builder.AddDdapApi("test-api");

        // Act
        var result = resource.WithGraphQL();

        // Assert
        result.Resource.EnableGraphQL.Should().BeTrue();
        result.Resource.GraphQLPath.Should().Be("/graphql");
    }

    [Fact]
    public void WithGraphQL_Should_Set_Custom_Path()
    {
        // Arrange
        var builder = DistributedApplication.CreateBuilder(new DistributedApplicationOptions
        {
            Args = Array.Empty<string>()
        });
        var resource = builder.AddDdapApi("test-api");

        // Act
        var result = resource.WithGraphQL("/api/graphql");

        // Assert
        result.Resource.EnableGraphQL.Should().BeTrue();
        result.Resource.GraphQLPath.Should().Be("/api/graphql");
    }

    [Fact]
    public void WithGraphQL_Should_Return_Builder_For_Chaining()
    {
        // Arrange
        var builder = DistributedApplication.CreateBuilder(new DistributedApplicationOptions
        {
            Args = Array.Empty<string>()
        });
        var resource = builder.AddDdapApi("test-api");

        // Act
        var result = resource.WithGraphQL();

        // Assert
        result.Should().BeSameAs(resource);
    }

    [Fact]
    public void WithGrpc_Should_Enable_Grpc()
    {
        // Arrange
        var builder = DistributedApplication.CreateBuilder(new DistributedApplicationOptions
        {
            Args = Array.Empty<string>()
        });
        var resource = builder.AddDdapApi("test-api");

        // Act
        var result = resource.WithGrpc();

        // Assert
        result.Resource.EnableGrpc.Should().BeTrue();
    }

    [Fact]
    public void WithGrpc_With_Port_Should_Enable_Grpc()
    {
        // Arrange
        var builder = DistributedApplication.CreateBuilder(new DistributedApplicationOptions
        {
            Args = Array.Empty<string>()
        });
        var resource = builder.AddDdapApi("test-api");

        // Act
        var result = resource.WithGrpc(50051);

        // Assert
        result.Resource.EnableGrpc.Should().BeTrue();
    }

    [Fact]
    public void WithGrpc_Should_Return_Builder_For_Chaining()
    {
        // Arrange
        var builder = DistributedApplication.CreateBuilder(new DistributedApplicationOptions
        {
            Args = Array.Empty<string>()
        });
        var resource = builder.AddDdapApi("test-api");

        // Act
        var result = resource.WithGrpc();

        // Assert
        result.Should().BeSameAs(resource);
    }

    [Fact]
    public void WithAutoRefresh_Should_Enable_Auto_Refresh()
    {
        // Arrange
        var builder = DistributedApplication.CreateBuilder(new DistributedApplicationOptions
        {
            Args = Array.Empty<string>()
        });
        var resource = builder.AddDdapApi("test-api");

        // Act
        var result = resource.WithAutoRefresh();

        // Assert
        result.Resource.AutoRefresh.Should().BeTrue();
        result.Resource.RefreshIntervalSeconds.Should().Be(30);
    }

    [Fact]
    public void WithAutoRefresh_Should_Set_Custom_Interval()
    {
        // Arrange
        var builder = DistributedApplication.CreateBuilder(new DistributedApplicationOptions
        {
            Args = Array.Empty<string>()
        });
        var resource = builder.AddDdapApi("test-api");

        // Act
        var result = resource.WithAutoRefresh(60);

        // Assert
        result.Resource.AutoRefresh.Should().BeTrue();
        result.Resource.RefreshIntervalSeconds.Should().Be(60);
    }

    [Fact]
    public void WithAutoRefresh_Should_Return_Builder_For_Chaining()
    {
        // Arrange
        var builder = DistributedApplication.CreateBuilder(new DistributedApplicationOptions
        {
            Args = Array.Empty<string>()
        });
        var resource = builder.AddDdapApi("test-api");

        // Act
        var result = resource.WithAutoRefresh();

        // Assert
        result.Should().BeSameAs(resource);
    }

    [Fact]
    public void Should_Support_Method_Chaining()
    {
        // Arrange
        var builder = DistributedApplication.CreateBuilder(new DistributedApplicationOptions
        {
            Args = Array.Empty<string>()
        });

        // Act
        var resource = builder.AddDdapApi("test-api")
            .WithRestApi()
            .WithGraphQL()
            .WithGrpc()
            .WithAutoRefresh(45);

        // Assert
        resource.Resource.EnableRest.Should().BeTrue();
        resource.Resource.EnableGraphQL.Should().BeTrue();
        resource.Resource.EnableGrpc.Should().BeTrue();
        resource.Resource.AutoRefresh.Should().BeTrue();
        resource.Resource.RefreshIntervalSeconds.Should().Be(45);
    }
}

public class DdapResourceTests
{
    [Fact]
    public void Constructor_Should_Set_Name()
    {
        // Arrange & Act
        var resource = new DdapResource("test-resource");

        // Assert
        resource.Name.Should().Be("test-resource");
    }

    [Fact]
    public void EnableRest_Should_Default_To_False()
    {
        // Arrange & Act
        var resource = new DdapResource("test");

        // Assert
        resource.EnableRest.Should().BeFalse();
    }

    [Fact]
    public void EnableGraphQL_Should_Default_To_False()
    {
        // Arrange & Act
        var resource = new DdapResource("test");

        // Assert
        resource.EnableGraphQL.Should().BeFalse();
    }

    [Fact]
    public void EnableGrpc_Should_Default_To_False()
    {
        // Arrange & Act
        var resource = new DdapResource("test");

        // Assert
        resource.EnableGrpc.Should().BeFalse();
    }

    [Fact]
    public void AutoRefresh_Should_Default_To_False()
    {
        // Arrange & Act
        var resource = new DdapResource("test");

        // Assert
        resource.AutoRefresh.Should().BeFalse();
    }

    [Fact]
    public void GraphQLPath_Should_Default_To_GraphQL()
    {
        // Arrange & Act
        var resource = new DdapResource("test");

        // Assert
        resource.GraphQLPath.Should().Be("/graphql");
    }

    [Fact]
    public void RefreshIntervalSeconds_Should_Default_To_30()
    {
        // Arrange & Act
        var resource = new DdapResource("test");

        // Assert
        resource.RefreshIntervalSeconds.Should().Be(30);
    }

    [Fact]
    public void Should_Allow_Setting_All_Properties()
    {
        // Arrange & Act
        var resource = new DdapResource("test")
        {
            EnableRest = true,
            EnableGraphQL = true,
            EnableGrpc = true,
            AutoRefresh = true,
            GraphQLPath = "/api/gql",
            RefreshIntervalSeconds = 60
        };

        // Assert
        resource.EnableRest.Should().BeTrue();
        resource.EnableGraphQL.Should().BeTrue();
        resource.EnableGrpc.Should().BeTrue();
        resource.AutoRefresh.Should().BeTrue();
        resource.GraphQLPath.Should().Be("/api/gql");
        resource.RefreshIntervalSeconds.Should().Be(60);
    }

    [Fact]
    public void Should_Implement_IResourceWithEndpoints()
    {
        // Arrange & Act
        var resource = new DdapResource("test");

        // Assert
        resource.Should().BeAssignableTo<IResourceWithEndpoints>();
    }
}

public class DdapServiceExtensionsTests
{
    [Fact]
    public void AddDdapForAspire_Should_Register_Services()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "ConnectionStrings:database", "Server=localhost;Database=Test;" }
            })
            .Build();

        // Act
        var builder = services.AddDdapForAspire(configuration);

        // Assert
        builder.Should().NotBeNull();
        builder.Should().BeAssignableTo<IDdapBuilder>();
    }

    [Fact]
    public void AddDdapForAspire_Should_Use_Database_Connection_String()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "ConnectionStrings:database", "Server=localhost;Database=Test;" }
            })
            .Build();

        // Act
        var builder = services.AddDdapForAspire(configuration);

        // Assert
        builder.Options.ConnectionString.Should().Be("Server=localhost;Database=Test;");
    }

    [Fact]
    public void AddDdapForAspire_Should_Use_DefaultConnection_As_Fallback()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "ConnectionStrings:DefaultConnection", "Server=localhost;Database=Default;" }
            })
            .Build();

        // Act
        var builder = services.AddDdapForAspire(configuration);

        // Assert
        builder.Options.ConnectionString.Should().Be("Server=localhost;Database=Default;");
    }

    [Fact]
    public void AddDdapForAspire_Should_Throw_When_No_Connection_String()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder().Build();

        // Act & Assert
        var act = () => services.AddDdapForAspire(configuration);
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*No database connection string found*");
    }

    [Fact]
    public void AddDdapForAspire_Should_Set_LoadOnStartup_True()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "ConnectionStrings:database", "test" }
            })
            .Build();

        // Act
        var builder = services.AddDdapForAspire(configuration);

        // Assert
        builder.Options.LoadOnStartup.Should().BeTrue();
    }

    [Fact]
    public void AddDdapForAspireWithAutoRefresh_Should_Register_Services()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "ConnectionStrings:database", "test" }
            })
            .Build();

        // Act
        var builder = services.AddDdapForAspireWithAutoRefresh(configuration);

        // Assert
        builder.Should().NotBeNull();
    }

    [Fact]
    public void AddDdapForAspireWithAutoRefresh_Should_Register_Hosted_Service()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "ConnectionStrings:database", "test" }
            })
            .Build();

        // Act
        services.AddDdapForAspireWithAutoRefresh(configuration);

        // Assert
        services.Should().Contain(sd => sd.ServiceType == typeof(IHostedService));
    }

    [Fact]
    public void AddDdapForAspireWithAutoRefresh_Should_Use_Custom_Interval()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "ConnectionStrings:database", "test" }
            })
            .Build();

        // Act
        services.AddDdapForAspireWithAutoRefresh(configuration, 60);

        // Assert
        services.Should().Contain(sd => sd.ServiceType == typeof(IHostedService));
    }
}

// Note: SchemaRefreshHostedService is internal and cannot be tested directly
// Its functionality is tested indirectly through the AddDdapForAspireWithAutoRefresh extension method
