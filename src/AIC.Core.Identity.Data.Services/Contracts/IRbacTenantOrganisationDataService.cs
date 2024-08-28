namespace AIC.Core.Identity.Data.Services.Contracts;

using System.Linq.Expressions;
using AIC.Core.Data.Contracts;
using AIC.Core.Identity.Models.Contracts;
using AIC.Core.Identity.Models.Implementations;
using AIC.Core.Identity.Tenants.Models.Contracts;

public interface IRbacTenantOrganisationDataService<T, in TId> : IRbacOrganisationDataService<T, TId>
    where T : class, IBaseEntity<TId>, IHasId<TId> where TId : struct
{
    Task<T> GetByIdAsync(TId id, IUser user, IOrganisation organisation, ITenant tenant);

    Task<T> GetByPredicateAsync(Expression<Func<T, bool>>? predicate, IUser user, IOrganisation organisation,
        ITenant tenant);

    Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>>? predicate, IUser user, IOrganisation organisation,
        ITenant tenant);

    Task<T> CreateOrUpdateAsync(T entity, IUser user, IOrganisation organisation, ITenant tenant);

    Task<T> CreateOrUpdateAsync(T entity, Expression<Func<T, bool>>? predicate, IUser user, IOrganisation organisation,
        ITenant tenant);

    Task<bool> DeleteAsync(T entity, IUser user, IOrganisation organisation, ITenant tenant);
    Task<bool> DeleteByIdAsync(TId id, IUser user, IOrganisation organisation, ITenant tenant);
    Task<long> CountAsync(Expression<Func<T, bool>>? predicate, IUser user, IOrganisation organisation, ITenant tenant);

    void ThrowIfNotAuthorised(IUser user, IOrganisation organisation, ITenant tenant,
        IdentityRole minimumIdentityRole = IdentityRole.Administrator);
}