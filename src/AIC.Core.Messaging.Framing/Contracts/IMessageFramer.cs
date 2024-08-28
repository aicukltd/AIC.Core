namespace AIC.Core.Messaging.Framing.Contracts;

using AIC.Core.Messaging.Models.Contracts;

public interface IMessageFramer<TMessage> where TMessage : IMessage
{
    Task<string> FrameAsync(TMessage message);
    Task<TMessage> DeFrameAsync(string message);
}

public interface IMessageFramer<TMessage, TPayload> where TMessage : IMessage<TPayload>
{
    Task<string> FrameAsync(TMessage message);
    Task<TMessage> DeFrameAsync(string message);
}