namespace AIC.Core.Caching.Implementations;

using AIC.Core.Caching.Contracts;
using AIC.Core.Caching.Extensions;

/// <summary>
///     Implements <see cref="ICache" /> to wrap a <see cref="ICache" />.
/// </summary>
/// <typeparam name="TValue"></typeparam>
/// <typeparam name="TValue"></typeparam>
public class TypedCacheWrapper<TKey, TValue> : ITypedCache<TKey, TValue>
{
    /// <summary>
    ///     Creates a new <see cref="TypedCacheWrapper{TKey,TValue}" /> object.
    /// </summary>
    /// <param name="internalCache">The actual internal cache</param>
    public TypedCacheWrapper(ICache internalCache)
    {
        this.InternalCache = internalCache;
    }

    public TimeSpan? DefaultAbsoluteExpireTime
    {
        get => this.InternalCache.DefaultAbsoluteExpireTime;
        set => this.InternalCache.DefaultAbsoluteExpireTime = value;
    }

    public string Name => this.InternalCache.Name;

    public TimeSpan DefaultSlidingExpireTime
    {
        get => this.InternalCache.DefaultSlidingExpireTime;
        set => this.InternalCache.DefaultSlidingExpireTime = value;
    }

    public ICache InternalCache { get; }

    public void Dispose()
    {
        this.InternalCache.Dispose();
    }

    public void Clear()
    {
        this.InternalCache.Clear();
    }

    public Task ClearAsync()
    {
        return this.InternalCache.ClearAsync();
    }

    public TValue Get(TKey key, Func<TKey, TValue> factory)
    {
        return this.InternalCache.Get(key, factory);
    }

    public TValue[] Get(TKey[] keys, Func<TKey, TValue> factory)
    {
        return this.InternalCache.Get(keys, factory);
    }

    public Task<TValue> GetAsync(TKey key, Func<TKey, Task<TValue>> factory)
    {
        return this.InternalCache.GetAsync(key, factory);
    }

    public async Task<TValue> FindAsync(Func<TValue, bool> predicate)
    {
        throw new NotImplementedException();
    }

    public Task<TValue[]> GetAsync(TKey[] keys, Func<TKey, Task<TValue>> factory)
    {
        return this.InternalCache.GetAsync(keys, factory);
    }

    public async Task<TValue[]> FindAllAsync(Func<TValue, bool> predicate)
    {
        throw new NotImplementedException();
    }

    public TValue GetOrDefault(TKey key)
    {
        return this.InternalCache.GetOrDefault<TKey, TValue>(key);
    }

    public TValue[] GetOrDefault(TKey[] keys)
    {
        return this.InternalCache.GetOrDefault<TKey, TValue>(keys);
    }

    public Task<TValue> GetOrDefaultAsync(TKey key)
    {
        return this.InternalCache.GetOrDefaultAsync<TKey, TValue>(key);
    }

    public Task<TValue[]> GetOrDefaultAsync(TKey[] keys)
    {
        return this.InternalCache.GetOrDefaultAsync<TKey, TValue>(keys);
    }

    public void Set(TKey key, TValue value, TimeSpan? slidingExpireTime = null, TimeSpan? absoluteExpireTime = null)
    {
        this.InternalCache.Set(key.ToString(), value, slidingExpireTime, absoluteExpireTime);
    }

    public void Set(KeyValuePair<TKey, TValue>[] pairs, TimeSpan? slidingExpireTime = null,
        TimeSpan? absoluteExpireTime = null)
    {
        var stringPairs = pairs.Select(p => new KeyValuePair<string, object>(p.Key.ToString(), p.Value));
        this.InternalCache.Set(stringPairs.ToArray(), slidingExpireTime, absoluteExpireTime);
    }

    public Task SetAsync(TKey key, TValue value, TimeSpan? slidingExpireTime = null,
        TimeSpan? absoluteExpireTime = null)
    {
        return this.InternalCache.SetAsync(key.ToString(), value, slidingExpireTime, absoluteExpireTime);
    }

    public Task SetAsync(KeyValuePair<TKey, TValue>[] pairs, TimeSpan? slidingExpireTime = null,
        TimeSpan? absoluteExpireTime = null)
    {
        var stringPairs = pairs.Select(p => new KeyValuePair<string, object>(p.Key.ToString(), p.Value));
        return this.InternalCache.SetAsync(stringPairs.ToArray(), slidingExpireTime, absoluteExpireTime);
    }

    public void Remove(TKey key)
    {
        this.InternalCache.Remove(key.ToString());
    }

    public void Remove(TKey[] keys)
    {
        this.InternalCache.Remove(keys.Select(key => key.ToString()).ToArray());
    }

    public Task RemoveAsync(TKey key)
    {
        return this.InternalCache.RemoveAsync(key.ToString());
    }

    public Task RemoveAsync(TKey[] keys)
    {
        return this.InternalCache.RemoveAsync(keys.Select(key => key.ToString()).ToArray());
    }
}