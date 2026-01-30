using Ddap.Client.Core;
using Ddap.Client.Rest;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace Ddap.Client.Rest.Tests;

public class DdapRestClientServiceCollectionExtensionsTests
{
    [Fact]
    public void AddDdapRestClient_ShouldRegisterServices()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddDdapRestClient(options =>
        {
            options.BaseUrl = "https://api.example.com";
            options.Timeout = TimeSpan.FromSeconds(60);
        });

        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var options = serviceProvider.GetService<DdapClientOptions>();
        options.Should().NotBeNull();
        options!.BaseUrl.Should().Be("https://api.example.com");

        var httpClientFactory = serviceProvider.GetService<IHttpClientFactory>();
        httpClientFactory.Should().NotBeNull();
    }
}
