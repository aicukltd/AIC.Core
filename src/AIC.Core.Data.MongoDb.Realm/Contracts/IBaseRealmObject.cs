namespace AIC.Core.Data.MongoDb.Realm.Contracts;

using AIC.Core.Data.MongoDb.Contracts;
using Realms;

public interface IBaseRealmObject : IBaseRealmObject<Guid>
{
}

public interface IBaseRealmObject<TId> : IRealmObject, IMongoDbDocument<TId>
    where TId : struct, IEquatable<TId>
{
}