using Ddap.Client.Core;
using FluentAssertions;

namespace Ddap.Client.Core.Tests;

public class ExceptionsTests
{
    [Fact]
    public void DdapClientException_ShouldBeInstantiable()
    {
        // Act
        var exception = new DdapClientException("Test message");

        // Assert
        exception.Should().NotBeNull();
        exception.Message.Should().Be("Test message");
    }

    [Fact]
    public void DdapConnectionException_ShouldInheritFromDdapClientException()
    {
        // Act
        var exception = new DdapConnectionException("Connection failed");

        // Assert
        exception.Should().BeAssignableTo<DdapClientException>();
        exception.Message.Should().Be("Connection failed");
    }

    [Fact]
    public void DdapApiException_ShouldHaveStatusCode()
    {
        // Act
        var exception = new DdapApiException("API error", 404);

        // Assert
        exception.Should().BeAssignableTo<DdapClientException>();
        exception.Message.Should().Be("API error");
        exception.StatusCode.Should().Be(404);
    }
}
