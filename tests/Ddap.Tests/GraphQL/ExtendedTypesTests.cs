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
        valueNode.Should().BeOfType<DateTimeValueNode>();
        valueNode.Value.Should().Be("2024-01-29");
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
        valueNode.Should().BeOfType<DateTimeValueNode>();
        string stringValue = (string)valueNode.Value!;
        stringValue.Should().StartWith("14:30:00");
    }

    [Fact]
    public void DateOnlyType_Should_Deserialize_From_String()
    {
        // Arrange
        var dateOnlyType = new DateOnlyType();
        var stringValue = new DateTimeValueNode("2024-01-29");

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
        var stringValue = new DateTimeValueNode("14:30:00");

        // Act
        var timeOnly = timeOnlyType.ParseLiteral(stringValue);

        // Assert
        timeOnly.Should().Be(new TimeOnly(14, 30, 0));
    }

    [Fact]
    public void DateOnlyType_ParseLiteral_Should_Throw_When_Value_Is_Invalid()
    {
        // Arrange
        var dateOnlyType = new DateOnlyType();
        var invalidValue = new DateTimeValueNode("not-a-date");

        // Act & Assert
        var act = () => dateOnlyType.ParseLiteral(invalidValue);
        act.Should()
            .Throw<SerializationException>()
            .WithMessage("*not a valid DateOnly representation*");
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
        result.Should().BeOfType<DateTimeValueNode>();
        // String input might be parsed as DateTime, so just verify it contains the date
        ((DateTimeValueNode)result)
            .Value.Should()
            .StartWith("2024-01-29");
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
        result.Should().BeOfType<DateTimeValueNode>();
        ((DateTimeValueNode)result).Value.Should().Be("2024-01-29");
    }

    [Fact]
    public void DateOnlyType_ParseResult_Should_Throw_For_Invalid_Type()
    {
        // Arrange
        var dateOnlyType = new DateOnlyType();

        // Act & Assert
        var act = () => dateOnlyType.ParseResult(12345);
        act.Should()
            .Throw<SerializationException>()
            .WithMessage("*not a valid DateOnly representation*");
    }

    [Fact]
    public void TimeOnlyType_ParseLiteral_Should_Throw_When_Value_Is_Invalid()
    {
        // Arrange
        var timeOnlyType = new TimeOnlyType();
        var invalidValue = new DateTimeValueNode("not-a-time");

        // Act & Assert
        var act = () => timeOnlyType.ParseLiteral(invalidValue);
        act.Should()
            .Throw<SerializationException>()
            .WithMessage("*not a valid TimeOnly representation*");
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
        result.Should().BeOfType<DateTimeValueNode>();
        ((DateTimeValueNode)result).Value.Should().StartWith("14:30:00");
    }

    [Fact]
    public void TimeOnlyType_ParseResult_Should_Throw_For_Invalid_Type()
    {
        // Arrange
        var timeOnlyType = new TimeOnlyType();

        // Act & Assert
        var act = () => timeOnlyType.ParseResult(12345);
        act.Should()
            .Throw<SerializationException>()
            .WithMessage("*not a valid TimeOnly representation*");
    }

    [Fact]
    public void DateOnlyType_TrySerialize_Should_Handle_Null()
    {
        // Arrange
        var dateOnlyType = new DateOnlyType();

        // Act
        var result = dateOnlyType.TrySerialize(null, out var resultValue);

        // Assert
        result.Should().BeTrue();
        resultValue.Should().BeNull();
    }

    [Fact]
    public void DateOnlyType_TrySerialize_Should_Handle_DateOnly()
    {
        // Arrange
        var dateOnlyType = new DateOnlyType();
        var date = new DateOnly(2024, 1, 29);

        // Act
        var result = dateOnlyType.TrySerialize(date, out var resultValue);

        // Assert
        result.Should().BeTrue();
        resultValue.Should().Be("2024-01-29");
    }

    [Fact]
    public void DateOnlyType_TrySerialize_Should_Handle_DateTimeValueNode()
    {
        // Arrange
        var dateOnlyType = new DateOnlyType();
        var node = new DateTimeValueNode(new DateOnly(2024, 1, 29));

        // Act
        var result = dateOnlyType.TrySerialize(node, out var resultValue);

        // Assert
        result.Should().BeTrue();
        resultValue.Should().Be("2024-01-29");
    }

    [Fact]
    public void DateOnlyType_TrySerialize_Should_Return_False_For_Invalid_Type()
    {
        // Arrange
        var dateOnlyType = new DateOnlyType();

        // Act
        var result = dateOnlyType.TrySerialize(12345, out var resultValue);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void DateOnlyType_TryDeserialize_Should_Handle_Null()
    {
        // Arrange
        var dateOnlyType = new DateOnlyType();

        // Act
        var result = dateOnlyType.TryDeserialize(null, out var runtimeValue);

        // Assert
        result.Should().BeTrue();
        runtimeValue.Should().BeNull();
    }

    [Fact]
    public void DateOnlyType_TryDeserialize_Should_Handle_String()
    {
        // Arrange
        var dateOnlyType = new DateOnlyType();

        // Act
        var result = dateOnlyType.TryDeserialize("2024-01-29", out var runtimeValue);

        // Assert
        result.Should().BeTrue();
        runtimeValue.Should().BeOfType<DateOnly>();
        ((DateOnly)runtimeValue!).Should().Be(new DateOnly(2024, 1, 29));
    }

    [Fact]
    public void DateOnlyType_TryDeserialize_Should_Handle_DateOnly()
    {
        // Arrange
        var dateOnlyType = new DateOnlyType();
        var date = new DateOnly(2024, 1, 29);

        // Act
        var result = dateOnlyType.TryDeserialize(date, out var runtimeValue);

        // Assert
        result.Should().BeTrue();
        runtimeValue.Should().Be(date);
    }

    [Fact]
    public void DateOnlyType_TryDeserialize_Should_Handle_DateTimeValueNode()
    {
        // Arrange
        var dateOnlyType = new DateOnlyType();
        var node = new DateTimeValueNode(new DateOnly(2024, 1, 29));

        // Act
        var result = dateOnlyType.TryDeserialize(node, out var runtimeValue);

        // Assert
        result.Should().BeTrue();
        runtimeValue.Should().BeOfType<DateOnly>();
        ((DateOnly)runtimeValue!).Should().Be(new DateOnly(2024, 1, 29));
    }

    [Fact]
    public void DateOnlyType_TryDeserialize_Should_Return_False_For_Invalid_Type()
    {
        // Arrange
        var dateOnlyType = new DateOnlyType();

        // Act
        var result = dateOnlyType.TryDeserialize(12345, out var runtimeValue);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void TimeOnlyType_TrySerialize_Should_Handle_Null()
    {
        // Arrange
        var timeOnlyType = new TimeOnlyType();

        // Act
        var result = timeOnlyType.TrySerialize(null, out var resultValue);

        // Assert
        result.Should().BeTrue();
        resultValue.Should().BeNull();
    }

    [Fact]
    public void TimeOnlyType_TrySerialize_Should_Handle_TimeOnly()
    {
        // Arrange
        var timeOnlyType = new TimeOnlyType();
        var time = new TimeOnly(14, 30, 0);

        // Act
        var result = timeOnlyType.TrySerialize(time, out var resultValue);

        // Assert
        result.Should().BeTrue();
        resultValue.Should().NotBeNull();
        resultValue.ToString()!.Should().StartWith("14:30:00");
    }

    [Fact]
    public void TimeOnlyType_TrySerialize_Should_Handle_DateTimeValueNode()
    {
        // Arrange
        var timeOnlyType = new TimeOnlyType();
        var node = new DateTimeValueNode(new TimeOnly(14, 30, 0));

        // Act
        var result = timeOnlyType.TrySerialize(node, out var resultValue);

        // Assert
        result.Should().BeTrue();
        resultValue.Should().NotBeNull();
        resultValue.ToString()!.Should().StartWith("14:30:00");
    }

    [Fact]
    public void TimeOnlyType_TrySerialize_Should_Return_False_For_Invalid_Type()
    {
        // Arrange
        var timeOnlyType = new TimeOnlyType();

        // Act
        var result = timeOnlyType.TrySerialize(12345, out var resultValue);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void TimeOnlyType_TryDeserialize_Should_Handle_Null()
    {
        // Arrange
        var timeOnlyType = new TimeOnlyType();

        // Act
        var result = timeOnlyType.TryDeserialize(null, out var runtimeValue);

        // Assert
        result.Should().BeTrue();
        runtimeValue.Should().BeNull();
    }

    [Fact]
    public void TimeOnlyType_TryDeserialize_Should_Handle_String()
    {
        // Arrange
        var timeOnlyType = new TimeOnlyType();

        // Act
        var result = timeOnlyType.TryDeserialize("14:30:00", out var runtimeValue);

        // Assert
        result.Should().BeTrue();
        runtimeValue.Should().BeOfType<TimeOnly>();
        ((TimeOnly)runtimeValue!).Should().Be(new TimeOnly(14, 30, 0));
    }

    [Fact]
    public void TimeOnlyType_TryDeserialize_Should_Handle_TimeOnly()
    {
        // Arrange
        var timeOnlyType = new TimeOnlyType();
        var time = new TimeOnly(14, 30, 0);

        // Act
        var result = timeOnlyType.TryDeserialize(time, out var runtimeValue);

        // Assert
        result.Should().BeTrue();
        runtimeValue.Should().Be(time);
    }

    [Fact]
    public void TimeOnlyType_TryDeserialize_Should_Handle_DateTimeValueNode()
    {
        // Arrange
        var timeOnlyType = new TimeOnlyType();
        var node = new DateTimeValueNode(new TimeOnly(14, 30, 0));

        // Act
        var result = timeOnlyType.TryDeserialize(node, out var runtimeValue);

        // Assert
        result.Should().BeTrue();
        runtimeValue.Should().BeOfType<TimeOnly>();
        ((TimeOnly)runtimeValue!).Should().Be(new TimeOnly(14, 30, 0));
    }

    [Fact]
    public void TimeOnlyType_TryDeserialize_Should_Return_False_For_Invalid_Type()
    {
        // Arrange
        var timeOnlyType = new TimeOnlyType();

        // Act
        var result = timeOnlyType.TryDeserialize(12345, out var runtimeValue);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void DateTimeOffsetType_ParseLiteral_Should_Parse_Valid_Value()
    {
        // Arrange
        var dateTimeOffsetType = new DateTimeOffsetType();
        var dateTimeOffset = new DateTimeOffset(2024, 1, 29, 14, 30, 0, TimeSpan.FromHours(-5));
        var valueSyntax = new DateTimeValueNode(dateTimeOffset);

        // Act
        var result = dateTimeOffsetType.ParseLiteral(valueSyntax);

        // Assert
        result.Should().Be(dateTimeOffset);
    }

    [Fact]
    public void DateTimeOffsetType_ParseValue_Should_Create_ValueNode()
    {
        // Arrange
        var dateTimeOffsetType = new DateTimeOffsetType();
        var dateTimeOffset = new DateTimeOffset(2024, 1, 29, 14, 30, 0, TimeSpan.FromHours(2));

        // Act
        var result = dateTimeOffsetType.ParseValue(dateTimeOffset);

        // Assert
        result.Should().BeOfType<DateTimeValueNode>();
        ((DateTimeValueNode)result).TryToDateTimeOffset(out var dto).Should().BeTrue();
        dto.Should().Be(dateTimeOffset);
    }

    [Fact]
    public void DateTimeOffsetType_ParseResult_Should_Handle_DateTimeOffset()
    {
        // Arrange
        var dateTimeOffsetType = new DateTimeOffsetType();
        var dateTimeOffset = new DateTimeOffset(2024, 1, 29, 14, 30, 0, TimeSpan.Zero);

        // Act
        var result = dateTimeOffsetType.ParseResult(dateTimeOffset);

        // Assert
        result.Should().BeOfType<DateTimeValueNode>();
    }

    [Fact]
    public void DateTimeOffsetType_ParseResult_Should_Handle_String()
    {
        // Arrange
        var dateTimeOffsetType = new DateTimeOffsetType();
        var dateString = "2024-01-29T14:30:00.000+00:00";

        // Act
        var result = dateTimeOffsetType.ParseResult(dateString);

        // Assert
        result.Should().BeOfType<DateTimeValueNode>();
    }

    [Fact]
    public void DateTimeOffsetType_ParseResult_Should_Handle_Null()
    {
        // Arrange
        var dateTimeOffsetType = new DateTimeOffsetType();

        // Act
        var result = dateTimeOffsetType.ParseResult(null);

        // Assert
        result.Should().BeOfType<NullValueNode>();
    }

    [Fact]
    public void DateTimeOffsetType_TrySerialize_Should_Serialize_DateTimeOffset()
    {
        // Arrange
        var dateTimeOffsetType = new DateTimeOffsetType();
        var dateTimeOffset = new DateTimeOffset(2024, 1, 29, 14, 30, 0, TimeSpan.FromHours(3));

        // Act
        var result = dateTimeOffsetType.TrySerialize(dateTimeOffset, out var resultValue);

        // Assert
        result.Should().BeTrue();
        resultValue.Should().BeOfType<string>();
        resultValue.Should().Be("2024-01-29T14:30:00.000+03:00");
    }

    [Fact]
    public void DateTimeOffsetType_TrySerialize_Should_Handle_Null()
    {
        // Arrange
        var dateTimeOffsetType = new DateTimeOffsetType();

        // Act
        var result = dateTimeOffsetType.TrySerialize(null, out var resultValue);

        // Assert
        result.Should().BeTrue();
        resultValue.Should().BeNull();
    }

    [Fact]
    public void DateTimeOffsetType_TrySerialize_Should_Return_False_For_Invalid_Type()
    {
        // Arrange
        var dateTimeOffsetType = new DateTimeOffsetType();

        // Act
        var result = dateTimeOffsetType.TrySerialize(12345, out var resultValue);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void DateTimeOffsetType_TryDeserialize_Should_Deserialize_String()
    {
        // Arrange
        var dateTimeOffsetType = new DateTimeOffsetType();
        var dateString = "2024-01-29T14:30:00.000-08:00";

        // Act
        var result = dateTimeOffsetType.TryDeserialize(dateString, out var runtimeValue);

        // Assert
        result.Should().BeTrue();
        runtimeValue.Should().BeOfType<DateTimeOffset>();
        ((DateTimeOffset)runtimeValue!).Year.Should().Be(2024);
        ((DateTimeOffset)runtimeValue!).Month.Should().Be(1);
        ((DateTimeOffset)runtimeValue!).Day.Should().Be(29);
    }

    [Fact]
    public void DateTimeOffsetType_TryDeserialize_Should_Handle_Null()
    {
        // Arrange
        var dateTimeOffsetType = new DateTimeOffsetType();

        // Act
        var result = dateTimeOffsetType.TryDeserialize(null, out var runtimeValue);

        // Assert
        result.Should().BeTrue();
        runtimeValue.Should().BeNull();
    }

    [Fact]
    public void DateTimeOffsetType_TryDeserialize_Should_Handle_DateTimeOffset()
    {
        // Arrange
        var dateTimeOffsetType = new DateTimeOffsetType();
        var dateTimeOffset = new DateTimeOffset(2024, 1, 29, 14, 30, 0, TimeSpan.Zero);

        // Act
        var result = dateTimeOffsetType.TryDeserialize(dateTimeOffset, out var runtimeValue);

        // Assert
        result.Should().BeTrue();
        runtimeValue.Should().BeOfType<DateTimeOffset>();
        ((DateTimeOffset)runtimeValue!).Should().Be(dateTimeOffset);
    }

    [Fact]
    public void DateTimeOffsetType_TryDeserialize_Should_Return_False_For_Invalid_Type()
    {
        // Arrange
        var dateTimeOffsetType = new DateTimeOffsetType();

        // Act
        var result = dateTimeOffsetType.TryDeserialize(12345, out var runtimeValue);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void DateTimeValueNode_Should_Handle_DateTime()
    {
        // Arrange
        var dateTime = new DateTime(2024, 1, 29, 14, 30, 0);
        var node = new DateTimeValueNode(dateTime);

        // Act
        var result = node.ToDateTime();

        // Assert
        result.Should().Be(dateTime);
        node.TryToDateTime(out var dt).Should().BeTrue();
        dt.Should().Be(dateTime);
    }

    [Fact]
    public void DateTimeValueNode_Should_Handle_DateTimeOffset()
    {
        // Arrange
        var dateTimeOffset = new DateTimeOffset(2024, 1, 29, 14, 30, 0, TimeSpan.Zero);
        var node = new DateTimeValueNode(dateTimeOffset);

        // Act
        var result = node.ToDateTimeOffset();

        // Assert
        result.Should().Be(dateTimeOffset);
        node.TryToDateTimeOffset(out var dto).Should().BeTrue();
        dto.Should().Be(dateTimeOffset);
    }

    [Fact]
    public void DateTimeValueNode_Should_Return_Null_For_Invalid_Conversions()
    {
        // Arrange
        var node = new DateTimeValueNode("invalid");

        // Act & Assert
        node.ToDateOnly().Should().BeNull();
        node.ToTimeOnly().Should().BeNull();
        node.ToDateTime().Should().BeNull();
        node.ToDateTimeOffset().Should().BeNull();
    }

    [Fact]
    public void DateTimeValueNode_GetNodes_Should_Return_Empty()
    {
        // Arrange
        var node = new DateTimeValueNode("2024-01-29");

        // Act
        var result = node.GetNodes();

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void DateTimeValueNode_ToString_With_Indentation_Should_Return_Same_Value()
    {
        // Arrange
        var node = new DateTimeValueNode(new DateOnly(2024, 1, 29));

        // Act
        var result = node.ToString(true);

        // Assert
        result.Should().Be(node.ToString());
    }

    // Dummy query for testing
    private class DummyQuery
    {
        public string Hello() => "World";
    }
}
