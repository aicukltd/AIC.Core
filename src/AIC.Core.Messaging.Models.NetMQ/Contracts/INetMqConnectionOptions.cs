namespace AIC.Core.Messaging.Models.NetMQ.Contracts;

using AIC.Core.Messaging.Models.Contracts;

public interface INetMqConnectionOptions : IConnectionOptions
{
    string ServerUrl => $"@{this.Url}";
    string ClientUrl => $">{this.Url}";
}