using Ddap.Core;
using Ddap.Data.Dapper.SqlServer;
using Ddap.Data.Dapper.MySQL;
using Ddap.Data.Dapper.PostgreSQL;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Xunit;

namespace Ddap.Tests.Data;

public class SqlServerDataProviderExtensionsTests
{
    [Fact]
    public void AddSqlServerDapper_Should_Register_DataProvider()
    {
        // Arrange
        var services = new ServiceCollection();
        var builder = services.AddDdap(options =>
        {
            options.ConnectionString = "Server=localhost;Database=Test;";
            options.Provider = DatabaseProvider.SQLServer;
        });

        // Act
        builder.AddSqlServerDapper();

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var dataProvider = serviceProvider.GetService<IDataProvider>();
        dataProvider.Should().NotBeNull();
        dataProvider.Should().BeOfType<SqlServerDataProvider>();
    }

    [Fact]
    public void AddSqlServerDapper_Should_Register_As_Singleton()
    {
        // Arrange
        var services = new ServiceCollection();
        var builder = services.AddDdap(options =>
        {
            options.ConnectionString = "test";
        });

        // Act
        builder.AddSqlServerDapper();

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var provider1 = serviceProvider.GetService<IDataProvider>();
        var provider2 = serviceProvider.GetService<IDataProvider>();
        provider1.Should().BeSameAs(provider2);
    }

    [Fact]
    public void AddSqlServerDapper_Should_Register_Hosted_Service()
    {
        // Arrange
        var services = new ServiceCollection();
        var builder = services.AddDdap(options =>
        {
            options.ConnectionString = "test";
        });

        // Act
        builder.AddSqlServerDapper();

        // Assert
        var hostedService = services.FirstOrDefault(sd =>
            sd.ServiceType == typeof(IHostedService) &&
            sd.ImplementationType != null &&
            sd.ImplementationType.Name == "EntityLoaderHostedService");
        hostedService.Should().NotBeNull();
    }

    [Fact]
    public void AddSqlServerDapper_Should_Return_Builder_For_Chaining()
    {
        // Arrange
        var services = new ServiceCollection();
        var builder = services.AddDdap(options => { });

        // Act
        var result = builder.AddSqlServerDapper();

        // Assert
        result.Should().BeAssignableTo<IDdapBuilder>();
        result.Services.Should().BeSameAs(services);
    }

    [Fact]
    public void AddSqlServerDapper_Should_Support_Multiple_Calls()
    {
        // Arrange
        var services = new ServiceCollection();
        var builder = services.AddDdap(options => { });

        // Act
        var result1 = builder.AddSqlServerDapper();
        var result2 = result1.AddSqlServerDapper();

        // Assert
        result2.Should().NotBeNull();
    }

    [Fact]
    public void AddSqlServerDapper_Should_Work_With_Empty_Connection_String()
    {
        // Arrange
        var services = new ServiceCollection();
        var builder = services.AddDdap(options =>
        {
            options.ConnectionString = "";
        });

        // Act
        builder.AddSqlServerDapper();

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var dataProvider = serviceProvider.GetService<IDataProvider>();
        dataProvider.Should().NotBeNull();
    }

    [Fact]
    public void AddSqlServerDapper_Should_Register_Both_Services()
    {
        // Arrange
        var services = new ServiceCollection();
        var builder = services.AddDdap(options => { });

        // Act
        builder.AddSqlServerDapper();

        // Assert
        services.Should().Contain(sd => sd.ServiceType == typeof(IDataProvider));
        services.Should().Contain(sd => sd.ServiceType == typeof(IHostedService));
    }
}

public class MySqlDataProviderExtensionsTests
{
    [Fact]
    public void AddMySqlDapper_Should_Register_DataProvider()
    {
        // Arrange
        var services = new ServiceCollection();
        var builder = services.AddDdap(options =>
        {
            options.ConnectionString = "Server=localhost;Database=Test;User=root;";
            options.Provider = DatabaseProvider.MySQL;
        });

        // Act
        builder.AddMySqlDapper();

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var dataProvider = serviceProvider.GetService<IDataProvider>();
        dataProvider.Should().NotBeNull();
        dataProvider.Should().BeOfType<MySqlDataProvider>();
    }

    [Fact]
    public void AddMySqlDapper_Should_Register_As_Singleton()
    {
        // Arrange
        var services = new ServiceCollection();
        var builder = services.AddDdap(options =>
        {
            options.ConnectionString = "test";
        });

        // Act
        builder.AddMySqlDapper();

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var provider1 = serviceProvider.GetService<IDataProvider>();
        var provider2 = serviceProvider.GetService<IDataProvider>();
        provider1.Should().BeSameAs(provider2);
    }

    [Fact]
    public void AddMySqlDapper_Should_Register_Hosted_Service()
    {
        // Arrange
        var services = new ServiceCollection();
        var builder = services.AddDdap(options =>
        {
            options.ConnectionString = "test";
        });

        // Act
        builder.AddMySqlDapper();

        // Assert
        var hostedService = services.FirstOrDefault(sd =>
            sd.ServiceType == typeof(IHostedService) &&
            sd.ImplementationType != null &&
            sd.ImplementationType.Name == "EntityLoaderHostedService");
        hostedService.Should().NotBeNull();
    }

