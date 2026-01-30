using Ddap.Client.Core;
using Ddap.Client.Grpc;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace Ddap.Client.Grpc.Tests;

public class DdapGrpcClientServiceCollectionExtensionsTests
{
    [Fact]
    public void AddDdapGrpcClient_ShouldRegisterServices()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddDdapGrpcClient(options =>
        {
            options.BaseUrl = "https://api.example.com";
            options.Timeout = TimeSpan.FromSeconds(60);
        });

        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var options = serviceProvider.GetService<DdapClientOptions>();
        options.Should().NotBeNull();
        options!.BaseUrl.Should().Be("https://api.example.com");

        var client = serviceProvider.GetService<DdapGrpcClient>();
        client.Should().NotBeNull();
    }
}
