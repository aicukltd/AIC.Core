namespace AIC.Core.Services.Networking.Connections.Serial.Implementations;

using System.IO.Ports;
using System.Text;
using AIC.Core.Models.Networking.Contracts;
using AIC.Core.Services.Networking.Connections.Implementations;
using AIC.Core.Services.Networking.Connections.Serial.Contracts;
using Microsoft.Extensions.Logging;

public sealed class SerialConnectionHandlingService : BaseConnectionHandlingService, ISerialConnectionHandlingService
{
    private SerialPort serialPort;

    public SerialConnectionHandlingService(ILogger logger) : base(logger)
    {
    }

    protected override async Task ConnectInternalAsync()
    {
        this.serialPort = new SerialPort(this.ConnectionInformation.SerialPortName, this.ConnectionInformation.BaudRate)
        {
            Parity = Parity.None,
            StopBits = StopBits.One,
            DataBits = this.ConnectionInformation.DataBits,
            Handshake = Handshake.None,
            Encoding = Encoding.ASCII,
            ReadTimeout = this.ConnectionInformation.DefaultTimeOut,
            WriteTimeout = this.ConnectionInformation.DefaultTimeOut
        };

        this.serialPort.DataReceived += async (sender, args) => await this.OnSerialPortDataReceived(sender, args);
        this.serialPort.ErrorReceived += this.OnSerialPortErrorReceived;

        this.serialPort.Open();

        _ = Task.Run(this.ListenForDataAsync);
    }

    private void OnSerialPortErrorReceived(object sender, SerialErrorReceivedEventArgs serialErrorReceivedEventArgs)
    {
        this.Logger.LogError($"Error received on Serial port - {serialErrorReceivedEventArgs.EventType}");
    }

    private async Task OnSerialPortDataReceived(object sender, SerialDataReceivedEventArgs serialDataReceivedEventArgs)
    {
        this.Logger.LogInformation($"Data received on Serial port - {serialDataReceivedEventArgs.EventType}");
    }

    protected override async Task DisconnectInternalAsync()
    {
        if (this.serialPort is { IsOpen: true })
        {
            this.serialPort.Close();
            this.serialPort = null;
            this.Logger.LogInformation("Serial port disconnected.");
        }
    }

    protected override async Task SendCommandInternalAsync(INetworkCommand networkCommand)
    {
        if (this.serialPort == null || !this.serialPort.IsOpen)
            throw new InvalidOperationException("Serial port is not connected.");

        this.serialPort.Write(networkCommand.Data, 0, networkCommand.Data.Length);

        this.Logger.LogInformation("Command sent over Serial port.");
    }

    private async Task ListenForDataAsync()
    {
        while (this.serialPort is { IsOpen: true })
        {
            var data = this.serialPort.ReadLine();
            this.Logger.LogInformation($"Data received on Serial port: {data}");
            await this.OnDataReceived(Encoding.ASCII.GetBytes(data));
        }
    }
}