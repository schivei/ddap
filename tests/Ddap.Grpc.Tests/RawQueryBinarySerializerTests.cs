using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Ddap.Grpc.Tests;

public class RawQueryBinarySerializerTests
{
    [Fact]
    public void SerializeScalar_WithNull_ReturnsEmptyArray()
    {
        var result = RawQueryBinarySerializer.SerializeScalar(null);
        Assert.Empty(result);
    }

    [Fact]
    public void SerializeScalar_WithByteArray_ReturnsSameArray()
    {
        var bytes = new byte[] { 1, 2, 3, 4 };
        var result = RawQueryBinarySerializer.SerializeScalar(bytes);
        Assert.Equal(bytes, result);
    }

    [Fact]
    public void SerializeScalar_WithString_ReturnsUtf8Bytes()
    {
        var text = "Hello World";
        var result = RawQueryBinarySerializer.SerializeScalar(text);
        var decoded = System.Text.Encoding.UTF8.GetString(result);
        Assert.Equal(text, decoded);
    }

    [Fact]
    public void SerializeScalar_WithInt_ReturnsBinaryRepresentation()
    {
        var value = 42;
        var result = RawQueryBinarySerializer.SerializeScalar(value);
        var decoded = BitConverter.ToInt32(result, 0);
        Assert.Equal(value, decoded);
    }

    [Fact]
    public void SerializeScalar_WithLong_ReturnsBinaryRepresentation()
    {
        var value = 9223372036854775807L;
        var result = RawQueryBinarySerializer.SerializeScalar(value);
        var decoded = BitConverter.ToInt64(result, 0);
        Assert.Equal(value, decoded);
    }

    [Fact]
    public void SerializeScalar_WithDouble_ReturnsBinaryRepresentation()
    {
        var value = 3.14159;
        var result = RawQueryBinarySerializer.SerializeScalar(value);
        var decoded = BitConverter.ToDouble(result, 0);
        Assert.Equal(value, decoded);
    }

    [Fact]
    public void SerializeScalar_WithFloat_ReturnsBinaryRepresentation()
    {
        var value = 3.14f;
        var result = RawQueryBinarySerializer.SerializeScalar(value);
        var decoded = BitConverter.ToSingle(result, 0);
        Assert.Equal(value, decoded);
    }

    [Fact]
    public void SerializeScalar_WithBool_ReturnsBinaryRepresentation()
    {
        var value = true;
        var result = RawQueryBinarySerializer.SerializeScalar(value);
        var decoded = BitConverter.ToBoolean(result, 0);
        Assert.Equal(value, decoded);
    }

    [Fact]
    public void SerializeScalar_WithDateTime_ReturnsIso8601String()
    {
        var value = new DateTime(2024, 1, 15, 10, 30, 0, DateTimeKind.Utc);
        var result = RawQueryBinarySerializer.SerializeScalar(value);
        var decoded = System.Text.Encoding.UTF8.GetString(result);
        Assert.Contains("2024-01-15", decoded);
    }

    [Fact]
    public void SerializeScalar_WithGuid_ReturnsGuidBytes()
    {
        var value = Guid.NewGuid();
        var result = RawQueryBinarySerializer.SerializeScalar(value);
        var decoded = new Guid(result);
        Assert.Equal(value, decoded);
    }

    [Fact]
    public void SerializeScalar_WithComplexObject_ReturnsJsonBytes()
    {
        var value = new { Name = "Test", Value = 42 };
        var result = RawQueryBinarySerializer.SerializeScalar(value);
        var decoded = System.Text.Encoding.UTF8.GetString(result);
        Assert.Contains("Test", decoded);
        Assert.Contains("42", decoded);
    }

    [Fact]
    public void SerializeSingleRow_WithNull_ReturnsEmptyArray()
    {
        var result = RawQueryBinarySerializer.SerializeSingleRow(null);
        Assert.Empty(result);
    }

    [Fact]
    public void SerializeSingleRow_WithDynamicObject_ReturnsJsonBytes()
    {
        dynamic row = new { Id = 1, Name = "Test" };
        var result = RawQueryBinarySerializer.SerializeSingleRow(row);
        var decoded = System.Text.Encoding.UTF8.GetString(result);
        Assert.Contains("Test", decoded);
    }

