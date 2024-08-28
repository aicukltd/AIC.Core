namespace AIC.Core.Caching.InMemory.Implementations;

using System.Collections.Concurrent;
using System.Linq.Expressions;
using AIC.Core.Caching.Contracts;
using AIC.Core.Caching.Implementations;
using AIC.Core.Data.Contracts;

public class InMemoryCache : CacheBase
{
    private readonly IDictionary<string, object> cache;

    public InMemoryCache(string name) : base(name)
    {
        this.cache = new Dictionary<string, object>();
    }

    public override object GetOrDefault(string key)
    {
        this.cache.TryGetValue(key, out var model);

        return model;
    }

    public override object GetOrDefault(Func<object, bool> predicate)
    {
        return this.cache.Values.Where(predicate).FirstOrDefault();
    }

    public override object[] GetMultipleOrDefault(Func<object, bool> predicate)
    {
        return this.cache.Values.Where(predicate).ToArray();
    }

    public override void Set(string key, object value, TimeSpan? slidingExpireTime = null,
        TimeSpan? absoluteExpireTime = null)
    {
        if (!this.cache.ContainsKey(key))
            this.cache.Add(key, value);
        else
            this.cache[key] = value;
    }

    public override void Remove(string key)
    {
        this.cache.Remove(key);
    }

    public override void Clear()
    {
        this.cache.Clear();
    }
}

public class InMemoryCache<TKey, TModel> : ICache<TKey, TModel>
    where TKey : struct where TModel : class, IHasId<TKey>
{
    private readonly IDictionary<TKey, ICacheItem<TModel>> cache;

    public InMemoryCache()
    {
        this.cache = new ConcurrentDictionary<TKey, ICacheItem<TModel>>();
    }

    public async Task<TModel> GetOrAddAsync(TKey key, Func<TKey, Task<TModel>> valueFactory)
    {
        if (this.TryGet(key, out var result)) return result;

        var model = await valueFactory(key);

        this.TryAdd(model);

        return model;
    }

    public TModel GetOrAdd(TKey key, Func<TKey, TModel> valueFactory)
    {
        if (this.TryGet(key, out var result)) return result;

        var model = valueFactory(key);

        this.TryAdd(model);

        return model;
    }


    public bool TryAdd(TModel model)
    {
        if (this.cache.ContainsKey(model.Id)) return false;

        var cacheItem = new CacheItem<TModel>();
        cacheItem.Set(model, TimeSpan.FromHours(2));

        this.cache.Add(model.Id, cacheItem);

        return true;
    }

    public bool TryGet(TKey key, out TModel value)
    {
        if (!this.cache.TryGetValue(key, out var result) || result.HasExpired)
        {
            value = default;

            return false;
        }

        value = result.Item;

        return true;
    }

    public void Remove(TKey key)
    {
        this.cache.Remove(key);
    }

    public async Task<TModel> GetOrAddAsync(Expression<Func<TModel, bool>> predicate,
        Func<Task<TModel>> valueFactory)
    {
        var wherePredicate = predicate.Compile();

        var cacheResult = this.cache.Values.Where(x => !x.HasExpired).Select(x => x.Item)
            .FirstOrDefault(wherePredicate);

        if (cacheResult != null)
            return cacheResult;

        var model = await valueFactory();

        this.TryAdd(model);

        return cacheResult;
    }

    public TModel GetOrAdd(Expression<Func<TModel, bool>> predicate, Func<TModel> valueFactory)
    {
        var wherePredicate = predicate.Compile();

        var cacheResult = this.cache.Values.Where(x => !x.HasExpired).Select(x => x.Item)
            .FirstOrDefault(wherePredicate);

        if (cacheResult != null)
            return cacheResult;

        var model = valueFactory();

        this.TryAdd(model);

        return cacheResult;
    }


    public async Task<IList<TModel>> GetOrAddAsync(Expression<Func<TModel, bool>> predicate,
        Func<Task<IEnumerable<TModel>>> valueFactory)
    {
        var wherePredicate = predicate.Compile();

        var cacheResults = this.cache.Values.Where(x => !x.HasExpired).Select(x => x.Item).Where(wherePredicate)
            .ToList();

        if (cacheResults.Any())
            return cacheResults;

        var models = await valueFactory();

        foreach (var model in models) this.TryAdd(model);

        return cacheResults;
    }


    public IList<TModel> GetOrAdd(Expression<Func<TModel, bool>> predicate,
        Func<IEnumerable<TModel>> valueFactory)
    {
        var wherePredicate = predicate.Compile();

        var cacheResults = this.cache.Values.Where(x => !x.HasExpired).Select(x => x.Item).Where(wherePredicate)
            .ToList();

        if (cacheResults.Any())
            return cacheResults;

        var models = valueFactory();

        foreach (var model in models) this.TryAdd(model);

        return cacheResults;
    }
}