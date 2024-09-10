namespace AIC.Core.Services.Networking.Connections.Tcp.Implementations;

using System.Net;
using System.Net.Sockets;
using AIC.Core.Models.Networking.Connections.Contracts;
using AIC.Core.Models.Networking.Contracts;
using AIC.Core.Services.Networking.Connections.Implementations;
using AIC.Core.Services.Networking.Connections.Tcp.Contracts;
using Microsoft.Extensions.Logging;

public sealed class TcpConnectionHandlingService : BaseConnectionHandlingService, ITcpConnectionHandlingService
{
    private NetworkStream? networkStream;
    private TcpClient? tcpClient;
    private TcpListener? tcpListener;
    private readonly IDictionary<Guid, (TcpClient, NetworkStream)> tcpClients = new Dictionary<Guid, (TcpClient, NetworkStream)>();

    public TcpConnectionHandlingService(ILogger logger) : base(logger)
    {
    }

    protected override async Task ConnectInternalAsync()
    {
        switch (this.ConnectionInformation.Mode)
        {
            case ConnectionInformationMode.Client:
                await this.SetupHandlerAsClient();
                break;
            case ConnectionInformationMode.Server:
                await this.SetupHandlerAsServer();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private async Task SetupHandlerAsClient()
    {
        this.tcpClient = new TcpClient();

        await this.tcpClient.ConnectAsync(this.ConnectionInformation.Host, this.ConnectionInformation.Port);

        this.Logger.LogInformation($"{this.GetLoggingPrefix()} - Connected to [{this.ConnectionInformation.Host}:{this.ConnectionInformation.Port}]");

        this.networkStream = this.tcpClient.GetStream();

        _ = Task.Run(this.ListenForDataAsync);
    }

    private string GetLoggingPrefix()
    {
        return $"TCP [{this.ConnectionInformation.Mode}]";
    }

    private async Task SetupHandlerAsServer()
    {
        this.tcpListener = new TcpListener(IPAddress.Parse(this.ConnectionInformation.Host),
            this.ConnectionInformation.Port);

        this.tcpListener.Start();

        this.Logger.LogInformation($"{this.GetLoggingPrefix()} - Listening on [{this.ConnectionInformation.Host}:{this.ConnectionInformation.Port}]");

        _ = Task.Run(async () => await this.HandleClientNetworkStreams(), this.CancellationTokenSource.Token);

        while (!this.CancellationTokenSource.IsCancellationRequested && this.tcpListener.Server.IsBound)
        {
            var tcpClient = await this.tcpListener.AcceptTcpClientAsync();
            
            var networkStream = tcpClient.GetStream();

            this.tcpClients.Add(Guid.NewGuid(), (tcpClient,networkStream));

            this.Logger.LogInformation($"{this.GetLoggingPrefix()} - Client connected from [{tcpClient.Client.RemoteEndPoint}]");
        }
    }

    private async Task HandleClientNetworkStreams()
    {
        while (!this.CancellationTokenSource.IsCancellationRequested && this.tcpListener.Server.IsBound && this.tcpClients.Any())
        {
            var buffer = new byte[4096];

            foreach (var (client, stream) in this.tcpClients.Values)
            {
                var bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);

                if (bytesRead <= 0) continue;

                this.Logger.LogInformation($"{this.GetLoggingPrefix()} - Data received from [{client.Client.RemoteEndPoint}]");

                var receivedData = buffer.Take(bytesRead).ToArray();
                await this.OnDataReceived(receivedData);
            }
        }
    }

    protected override async Task DisconnectInternalAsync()
    {
        foreach (var client in this.tcpClients)
        {
            client.Value.Item2.Close();
            await client.Value.Item2.DisposeAsync();

            client.Value.Item1.Close();
            client.Value.Item1.Dispose();
        }

        this.tcpClients.Clear();

        if (this.networkStream != null)
        {
            this.networkStream.Close();
            await this.networkStream.DisposeAsync();
        }

        if (this.tcpClient != null)
        {
            this.tcpClient.Close();
            this.tcpClient = null;
        }
    }

    protected override async Task SendCommandInternalAsync(INetworkCommand networkCommand)
    {
        switch (this.ConnectionInformation.Mode)
        {
            case ConnectionInformationMode.Client:
                await this.SendCommandToServer(networkCommand);
                break;
            case ConnectionInformationMode.Server:
                await this.SendCommandToClients(networkCommand);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private async Task SendCommandToServer(INetworkCommand networkCommand)
    {
        await this.networkStream.WriteAsync(networkCommand.Data, 0, networkCommand.Data.Length);
        await this.networkStream.FlushAsync();
    }

    private async Task SendCommandToClients(INetworkCommand networkCommand)
    {
        foreach (var client in this.tcpClients)
        {
            await client.Value.Item2.WriteAsync(networkCommand.Data, 0, networkCommand.Data.Length);
            await client.Value.Item2.FlushAsync();
        }
    }

    private async Task ListenForDataAsync()
    {
        var buffer = new byte[4096];

        while (this.tcpClient.Connected && !this.CancellationTokenSource.IsCancellationRequested)
        {
            try
            {
                var bytesRead = await this.networkStream.ReadAsync(buffer, 0, buffer.Length);

                if (bytesRead <= 0) continue;

                this.Logger.LogInformation(
                    $"{this.GetLoggingPrefix()} - Data received from [{this.ConnectionInformation.Host}:{this.ConnectionInformation.Port}]");

                var receivedData = buffer.Take(bytesRead).ToArray();
                await this.OnDataReceived(receivedData);
            }
            catch (Exception ex)
            {
                this.Logger.LogError(ex, "Error while listening for data.");
                break;
            }
        }
    }
}