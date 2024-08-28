namespace AIC.Core.Services.Networking.Connections.Provisioning.Implementations;

using AIC.Core.Models.Networking.Connections.Contracts;
using AIC.Core.Models.Networking.Connections.Implementations;
using AIC.Core.Models.Networking.Connections.Serial.Implementations;
using AIC.Core.Models.Networking.Connections.Tcp.Implementations;
using AIC.Core.Models.Networking.Connections.Udp.Implementations;
using AIC.Core.Services.Networking.Connections.Contracts;
using AIC.Core.Services.Networking.Connections.Provisioning.Contracts;
using AIC.Core.Services.Networking.Connections.Serial.Implementations;
using AIC.Core.Services.Networking.Connections.Tcp.Implementations;
using AIC.Core.Services.Networking.Connections.Udp.Implementations;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

public class ConnectionProvisioningService : IConnectionProvisioningService
{
    protected readonly ILogger Logger;
    private readonly IOptions<IConnectionOptions> options;

    public ConnectionProvisioningService(ILogger logger, IOptions<ConnectionOptions> options)
    {
        this.Logger = logger;
        this.options = options;
    }

    public async Task<IConnectionInstance> GetConnectionInstanceAsync(
        ConnectionInformationType connectionInformationType)
    {
        try
        {
            switch (connectionInformationType)
            {
                case ConnectionInformationType.Tcp:
                    return new TcpConnectionInstance
                    {
                        Port = this.options.Value.Port,
                        Host = this.options.Value.Host,
                        Type = connectionInformationType,
                        Mode = this.options.Value.Mode
                    };
                case ConnectionInformationType.Udp:
                    return new UdpConnectionInstance
                    {
                        Port = this.options.Value.Port,
                        Host = this.options.Value.Host,
                        Type = connectionInformationType,
                        Mode = this.options.Value.Mode,
                        UdpMode = this.options.Value.UdpMode
                    };
                case ConnectionInformationType.Serial:
                    return new SerialConnectionInstance
                    {
                        SerialPortName = this.options.Value.SerialPortName,
                        BaudRate = this.options.Value.BaudRate,
                        DataBits = this.options.Value.DataBits,
                        DefaultTimeOut = this.options.Value.DefaultTimeOut,
                        Type = connectionInformationType,
                        Mode = this.options.Value.Mode
                    };
                default:
                    throw new ArgumentOutOfRangeException(nameof(connectionInformationType), connectionInformationType,
                        null);
            }
        }
        catch (Exception exception)
        {
            this.Logger.LogError(exception, exception.Message);

            return default;
        }
    }

    public async Task<IConnectionHandlingService> GetConnectionHandlerAsync(IConnectionInstance connectionInstance)
    {
        try
        {
            IConnectionHandlingService handler;

            switch (connectionInstance.Type)
            {
                case ConnectionInformationType.Tcp:
                    handler = new TcpConnectionHandlingService(this.Logger);
                    handler.SetConnectionInformation(connectionInstance);
                    break;
                case ConnectionInformationType.Udp:
                    handler = new UdpConnectionHandlingService(this.Logger);
                    handler.SetConnectionInformation(connectionInstance);
                    break;
                case ConnectionInformationType.Serial:
                    handler = new SerialConnectionHandlingService(this.Logger);
                    handler.SetConnectionInformation(connectionInstance);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return handler;
        }
        catch (Exception exception)
        {
            this.Logger.LogError(exception, exception.Message);

            return default;
        }
    }
}