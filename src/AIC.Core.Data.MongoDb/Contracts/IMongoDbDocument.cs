namespace AIC.Core.Data.MongoDb.Contracts;

using MongoDbGenericRepository.Models;

public interface IMongoDbDocument : IMongoDbDocument<Guid>, IDocument
{
}

public interface IMongoDbDocument<TId> : IDocument<TId> where TId : struct, IEquatable<TId>
{
    TId Id { get; set; }
    int Version { get; set; }
    string? DisplayName { get; }
    DateTime Created { get; set; }
    DateTime Updated { get; set; }
    bool? Deleted { get; set; }
    Guid OrganisationId { get; set; }
    Guid RootUserId { get; set; }
    Guid TenantId { get; set; }
}