    [Fact]
    public void AddMySqlDapper_Should_Return_Builder_For_Chaining()
    {
        // Arrange
        var services = new ServiceCollection();
        var builder = services.AddDdap(options => { });

        // Act
        var result = builder.AddMySqlDapper();

        // Assert
        result.Should().BeAssignableTo<IDdapBuilder>();
        result.Services.Should().BeSameAs(services);
    }

    [Fact]
    public void AddMySqlDapper_Should_Support_Multiple_Calls()
    {
        // Arrange
        var services = new ServiceCollection();
        var builder = services.AddDdap(options => { });

        // Act
        var result1 = builder.AddMySqlDapper();
        var result2 = result1.AddMySqlDapper();

        // Assert
        result2.Should().NotBeNull();
    }

    [Fact]
    public void AddMySqlDapper_Should_Work_With_Empty_Connection_String()
    {
        // Arrange
        var services = new ServiceCollection();
        var builder = services.AddDdap(options =>
        {
            options.ConnectionString = "";
        });

        // Act
        builder.AddMySqlDapper();

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var dataProvider = serviceProvider.GetService<IDataProvider>();
        dataProvider.Should().NotBeNull();
    }

    [Fact]
    public void AddMySqlDapper_Should_Register_Both_Services()
    {
        // Arrange
        var services = new ServiceCollection();
        var builder = services.AddDdap(options => { });

        // Act
        builder.AddMySqlDapper();

        // Assert
        services.Should().Contain(sd => sd.ServiceType == typeof(IDataProvider));
        services.Should().Contain(sd => sd.ServiceType == typeof(IHostedService));
    }
}

public class PostgreSqlDataProviderExtensionsTests
{
    [Fact]
    public void AddPostgreSqlDapper_Should_Register_DataProvider()
    {
        // Arrange
        var services = new ServiceCollection();
        var builder = services.AddDdap(options =>
        {
            options.ConnectionString = "Host=localhost;Database=Test;Username=postgres;";
            options.Provider = DatabaseProvider.PostgreSQL;
        });

        // Act
        builder.AddPostgreSqlDapper();

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var dataProvider = serviceProvider.GetService<IDataProvider>();
        dataProvider.Should().NotBeNull();
        dataProvider.Should().BeOfType<PostgreSqlDataProvider>();
    }

    [Fact]
    public void AddPostgreSqlDapper_Should_Register_As_Singleton()
    {
        // Arrange
        var services = new ServiceCollection();
        var builder = services.AddDdap(options =>
        {
            options.ConnectionString = "test";
        });

        // Act
        builder.AddPostgreSqlDapper();

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var provider1 = serviceProvider.GetService<IDataProvider>();
        var provider2 = serviceProvider.GetService<IDataProvider>();
        provider1.Should().BeSameAs(provider2);
    }

    [Fact]
    public void AddPostgreSqlDapper_Should_Register_Hosted_Service()
    {
        // Arrange
        var services = new ServiceCollection();
        var builder = services.AddDdap(options =>
        {
            options.ConnectionString = "test";
        });

        // Act
        builder.AddPostgreSqlDapper();

        // Assert
        var hostedService = services.FirstOrDefault(sd =>
            sd.ServiceType == typeof(IHostedService) &&
            sd.ImplementationType != null &&
            sd.ImplementationType.Name == "EntityLoaderHostedService");
        hostedService.Should().NotBeNull();
    }

    [Fact]
    public void AddPostgreSqlDapper_Should_Return_Builder_For_Chaining()
    {
        // Arrange
        var services = new ServiceCollection();
        var builder = services.AddDdap(options => { });

        // Act
        var result = builder.AddPostgreSqlDapper();

        // Assert
        result.Should().BeAssignableTo<IDdapBuilder>();
        result.Services.Should().BeSameAs(services);
    }

    [Fact]
    public void AddPostgreSqlDapper_Should_Support_Multiple_Calls()
    {
        // Arrange
        var services = new ServiceCollection();
        var builder = services.AddDdap(options => { });

        // Act
        var result1 = builder.AddPostgreSqlDapper();
        var result2 = result1.AddPostgreSqlDapper();

        // Assert
        result2.Should().NotBeNull();
    }

    [Fact]
    public void AddPostgreSqlDapper_Should_Work_With_Empty_Connection_String()
    {
        // Arrange
        var services = new ServiceCollection();
        var builder = services.AddDdap(options =>
        {
            options.ConnectionString = "";
        });

        // Act
        builder.AddPostgreSqlDapper();

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var dataProvider = serviceProvider.GetService<IDataProvider>();
        dataProvider.Should().NotBeNull();
    }

