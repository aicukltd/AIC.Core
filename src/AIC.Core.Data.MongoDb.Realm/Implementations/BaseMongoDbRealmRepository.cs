namespace AIC.Core.Data.MongoDb.Realm.Implementations;

using System.Linq.Expressions;
using AIC.Core.Data.MongoDb.Contracts;
using AIC.Core.Data.MongoDb.Realm.Contracts;
using MongoDB.Driver;
using MongoDB.Driver.GeoJsonObjectModel;
using Realms;

public abstract class BaseMongoDbRealmRepository<TModel> : BaseMongoDbRealmRepository<TModel, Guid>
    where TModel : class, IBaseRealmObject<Guid>
{
}

public abstract class BaseMongoDbRealmRepository<TModel, TId> : IMongoDbRepository<TModel, TId>
    where TModel : class, IBaseRealmObject<TId> where TId : struct, IEquatable<TId>
{
    /// <summary>
    ///     Creates the or update.
    /// </summary>
    /// <param name="entity">The entity.</param>
    /// <returns></returns>
    public async Task<TModel> CreateOrUpdateAsync(TModel entity)
    {
        var realm = await Realm.GetInstanceAsync();

        await realm.WriteAsync(() => { realm.Add(entity, !entity.Id.Equals(default)); });

        return entity;
    }

    /// <summary>
    ///     Creates the or update.
    /// </summary>
    /// <param name="entities">The entity.</param>
    /// <returns></returns>
    public async Task<IEnumerable<TModel>> CreateOrUpdateAsync(IEnumerable<TModel> entities)
    {
        var realm = await Realm.GetInstanceAsync();

        await realm.WriteAsync(() => { realm.Add(entities); });

        return entities;
    }

    /// <summary>
    ///     CreateAsync Or UpdateAsync based on an existing predicate
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="existsPredicate"></param>
    /// <returns></returns>
    public async Task<TModel> CreateOrUpdateAsync(TModel entity, Expression<Func<TModel, bool>> existsPredicate)
    {
        return await this.CreateOrUpdateAsync(entity);
    }

    /// <summary>
    ///     CreateAsync Or UpdateAsync based on an existing predicate
    /// </summary>
    /// <param name="entities"></param>
    /// <param name="existsPredicate"></param>
    /// <returns></returns>
    public async Task<IEnumerable<TModel>> CreateOrUpdateAsync(IEnumerable<TModel> entities,
        Expression<Func<TModel, bool>> existsPredicate)
    {
        return await this.CreateOrUpdateAsync(entities);
    }

    /// <summary>
    ///     Gets the models.
    /// </summary>
    /// <returns></returns>
    public async Task<IEnumerable<TModel>> GetModelsAsync()
    {
        var realm = await Realm.GetInstanceAsync();

        return realm.All<TModel>().ToList();
    }

    /// <summary>
    ///     Gets the model.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <returns></returns>
    public async Task<TModel> GetModelAsync(TId id)
    {
        var realm = await Realm.GetInstanceAsync();

        return realm.Find<TModel>(id.ToString());
    }

    /// <summary>
    ///     Gets or sets a value indicating whether [skip model relationships].
    /// </summary>
    /// <value>
    ///     <c>true</c> if [skip model relationships]; otherwise, <c>false</c>.
    /// </value>
    public bool SkipModelRelationships { get; set; }

    /// <summary>
    ///     Gets the model.
    /// </summary>
    /// <param name="findPredicate">The find predicate.</param>
    /// <param name="includes"></param>
    /// <returns></returns>
    public async Task<TModel> GetModelAsync(Expression<Func<TModel, bool>> findPredicate,
        params Expression<Func<TModel, object>>[] includes)
    {
        var realm = await Realm.GetInstanceAsync();

        return realm.All<TModel>().FirstOrDefault(findPredicate);
    }

    /// <summary>
    ///     Gets the models asynchronous.
    /// </summary>
    /// <param name="includes">The includes.</param>
    /// <returns></returns>
    public async Task<IEnumerable<TModel>> GetModelsAsync(params Expression<Func<TModel, object>>[] includes)
    {
        var realm = await Realm.GetInstanceAsync();

        return realm.All<TModel>();
    }

    /// <summary>
    ///     Gets the models asynchronous.
    /// </summary>
    /// <param name="findPredicate">The find predicate.</param>
    /// <param name="includes"></param>
    /// <returns></returns>
    public async Task<IEnumerable<TModel>> GetModelsAsync(Expression<Func<TModel, bool>> findPredicate,
        params Expression<Func<TModel, object>>[] includes)
    {
        var realm = await Realm.GetInstanceAsync();

        return realm.All<TModel>().Where(findPredicate).ToList();
    }

    /// <summary>
    ///     Gets the models asynchronous.
    /// </summary>
    /// <typeparam name="TOrderBy">The type of the order by.</typeparam>
    /// <param name="page">The page.</param>
    /// <param name="size">The size.</param>
    /// <param name="orderBy">The order by.</param>
    /// <returns></returns>
    public async Task<IEnumerable<TModel>> GetModelsAsync<TOrderBy>(int page, int size,
        Expression<Func<TModel, TOrderBy>> orderBy, bool descending = false)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    ///     Gets the models asynchronous.
    /// </summary>
    /// <typeparam name="TOrderBy">The type of the order by.</typeparam>
    /// <param name="page">The page.</param>
    /// <param name="size">The size.</param>
    /// <param name="orderBy">The order by.</param>
    /// <param name="includes">The includes.</param>
    /// <returns></returns>
    public async Task<IEnumerable<TModel>> GetModelsAsync<TOrderBy>(int page, int size,
        Expression<Func<TModel, TOrderBy>> orderBy, bool descending = false,
        params Expression<Func<TModel, object>>[] includes)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    ///     Gets the models asynchronous.
    /// </summary>
    /// <typeparam name="TOrderBy">The type of the order by.</typeparam>
    /// <param name="page">The page.</param>
    /// <param name="size">The size.</param>
    /// <param name="findPredicate">The find predicate.</param>
    /// <param name="orderBy">The order by.</param>
    /// <param name="includes">The includes.</param>
    /// <returns></returns>
    public async Task<IEnumerable<TModel>> GetModelsAsync<TOrderBy>(int page, int size,
        Expression<Func<TModel, bool>> findPredicate, Expression<Func<TModel, TOrderBy>> orderBy,
        bool descending = false,
        params Expression<Func<TModel, object>>[] includes)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    ///     Builds the model relationships.
    /// </summary>
    /// <param name="model">The model.</param>
    /// <returns></returns>
    public async Task BuildModelRelationshipsAsync(TModel model)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    ///     Searches the specified query.
    /// </summary>
    /// <typeparam name="TQuery">The type of the query.</typeparam>
    /// <typeparam name="TResponse">The type of the response.</typeparam>
    /// <param name="query">The query.</param>
    /// <returns></returns>
    public async Task<TResponse> SearchAsync<TQuery, TResponse>(TQuery query)
        where TQuery : class where TResponse : class, new()
    {
        throw new NotImplementedException();
    }

    public IMongoCollection<TModel> Collection { get; }

    public async Task<IEnumerable<TModel>> Find(FilterDefinition<TModel> filterDefinition)
    {
        throw new NotImplementedException();
    }

    public async Task<IAsyncCursor<TGeoAwareModel>>
        GeoWithinAsync<TGeoAwareModel, TCoordinates>(GeoJsonPolygon<TCoordinates> polygon)
        where TGeoAwareModel : TModel, IHasGeographicContext<TCoordinates> where TCoordinates : GeoJsonCoordinates
    {
        throw new NotImplementedException();
    }

    public async Task<IAsyncCursor<TGeoAwareModel>> GeoWithinAsync<TGeoAwareModel, TCoordinates>(
        GeoJsonPolygon<TCoordinates> polygon, Expression<Func<TGeoAwareModel, bool>> findPredicate)
        where TGeoAwareModel : TModel, IHasGeographicContext<TCoordinates> where TCoordinates : GeoJsonCoordinates
    {
        throw new NotImplementedException();
    }

    public async Task<IAsyncCursor<TGeoAwareModel>>
        NearAsync<TGeoAwareModel, TCoordinates>(GeoJsonPoint<TCoordinates> point, double radius)
        where TGeoAwareModel : TModel, IHasGeographicContext<TCoordinates> where TCoordinates : GeoJsonCoordinates
    {
        throw new NotImplementedException();
    }

    public async Task<IAsyncCursor<TGeoAwareModel>> NearAsync<TGeoAwareModel, TCoordinates>(
        GeoJsonPoint<TCoordinates> point,
        double radius, Expression<Func<TGeoAwareModel, bool>> findPredicate)
        where TGeoAwareModel : TModel, IHasGeographicContext<TCoordinates> where TCoordinates : GeoJsonCoordinates
    {
        throw new NotImplementedException();
    }

    public async Task CreateGeoIndexAsync(string fieldName)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    ///     Deletes the specified identifier.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <returns></returns>
    public async Task<bool> DeleteAsync(TId id)
    {
        var realm = await Realm.GetInstanceAsync();

        var model = realm.Find<TModel>(id.ToString());

        if (model == null) return false;

        realm.Remove(model);

        return true;
    }

    /// <summary>
    ///     Counts this instance.
    /// </summary>
    /// <returns></returns>
    public async Task<int> CountAsync()
    {
        var realm = await Realm.GetInstanceAsync();

        return realm.All<TModel>().Count();
    }

    /// <summary>
    ///     Counts the specified find predicate.
    /// </summary>
    /// <param name="findPredicate">The find predicate.</param>
    /// <returns>Task&lt;System.Int32&gt;.</returns>
    public async Task<int> CountAsync(Expression<Func<TModel, bool>> findPredicate)
    {
        var realm = await Realm.GetInstanceAsync();

        return realm.All<TModel>().Count(findPredicate);
    }

    /// <summary>
    ///     Deletes the specified identifier.
    /// </summary>
    /// <param name="entity">The entity.</param>
    /// <returns>Task&lt;System.Boolean&gt;.</returns>
    public async Task<bool> DeleteAsync(TModel entity)
    {
        var realm = await Realm.GetInstanceAsync();

        realm.Remove(entity);

        return true;
    }

    /// <summary>
    ///     Deletes the specified identifier.
    /// </summary>
    /// <param name="findPredicate">The find predicate.</param>
    /// <returns>Task&lt;System.Boolean&gt;.</returns>
    public async Task<bool> DeleteAsync(Expression<Func<TModel, bool>> findPredicate)
    {
        throw new NotImplementedException();
    }
}