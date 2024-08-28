namespace AIC.Core.Data.MongoDb.Implementations;

using AIC.Core.Data.MongoDb.Contracts;
using MongoDB.Driver;
using MongoDB.Driver.GeoJsonObjectModel;
using MongoDB.Driver.GridFS;

public abstract class BaseMongoDbFileRepository<TModel> : BaseMongoDbFileRepository<TModel, Guid>
    where TModel : class, IMongoDbDocument, IHasDataPayload, new()
{
    protected BaseMongoDbFileRepository(IMongoConnectionOptions mongoConnectionOptions,
        GridFSBucketOptions gridFsBucketOptions = null) : base(mongoConnectionOptions, gridFsBucketOptions)
    {
    }
}

public abstract class BaseMongoDbFileRepository<TModel, TId> : BaseMongoDbRepository<TModel, TId>,
    IMongoDbFileRepository<TModel, TId>
    where TModel : class, IHasDataPayload, IMongoDbDocument<TId>, new()
    where TId : struct, IEquatable<TId>
{
    /// <summary>
    ///     The constructor taking a connection string and a database name.
    /// </summary>
    /// <param name="connectionString">The connection string of the MongoDb server.</param>
    /// <param name="databaseName">The name of the database against which you want to perform operations.</param>
    /// <param name="gridFsBucketOptions"></param>
    protected BaseMongoDbFileRepository(IMongoConnectionOptions mongoConnectionOptions,
        GridFSBucketOptions gridFsBucketOptions = null) : base(mongoConnectionOptions)
    {
        this.GridFsBucket = new GridFSBucket<TId>(this.MongoDbContext.Database, gridFsBucketOptions);
    }

    private IGridFSBucket<TId> GridFsBucket { get; }


    /// <summary>
    ///     Searches the by coordinates.
    /// </summary>
    /// <typeparam name="TGeographicModel">The type of the geographic model.</typeparam>
    /// <typeparam name="TGeographicContextCoordinate">The type of the geographic context coordinate.</typeparam>
    /// <param name="latitude">The latitude.</param>
    /// <param name="longitude">The longitude.</param>
    /// <returns></returns>
    public virtual async Task<IEnumerable<TGeographicModel>> SearchByCoordinates<TGeographicModel,
        TGeographicContextCoordinate>(double latitude, double longitude)
        where TGeographicModel : TModel, IHasGeographicContext<TGeographicContextCoordinate>
        where TGeographicContextCoordinate : GeoJsonCoordinates
    {
        var point = GeoJson.Point(GeoJson.Geographic(longitude, latitude));
        var locationQuery = new FilterDefinitionBuilder<TGeographicModel>().Near(tag => tag.Center, point, 100);
        var query = base.GetCollection<TGeographicModel>().Find(locationQuery);
        return await query.ToListAsync();
    }

    /// <summary>
    ///     Uploads the specified identifier.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <param name="model">The model.</param>
    /// <param name="filename">The filename.</param>
    /// <param name="gridFsUploadOptions">The grid fs upload options.</param>
    /// <returns></returns>
    public virtual async Task<TId> CreateAsync(TId id, TModel model, string filename = null,
        GridFSUploadOptions gridFsUploadOptions = null)
    {
        if (this.GridFsBucket == null) return id;

        await this.GridFsBucket.UploadFromBytesAsync(id, filename, model.Data, gridFsUploadOptions);

        return id;
    }

    /// <summary>
    ///     Reads the specified identifier.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <param name="filename">The filename.</param>
    /// <param name="gridFsDownloadOptions">The grid fs download options.</param>
    /// <param name="gridFsDownloadByNameOptions"></param>
    /// <returns></returns>
    public virtual async Task<TModel> ReadAsync(TId id, string filename,
        GridFSDownloadOptions gridFsDownloadOptions = null,
        GridFSDownloadByNameOptions gridFsDownloadByNameOptions = null)
    {
        var result = await this.GridFsBucket.DownloadAsBytesAsync(id, gridFsDownloadOptions) ??
                     await this.GridFsBucket.DownloadAsBytesByNameAsync(filename, gridFsDownloadByNameOptions);

        var model = new TModel { Data = result };
        return model;
    }
}