using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;
using YamlDotNet.Serialization;

namespace Ddap.Rest.Formatters;

/// <summary>
/// Custom output formatter for YAML content type.
/// Supports content negotiation for YAML format based on Accept header.
/// </summary>
public class YamlOutputFormatter : TextOutputFormatter
{
    private readonly ISerializer _serializer;

    /// <summary>
    /// Initializes a new instance of the <see cref="YamlOutputFormatter"/> class.
    /// </summary>
    public YamlOutputFormatter()
    {
        _serializer = new SerializerBuilder().Build();

        SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse("application/x-yaml"));
        SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse("application/yaml"));
        SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse("text/yaml"));
        SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse("text/x-yaml"));

        SupportedEncodings.Add(Encoding.UTF8);
        SupportedEncodings.Add(Encoding.Unicode);
    }

    /// <inheritdoc/>
    protected override bool CanWriteType(Type? type)
    {
        return type != null;
    }

    /// <inheritdoc/>
    public override async Task WriteResponseBodyAsync(
        OutputFormatterWriteContext context,
        Encoding selectedEncoding
    )
    {
        ArgumentNullException.ThrowIfNull(context);

        var response = context.HttpContext.Response;
        var yaml = _serializer.Serialize(context.Object);

        await response.WriteAsync(yaml, selectedEncoding);
    }
}
