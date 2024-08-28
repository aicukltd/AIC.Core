namespace AIC.Core.Identity.Data.Services.Contracts;

using System.Linq.Expressions;
using AIC.Core.Data.Contracts;
using AIC.Core.Identity.Subscriptions.Models.Contracts;
using AIC.Core.Identity.Subscriptions.Models.Implementations;

public interface ISubscriptionRbacOrganisationDataService<T, in TId> : IRbacOrganisationDataService<T, TId>
    where T : class, IBaseEntity<TId>, IHasId<TId> where TId : struct
{
    Task<bool> IsSubscriptionValidForAction(ISubscription subscription, SubscriptionAction action,
        Expression<Func<T, bool>>? countPredicate = null);
}