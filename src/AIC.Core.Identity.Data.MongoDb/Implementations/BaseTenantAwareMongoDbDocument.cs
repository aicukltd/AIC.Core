namespace AIC.Core.Identity.Data.MongoDb.Implementations;

using AIC.Core.Data.MongoDb.Implementations;
using AIC.Core.Identity.Data.Contracts;

public abstract class BaseTenantAwareMongoDbDocument<TId> : BaseMongoDbDocument<TId>, ITenantAwareBaseEntity<TId>
    where TId : struct, IEquatable<TId>
{
    public Guid TenantId { get; set; }
}

public abstract class BaseTenantAwareMongoDbDocument : BaseTenantAwareMongoDbDocument<Guid>
{
}