namespace AIC.Core.Messaging.Models.Contracts;

using AIC.Core.Data.Contracts;

public interface IMessage<TPayload> : IHasId<Guid>, IHasMetadata<IMessageMetadata>, IHasPayload<TPayload>,
    IHasChronology
{
}

public interface IMessage : IMessage<string>
{
}