namespace AIC.Core.Messaging.Services.NetMQ.Contracts;

using AIC.Core.Messaging.Models.Contracts;

public interface INetMqPubSubMessagingService<TMessage, TPayload> : IAsyncDisposable where TMessage : IMessage<TPayload>
{
    bool IsConnected { get; }
    string Topic { get; }
    Task ConnectAsync();
    Task DisconnectAsync();
}

public interface INetMqPubSubMessagingService<TMessage> : INetMqPubSubMessagingService<TMessage, string>
    where TMessage : IMessage
{
}