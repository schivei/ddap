using Ddap.Client.Core;
using Grpc.Net.Client;
using GrpcCore = Grpc.Core;

namespace Ddap.Client.Grpc;

/// <summary>
/// gRPC client for DDAP
/// </summary>
public class DdapGrpcClient : IDdapClient, IDisposable
{
    private readonly DdapClientOptions _options;
    private GrpcChannel? _channel;

    public DdapGrpcClient(DdapClientOptions options)
    {
        _options = options ?? throw new ArgumentNullException(nameof(options));
    }

    public string BaseUrl => _options.BaseUrl;

    public bool IsConnected =>
        _channel != null && _channel.State == GrpcCore.ConnectivityState.Ready;

    /// <summary>
    /// Gets or creates the gRPC channel
    /// </summary>
    public GrpcChannel GetChannel()
    {
        if (_channel == null)
        {
            _channel = GrpcChannel.ForAddress(
                _options.BaseUrl,
                new GrpcChannelOptions
                {
                    MaxReceiveMessageSize = 16 * 1024 * 1024, // 16 MB
                    MaxSendMessageSize = 16 * 1024 * 1024, // 16 MB
                }
            );
        }

        return _channel;
    }

    public async Task<bool> TestConnectionAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var channel = GetChannel();
            using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            cts.CancelAfter(TimeSpan.FromSeconds(5)); // 5 second timeout
            await channel.ConnectAsync(cts.Token);
            return IsConnected;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Creates a client for the specified gRPC service
    /// </summary>
    public TClient CreateClient<TClient>()
        where TClient : GrpcCore.ClientBase<TClient>
    {
        var channel = GetChannel();
        var constructor = typeof(TClient).GetConstructor(new[] { typeof(GrpcChannel) });

        if (constructor == null)
        {
            throw new DdapClientException($"Cannot find constructor for {typeof(TClient).Name}");
        }

        return (TClient)constructor.Invoke(new object[] { channel });
    }

    public void Dispose()
    {
        _channel?.Dispose();
        GC.SuppressFinalize(this);
    }
}
