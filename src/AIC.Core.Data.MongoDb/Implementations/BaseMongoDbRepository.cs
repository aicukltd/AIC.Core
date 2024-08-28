namespace AIC.Core.Data.MongoDb.Implementations;

using System.Linq.Expressions;
using AIC.Core.Caching.Contracts;
using AIC.Core.Data.MongoDb.Contracts;
using MongoDB.Driver;
using MongoDB.Driver.GeoJsonObjectModel;
using MongoDbGenericRepository;

public abstract class BaseMongoDbRepository<TModel> : BaseMongoDbRepository<TModel, Guid>, IMongoDbRepository<TModel>
    where TModel : class, IMongoDbDocument
{
    protected BaseMongoDbRepository(IMongoConnectionOptions mongoConnectionOptions) : base(mongoConnectionOptions)
    {
    }
}

public abstract class BaseMongoDbRepository<TModel, TId> : BaseMongoRepository<TId>, IMongoDbRepository<TModel, TId>
    where TModel : class, IMongoDbDocument<TId>
    where TId : struct, IEquatable<TId>

{
    /// <summary>
    ///     The constructor taking a connection string and a database name.
    /// </summary>
    /// <param name="connectionString">The connection string of the MongoDb server.</param>
    /// <param name="databaseName">The name of the database against which you want to perform operations.</param>
    protected BaseMongoDbRepository(IMongoConnectionOptions mongoConnectionOptions) :
        base(mongoConnectionOptions.ConnectionString, mongoConnectionOptions.DatabaseName)
    {
        this.Collection = base.GetCollection<TModel>();
    }

    public ITypedCache<Guid, TModel> Cache { get; }

    public bool SkipModelRelationships
    {
        get => throw new NotImplementedException();
        set => throw new NotImplementedException();
    }

    /// <summary>
    ///     Creates the or update.
    /// </summary>
    /// <param name="entities">The entity.</param>
    /// <returns></returns>
    public virtual async Task<IEnumerable<TModel>> CreateOrUpdateAsync(IEnumerable<TModel> entities)
    {
        var result = new List<TModel>();

        foreach (var entity in entities) result.Add(await this.CreateOrUpdateAsync(entity));

        return result;
    }

    public virtual async Task<TModel> CreateOrUpdateAsync(TModel entity, Expression<Func<TModel, bool>> existsPredicate)
    {
        var exists = await this.ReadAsync(existsPredicate);

        if (exists == null) return await this.CreateAsync(entity);

        entity.Id = exists.Id;

        return await this.UpdateAsync(entity);
    }

    public virtual async Task<IEnumerable<TModel>> CreateOrUpdateAsync(IEnumerable<TModel> entities,
        Expression<Func<TModel, bool>> existsPredicate)
    {
        var result = new List<TModel>();

        foreach (var entity in entities) result.Add(await this.CreateOrUpdateAsync(entity, existsPredicate));

        return result;
    }

    /// <summary>
    ///     Calculate and return models that intersect with a given polygon
    /// </summary>
    /// <typeparam name="TGeoAwareModel"></typeparam>
    /// <typeparam name="TCoordinates"></typeparam>
    /// <param name="polygon"></param>
    /// <returns></returns>
    public virtual async Task<IAsyncCursor<TGeoAwareModel>> GeoWithinAsync<TGeoAwareModel, TCoordinates>(
        GeoJsonPolygon<TCoordinates> polygon)
        where TGeoAwareModel : TModel, IHasGeographicContext<TCoordinates>
        where TCoordinates : GeoJsonCoordinates
    {
        var collection = this.MongoDbContext.GetCollection<TGeoAwareModel>();
        var filter = Builders<TGeoAwareModel>.Filter.GeoWithin(x => x.Center, polygon);

        var result = await collection.FindAsync(filter);

        return result;
    }

    public virtual async Task<IAsyncCursor<TGeoAwareModel>> GeoWithinAsync<TGeoAwareModel, TCoordinates>(
        GeoJsonPolygon<TCoordinates> polygon,
        Expression<Func<TGeoAwareModel, bool>> findPredicate)
        where TGeoAwareModel : TModel, IHasGeographicContext<TCoordinates>
        where TCoordinates : GeoJsonCoordinates
    {
        var collection = this.MongoDbContext.GetCollection<TGeoAwareModel>();
        var filter = Builders<TGeoAwareModel>.Filter.GeoWithin(x => x.Center, polygon);
        filter &= Builders<TGeoAwareModel>.Filter.Where(findPredicate);

        var result = await collection.FindAsync(filter);

        return result;
    }

    public virtual async Task<IAsyncCursor<TGeoAwareModel>> NearAsync<TGeoAwareModel, TCoordinates>(
        GeoJsonPoint<TCoordinates> point, double radius)
        where TGeoAwareModel : TModel, IHasGeographicContext<TCoordinates>
        where TCoordinates : GeoJsonCoordinates
    {
        var collection = this.MongoDbContext.GetCollection<TGeoAwareModel>();
        var filter = Builders<TGeoAwareModel>.Filter.NearSphere(x => x.Center, point, radius);

        var result = await collection.FindAsync(filter);

        return result;
    }

    public virtual async Task<IAsyncCursor<TGeoAwareModel>> NearAsync<TGeoAwareModel, TCoordinates>(
        GeoJsonPoint<TCoordinates> point, double radius,
        Expression<Func<TGeoAwareModel, bool>> findPredicate)
        where TGeoAwareModel : TModel, IHasGeographicContext<TCoordinates>
        where TCoordinates : GeoJsonCoordinates
    {
        var collection = this.MongoDbContext.GetCollection<TGeoAwareModel>();
        var filter = Builders<TGeoAwareModel>.Filter.NearSphere(x => x.Center, point, radius);
        filter &= Builders<TGeoAwareModel>.Filter.Where(findPredicate);

        var result = await collection.FindAsync(filter);

        return result;
    }

    public virtual async Task CreateGeoIndexAsync(string fieldName)
    {
        //throw new NotImplementedException();
        var collection = this.MongoDbContext.GetCollection<TModel>();

        var keys = Builders<TModel>.IndexKeys.Geo2DSphere(fieldName);

        await collection.Indexes.CreateOneAsync(new CreateIndexModel<TModel>(keys));
    }

    public virtual async Task<TModel> CreateOrUpdateAsync(TModel entity)
    {
        if (entity.Id.Equals(default)) return await this.CreateAsync(entity);
        return await this.UpdateAsync(entity);
    }

    public virtual async Task<IEnumerable<TModel>> GetModelsAsync()
    {
        return await this.ReadAllAsync();
    }

    public virtual async Task<TModel> GetModelAsync(TId id)
    {
        return await this.ReadAsync(id);
    }

    public virtual async Task<TModel> GetModelAsync(Expression<Func<TModel, bool>> findPredicate,
        params Expression<Func<TModel, object>>[] includes)
    {
        return await this.ReadAsync(findPredicate);
    }

    public virtual async Task<IEnumerable<TModel>> GetModelsAsync(params Expression<Func<TModel, object>>[] includes)
    {
        return await this.ReadAllAsync();
    }

    public virtual async Task<IEnumerable<TModel>> GetModelsAsync(Expression<Func<TModel, bool>> findPredicate,
        params Expression<Func<TModel, object>>[] includes)
    {
        return await this.ReadAllAsync(findPredicate);
    }

    public virtual async Task<IEnumerable<TModel>> GetModelsAsync<TOrderBy>(int page, int size,
        Expression<Func<TModel, TOrderBy>> orderBy, bool descending = false)
    {
        return await this.ReadAllAsync(page, size);
    }

    public virtual async Task<IEnumerable<TModel>> GetModelsAsync<TOrderBy>(int page, int size,
        Expression<Func<TModel, TOrderBy>> orderBy, bool descending = false,
        params Expression<Func<TModel, object>>[] includes)
    {
        return await this.ReadAllAsync(page, size);
    }

    public virtual async Task<IEnumerable<TModel>> GetModelsAsync<TOrderBy>(int page, int size,
        Expression<Func<TModel, bool>> findPredicate, Expression<Func<TModel, TOrderBy>> orderBy,
        bool descending = false, params Expression<Func<TModel, object>>[] includes)
    {
        return await this.ReadAllAsync(page, size, findPredicate);
    }

    public virtual async Task BuildModelRelationshipsAsync(TModel model)
    {
    }

    public virtual async Task<TResponse> SearchAsync<TQuery, TResponse>(TQuery query)
        where TQuery : class where TResponse : class, new()
    {
        return new TResponse();
    }

    public Task<bool> DeleteAsync(TId id)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    ///     Gets the collection.
    /// </summary>
    /// <value>The collection.</value>
    /// <autogeneratedoc />
    /// TODO Edit XML Comment Template for Collection
    public IMongoCollection<TModel> Collection { get; }

    /// <summary>
    ///     Finds the specified filter definition.
    /// </summary>
    /// <param name="filterDefinition">The filter definition.</param>
    /// <returns>IEnumerable&lt;TModel&gt;.</returns>
    /// <exception cref="System.NotImplementedException"></exception>
    /// <autogeneratedoc />
    /// TODO Edit XML Comment Template for Find
    public async Task<IEnumerable<TModel>> Find(FilterDefinition<TModel> filterDefinition)
    {
        return await this.Collection.Find(filterDefinition).ToListAsync();
    }

    /// <summary>
    ///     Deletes the specified identifier.
    /// </summary>
    /// <param name="entity">The entity.</param>
    /// <returns>Task&lt;System.Boolean&gt;.</returns>
    public virtual async Task<bool> DeleteAsync(TModel entity)
    {
        return await base.DeleteOneAsync(entity) != 0;
    }

    /// <summary>
    ///     Deletes the specified identifier.
    /// </summary>
    /// <param name="findPredicate">The find predicate.</param>
    /// <returns>Task&lt;System.Boolean&gt;.</returns>
    public virtual async Task<bool> DeleteAsync(Expression<Func<TModel, bool>> findPredicate)
    {
        return await base.DeleteOneAsync(findPredicate) != 0;
    }

    /// <summary>
    ///     Counts this instance.
    /// </summary>
    /// <returns>Task&lt;System.Int32&gt;.</returns>
    public virtual async Task<int> CountAsync()
    {
        var count = await base.CountAsync<TModel>(x => !x.Id.Equals(default));
        return (int)count;
    }

    /// <summary>
    ///     Counts the specified find predicate.
    /// </summary>
    /// <param name="findPredicate">The find predicate.</param>
    /// <returns>Task&lt;System.Int32&gt;.</returns>
    public virtual async Task<int> CountAsync(Expression<Func<TModel, bool>> findPredicate)
    {
        var count = await base.CountAsync(findPredicate);
        return (int)count;
    }

    /// <summary>
    ///     Creates the specified entity.
    /// </summary>
    /// <param name="entity">The entity.</param>
    /// <returns>Task&lt;TModel&gt;.</returns>
    public virtual async Task<TModel> CreateAsync(TModel entity)
    {
        await base.AddOneAsync(entity);

        return entity;
    }

    /// <summary>
    ///     Creates the specified entity.
    /// </summary>
    /// <param name="entity">The entity.</param>
    /// <param name="existsPredicate">The exists predicate.</param>
    /// <returns></returns>
    public virtual async Task<TModel> CreateAsync(TModel entity, Expression<Func<TModel, bool>> existsPredicate)
    {
        var exists = await this.ReadAsync(existsPredicate);

        if (exists == null) return await this.CreateAsync(entity);

        await base.UpdateOneAsync(entity);

        return exists;
    }

    /// <summary>
    ///     Reads the specified identifier.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <returns></returns>
    public virtual async Task<TModel> ReadAsync(TId id)
    {
        var model = await this.GetByIdAsync<TModel>(id);

        if (model == null) return null;

        await this.BuildModelRelationshipsAsync(model);

        return model;
    }

    /// <summary>
    ///     Retrieves the specified identifier.
    /// </summary>
    /// <param name="findPredicate">The find predicate.</param>
    /// <returns>Task&lt;TModel&gt;.</returns>
    public virtual async Task<TModel> ReadAsync(Expression<Func<TModel, bool>> findPredicate)
    {
        var model = await this.GetOneAsync(findPredicate);

        if (model == null) return null;

        await this.BuildModelRelationshipsAsync(model);

        return model;
    }

    /// <summary>
    ///     Retrieves the specified identifier.
    /// </summary>
    /// <param name="findPredicate">The find predicate.</param>
    /// <param name="includes">The includes.</param>
    /// <returns>Task&lt;TModel&gt;.</returns>
    public virtual async Task<TModel> ReadAsync(Expression<Func<TModel, bool>> findPredicate,
        params Expression<Func<TModel, object>>[] includes)
    {
        return await this.ReadAsync(findPredicate);
    }

    /// <summary>
    ///     Retrieves all instances.
    /// </summary>
    /// <returns>Task&lt;IEnumerable&lt;TModel&gt;&gt;.</returns>
    public virtual async Task<IEnumerable<TModel>> ReadAllAsync()
    {
        var models = await this.GetAllAsync<TModel>(x => !x.Id.Equals(default));

        if (!models.Any()) return models;

        foreach (var model in models) await this.BuildModelRelationshipsAsync(model);

        return models;
    }

    /// <summary>
    ///     Reads all when they match the func.
    /// </summary>
    /// <param name="findPredicate">The find predicate.</param>
    /// <returns>Task&lt;IEnumerable&lt;TModel&gt;&gt;.</returns>
    public virtual async Task<IEnumerable<TModel>> ReadAllAsync(Expression<Func<TModel, bool>> findPredicate)
    {
        var models = await this.GetAllAsync(findPredicate);

        if (!models.Any()) return models;

        foreach (var model in models) await this.BuildModelRelationshipsAsync(model);

        return models;
    }

    /// <summary>
    ///     Reads all when they match the func.
    /// </summary>
    /// <param name="findPredicate">The find predicate.</param>
    /// <param name="includes">The includes.</param>
    /// <returns>Task&lt;IEnumerable&lt;TModel&gt;&gt;.</returns>
    public virtual async Task<IEnumerable<TModel>> ReadAllAsync(Expression<Func<TModel, bool>> findPredicate,
        params Expression<Func<TModel, object>>[] includes)
    {
        return await this.ReadAllAsync(findPredicate);
    }

    /// <summary>
    ///     Reads all when they match the func.
    /// </summary>
    /// <param name="includes">The includes.</param>
    /// <returns>Task&lt;IEnumerable&lt;TModel&gt;&gt;.</returns>
    public virtual async Task<IEnumerable<TModel>> ReadAllAsync(params Expression<Func<TModel, object>>[] includes)
    {
        return await this.ReadAllAsync();
    }

    /// <summary>
    ///     Reads all.
    /// </summary>
    /// <typeparam name="TOrderBy">The type of the order by.</typeparam>
    /// <param name="page">The page.</param>
    /// <param name="count">The count.</param>
    /// <returns></returns>
    public virtual async Task<IEnumerable<TModel>> ReadAllAsync(int page, int count)
    {
        return await this.ReadAllAsync(page, count, x => !x.Id.Equals(default));
    }

    /// <summary>
    ///     Reads all.
    /// </summary>
    /// <typeparam name="TOrderBy">The type of the order by.</typeparam>
    /// <param name="page">The page.</param>
    /// <param name="count">The count.</param>
    /// <param name="findPredicate">The find predicate.</param>
    /// <returns></returns>
    public virtual async Task<IEnumerable<TModel>> ReadAllAsync(int page, int count,
        Expression<Func<TModel, bool>> findPredicate)
    {
        var take = count;
        var skip = page * count;

        var models = await base.GetSortedPaginatedAsync(findPredicate, x => x.Created, false, skip, take);

        if (!models.Any()) return models;

        foreach (var model in models) await this.BuildModelRelationshipsAsync(model);

        return models;
    }

    /// <summary>
    ///     Reads all.
    /// </summary>
    /// <param name="page">The page.</param>
    /// <param name="count">The count.</param>
    /// <param name="includes">The includes.</param>
    /// <returns></returns>
    public virtual async Task<IEnumerable<TModel>> ReadAllAsync(int page, int count,
        params Expression<Func<TModel, object>>[] includes)
    {
        return await this.ReadAllAsync(page, count, x => !x.Id.Equals(default));
    }

    /// <summary>
    ///     Reads all.
    /// </summary>
    /// <typeparam name="TOrderBy">The type of the order by.</typeparam>
    /// <param name="page">The page.</param>
    /// <param name="count">The count.</param>
    /// <param name="findPredicate">The find predicate.</param>
    /// <param name="includes">The includes.</param>
    /// <returns></returns>
    public virtual async Task<IEnumerable<TModel>> ReadAllAsync(int page, int count,
        Expression<Func<TModel, bool>> findPredicate, params Expression<Func<TModel, object>>[] includes)
    {
        return await this.ReadAllAsync(page, count, findPredicate);
    }

    /// <summary>
    ///     Reads all.
    /// </summary>
    /// <param name="page">The page.</param>
    /// <param name="count">The count.</param>
    /// <param name="includes">The includes.</param>
    /// <returns></returns>
    public virtual async Task<IEnumerable<TModel>> ReadAllAsync<TOrderBy>(int page, int count,
        Expression<Func<TModel, TOrderBy>> orderBy, bool descending = false,
        params Expression<Func<TModel, object>>[] includes)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    ///     Reads all.
    /// </summary>
    /// <param name="page">The page.</param>
    /// <param name="count">The count.</param>
    /// <param name="findPredicate">The find predicate.</param>
    /// <param name="includes">The includes.</param>
    /// <returns></returns>
    public virtual async Task<IEnumerable<TModel>> ReadAllAsync<TOrderBy>(int page, int count,
        Expression<Func<TModel, bool>> findPredicate, Expression<Func<TModel, TOrderBy>> orderBy,
        bool descending = false,
        params Expression<Func<TModel, object>>[] includes)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    ///     Reads all.
    /// </summary>
    /// <param name="page">The page.</param>
    /// <param name="count">The count.</param>
    /// <param name="findPredicate">The find predicate.</param>
    /// <returns></returns>
    public virtual async Task<IEnumerable<TModel>> ReadAllAsync<TOrderBy>(int page, int count,
        Expression<Func<TModel, bool>> findPredicate, Expression<Func<TModel, TOrderBy>> orderBy,
        bool descending = false)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    ///     Reads all when they match the func.
    /// </summary>
    /// <param name="findPredicate">The find predicate.</param>
    /// <param name="includes">The includes.</param>
    /// <returns>Task&lt;IEnumerable&lt;TModel&gt;&gt;.</returns>
    public virtual async Task<IEnumerable<TModel>> WhereAsync(Expression<Func<TModel, bool>> findPredicate,
        params Expression<Func<TModel, object>>[] includes)
    {
        return await this.ReadAllAsync(findPredicate);
    }

    /// <summary>
    ///     Reads all when they match the func.
    /// </summary>
    /// <param name="findPredicate">The find predicate.</param>
    /// <returns>Task&lt;IEnumerable&lt;TModel&gt;&gt;.</returns>
    public virtual async Task<IEnumerable<TModel>> WhereAsync(Expression<Func<TModel, bool>> findPredicate)
    {
        return await this.ReadAllAsync(findPredicate);
    }

    /// <summary>
    ///     Updates the specified entity.
    /// </summary>
    /// <param name="entity">The entity.</param>
    /// <returns>Task&lt;TModel&gt;.</returns>
    public virtual async Task<TModel> UpdateAsync(TModel entity)
    {
        await base.UpdateOneAsync(entity);
        return entity;
    }

    public Guid GetIdFromEntity(TModel entity)
    {
        return default;
    }

    public virtual async Task<IEnumerable<TModel>> CreateAsync(IEnumerable<TModel> entities)
    {
        var results = new List<TModel>();

        foreach (var document in entities) results.Add(await this.CreateAsync(document));

        return results;
    }

    /// <summary>
    ///     Deletes the specified identifier.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <returns></returns>
    public virtual async Task<bool> DeleteAsync(Guid id)
    {
        return await base.DeleteOneAsync<TModel>(x => x.Id.Equals(id)) > 0;
    }
}