namespace AIC.Core.Identity.Subscriptions.Models.References.Contracts;

public interface IHasSubscriptionId
{
    Guid SubscriptionId { get; set; }
}