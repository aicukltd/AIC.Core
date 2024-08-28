namespace AIC.Core.Messaging.Models.Implementations;

using AIC.Core.Messaging.Models.Contracts;

public abstract class BaseMessage : BaseMessage<string>
{
}

public abstract class BaseMessage<TPayload> : IMessage<TPayload>
{
    public Guid Id { get; set; }
    public IMessageMetadata Metadata { get; set; }
    public TPayload Payload { get; set; }
    public DateTime Sent { get; set; }
    public DateTime Delivered { get; set; }
    public DateTime Read { get; set; }
}