    [Fact]
    public void SerializeMultipleRows_WithNull_ReturnsEmptyArray()
    {
        var result = RawQueryBinarySerializer.SerializeMultipleRows(null!);
        Assert.Empty(result);
    }

    [Fact]
    public void SerializeMultipleRows_WithEmptyCollection_ReturnsEmptyArray()
    {
        var result = RawQueryBinarySerializer.SerializeMultipleRows(new List<dynamic>());
        Assert.Empty(result);
    }

    [Fact]
    public void SerializeMultipleRows_WithRows_ReturnsJsonArray()
    {
        var rows = new List<dynamic>
        {
            new { Id = 1, Name = "First" },
            new { Id = 2, Name = "Second" },
        };
        var result = RawQueryBinarySerializer.SerializeMultipleRows(rows);
        var decoded = System.Text.Encoding.UTF8.GetString(result);
        Assert.Contains("First", decoded);
        Assert.Contains("Second", decoded);
    }

    [Fact]
    public void DeserializeScalar_WithNullData_ReturnsDefault()
    {
        var result = RawQueryBinarySerializer.DeserializeScalar<string>(null!, "System.String");
        Assert.Null(result);
    }

    [Fact]
    public void DeserializeScalar_WithEmptyData_ReturnsDefault()
    {
        var result = RawQueryBinarySerializer.DeserializeScalar<string>(
            Array.Empty<byte>(),
            "System.String"
        );
        Assert.Null(result);
    }

    [Fact]
    public void DeserializeScalar_String_ReturnsCorrectValue()
    {
        var original = "Hello World";
        var serialized = System.Text.Encoding.UTF8.GetBytes(original);
        var result = RawQueryBinarySerializer.DeserializeScalar<string>(
            serialized,
            "System.String"
        );
        Assert.Equal(original, result);
    }

    [Fact]
    public void DeserializeScalar_Int32_ReturnsCorrectValue()
    {
        var original = 42;
        var serialized = BitConverter.GetBytes(original);
        var result = RawQueryBinarySerializer.DeserializeScalar<int>(serialized, "System.Int32");
        Assert.Equal(original, result);
    }

    [Fact]
    public void DeserializeScalar_Int64_ReturnsCorrectValue()
    {
        var original = 9223372036854775807L;
        var serialized = BitConverter.GetBytes(original);
        var result = RawQueryBinarySerializer.DeserializeScalar<long>(serialized, "System.Int64");
        Assert.Equal(original, result);
    }

    [Fact]
    public void DeserializeScalar_Double_ReturnsCorrectValue()
    {
        var original = 3.14159;
        var serialized = BitConverter.GetBytes(original);
        var result = RawQueryBinarySerializer.DeserializeScalar<double>(
            serialized,
            "System.Double"
        );
        Assert.Equal(original, result);
    }

    [Fact]
    public void DeserializeScalar_Single_ReturnsCorrectValue()
    {
        var original = 3.14f;
        var serialized = BitConverter.GetBytes(original);
        var result = RawQueryBinarySerializer.DeserializeScalar<float>(serialized, "System.Single");
        Assert.Equal(original, result);
    }

    [Fact]
    public void DeserializeScalar_Boolean_ReturnsCorrectValue()
    {
        var original = true;
        var serialized = BitConverter.GetBytes(original);
        var result = RawQueryBinarySerializer.DeserializeScalar<bool>(serialized, "System.Boolean");
        Assert.Equal(original, result);
    }

    [Fact]
    public void DeserializeScalar_DateTime_ReturnsCorrectValue()
    {
        var original = new DateTime(2024, 1, 15, 10, 30, 0, DateTimeKind.Utc);
        var serialized = System.Text.Encoding.UTF8.GetBytes(original.ToString("O"));
        var result = RawQueryBinarySerializer.DeserializeScalar<DateTime>(
            serialized,
            "System.DateTime"
        );
        Assert.Equal(original, result);
    }

    [Fact]
    public void DeserializeScalar_Guid_ReturnsCorrectValue()
    {
        var original = Guid.NewGuid();
        var serialized = original.ToByteArray();
        var result = RawQueryBinarySerializer.DeserializeScalar<Guid>(serialized, "System.Guid");
        Assert.Equal(original, result);
    }

