using Ddap.Client.Core;
using FluentAssertions;

namespace Ddap.Client.Core.Tests;

public class DdapClientOptionsTests
{
    [Fact]
    public void DefaultOptions_ShouldHaveCorrectDefaults()
    {
        // Arrange & Act
        var options = new DdapClientOptions();

        // Assert
        options.BaseUrl.Should().BeEmpty();
        options.Timeout.Should().Be(TimeSpan.FromSeconds(30));
        options.RetryCount.Should().Be(3);
        options.RetryDelay.Should().Be(TimeSpan.FromSeconds(1));
        options.UseExponentialBackoff.Should().BeTrue();
    }

    [Fact]
    public void Options_ShouldBeConfigurable()
    {
        // Arrange
        var options = new DdapClientOptions
        {
            BaseUrl = "https://api.example.com",
            Timeout = TimeSpan.FromSeconds(60),
            RetryCount = 5,
            RetryDelay = TimeSpan.FromSeconds(2),
            UseExponentialBackoff = false,
        };

        // Assert
        options.BaseUrl.Should().Be("https://api.example.com");
        options.Timeout.Should().Be(TimeSpan.FromSeconds(60));
        options.RetryCount.Should().Be(5);
        options.RetryDelay.Should().Be(TimeSpan.FromSeconds(2));
        options.UseExponentialBackoff.Should().BeFalse();
    }
}
