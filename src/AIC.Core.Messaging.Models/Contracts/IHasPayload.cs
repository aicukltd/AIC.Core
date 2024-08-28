namespace AIC.Core.Messaging.Models.Contracts;

public interface IHasPayload<TPayload>
{
    TPayload Payload { get; set; }
}