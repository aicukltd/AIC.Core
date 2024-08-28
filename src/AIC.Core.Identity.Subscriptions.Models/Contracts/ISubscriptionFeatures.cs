namespace AIC.Core.Identity.Subscriptions.Models.Contracts;

public interface ISubscriptionFeatures
{
    IDictionary<string, bool> Privileges { get; }
    IDictionary<string, long> Quotas { get; }
}