namespace AIC.Core.Identity.Subscriptions.Models.MongoDb.Implementations;

using AIC.Core.Data.MongoDb.Implementations;
using AIC.Core.Identity.Subscriptions.Models.Contracts;
using AIC.Core.Identity.Subscriptions.Models.Implementations;

public class Subscription : BaseMongoDbDocument, ISubscription
{
    public SubscriptionType Type { get; set; }
    public SubscriptionStatus Status { get; set; }
    public DateTime ValidFrom { get; set; }
    public DateTime ValidTo { get; set; }
    public ISubscriptionFeatures Features { get; set; }
    public string ExternalId { get; set; }
}