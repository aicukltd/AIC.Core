namespace AIC.Core.Messaging.Services.InMemory.Implementations;

using System.Collections.Concurrent;
using AIC.Core.Messaging.Models.Contracts;
using AIC.Core.Messaging.Services.Implementations;
using AIC.Core.Messaging.Services.InMemory.Contracts;

public abstract class
    BaseInMemoryPubSubMessagingService<TMessage> : BaseInMemoryPubSubMessagingService<TMessage, string>
    where TMessage : IMessage
{
}

public abstract class
    BaseInMemoryPubSubMessagingService<TMessage, TPayload> : BasePubSubMessagingService<TMessage, TPayload>,
    IInMemoryPubSubMessagingService<TMessage, TPayload> where TMessage : IMessage<TPayload>
{
    protected BaseInMemoryPubSubMessagingService()
    {
        this.Queue = new ConcurrentQueue<TMessage>();
    }

    private ConcurrentQueue<TMessage> Queue { get; }

    public override async Task Publish(TMessage message)
    {
        this.Queue.Enqueue(message);
        await this.OnMessagePublished(message);
    }

    public override async Task Subscribe(Func<TMessage, Task> messageReceived)
    {
        this.MessagePublished += async message =>
        {
            this.Queue.TryDequeue(out message);
            await messageReceived(message);
        };
    }
}