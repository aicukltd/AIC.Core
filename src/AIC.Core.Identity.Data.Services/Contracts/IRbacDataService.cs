namespace AIC.Core.Identity.Data.Services.Contracts;

using System.Linq.Expressions;
using AIC.Core.Data.Contracts;
using AIC.Core.Data.Services.Contracts;
using AIC.Core.Identity.Models.Contracts;
using AIC.Core.Identity.Models.Implementations;

public interface IRbacDataService<T, in TId> : IDataService<T, TId>
    where T : class, IBaseEntity<TId>, IHasId<TId> where TId : struct
{
    Task<T> GetByIdAsync(TId id, IUser user);
    Task<T> GetByPredicateAsync(Expression<Func<T, bool>> predicate, IUser user);
    Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> predicate, IUser user);
    Task<T> CreateOrUpdateAsync(T entity, IUser user);
    Task<T> CreateOrUpdateAsync(T entity, Expression<Func<T, bool>> predicate, IUser user);
    Task<bool> DeleteAsync(T entity, IUser user);
    Task<bool> DeleteByIdAsync(TId id, IUser user);
    Task<long> CountAsync(Expression<Func<T, bool>> predicate, IUser user);
    void ThrowIfNotAuthorised(IUser user, IdentityRole minimumIdentityRole = IdentityRole.Administrator);
}