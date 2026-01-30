using Ddap.Client.Core;
using Ddap.Client.GraphQL;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace Ddap.Client.GraphQL.Tests;

public class DdapGraphQLClientServiceCollectionExtensionsTests
{
    [Fact]
    public void AddDdapGraphQLClient_ShouldRegisterServices()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddDdapGraphQLClient(options =>
        {
            options.BaseUrl = "https://api.example.com";
            options.Timeout = TimeSpan.FromSeconds(60);
        });

        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var options = serviceProvider.GetService<DdapClientOptions>();
        options.Should().NotBeNull();
        options!.BaseUrl.Should().Be("https://api.example.com");
    }
}
