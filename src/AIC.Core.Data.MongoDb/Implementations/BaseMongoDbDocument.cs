namespace AIC.Core.Data.MongoDb.Implementations;

using AIC.Core.Data.MongoDb.Contracts;
using MongoDB.Bson.Serialization.Attributes;

public abstract class BaseMongoDbDocument<TId> : IMongoDbDocument<TId> where TId : struct, IEquatable<TId>
{
    public BaseMongoDbDocument()
    {
        this.Created = DateTime.UtcNow;
        this.Updated = DateTime.UtcNow;
        this.Deleted = false;
    }

    [BsonId] public TId Id { get; set; }

    public virtual int Version { get; set; }

    [BsonIgnore] public virtual string? DisplayName => this.ToString();

    public DateTime Created { get; set; }
    public DateTime Updated { get; set; }
    public bool? Deleted { get; set; }

    [BsonIgnoreIfDefault]
    [BsonIgnoreIfNull]
    public Guid OrganisationId { get; set; }

    [BsonIgnoreIfDefault]
    [BsonIgnoreIfNull]
    public Guid RootUserId { get; set; }

    [BsonIgnoreIfDefault]
    [BsonIgnoreIfNull]
    public Guid TenantId { get; set; }
}

public abstract class BaseMongoDbDocument : BaseMongoDbDocument<Guid>, IMongoDbDocument
{
}