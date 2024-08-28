namespace AIC.Core.Identity.Data.Services.Implementations;

using System.Linq.Expressions;
using AIC.Core.Data.Contracts;
using AIC.Core.Identity.Data.Services.Contracts;
using AIC.Core.Identity.Subscriptions.Models.Contracts;
using AIC.Core.Identity.Subscriptions.Models.Implementations;

public abstract class BaseSubscriptionRbacOrganisationDataService<T, TId> : BaseRbacOrganisationDataService<T, TId>,
    ISubscriptionRbacOrganisationDataService<T, TId>
    where TId : struct
    where T : class, IBaseEntity<TId>
{
    protected BaseSubscriptionRbacOrganisationDataService(IRepository<T, TId> repository) : base(
        repository)
    {
    }

    public async Task<bool> IsSubscriptionValidForAction(ISubscription subscription, SubscriptionAction action,
        Expression<Func<T, bool>>? countPredicate = null)
    {
        return true;

        //if (subscription == null) return false;

        //var count = await this.CountAsync(countPredicate);

        //var isValidForAction =
        //    subscription.IsAuthorisedForAction(action, count);

        //if (!isValidForAction)
        //    throw new SecurityException(
        //        $"You have exceeded your allowed subscription quota to perform: {SubscriptionAction.CreateJob.Wordify()}");

        //return isValidForAction;
    }
}