using System.Text;
using System.Text.Json;

namespace Ddap.Grpc;

/// <summary>
/// Provides binary serialization for raw query results.
/// Efficiently serializes query results to binary format for gRPC transmission.
/// </summary>
public static class RawQueryBinarySerializer
{
    /// <summary>
    /// Serializes a scalar value to binary format.
    /// </summary>
    /// <param name="value">The value to serialize.</param>
    /// <returns>Binary representation of the value.</returns>
    public static byte[] SerializeScalar(object? value)
    {
        if (value == null)
        {
            return Array.Empty<byte>();
        }

        return value switch
        {
            byte[] bytes => bytes,
            string str => Encoding.UTF8.GetBytes(str),
            int intVal => BitConverter.GetBytes(intVal),
            long longVal => BitConverter.GetBytes(longVal),
            double doubleVal => BitConverter.GetBytes(doubleVal),
            float floatVal => BitConverter.GetBytes(floatVal),
            bool boolVal => BitConverter.GetBytes(boolVal),
            DateTime dateTime => Encoding.UTF8.GetBytes(dateTime.ToString("O")),
            Guid guid => guid.ToByteArray(),
            _ => Encoding.UTF8.GetBytes(JsonSerializer.Serialize(value)),
        };
    }

    /// <summary>
    /// Serializes a single row to binary format.
    /// </summary>
    /// <param name="row">The row data as a dynamic object.</param>
    /// <returns>Binary representation of the row.</returns>
    public static byte[] SerializeSingleRow(dynamic? row)
    {
        if (row == null)
        {
            return Array.Empty<byte>();
        }

        var json = JsonSerializer.Serialize(row);
        return Encoding.UTF8.GetBytes(json);
    }

    /// <summary>
    /// Serializes multiple rows to binary format.
    /// </summary>
    /// <param name="rows">The rows to serialize.</param>
    /// <returns>Binary representation of the rows.</returns>
    public static byte[] SerializeMultipleRows(IEnumerable<dynamic> rows)
    {
        if (rows == null || !rows.Any())
        {
            return Array.Empty<byte>();
        }

        var json = JsonSerializer.Serialize(rows);
        return Encoding.UTF8.GetBytes(json);
    }

    /// <summary>
    /// Deserializes a scalar value from binary format.
    /// </summary>
    /// <typeparam name="T">The target type.</typeparam>
    /// <param name="data">The binary data.</param>
    /// <param name="typeName">The type name for deserialization.</param>
    /// <returns>The deserialized value.</returns>
    public static T? DeserializeScalar<T>(byte[] data, string typeName)
    {
        if (data == null || data.Length == 0)
        {
            return default;
        }

        return typeName switch
        {
            "System.String" => (T?)(object)Encoding.UTF8.GetString(data),
            "System.Int32" => (T?)(object)BitConverter.ToInt32(data, 0),
            "System.Int64" => (T?)(object)BitConverter.ToInt64(data, 0),
            "System.Double" => (T?)(object)BitConverter.ToDouble(data, 0),
            "System.Single" => (T?)(object)BitConverter.ToSingle(data, 0),
            "System.Boolean" => (T?)(object)BitConverter.ToBoolean(data, 0),
            "System.DateTime" => (T?)
                (object)
                    DateTime.ParseExact(
                        Encoding.UTF8.GetString(data),
                        "O",
                        System.Globalization.CultureInfo.InvariantCulture
                    ),
            "System.Guid" => (T?)(object)new Guid(data),
            _ => JsonSerializer.Deserialize<T>(data),
        };
    }

    /// <summary>
    /// Deserializes a single row from binary format.
    /// </summary>
    /// <param name="data">The binary data.</param>
    /// <returns>The deserialized row as a dictionary.</returns>
    public static Dictionary<string, object?>? DeserializeSingleRow(byte[] data)
    {
        if (data == null || data.Length == 0)
        {
            return null;
        }

        var json = Encoding.UTF8.GetString(data);
        return JsonSerializer.Deserialize<Dictionary<string, object?>>(json);
    }

    /// <summary>
    /// Deserializes multiple rows from binary format.
    /// </summary>
    /// <param name="data">The binary data.</param>
    /// <returns>The deserialized rows.</returns>
    public static List<Dictionary<string, object?>>? DeserializeMultipleRows(byte[] data)
    {
        if (data == null || data.Length == 0)
        {
            return new List<Dictionary<string, object?>>();
        }

        var json = Encoding.UTF8.GetString(data);
        return JsonSerializer.Deserialize<List<Dictionary<string, object?>>>(json);
    }

    /// <summary>
    /// Extracts column information from a row.
    /// </summary>
    /// <param name="row">The row data.</param>
    /// <returns>Tuple of column names and types.</returns>
    public static (string[] names, string[] types) ExtractColumnInfo(dynamic? row)
    {
        if (row == null)
        {
            return (Array.Empty<string>(), Array.Empty<string>());
        }

        if (row is IDictionary<string, object?> dict)
        {
            var names = dict.Keys.ToArray();
            var types = dict.Values.Select(v => v?.GetType().FullName ?? "System.Object").ToArray();
            return (names, types);
        }

        return (Array.Empty<string>(), Array.Empty<string>());
    }
}
