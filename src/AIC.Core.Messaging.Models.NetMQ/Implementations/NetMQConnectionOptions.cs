namespace AIC.Core.Messaging.Models.NetMQ.Implementations;

using AIC.Core.Messaging.Models.NetMQ.Contracts;

public class NetMqConnectionOptions : INetMqConnectionOptions
{
    public string Protocol { get; set; }
    public string Host { get; set; }
    public int Port { get; set; }
}