namespace AIC.Core.Models.Networking.Connections.Implementations;

using AIC.Core.Models.Networking.Connections.Contracts;

public abstract class BaseConnectionInstance : IConnectionInstance
{
    protected BaseConnectionInstance()
    {
        this.Id = Guid.NewGuid();
    }

    public Guid Id { get; set; }
    public string Host { get; set; }
    public int Port { get; set; }
    public ConnectionInformationType Type { get; set; }
    public ConnectionInformationMode Mode { get; set; }
    public UdpConnectionInformationMode UdpMode { get; set; }
    public string SerialPortName { get; set; }
    public int BaudRate { get; set; }
    public int DataBits { get; set; }
    public int DefaultTimeOut { get; set; }
}