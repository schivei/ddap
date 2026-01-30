using Ddap.Client.Core;
using FluentAssertions;
using Polly;

namespace Ddap.Client.Core.Tests;

public class ResiliencePolicyProviderTests
{
    [Fact]
    public void GetRetryPolicy_WithExponentialBackoff_ShouldCreatePolicy()
    {
        // Arrange
        var options = new DdapClientOptions { RetryCount = 3, UseExponentialBackoff = true };

        // Act
        var policy = ResiliencePolicyProvider.GetRetryPolicy(options);

        // Assert
        policy.Should().NotBeNull();
    }

    [Fact]
    public void GetRetryPolicy_WithoutExponentialBackoff_ShouldCreatePolicy()
    {
        // Arrange
        var options = new DdapClientOptions
        {
            RetryCount = 3,
            RetryDelay = TimeSpan.FromSeconds(2),
            UseExponentialBackoff = false,
        };

        // Act
        var policy = ResiliencePolicyProvider.GetRetryPolicy(options);

        // Assert
        policy.Should().NotBeNull();
    }

    [Fact]
    public void GetCircuitBreakerPolicy_ShouldCreatePolicy()
    {
        // Act
        var policy = ResiliencePolicyProvider.GetCircuitBreakerPolicy();

        // Assert
        policy.Should().NotBeNull();
    }

    [Fact]
    public void GetTimeoutPolicy_ShouldCreatePolicy()
    {
        // Arrange
        var timeout = TimeSpan.FromSeconds(30);

        // Act
        var policy = ResiliencePolicyProvider.GetTimeoutPolicy(timeout);

        // Assert
        policy.Should().NotBeNull();
    }
}
