namespace AIC.Core.Identity.Subscriptions.Models.MongoDb.Implementations;

using AIC.Core.Identity.Subscriptions.Models.Contracts;

public class SubscriptionFeatures : ISubscriptionFeatures
{
    /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
    public SubscriptionFeatures()
    {
        this.Privileges = new Dictionary<string, bool>();
        this.Quotas = new Dictionary<string, long>();
    }

    public IDictionary<string, bool> Privileges { get; }
    public IDictionary<string, long> Quotas { get; }
}