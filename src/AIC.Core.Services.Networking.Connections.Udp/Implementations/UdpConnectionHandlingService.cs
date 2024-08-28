namespace AIC.Core.Services.Networking.Connections.Udp.Implementations;

using System.Net;
using System.Net.Sockets;
using AIC.Core.Models.Networking.Connections.Contracts;
using AIC.Core.Models.Networking.Contracts;
using AIC.Core.Services.Networking.Connections.Implementations;
using AIC.Core.Services.Networking.Connections.Udp.Contracts;
using Microsoft.Extensions.Logging;

public sealed class UdpConnectionHandlingService : BaseConnectionHandlingService, IUdpConnectionHandlingService
{
    private IPEndPoint remoteEndPoint;
    private UdpClient udpClient;

    public UdpConnectionHandlingService(ILogger logger) : base(logger)
    {
    }

    protected override async Task ConnectInternalAsync()
    {
        this.remoteEndPoint = new IPEndPoint(IPAddress.Parse(this.ConnectionInformation.Host),
            this.ConnectionInformation.Port);
        this.udpClient = new UdpClient(this.remoteEndPoint);

        _ = Task.Run(this.ListenForDataAsync, base.CancellationTokenSource.Token);
    }

    protected override async Task DisconnectInternalAsync()
    {
        this.udpClient.Close();
        this.udpClient = null;
        this.remoteEndPoint = null;
    }

    protected override async Task SendCommandInternalAsync(INetworkCommand networkCommand)
    {
        if (this.udpClient == null || this.remoteEndPoint == null)
            throw new InvalidOperationException("UDP client is not initialized or connected.");

        if (this.ConnectionInformation.Mode == ConnectionInformationMode.Client)
        {
            if (this.remoteEndPoint == null)
                throw new InvalidOperationException("Remote endpoint is not initialized.");

            await this.udpClient.SendAsync(networkCommand.Data, networkCommand.Data.Length, this.remoteEndPoint);
        }
        else
        {
            if (this.ConnectionInformation.UdpMode == UdpConnectionInformationMode.Unicast)
            {
                throw new InvalidOperationException("Server mode requires multicast address for sending data.");
            }

            var multicastAddress = IPAddress.Parse(this.ConnectionInformation.Host);
            this.remoteEndPoint = new IPEndPoint(multicastAddress, this.ConnectionInformation.Port);

            await this.udpClient.SendAsync(networkCommand.Data, networkCommand.Data.Length, this.remoteEndPoint);

        }
    }

    private async Task ListenForDataAsync()
    {
        while (this.udpClient != null && !base.CancellationTokenSource.IsCancellationRequested)
        {
            try
            {
                var receivedResult = await this.udpClient.ReceiveAsync();
                var receivedData = receivedResult.Buffer;

                this.Logger.LogInformation($"Received data from {this.remoteEndPoint}");

                await this.OnDataReceived(receivedData);
            }
            catch (Exception ex)
            {
                this.Logger.LogError(ex, "Error while listening for data over UDP.");
                break;
            }
        }
    }
}