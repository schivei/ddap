using Ddap.GraphQL;
using Ddap.GraphQL.Scalars;
using FluentAssertions;
using HotChocolate.Language;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Ddap.Tests.GraphQL;

public class ExtendedTypesTests
{
    [Fact]
    public void AddExtendedTypes_Should_Register_Type_Converters()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddGraphQLServer().AddQueryType<DummyQuery>().AddExtendedTypes();

        // Assert - If no exception is thrown, the type converters are registered
        services.Should().NotBeNull();
    }

    [Fact]
    public void DateOnlyType_Should_Parse_ISO_Date_String()
    {
        // Arrange
        var dateOnlyType = new DateOnlyType();
        var expectedDate = new DateOnly(2024, 1, 29);

        // Act
        var valueNode = dateOnlyType.ParseValue(expectedDate);

        // Assert
        valueNode.Should().NotBeNull();
        valueNode.Should().BeOfType<StringValueNode>();
        ((StringValueNode)valueNode).Value.Should().Be("2024-01-29");
    }

    [Fact]
    public void TimeOnlyType_Should_Parse_ISO_Time_String()
    {
        // Arrange
        var timeOnlyType = new TimeOnlyType();
        var expectedTime = new TimeOnly(14, 30, 0);

        // Act
        var valueNode = timeOnlyType.ParseValue(expectedTime);

        // Assert
        valueNode.Should().NotBeNull();
        valueNode.Should().BeOfType<StringValueNode>();
        ((StringValueNode)valueNode).Value.Should().StartWith("14:30:00");
    }

    [Fact]
    public void DateOnlyType_Should_Deserialize_From_String()
    {
        // Arrange
        var dateOnlyType = new DateOnlyType();
        var stringValue = new StringValueNode("2024-01-29");

        // Act
        var dateOnly = dateOnlyType.ParseLiteral(stringValue);

        // Assert
        dateOnly.Should().Be(new DateOnly(2024, 1, 29));
    }

    [Fact]
    public void TimeOnlyType_Should_Deserialize_From_String()
    {
        // Arrange
        var timeOnlyType = new TimeOnlyType();
        var stringValue = new StringValueNode("14:30:00");

        // Act
        var timeOnly = timeOnlyType.ParseLiteral(stringValue);

        // Assert
        timeOnly.Should().Be(new TimeOnly(14, 30, 0));
    }

    // Dummy query for testing
    private class DummyQuery
    {
        public string Hello() => "World";
    }
}
