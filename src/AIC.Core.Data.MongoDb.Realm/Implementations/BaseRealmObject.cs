namespace AIC.Core.Data.MongoDb.Realm.Implementations;

using AIC.Core.Data.MongoDb.Implementations;
using AIC.Core.Data.MongoDb.Realm.Contracts;
using Realms;
using Realms.Schema;
using Realms.Weaving;

public abstract class BaseRealmObject : BaseRealmObject<Guid>
{
}

public abstract class BaseRealmObject<TId> : BaseMongoDbDocument<TId>, IBaseRealmObject<TId>
    where TId : struct, IEquatable<TId>
{
    [PrimaryKey] private new TId Id { get; set; }

    public void SetManagedAccessor(IRealmAccessor accessor, IRealmObjectHelper? helper = null, bool update = false,
        bool skipDefaults = false)
    {
        throw new NotImplementedException();
    }

    public IRealmAccessor Accessor { get; }
    public bool IsManaged { get; }
    public bool IsValid { get; }
    public bool IsFrozen { get; }
    public Realm? Realm { get; }
    public ObjectSchema? ObjectSchema { get; }
    public DynamicObjectApi DynamicApi { get; }
    public int BacklinksCount { get; }
}