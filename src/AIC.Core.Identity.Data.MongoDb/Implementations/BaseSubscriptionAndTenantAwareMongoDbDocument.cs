namespace AIC.Core.Identity.Data.MongoDb.Implementations;

using AIC.Core.Data.MongoDb.Implementations;
using AIC.Core.Identity.Data.Contracts;

public abstract class BaseSubscriptionAndTenantAwareMongoDbDocument<TId> : BaseMongoDbDocument<TId>,
    ISubscriptionAndTenantAwareBaseEntity<TId> where TId : struct, IEquatable<TId>
{
    public Guid SubscriptionId { get; set; }
    public Guid TenantId { get; set; }
}

public abstract class
    BaseSubscriptionAndTenantAwareMongoDbDocument : BaseSubscriptionAndTenantAwareMongoDbDocument<Guid>
{
}