using Ddap.Client.Core;
using Ddap.Client.GraphQL;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace Ddap.Client.GraphQL.Tests;

public class DdapGraphQLClientServiceCollectionExtensionsAdvancedTests
{
    [Fact]
    public void AddDdapGraphQLClient_ShouldConfigureHttpClientProperly()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddDdapGraphQLClient(options =>
        {
            options.BaseUrl = "https://graphql.example.com";
            options.Timeout = TimeSpan.FromSeconds(45);
            options.RetryCount = 4;
        });

        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var httpClientFactory = serviceProvider.GetService<IHttpClientFactory>();
        httpClientFactory.Should().NotBeNull();

        var graphQLClient = serviceProvider.GetService<DdapGraphQLClient>();
        graphQLClient.Should().NotBeNull();
        graphQLClient!.BaseUrl.Should().Be("https://graphql.example.com");
    }

    [Fact]
    public void AddDdapGraphQLClient_WithDifferentOptions_ShouldApplyCorrectly()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddDdapGraphQLClient(options =>
        {
            options.BaseUrl = "https://different-graphql.com";
            options.Timeout = TimeSpan.FromMinutes(3);
            options.RetryCount = 1;
            options.UseExponentialBackoff = false;
        });

        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var options = serviceProvider.GetService<DdapClientOptions>();
        options.Should().NotBeNull();
        options!.BaseUrl.Should().Be("https://different-graphql.com");
        options.Timeout.Should().Be(TimeSpan.FromMinutes(3));
        options.RetryCount.Should().Be(1);
        options.UseExponentialBackoff.Should().BeFalse();
    }
}
