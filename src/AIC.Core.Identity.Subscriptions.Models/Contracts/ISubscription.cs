namespace AIC.Core.Identity.Subscriptions.Models.Contracts;

using AIC.Core.Data.Contracts;
using AIC.Core.Identity.Subscriptions.Models.Implementations;

public interface ISubscription : IBaseEntity
{
    SubscriptionType Type { get; set; }
    SubscriptionStatus Status { get; set; }
    DateTime ValidFrom { get; set; }
    DateTime ValidTo { get; set; }
    ISubscriptionFeatures Features { get; set; }
    string ExternalId { get; set; }
}