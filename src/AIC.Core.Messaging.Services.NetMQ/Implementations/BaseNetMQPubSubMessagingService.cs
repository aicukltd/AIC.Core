namespace AIC.Core.Messaging.Services.NetMQ.Implementations;

using AIC.Core.Messaging.Framing.Contracts;
using AIC.Core.Messaging.Models.Contracts;
using AIC.Core.Messaging.Models.NetMQ.Contracts;
using AIC.Core.Messaging.Services.Implementations;
using AIC.Core.Messaging.Services.NetMQ.Contracts;
using global::NetMQ;
using global::NetMQ.Sockets;

public abstract class BaseNetMqPubSubMessagingService<TMessage> : BaseNetMqPubSubMessagingService<TMessage, string>
    where TMessage : IMessage
{
    protected BaseNetMqPubSubMessagingService(INetMqConnectionOptions options, IMessageFramer<TMessage, string> framer)
        : base(options, framer)
    {
    }
}

public abstract class
    BaseNetMqPubSubMessagingService<TMessage, TPayload> : BasePubSubMessagingService<TMessage, TPayload>,
    INetMqPubSubMessagingService<TMessage, TPayload> where TMessage : IMessage<TPayload>
{
    private readonly IMessageFramer<TMessage, TPayload> framer;
    private readonly INetMqConnectionOptions options;

    protected BaseNetMqPubSubMessagingService(INetMqConnectionOptions options,
        IMessageFramer<TMessage, TPayload> framer)
    {
        this.options = options;
        this.framer = framer;
    }

    protected PublisherSocket PublisherSocket { get; set; }
    protected SubscriberSocket SubscriberSocket { get; set; }

    public bool IsConnected { get; private set; }

    public string Topic => typeof(TMessage).Name;

    public async Task ConnectAsync()
    {
        this.PublisherSocket = new PublisherSocket(this.options.Url);
        this.SubscriberSocket = new SubscriberSocket(this.options.Url);

        this.IsConnected = true;
    }

    public async Task DisconnectAsync()
    {
        this.PublisherSocket.Close();
        this.SubscriberSocket.Close();

        this.IsConnected = false;
    }

    public async ValueTask DisposeAsync()
    {
        await this.DisconnectAsync();

        this.PublisherSocket.Dispose();
        this.SubscriberSocket.Dispose();
    }

    public override async Task Publish(TMessage message)
    {
        if (message == null) throw new ArgumentNullException(nameof(message));

        if (!this.IsConnected) await this.ConnectAsync();

        if (this.PublisherSocket.IsDisposed) throw new InvalidOperationException("The request socket is disposed.");

        //TODO: need to introduce message framer and de-framer
        var serialisedMessage = await this.framer.FrameAsync(message);

        //TODO: need to confirm this is correct
        this.PublisherSocket.SendMoreFrame(this.Topic).SendFrame(serialisedMessage);

        await this.OnMessagePublished(message);
    }

    public override async Task Subscribe(Func<TMessage, Task> messageReceived)
    {
        if (messageReceived == null) throw new ArgumentNullException(nameof(messageReceived));

        if (!this.IsConnected) await this.ConnectAsync();

        if (this.SubscriberSocket.IsDisposed) throw new InvalidOperationException("The response socket is disposed.");

        using var runtime = new NetMQRuntime();

        runtime.Run(Task.Factory.StartNew(async () =>
        {
            this.SubscriberSocket.Subscribe(this.Topic);

            var serialisedMessage = await this.SubscriberSocket.ReceiveFrameStringAsync();

            //TODO: need to introduce message framer and de-framer
            var message = await this.framer.DeFrameAsync(serialisedMessage.Item1);

            await messageReceived(message);

            await this.OnMessageReceived(message);
        }));
    }
}