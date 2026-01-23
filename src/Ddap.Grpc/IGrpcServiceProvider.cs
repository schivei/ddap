namespace Ddap.Grpc;

/// <summary>
/// Represents a provider for gRPC service functionality.
/// </summary>
public interface IGrpcServiceProvider
{
    /// <summary>
    /// Gets a value indicating whether the gRPC service is enabled.
    /// </summary>
    bool IsEnabled { get; }

    /// <summary>
    /// Gets the path for downloading the .proto file.
    /// </summary>
    string ProtoFilePath { get; }
}

/// <summary>
/// Default implementation of <see cref="IGrpcServiceProvider"/>.
/// </summary>
internal class GrpcServiceProvider : IGrpcServiceProvider
{
    /// <inheritdoc/>
    public bool IsEnabled => true;

    /// <inheritdoc/>
    public string ProtoFilePath => "/proto/ddap.proto";
}
