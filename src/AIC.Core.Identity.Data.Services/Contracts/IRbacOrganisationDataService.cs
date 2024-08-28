namespace AIC.Core.Identity.Data.Services.Contracts;

using System.Linq.Expressions;
using AIC.Core.Data.Contracts;
using AIC.Core.Identity.Models.Contracts;
using AIC.Core.Identity.Models.Implementations;

public interface IRbacOrganisationDataService<T, in TId> : IRbacDataService<T, TId>
    where T : class, IBaseEntity<TId>, IHasId<TId> where TId : struct
{
    Task<T> GetByIdAsync(TId id, IUser user, IOrganisation organisation);
    Task<T> GetByPredicateAsync(Expression<Func<T, bool>>? predicate, IUser user, IOrganisation organisation);
    Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>>? predicate, IUser user, IOrganisation organisation);
    Task<T> CreateOrUpdateAsync(T entity, IUser user, IOrganisation organisation);
    Task<T> CreateOrUpdateAsync(T entity, Expression<Func<T, bool>>? predicate, IUser user, IOrganisation organisation);
    Task<bool> DeleteAsync(T entity, IUser user, IOrganisation organisation);
    Task<bool> DeleteByIdAsync(TId id, IUser user, IOrganisation organisation);
    Task<long> CountAsync(Expression<Func<T, bool>>? predicate, IUser user, IOrganisation organisation);

    void ThrowIfNotAuthorised(IUser user, IOrganisation organisation,
        IdentityRole minimumIdentityRole = IdentityRole.Administrator);
}