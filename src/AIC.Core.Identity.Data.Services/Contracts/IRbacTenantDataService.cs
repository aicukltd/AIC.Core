namespace AIC.Core.Identity.Data.Services.Contracts;

using System.Linq.Expressions;
using AIC.Core.Data.Contracts;
using AIC.Core.Identity.Models.Contracts;
using AIC.Core.Identity.Models.Implementations;
using AIC.Core.Identity.Tenants.Models.Contracts;

public interface IRbacTenantDataService<T, in TId> : IRbacDataService<T, TId>
    where T : class, IBaseEntity<TId>, IHasId<TId> where TId : struct
{
    Task<T> GetByIdAsync(TId id, IUser user, ITenant tenant);
    Task<T> GetByPredicateAsync(Expression<Func<T, bool>>? predicate, IUser user, ITenant tenant);
    Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>>? predicate, IUser user, ITenant tenant);
    Task<T> CreateOrUpdateAsync(T entity, IUser user, ITenant tenant);
    Task<T> CreateOrUpdateAsync(T entity, Expression<Func<T, bool>>? predicate, IUser user, ITenant tenant);
    Task<bool> DeleteAsync(T entity, IUser user, ITenant tenant);
    Task<bool> DeleteByIdAsync(TId id, IUser user, ITenant tenant);
    Task<long> CountAsync(Expression<Func<T, bool>>? predicate, IUser user, ITenant tenant);

    void ThrowIfNotAuthorised(IUser user, ITenant tenant,
        IdentityRole minimumIdentityRole = IdentityRole.Administrator);
}