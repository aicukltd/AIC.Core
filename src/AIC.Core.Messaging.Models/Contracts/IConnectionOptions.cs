namespace AIC.Core.Messaging.Models.Contracts;

public interface IConnectionOptions
{
    string Protocol { get; set; }
    string Host { get; set; }
    int Port { get; set; }
    string Url => $"{this.Protocol}://{this.Host}:{this.Port}";
}