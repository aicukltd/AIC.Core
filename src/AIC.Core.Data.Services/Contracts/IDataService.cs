namespace AIC.Core.Data.Services.Contracts;

using System.Linq.Expressions;
using AIC.Core.Data.Contracts;

public interface IDataService<T, in TId>
    where T : class, IBaseEntity<TId>, IHasId<TId> where TId : struct
{
    Task<T> GetByIdAsync(TId id);
    Task<T> GetByPredicateAsync(Expression<Func<T, bool>> predicate);
    Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> predicate);
    Task<T> CreateOrUpdateAsync(T entity);
    Task<T> CreateOrUpdateAsync(T entity, Expression<Func<T, bool>> predicate);
    Task<bool> DeleteAsync(T entity);
    Task<bool> DeleteByIdAsync(TId id);
    Task<long> CountAsync(Expression<Func<T, bool>> predicate);
}