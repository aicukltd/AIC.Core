namespace AIC.Core.Messaging.Framing.Json.Implementations;

using AIC.Core.Messaging.Framing.Contracts;
using AIC.Core.Messaging.Models.Contracts;
using Newtonsoft.Json;

public abstract class BaseJsonMessageFramer<TMessage, TPayload> : IMessageFramer<TMessage, TPayload>
    where TMessage : IMessage<TPayload>
{
    public virtual async Task<string> FrameAsync(TMessage message)
    {
        return JsonConvert.SerializeObject(message);
    }

    public virtual async Task<TMessage> DeFrameAsync(string message)
    {
        return JsonConvert.DeserializeObject<TMessage>(message);
    }
}

public abstract class BaseJsonMessageFramer<TMessage> : BaseJsonMessageFramer<TMessage, string>
    where TMessage : IMessage
{
}