    [Fact]
    public void DeserializeScalar_UnknownType_UsesJsonDeserialization()
    {
        var original = new { Name = "Test", Value = 42 };
        var json = System.Text.Json.JsonSerializer.Serialize(original);
        var serialized = System.Text.Encoding.UTF8.GetBytes(json);
        var result = RawQueryBinarySerializer.DeserializeScalar<System.Text.Json.JsonElement>(
            serialized,
            "CustomType"
        );
        Assert.NotEqual(default, result);
    }

    [Fact]
    public void DeserializeSingleRow_WithNullData_ReturnsNull()
    {
        var result = RawQueryBinarySerializer.DeserializeSingleRow(null!);
        Assert.Null(result);
    }

    [Fact]
    public void DeserializeSingleRow_WithEmptyData_ReturnsNull()
    {
        var result = RawQueryBinarySerializer.DeserializeSingleRow(Array.Empty<byte>());
        Assert.Null(result);
    }

    [Fact]
    public void DeserializeSingleRow_WithValidJson_ReturnsDictionary()
    {
        var json = "{\"Id\":1,\"Name\":\"Test\"}";
        var serialized = System.Text.Encoding.UTF8.GetBytes(json);
        var result = RawQueryBinarySerializer.DeserializeSingleRow(serialized);
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public void DeserializeMultipleRows_WithNullData_ReturnsEmptyList()
    {
        var result = RawQueryBinarySerializer.DeserializeMultipleRows(null!);
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public void DeserializeMultipleRows_WithEmptyData_ReturnsEmptyList()
    {
        var result = RawQueryBinarySerializer.DeserializeMultipleRows(Array.Empty<byte>());
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public void DeserializeMultipleRows_WithValidJson_ReturnsList()
    {
        var json = "[{\"Id\":1,\"Name\":\"First\"},{\"Id\":2,\"Name\":\"Second\"}]";
        var serialized = System.Text.Encoding.UTF8.GetBytes(json);
        var result = RawQueryBinarySerializer.DeserializeMultipleRows(serialized);
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public void ExtractColumnInfo_WithNull_ReturnsEmptyArrays()
    {
        var (names, types) = RawQueryBinarySerializer.ExtractColumnInfo(null);
        Assert.Empty(names);
        Assert.Empty(types);
    }

    [Fact]
    public void ExtractColumnInfo_WithDictionary_ReturnsColumnNamesAndTypes()
    {
        var row = new Dictionary<string, object?>
        {
            { "Id", 1 },
            { "Name", "Test" },
            { "Value", 42.5 },
        };

        var (names, types) = RawQueryBinarySerializer.ExtractColumnInfo(row);

        Assert.Equal(3, names.Length);
        Assert.Contains("Id", names);
        Assert.Contains("Name", names);
        Assert.Contains("Value", names);
        Assert.Equal(3, types.Length);
    }

    [Fact]
    public void ExtractColumnInfo_WithNonDictionary_ReturnsEmptyArrays()
    {
        var row = new { Id = 1, Name = "Test" };
        var (names, types) = RawQueryBinarySerializer.ExtractColumnInfo(row);
        Assert.Empty(names);
        Assert.Empty(types);
    }

    [Fact]
    public void RoundTrip_SerializeAndDeserialize_String()
    {
        var original = "Test String";
        var serialized = RawQueryBinarySerializer.SerializeScalar(original);
        var deserialized = RawQueryBinarySerializer.DeserializeScalar<string>(
            serialized,
            "System.String"
        );
        Assert.Equal(original, deserialized);
    }

    [Fact]
    public void RoundTrip_SerializeAndDeserialize_Int()
    {
        var original = 123456;
        var serialized = RawQueryBinarySerializer.SerializeScalar(original);
        var deserialized = RawQueryBinarySerializer.DeserializeScalar<int>(
            serialized,
            "System.Int32"
        );
        Assert.Equal(original, deserialized);
    }

    [Fact]
    public void RoundTrip_SerializeAndDeserialize_Guid()
    {
        var original = Guid.NewGuid();
        var serialized = RawQueryBinarySerializer.SerializeScalar(original);
        var deserialized = RawQueryBinarySerializer.DeserializeScalar<Guid>(
            serialized,
            "System.Guid"
        );
        Assert.Equal(original, deserialized);
    }
}
