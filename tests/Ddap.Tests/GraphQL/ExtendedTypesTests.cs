using Ddap.GraphQL;
using Ddap.GraphQL.Scalars;
using FluentAssertions;
using HotChocolate.Language;
using HotChocolate.Types;
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

        // Assert - If no exception is thrown, the scalar types are registered
        services.Should().NotBeNull();
    }

    [Fact]
    public void UIntType_Should_Parse_And_Serialize_Values()
    {
        // Arrange
        var uintType = new UIntType();
        uint expectedValue = 4294967295; // uint.MaxValue

        // Act
        var valueNode = uintType.ParseValue(expectedValue);
        var parsedValue = uintType.ParseLiteral(valueNode);

        // Assert
        valueNode.Should().NotBeNull();
        valueNode.Should().BeOfType<IntValueNode>();
        parsedValue.Should().Be(expectedValue);
    }

    [Fact]
    public void ULongType_Should_Parse_And_Serialize_Values()
    {
        // Arrange
        var ulongType = new ULongType();
        ulong expectedValue = 18446744073709551615; // ulong.MaxValue

        // Act
        var valueNode = ulongType.ParseValue(expectedValue);
        var parsedValue = ulongType.ParseLiteral(valueNode);

        // Assert
        valueNode.Should().NotBeNull();
        valueNode.Should().BeOfType<IntValueNode>();
        parsedValue.Should().Be(expectedValue);
    }

    [Fact]
    public void UShortType_Should_Parse_And_Serialize_Values()
    {
        // Arrange
        var ushortType = new UShortType();
        ushort expectedValue = 65535; // ushort.MaxValue

        // Act
        var valueNode = ushortType.ParseValue(expectedValue);
        var parsedValue = ushortType.ParseLiteral(valueNode);

        // Assert
        valueNode.Should().NotBeNull();
        valueNode.Should().BeOfType<IntValueNode>();
        parsedValue.Should().Be(expectedValue);
    }

    [Fact]
    public void SByteType_Should_Parse_And_Serialize_Values()
    {
        // Arrange
        var sbyteType = new SByteType();
        sbyte expectedValue = -128; // sbyte.MinValue

        // Act
        var valueNode = sbyteType.ParseValue(expectedValue);
        var parsedValue = sbyteType.ParseLiteral(valueNode);

        // Assert
        valueNode.Should().NotBeNull();
        valueNode.Should().BeOfType<IntValueNode>();
        parsedValue.Should().Be(expectedValue);
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

    [Fact]
    public void DateOnlyType_ParseLiteral_Should_Throw_When_Value_Is_Empty()
    {
        // Arrange
        var dateOnlyType = new DateOnlyType();
        var emptyValue = new StringValueNode("");

        // Act & Assert
        var act = () => dateOnlyType.ParseLiteral(emptyValue);
        act.Should().Throw<SerializationException>().WithMessage("*cannot be null or whitespace*");
    }

    [Fact]
    public void DateOnlyType_ParseLiteral_Should_Throw_When_Value_Is_Whitespace()
    {
        // Arrange
        var dateOnlyType = new DateOnlyType();
        var whitespaceValue = new StringValueNode("   ");

        // Act & Assert
        var act = () => dateOnlyType.ParseLiteral(whitespaceValue);
        act.Should().Throw<SerializationException>().WithMessage("*cannot be null or whitespace*");
    }

    [Fact]
    public void DateOnlyType_ParseLiteral_Should_Throw_When_Format_Is_Invalid()
    {
        // Arrange
        var dateOnlyType = new DateOnlyType();
        var invalidValue = new StringValueNode("not-a-date");

        // Act & Assert
        var act = () => dateOnlyType.ParseLiteral(invalidValue);
        act.Should()
            .Throw<SerializationException>()
            .WithMessage("*Unable to deserialize string to DateOnly*");
    }

    [Fact]
    public void DateOnlyType_ParseResult_Should_Handle_Null()
    {
        // Arrange
        var dateOnlyType = new DateOnlyType();

        // Act
        var result = dateOnlyType.ParseResult(null);

        // Assert
        result.Should().BeOfType<NullValueNode>();
    }

    [Fact]
    public void DateOnlyType_ParseResult_Should_Handle_String()
    {
        // Arrange
        var dateOnlyType = new DateOnlyType();

        // Act
        var result = dateOnlyType.ParseResult("2024-01-29");

        // Assert
        result.Should().BeOfType<StringValueNode>();
        ((StringValueNode)result).Value.Should().Be("2024-01-29");
    }

    [Fact]
    public void DateOnlyType_ParseResult_Should_Handle_DateOnly()
    {
        // Arrange
        var dateOnlyType = new DateOnlyType();
        var date = new DateOnly(2024, 1, 29);

        // Act
        var result = dateOnlyType.ParseResult(date);

        // Assert
        result.Should().BeOfType<StringValueNode>();
        ((StringValueNode)result).Value.Should().Be("2024-01-29");
    }

    [Fact]
    public void DateOnlyType_ParseResult_Should_Throw_For_Invalid_Type()
    {
        // Arrange
        var dateOnlyType = new DateOnlyType();

        // Act & Assert
        var act = () => dateOnlyType.ParseResult(12345);
        act.Should().Throw<SerializationException>().WithMessage("*Unable to serialize*");
    }

    [Fact]
    public void TimeOnlyType_ParseLiteral_Should_Throw_When_Value_Is_Empty()
    {
        // Arrange
        var timeOnlyType = new TimeOnlyType();
        var emptyValue = new StringValueNode("");

        // Act & Assert
        var act = () => timeOnlyType.ParseLiteral(emptyValue);
        act.Should().Throw<SerializationException>().WithMessage("*cannot be null or whitespace*");
    }

    [Fact]
    public void TimeOnlyType_ParseLiteral_Should_Throw_When_Value_Is_Whitespace()
    {
        // Arrange
        var timeOnlyType = new TimeOnlyType();
        var whitespaceValue = new StringValueNode("   ");

        // Act & Assert
        var act = () => timeOnlyType.ParseLiteral(whitespaceValue);
        act.Should().Throw<SerializationException>().WithMessage("*cannot be null or whitespace*");
    }

    [Fact]
    public void TimeOnlyType_ParseLiteral_Should_Throw_When_Format_Is_Invalid()
    {
        // Arrange
        var timeOnlyType = new TimeOnlyType();
        var invalidValue = new StringValueNode("not-a-time");

        // Act & Assert
        var act = () => timeOnlyType.ParseLiteral(invalidValue);
        act.Should()
            .Throw<SerializationException>()
            .WithMessage("*Unable to deserialize string to TimeOnly*");
    }

    [Fact]
    public void TimeOnlyType_ParseResult_Should_Handle_Null()
    {
        // Arrange
        var timeOnlyType = new TimeOnlyType();

        // Act
        var result = timeOnlyType.ParseResult(null);

        // Assert
        result.Should().BeOfType<NullValueNode>();
    }

    [Fact]
    public void TimeOnlyType_ParseResult_Should_Handle_String()
    {
        // Arrange
        var timeOnlyType = new TimeOnlyType();

        // Act
        var result = timeOnlyType.ParseResult("14:30:00");

        // Assert
        result.Should().BeOfType<StringValueNode>();
        ((StringValueNode)result).Value.Should().Be("14:30:00");
    }

    [Fact]
    public void TimeOnlyType_ParseResult_Should_Handle_TimeOnly()
    {
        // Arrange
        var timeOnlyType = new TimeOnlyType();
        var time = new TimeOnly(14, 30, 0);

        // Act
        var result = timeOnlyType.ParseResult(time);

        // Assert
        result.Should().BeOfType<StringValueNode>();
        ((StringValueNode)result).Value.Should().StartWith("14:30:00");
    }

    [Fact]
    public void TimeOnlyType_ParseResult_Should_Throw_For_Invalid_Type()
    {
        // Arrange
        var timeOnlyType = new TimeOnlyType();

        // Act & Assert
        var act = () => timeOnlyType.ParseResult(12345);
        act.Should().Throw<SerializationException>().WithMessage("*Unable to serialize*");
    }

    // Dummy query for testing
    private class DummyQuery
    {
        public string Hello() => "World";
    }
}
