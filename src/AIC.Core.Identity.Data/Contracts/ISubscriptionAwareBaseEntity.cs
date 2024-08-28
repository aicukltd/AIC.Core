namespace AIC.Core.Identity.Data.Contracts;

using AIC.Core.Data.Contracts;
using AIC.Core.Identity.Subscriptions.Models.References.Contracts;

public interface ISubscriptionAwareBaseEntity<TId> : IBaseEntity<TId>, IHasSubscriptionId where TId : struct
{
}

public interface ISubscriptionAwareBaseEntity : IBaseEntity, IHasSubscriptionId
{
}