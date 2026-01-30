using Ddap.Grpc;
using FluentAssertions;
using Xunit;

namespace Ddap.Tests.RawQuery;

public class RawQueryBinarySerializerTests
{
    [Fact]
    public void SerializeScalar_Should_Handle_Null_Value()
    {
        // Act
        var result = RawQueryBinarySerializer.SerializeScalar(null);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void SerializeScalar_Should_Serialize_Int()
    {
        // Arrange
        int value = 42;

        // Act
        var result = RawQueryBinarySerializer.SerializeScalar(value);

        // Assert
        result.Should().NotBeEmpty();
        result.Length.Should().Be(4); // int is 4 bytes
    }

    [Fact]
    public void SerializeScalar_Should_Serialize_String()
    {
        // Arrange
        string value = "test";

        // Act
        var result = RawQueryBinarySerializer.SerializeScalar(value);

        // Assert
        result.Should().NotBeEmpty();
        System.Text.Encoding.UTF8.GetString(result).Should().Be(value);
    }

    [Fact]
    public void SerializeScalar_Should_Serialize_Bool()
    {
        // Arrange
        bool value = true;

        // Act
        var result = RawQueryBinarySerializer.SerializeScalar(value);

        // Assert
        result.Should().NotBeEmpty();
        result.Length.Should().Be(1); // bool is 1 byte
    }

    [Fact]
    public void SerializeScalar_Should_Serialize_Guid()
    {
        // Arrange
        var value = Guid.NewGuid();

        // Act
        var result = RawQueryBinarySerializer.SerializeScalar(value);

        // Assert
        result.Should().NotBeEmpty();
        result.Length.Should().Be(16); // Guid is 16 bytes
    }

    [Fact]
    public void DeserializeScalar_Should_Deserialize_Int()
    {
        // Arrange
        int original = 42;
        var serialized = RawQueryBinarySerializer.SerializeScalar(original);

        // Act
        var result = RawQueryBinarySerializer.DeserializeScalar<int>(serialized, "System.Int32");

        // Assert
        result.Should().Be(original);
    }

    [Fact]
    public void DeserializeScalar_Should_Deserialize_String()
    {
        // Arrange
        string original = "test";
        var serialized = RawQueryBinarySerializer.SerializeScalar(original);

        // Act
        var result = RawQueryBinarySerializer.DeserializeScalar<string>(
            serialized,
            "System.String"
        );

        // Assert
        result.Should().Be(original);
    }

    [Fact]
    public void DeserializeScalar_Should_Deserialize_Bool()
    {
        // Arrange
        bool original = true;
        var serialized = RawQueryBinarySerializer.SerializeScalar(original);

        // Act
        var result = RawQueryBinarySerializer.DeserializeScalar<bool>(serialized, "System.Boolean");

        // Assert
        result.Should().Be(original);
    }

    [Fact]
    public void DeserializeScalar_Should_Return_Default_For_Empty_Data()
    {
        // Act
        var result = RawQueryBinarySerializer.DeserializeScalar<int>(
            Array.Empty<byte>(),
            "System.Int32"
        );

        // Assert
        result.Should().Be(0);
    }

    [Fact]
    public void SerializeSingleRow_Should_Handle_Null()
    {
        // Act
        var result = RawQueryBinarySerializer.SerializeSingleRow(null);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void SerializeSingleRow_Should_Serialize_Dictionary()
    {
        // Arrange
        var row = new Dictionary<string, object?>
        {
            { "Id", 1 },
            { "Name", "Test" },
            { "Active", true },
        };

        // Act
        var result = RawQueryBinarySerializer.SerializeSingleRow(row);

        // Assert
        result.Should().NotBeEmpty();
    }

    [Fact]
    public void DeserializeSingleRow_Should_Return_Null_For_Empty_Data()
    {
        // Act
        var result = RawQueryBinarySerializer.DeserializeSingleRow(Array.Empty<byte>());

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void SerializeMultipleRows_Should_Handle_Empty_Collection()
    {
        // Act
        var result = RawQueryBinarySerializer.SerializeMultipleRows(
            new List<Dictionary<string, object?>>()
        );

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void SerializeMultipleRows_Should_Serialize_Collection()
    {
        // Arrange
        var rows = new List<Dictionary<string, object?>>
        {
            new() { { "Id", 1 }, { "Name", "Test1" } },
            new() { { "Id", 2 }, { "Name", "Test2" } },
        };

        // Act
        var result = RawQueryBinarySerializer.SerializeMultipleRows(rows);

        // Assert
        result.Should().NotBeEmpty();
    }

    [Fact]
    public void DeserializeMultipleRows_Should_Return_Empty_For_Empty_Data()
    {
        // Act
        var result = RawQueryBinarySerializer.DeserializeMultipleRows(Array.Empty<byte>());

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void ExtractColumnInfo_Should_Return_Empty_For_Null()
    {
        // Act
        var (names, types) = RawQueryBinarySerializer.ExtractColumnInfo(null);

        // Assert
        names.Should().BeEmpty();
        types.Should().BeEmpty();
    }

    [Fact]
    public void ExtractColumnInfo_Should_Extract_From_Dictionary()
    {
        // Arrange
        var row = new Dictionary<string, object?>
        {
            { "Id", 1 },
            { "Name", "Test" },
            { "Active", true },
        };

        // Act
        var (names, types) = RawQueryBinarySerializer.ExtractColumnInfo(row);

        // Assert
        names.Should().Contain("Id").And.Contain("Name").And.Contain("Active");
        types.Should().HaveCount(3);
    }

    [Fact]
    public void RoundTrip_Should_Preserve_Scalar_Values()
    {
        // Arrange
        var testValues = new object[] { 42, "test", true, 3.14, 123L };

        foreach (var original in testValues)
        {
            // Act
            var serialized = RawQueryBinarySerializer.SerializeScalar(original);
            var typeName = original.GetType().FullName!;
            var deserialized = RawQueryBinarySerializer.DeserializeScalar<object>(
                serialized,
                typeName
            );

            // Assert
            deserialized.Should().Be(original, $"Failed for type {typeName}");
        }
    }
}
