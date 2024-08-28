namespace AIC.Core.Data.MongoDb.Contracts;

using System.Linq.Expressions;
using AIC.Core.Data.Contracts;
using MongoDB.Driver;
using MongoDB.Driver.GeoJsonObjectModel;

public interface IMongoDbRepository<TModel> : IMongoDbRepository<TModel, Guid>
    where TModel : class, IMongoDbDocument
{
}

public interface IMongoDbRepository<TModel, in TId> : IRepository<TModel, TId>
    where TModel : class, IMongoDbDocument<TId>
    where TId : struct, IEquatable<TId>
{
    IMongoCollection<TModel> Collection { get; }

    Task<IEnumerable<TModel>> Find(FilterDefinition<TModel> filterDefinition);

    Task<IAsyncCursor<TGeoAwareModel>> GeoWithinAsync<TGeoAwareModel, TCoordinates>(
        GeoJsonPolygon<TCoordinates> polygon)
        where TGeoAwareModel : TModel, IHasGeographicContext<TCoordinates>
        where TCoordinates : GeoJsonCoordinates;

    Task<IAsyncCursor<TGeoAwareModel>> GeoWithinAsync<TGeoAwareModel, TCoordinates>(
        GeoJsonPolygon<TCoordinates> polygon,
        Expression<Func<TGeoAwareModel, bool>> findPredicate)
        where TGeoAwareModel : TModel, IHasGeographicContext<TCoordinates>
        where TCoordinates : GeoJsonCoordinates;

    Task<IAsyncCursor<TGeoAwareModel>> NearAsync<TGeoAwareModel, TCoordinates>(GeoJsonPoint<TCoordinates> point,
        double radius)
        where TGeoAwareModel : TModel, IHasGeographicContext<TCoordinates>
        where TCoordinates : GeoJsonCoordinates;

    Task<IAsyncCursor<TGeoAwareModel>> NearAsync<TGeoAwareModel, TCoordinates>(GeoJsonPoint<TCoordinates> point,
        double radius,
        Expression<Func<TGeoAwareModel, bool>> findPredicate)
        where TGeoAwareModel : TModel, IHasGeographicContext<TCoordinates>
        where TCoordinates : GeoJsonCoordinates;

    Task CreateGeoIndexAsync(string fieldName);
}