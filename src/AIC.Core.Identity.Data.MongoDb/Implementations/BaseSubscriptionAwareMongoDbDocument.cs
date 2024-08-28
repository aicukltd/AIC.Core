namespace AIC.Core.Identity.Data.MongoDb.Implementations;

using AIC.Core.Data.MongoDb.Implementations;
using AIC.Core.Identity.Data.Contracts;

public abstract class BaseSubscriptionAwareMongoDbDocument<TId> : BaseMongoDbDocument<TId>,
    ISubscriptionAwareBaseEntity<TId> where TId : struct, IEquatable<TId>
{
    public Guid SubscriptionId { get; set; }
}

public abstract class BaseSubscriptionAwareMongoDbDocument : BaseSubscriptionAwareMongoDbDocument<Guid>
{
}