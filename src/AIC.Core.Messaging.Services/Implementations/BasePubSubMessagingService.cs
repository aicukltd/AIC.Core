namespace AIC.Core.Messaging.Services.Implementations;

using AIC.Core.Messaging.Delegates;
using AIC.Core.Messaging.Models.Contracts;
using AIC.Core.Messaging.Services.Contracts;

public abstract class BasePubSubMessagingService<TMessage> : BasePubSubMessagingService<TMessage, string>
    where TMessage : IMessage
{
    protected new event MessagePublished<TMessage>? MessagePublished;
    protected new event MessageReceived<TMessage>? MessageReceived;

    protected new virtual async Task OnMessageReceived(TMessage message)
    {
        if (this.MessageReceived != null)
            await this.MessageReceived?.Invoke(message);
    }

    protected new virtual async Task OnMessagePublished(TMessage message)
    {
        if (this.MessagePublished != null)
            await this.MessagePublished?.Invoke(message);
    }
}

public abstract class BasePubSubMessagingService<TMessage, TPayload> : IPubSubMessagingService<TMessage, TPayload>
    where TMessage : IMessage<TPayload>
{
    public abstract Task Publish(TMessage message);
    public abstract Task Subscribe(Func<TMessage, Task> messageReceived);

    protected event MessagePublished<TMessage, TPayload>? MessagePublished;
    protected event MessageReceived<TMessage, TPayload>? MessageReceived;

    protected virtual async Task OnMessageReceived(TMessage message)
    {
        if (this.MessageReceived != null)
            await this.MessageReceived?.Invoke(message);
    }

    protected virtual async Task OnMessagePublished(TMessage message)
    {
        if (this.MessagePublished != null)
            await this.MessagePublished?.Invoke(message);
    }
}