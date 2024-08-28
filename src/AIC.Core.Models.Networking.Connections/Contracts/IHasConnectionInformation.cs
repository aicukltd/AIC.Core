namespace AIC.Core.Models.Networking.Connections.Contracts;

using AIC.Core.Data.Contracts;

public interface IHasConnectionInformation : IHasId<Guid>
{
    string Host { get; set; }
    int Port { get; set; }
    ConnectionInformationType Type { get; set; }
    ConnectionInformationMode Mode { get; set; }
    UdpConnectionInformationMode UdpMode { get; set; }
    string SerialPortName { get; set; }
    int BaudRate { get; set; }
    int DataBits { get; set; }
    int DefaultTimeOut { get; set; }
}