using Ddap.Client.Core;
using Ddap.Client.Rest;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace Ddap.Client.Rest.Tests;

public class DdapRestClientServiceCollectionExtensionsAdvancedTests
{
    [Fact]
    public void AddDdapRestClient_ShouldConfigureHttpClientProperly()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddDdapRestClient(options =>
        {
            options.BaseUrl = "https://api.example.com";
            options.Timeout = TimeSpan.FromSeconds(60);
            options.RetryCount = 5;
        });

        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var httpClientFactory = serviceProvider.GetService<IHttpClientFactory>();
        httpClientFactory.Should().NotBeNull();

        var restClient = serviceProvider.GetService<DdapRestClient>();
        restClient.Should().NotBeNull();
        restClient!.BaseUrl.Should().Be("https://api.example.com");
    }

    [Fact]
    public void AddDdapRestClient_WithDifferentOptions_ShouldApplyCorrectly()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddDdapRestClient(options =>
        {
            options.BaseUrl = "https://different-api.com";
            options.Timeout = TimeSpan.FromMinutes(2);
            options.RetryCount = 2;
            options.UseExponentialBackoff = false;
        });

        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var options = serviceProvider.GetService<DdapClientOptions>();
        options.Should().NotBeNull();
        options!.BaseUrl.Should().Be("https://different-api.com");
        options.Timeout.Should().Be(TimeSpan.FromMinutes(2));
        options.RetryCount.Should().Be(2);
        options.UseExponentialBackoff.Should().BeFalse();
    }
}
