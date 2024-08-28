namespace AIC.Core.Identity.Data.Contracts;

using AIC.Core.Data.Contracts;
using AIC.Core.Identity.Subscriptions.Models.References.Contracts;
using AIC.Core.Identity.Tenants.Models.References.Contracts;

public interface ISubscriptionAndTenantAwareBaseEntity<TId> : IBaseEntity<TId>, IHasSubscriptionId, IHasTenantId
    where TId : struct
{
}

public interface ISubscriptionAndTenantAwareBaseEntity : IBaseEntity, IHasSubscriptionId, IHasTenantId
{
}