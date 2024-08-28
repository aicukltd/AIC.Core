namespace AIC.Core.Messaging.Services.Contracts;

using AIC.Core.Messaging.Models.Contracts;

public interface IPubSubMessagingService<TMessage, TPayload> where TMessage : IMessage<TPayload>
{
    Task Publish(TMessage message);
    Task Subscribe(Func<TMessage, Task> messageReceived);
}

public interface IPubSubMessagingService<TMessage> : IPubSubMessagingService<TMessage, string> where TMessage : IMessage
{
}