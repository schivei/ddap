using System.Globalization;
using HotChocolate.Language;

namespace Ddap.GraphQL.Scalars;

/// <summary>
/// Custom value node for representing date and time values in GraphQL.
/// Supports DateTime, DateTimeOffset, DateOnly, and TimeOnly.
/// </summary>
public class DateTimeValueNode : IValueNode<string>, IValueNode, ISyntaxNode, IHasSpan
{
    private const string TimeFormat = "HH:mm:ss.fff";
    private const string DateFormat = "yyyy-MM-dd";
    private const string DateTimeFormat = "o"; // ISO 8601 round-trip format
    private const string DateTimeOffsetFormat = "yyyy-MM-ddTHH\\:mm\\:ss.fffzzz";

    private readonly DateTime? _dateTimeValue;
    private readonly DateTimeOffset? _dateTimeOffsetValue;
    private readonly DateOnly? _dateOnlyValue;
    private readonly TimeOnly? _timeOnlyValue;
    private readonly string? _stringValue;

    /// <summary>
    /// Gets the string value representation.
    /// </summary>
    public string Value => ToString();

    /// <summary>
    /// Gets the syntax kind (always StringValue).
    /// </summary>
    public SyntaxKind Kind => SyntaxKind.StringValue;

    /// <summary>
    /// Gets the location in the source (always null for runtime values).
    /// </summary>
    public HotChocolate.Language.Location? Location => null;

    /// <summary>
    /// Gets the object value.
    /// </summary>
    object? IValueNode.Value => Value;

    /// <summary>
    /// Initializes a new instance from a string value.
    /// </summary>
    public DateTimeValueNode(string? value)
    {
        _stringValue = value;
    }

    /// <summary>
    /// Initializes a new instance from a DateTimeOffset value.
    /// </summary>
    public DateTimeValueNode(DateTimeOffset? value)
    {
        _dateTimeOffsetValue = value;
    }

    /// <summary>
    /// Initializes a new instance from a DateTime value.
    /// </summary>
    public DateTimeValueNode(DateTime? value)
    {
        _dateTimeValue = value;
    }

    /// <summary>
    /// Initializes a new instance from a DateOnly value.
    /// </summary>
    public DateTimeValueNode(DateOnly? value)
    {
        _dateOnlyValue = value;
    }

    /// <summary>
    /// Initializes a new instance from a TimeOnly value.
    /// </summary>
    public DateTimeValueNode(TimeOnly? value)
    {
        _timeOnlyValue = value;
    }

    /// <summary>
    /// Returns the value as a byte span.
    /// </summary>
    public ReadOnlySpan<byte> AsSpan()
    {
        return System.Text.Encoding.UTF8.GetBytes(ToString());
    }

    /// <summary>
    /// Gets child syntax nodes (empty for value nodes).
    /// </summary>
    public IEnumerable<ISyntaxNode> GetNodes()
    {
        yield break;
    }

    /// <summary>
    /// Returns the string representation with optional indentation.
    /// </summary>
    public string ToString(bool indented)
    {
        return ToString();
    }

    /// <summary>
    /// Returns the formatted string representation based on the contained value type.
    /// </summary>
    public override string ToString()
    {
        return TryToDateTimeOffset(out var dateTimeOffset)
                ? dateTimeOffset.ToString(DateTimeOffsetFormat, CultureInfo.InvariantCulture)
            : TryToDateTime(out var dateTime)
                ? dateTime.ToString(DateTimeFormat, CultureInfo.InvariantCulture)
            : TryToDateOnly(out var dateOnly)
                ? dateOnly.ToString(DateFormat, CultureInfo.InvariantCulture)
            : TryToTimeOnly(out var timeOnly)
                ? timeOnly.ToString(TimeFormat, CultureInfo.InvariantCulture)
            : _stringValue ?? string.Empty;
    }

    /// <summary>
    /// Converts to DateTimeOffset if possible.
    /// </summary>
    public DateTimeOffset? ToDateTimeOffset()
    {
        return TryToDateTimeOffset(out var dateTimeOffset) ? dateTimeOffset : null;
    }

    /// <summary>
    /// Converts to DateTime if possible.
    /// </summary>
    public DateTime? ToDateTime()
    {
        return TryToDateTime(out var dateTime) ? dateTime : null;
    }

    /// <summary>
    /// Converts to DateOnly if possible.
    /// </summary>
    public DateOnly? ToDateOnly()
    {
        return TryToDateOnly(out var dateOnly) ? dateOnly : null;
    }

    /// <summary>
    /// Converts to TimeOnly if possible.
    /// </summary>
    public TimeOnly? ToTimeOnly()
    {
        return TryToTimeOnly(out var timeOnly) ? timeOnly : null;
    }

    /// <summary>
    /// Tries to convert to DateTimeOffset.
    /// </summary>
    public bool TryToDateTimeOffset(out DateTimeOffset dateTimeOffset)
    {
        dateTimeOffset = default;
        if (_dateTimeOffsetValue.HasValue)
        {
            dateTimeOffset = _dateTimeOffsetValue.Value;
            return true;
        }
        if (DateTimeOffset.TryParse(_stringValue, CultureInfo.InvariantCulture, out var dto))
        {
            dateTimeOffset = dto;
            return true;
        }
        return false;
    }

    /// <summary>
    /// Tries to convert to DateTime.
    /// </summary>
    public bool TryToDateTime(out DateTime dateTime)
    {
        dateTime = default;
        if (_dateTimeValue.HasValue)
        {
            dateTime = _dateTimeValue.Value;
            return true;
        }
        if (DateTime.TryParse(_stringValue, CultureInfo.InvariantCulture, out var dt))
        {
            dateTime = dt;
            return true;
        }
        return false;
    }

    /// <summary>
    /// Tries to convert to DateOnly.
    /// </summary>
    public bool TryToDateOnly(out DateOnly dateOnly)
    {
        dateOnly = default;
        if (_dateOnlyValue.HasValue)
        {
            dateOnly = _dateOnlyValue.Value;
            return true;
        }
        if (DateOnly.TryParse(_stringValue, CultureInfo.InvariantCulture, out var d))
        {
            dateOnly = d;
            return true;
        }
        return false;
    }

    /// <summary>
    /// Tries to convert to TimeOnly.
    /// </summary>
    public bool TryToTimeOnly(out TimeOnly timeOnly)
    {
        timeOnly = default;
        if (_timeOnlyValue.HasValue)
        {
            timeOnly = _timeOnlyValue.Value;
            return true;
        }
        if (TimeOnly.TryParse(_stringValue, CultureInfo.InvariantCulture, out var t))
        {
            timeOnly = t;
            return true;
        }
        return false;
    }
}
