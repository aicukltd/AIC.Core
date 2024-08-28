namespace AIC.Core.Data.Services.Implementations;

using System.Linq.Expressions;
using AIC.Core.Data.Contracts;
using AIC.Core.Data.Services.Contracts;

public abstract class BaseDataService<T, TId> : IDataService<T, TId>
    where TId : struct
    where T : class, IBaseEntity<TId>, IHasId<TId>
{
    protected readonly IRepository<T, TId> Repository;

    protected BaseDataService(IRepository<T, TId> repository)
    {
        this.Repository = repository;
    }

    public virtual async Task<T> GetByIdAsync(TId id)
    {
        return await this.Repository.GetModelAsync(id);
    }

    public virtual async Task<T> GetByPredicateAsync(Expression<Func<T, bool>>? predicate = null)
    {
        if (predicate == null) throw new ArgumentNullException(nameof(predicate));

        return await this.Repository.GetModelAsync(predicate);
    }

    public virtual async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>>? predicate = null)
    {
        return predicate == null
            ? await this.Repository.GetModelsAsync()
            : await this.Repository.GetModelsAsync(predicate);
    }

    public virtual async Task<T> CreateOrUpdateAsync(T entity)
    {
        return await this.Repository.CreateOrUpdateAsync(entity);
    }

    public async Task<T> CreateOrUpdateAsync(T entity, Expression<Func<T, bool>>? predicate = null)
    {
        return await this.Repository.CreateOrUpdateAsync(entity, predicate);
    }

    public virtual async Task<bool> DeleteAsync(T entity)
    {
        return await this.Repository.DeleteAsync(entity);
    }

    public virtual async Task<bool> DeleteByIdAsync(TId id)
    {
        return await this.Repository.DeleteAsync(id);
    }

    public virtual async Task<long> CountAsync(Expression<Func<T, bool>>? predicate = null)
    {
        return predicate == null
            ? await this.Repository.CountAsync()
            : await this.Repository.CountAsync(predicate);
    }

    protected T ReturnDefaultIfNull(T entity)
    {
        return entity == null ? default : entity;
    }

    protected void ThrowIfNull(T entity)
    {
        if (entity == null) throw new ArgumentNullException(nameof(entity));
    }
}