    [Fact]
    public void AddPostgreSqlDapper_Should_Register_Both_Services()
    {
        // Arrange
        var services = new ServiceCollection();
        var builder = services.AddDdap(options => { });

        // Act
        builder.AddPostgreSqlDapper();

        // Assert
        services.Should().Contain(sd => sd.ServiceType == typeof(IDataProvider));
        services.Should().Contain(sd => sd.ServiceType == typeof(IHostedService));
    }
}

public class DataProviderComparisonTests
{
    [Fact]
    public void All_DataProviders_Should_Register_IDataProvider()
    {
        // Arrange
        var sqlServerServices = new ServiceCollection();
        var mySqlServices = new ServiceCollection();
        var postgreSqlServices = new ServiceCollection();

        var sqlServerBuilder = sqlServerServices.AddDdap(options => { });
        var mySqlBuilder = mySqlServices.AddDdap(options => { });
        var postgreSqlBuilder = postgreSqlServices.AddDdap(options => { });

        // Act
        sqlServerBuilder.AddSqlServerDapper();
        mySqlBuilder.AddMySqlDapper();
        postgreSqlBuilder.AddPostgreSqlDapper();

        // Assert
        var sqlServerProvider = sqlServerServices.BuildServiceProvider().GetService<IDataProvider>();
        var mySqlProvider = mySqlServices.BuildServiceProvider().GetService<IDataProvider>();
        var postgreSqlProvider = postgreSqlServices.BuildServiceProvider().GetService<IDataProvider>();

        sqlServerProvider.Should().NotBeNull();
        mySqlProvider.Should().NotBeNull();
        postgreSqlProvider.Should().NotBeNull();
    }

    [Fact]
    public void All_DataProviders_Should_Register_HostedService()
    {
        // Arrange
        var sqlServerServices = new ServiceCollection();
        var mySqlServices = new ServiceCollection();
        var postgreSqlServices = new ServiceCollection();

        var sqlServerBuilder = sqlServerServices.AddDdap(options => { });
        var mySqlBuilder = mySqlServices.AddDdap(options => { });
        var postgreSqlBuilder = postgreSqlServices.AddDdap(options => { });

        // Act
        sqlServerBuilder.AddSqlServerDapper();
        mySqlBuilder.AddMySqlDapper();
        postgreSqlBuilder.AddPostgreSqlDapper();

        // Assert
        sqlServerServices.Should().Contain(sd => sd.ServiceType == typeof(IHostedService));
        mySqlServices.Should().Contain(sd => sd.ServiceType == typeof(IHostedService));
        postgreSqlServices.Should().Contain(sd => sd.ServiceType == typeof(IHostedService));
    }

    [Fact]
    public void DataProviders_Should_Be_Different_Types()
    {
        // Arrange
        var sqlServerServices = new ServiceCollection();
        var mySqlServices = new ServiceCollection();
        var postgreSqlServices = new ServiceCollection();

        var sqlServerBuilder = sqlServerServices.AddDdap(options => { });
        var mySqlBuilder = mySqlServices.AddDdap(options => { });
        var postgreSqlBuilder = postgreSqlServices.AddDdap(options => { });

        // Act
        sqlServerBuilder.AddSqlServerDapper();
        mySqlBuilder.AddMySqlDapper();
        postgreSqlBuilder.AddPostgreSqlDapper();

        // Assert
        var sqlServerProvider = sqlServerServices.BuildServiceProvider().GetService<IDataProvider>();
        var mySqlProvider = mySqlServices.BuildServiceProvider().GetService<IDataProvider>();
        var postgreSqlProvider = postgreSqlServices.BuildServiceProvider().GetService<IDataProvider>();

        sqlServerProvider.Should().BeOfType<SqlServerDataProvider>();
        mySqlProvider.Should().BeOfType<MySqlDataProvider>();
        postgreSqlProvider.Should().BeOfType<PostgreSqlDataProvider>();
    }

    [Fact]
    public void Can_Chain_DataProvider_With_Other_Extensions()
    {
        // Arrange
        var services = new ServiceCollection();
        var builder = services.AddDdap(options =>
        {
            options.ConnectionString = "test";
        });

        // Act
        var result = builder.AddSqlServerDapper();

        // Assert
        result.Should().BeAssignableTo<IDdapBuilder>();
    }

    [Theory]
    [InlineData(DatabaseProvider.SQLServer)]
    [InlineData(DatabaseProvider.MySQL)]
    [InlineData(DatabaseProvider.PostgreSQL)]
    public void DataProvider_Should_Work_With_All_Database_Providers(DatabaseProvider provider)
    {
        // Arrange
        var services = new ServiceCollection();
        var builder = services.AddDdap(options =>
        {
            options.Provider = provider;
            options.ConnectionString = "test";
        });

        // Act
        switch (provider)
        {
            case DatabaseProvider.SQLServer:
                builder.AddSqlServerDapper();
                break;
            case DatabaseProvider.MySQL:
                builder.AddMySqlDapper();
                break;
            case DatabaseProvider.PostgreSQL:
                builder.AddPostgreSqlDapper();
                break;
        }

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var dataProvider = serviceProvider.GetService<IDataProvider>();
        dataProvider.Should().NotBeNull();
    }
}
