namespace AIC.Core.Messaging.Services.InMemory.Contracts;

using AIC.Core.Messaging.Models.Contracts;
using AIC.Core.Messaging.Services.Contracts;

public interface IInMemoryPubSubMessagingService<TMessage> : IInMemoryPubSubMessagingService<TMessage, string>
    where TMessage : IMessage
{
}

public interface IInMemoryPubSubMessagingService<TMessage, TPayload> : IPubSubMessagingService<TMessage, TPayload>
    where TMessage : IMessage<TPayload>
{
}