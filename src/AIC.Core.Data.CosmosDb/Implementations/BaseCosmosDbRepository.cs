namespace AIC.Core.Data.CosmosDb.Implementations;

using System.Linq.Expressions;
using AIC.Core.Data.Contracts;
using Microsoft.Azure.CosmosRepository;

public abstract class BaseCosmosDbRepository<TModel> : IRepository<TModel, Guid> where TModel : class, IItem
{
    private readonly IRepository<TModel> cosmosRepository;

    public BaseCosmosDbRepository(IRepository<TModel> cosmosRepository)
    {
        this.cosmosRepository = cosmosRepository;
    }

    public async Task<TModel> CreateOrUpdateAsync(TModel entity)
    {
        var exists = await this.GetModelAsync(Guid.Parse(entity.Id));

        return exists ?? await this.cosmosRepository.CreateAsync(entity);
    }

    public async Task<IEnumerable<TModel>> CreateOrUpdateAsync(IEnumerable<TModel> entities)
    {
        var results = new List<TModel>();

        foreach (var entity in entities) results.Add(await this.CreateOrUpdateAsync(entity));

        return results;
    }

    public async Task<TModel> CreateOrUpdateAsync(TModel entity, Expression<Func<TModel, bool>> existsPredicate)
    {
        var exists = await this.GetModelAsync(existsPredicate);

        return exists ?? await this.cosmosRepository.CreateAsync(entity);
    }

    public async Task<IEnumerable<TModel>> CreateOrUpdateAsync(IEnumerable<TModel> entities,
        Expression<Func<TModel, bool>> existsPredicate)
    {
        var results = new List<TModel>();

        foreach (var entity in entities) results.Add(await this.CreateOrUpdateAsync(entity, existsPredicate));

        return results;
    }

    public async Task<IEnumerable<TModel>> GetModelsAsync()
    {
        return await this.cosmosRepository.GetByQueryAsync(string.Empty);
    }

    public async Task<TModel> GetModelAsync(Guid id)
    {
        return await this.cosmosRepository.GetAsync(id.ToString());
    }

    public bool SkipModelRelationships { get; set; }

    public async Task<TModel> GetModelAsync(Expression<Func<TModel, bool>> findPredicate,
        params Expression<Func<TModel, object>>[] includes)
    {
        return (await this.cosmosRepository.GetAsync(findPredicate)).FirstOrDefault();
    }

    public async Task<IEnumerable<TModel>> GetModelsAsync(params Expression<Func<TModel, object>>[] includes)
    {
        return await this.GetModelsAsync();
    }

    public async Task<IEnumerable<TModel>> GetModelsAsync(Expression<Func<TModel, bool>> findPredicate,
        params Expression<Func<TModel, object>>[] includes)
    {
        return await this.cosmosRepository.GetAsync(findPredicate);
    }

    public async Task<IEnumerable<TModel>> GetModelsAsync<TOrderBy>(int page, int size,
        Expression<Func<TModel, TOrderBy>> orderBy, bool descending = false)
    {
        return (await this.cosmosRepository.PageAsync(pageNumber: page, pageSize: size)).Items;
    }

    public async Task<IEnumerable<TModel>> GetModelsAsync<TOrderBy>(int page, int size,
        Expression<Func<TModel, TOrderBy>> orderBy, bool descending = false,
        params Expression<Func<TModel, object>>[] includes)
    {
        return await this.GetModelsAsync(page, size, orderBy);
    }

    public async Task<IEnumerable<TModel>> GetModelsAsync<TOrderBy>(int page, int size,
        Expression<Func<TModel, bool>> findPredicate, Expression<Func<TModel, TOrderBy>> orderBy,
        bool descending = false,
        params Expression<Func<TModel, object>>[] includes)
    {
        return (await this.cosmosRepository.PageAsync(pageNumber: page, pageSize: size, predicate: findPredicate))
            .Items;
    }

    public async Task BuildModelRelationshipsAsync(TModel model)
    {
        throw new NotImplementedException();
    }

    public async Task<TResponse> SearchAsync<TQuery, TResponse>(TQuery query)
        where TQuery : class where TResponse : class, new()
    {
        throw new NotImplementedException();
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        await this.cosmosRepository.DeleteAsync(id.ToString());

        return true;
    }

    public async Task<int> CountAsync()
    {
        return await this.cosmosRepository.CountAsync();
    }

    public async Task<int> CountAsync(Expression<Func<TModel, bool>> findPredicate)
    {
        return await this.cosmosRepository.CountAsync(findPredicate);
    }

    public async Task<bool> DeleteAsync(TModel entity)
    {
        await this.cosmosRepository.DeleteAsync(entity);

        return true;
    }

    public async Task<bool> DeleteAsync(Expression<Func<TModel, bool>> findPredicate)
    {
        var model = await this.GetModelAsync(findPredicate);

        return await this.DeleteAsync(model);
    }
}