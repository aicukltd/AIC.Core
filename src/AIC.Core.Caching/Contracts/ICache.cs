namespace AIC.Core.Caching.Contracts;

using System.Linq.Expressions;
using AIC.Core.Data.Contracts;

/// <summary>
///     Defines a cache that can be store and get items by keys.
/// </summary>
public interface ICache : IDisposable
{
    /// <summary>
    ///     Unique name of the cache.
    /// </summary>
    string Name { get; }

    /// <summary>
    ///     Default sliding expire time of cache items.
    ///     Default value: 60 minutes (1 hour).
    ///     Can be changed by configuration.
    /// </summary>
    TimeSpan DefaultSlidingExpireTime { get; set; }

    /// <summary>
    ///     Default absolute expire time of cache items.
    ///     Default value: null (not used).
    /// </summary>
    TimeSpan? DefaultAbsoluteExpireTime { get; set; }

    /// <summary>
    ///     Gets an item from the cache.
    ///     This method hides cache provider failures (and logs them),
    ///     uses the factory method to get the object if cache provider fails.
    /// </summary>
    /// <param name="key">Key</param>
    /// <param name="factory">Factory method to create cache item if not exists</param>
    /// <returns>Cached item</returns>
    object Get(string key, Func<string, object> factory);

    /// <summary>
    ///     Gets items from the cache.
    ///     This method hides cache provider failures (and logs them),
    ///     uses the factory method to get the object if cache provider fails.
    /// </summary>
    /// <param name="keys">Keys</param>
    /// <param name="factory">Factory method to create cache item if not exists</param>
    /// <returns>Cached item</returns>
    object[] Get(string[] keys, Func<string, object> factory);

    /// <summary>
    ///     Gets an item from the cache.
    ///     This method hides cache provider failures (and logs them),
    ///     uses the factory method to get the object if cache provider fails.
    /// </summary>
    /// <param name="key">Key</param>
    /// <param name="factory">Factory method to create cache item if not exists</param>
    /// <returns>Cached item</returns>
    Task<object> GetAsync(string key, Func<string, Task<object>> factory);

    /// <summary>
    ///     Gets items from the cache.
    ///     This method hides cache provider failures (and logs them),
    ///     uses the factory method to get the object if cache provider fails.
    /// </summary>
    /// <param name="keys">Keys</param>
    /// <param name="factory">Factory method to create cache item if not exists</param>
    /// <returns>Cached items</returns>
    Task<object[]> GetAsync(string[] keys, Func<string, Task<object>> factory);

    /// <summary>
    ///     Gets an item from the cache or null if not found.
    /// </summary>
    /// <param name="key">Key</param>
    /// <returns>Cached item or null if not found</returns>
    object GetOrDefault(string key);

    object GetOrDefault(Func<object, bool> predicate);

    /// <summary>
    ///     Gets items from the cache. For every key that is not found, a null value is returned.
    /// </summary>
    /// <param name="keys">Keys</param>
    /// <returns>Cached items</returns>
    object[] GetOrDefault(string[] keys);

    object[] GetMultipleOrDefault(Func<object, bool> predicate);

    /// <summary>
    ///     Gets an item from the cache or null if not found.
    /// </summary>
    /// <param name="key">Key</param>
    /// <returns>Cached item or null if not found</returns>
    Task<object> GetOrDefaultAsync(string key);

    /// <summary>
    ///     Gets items from the cache. For every key that is not found, a null value is returned.
    /// </summary>
    /// <param name="keys">Keys</param>
    /// <returns>Cached items</returns>
    Task<object[]> GetOrDefaultAsync(string[] keys);

    /// <summary>
    ///     Saves/Overrides an item in the cache by a key.
    ///     Use one of the expire times at most (<paramref name="slidingExpireTime" /> or
    ///     <paramref name="absoluteExpireTime" />).
    ///     If none of them is specified, then
    ///     <see cref="DefaultAbsoluteExpireTime" /> will be used if it's not null. Othewise,
    ///     <see cref="DefaultSlidingExpireTime" />
    ///     will be used.
    /// </summary>
    /// <param name="key">Key</param>
    /// <param name="value">Value</param>
    /// <param name="slidingExpireTime">Sliding expire time</param>
    /// <param name="absoluteExpireTime">Absolute expire time</param>
    void Set(string key, object value, TimeSpan? slidingExpireTime = null, TimeSpan? absoluteExpireTime = null);

    /// <summary>
    ///     Saves/Overrides items in the cache by the pairs.
    ///     Use one of the expire times at most (<paramref name="slidingExpireTime" /> or
    ///     <paramref name="absoluteExpireTime" />).
    ///     If none of them is specified, then
    ///     <see cref="DefaultAbsoluteExpireTime" /> will be used if it's not null. Othewise,
    ///     <see cref="DefaultSlidingExpireTime" />
    ///     will be used.
    /// </summary>
    /// <param name="pairs">Pairs</param>
    /// <param name="slidingExpireTime">Sliding expire time</param>
    /// <param name="absoluteExpireTime">Absolute expire time</param>
    void Set(KeyValuePair<string, object>[] pairs, TimeSpan? slidingExpireTime = null,
        TimeSpan? absoluteExpireTime = null);

    /// <summary>
    ///     Saves/Overrides an item in the cache by a key.
    ///     Use one of the expire times at most (<paramref name="slidingExpireTime" /> or
    ///     <paramref name="absoluteExpireTime" />).
    ///     If none of them is specified, then
    ///     <see cref="DefaultAbsoluteExpireTime" /> will be used if it's not null. Othewise,
    ///     <see cref="DefaultSlidingExpireTime" />
    ///     will be used.
    /// </summary>
    /// <param name="key">Key</param>
    /// <param name="value">Value</param>
    /// <param name="slidingExpireTime">Sliding expire time</param>
    /// <param name="absoluteExpireTime">Absolute expire time</param>
    Task SetAsync(string key, object value, TimeSpan? slidingExpireTime = null,
        TimeSpan? absoluteExpireTime = null);

    /// <summary>
    ///     Saves/Overrides items in the cache by the pairs.
    ///     Use one of the expire times at most (<paramref name="slidingExpireTime" /> or
    ///     <paramref name="absoluteExpireTime" />).
    ///     If none of them is specified, then
    ///     <see cref="DefaultAbsoluteExpireTime" /> will be used if it's not null. Othewise,
    ///     <see cref="DefaultSlidingExpireTime" />
    ///     will be used.
    /// </summary>
    /// <param name="pairs">Pairs</param>
    /// <param name="slidingExpireTime">Sliding expire time</param>
    /// <param name="absoluteExpireTime">Absolute expire time</param>
    Task SetAsync(KeyValuePair<string, object>[] pairs, TimeSpan? slidingExpireTime = null,
        TimeSpan? absoluteExpireTime = null);

    /// <summary>
    ///     Removes a cache item by it's key (does nothing if given key does not exists in the cache).
    /// </summary>
    /// <param name="key">Key</param>
    void Remove(string key);

    /// <summary>
    ///     Removes cache items by their keys.
    /// </summary>
    /// <param name="keys">Keys</param>
    void Remove(string[] keys);

    /// <summary>
    ///     Removes a cache item by it's key (does nothing if given key does not exists in the cache).
    /// </summary>
    /// <param name="key">Key</param>
    Task RemoveAsync(string key);

    /// <summary>
    ///     Removes cache items by their keys.
    /// </summary>
    /// <param name="keys">Keys</param>
    Task RemoveAsync(string[] keys);

    /// <summary>
    ///     Clears all items in this cache.
    /// </summary>
    void Clear();

    /// <summary>
    ///     Clears all items in this cache.
    /// </summary>
    Task ClearAsync();
}

public interface ICache<TKey, TValue> where TValue : class, IHasId<TKey> where TKey : struct
{
    Task<TValue> GetOrAddAsync(TKey key, Func<TKey, Task<TValue>> valueFactory);
    TValue GetOrAdd(TKey key, Func<TKey, TValue> valueFactory);
    bool TryGet(TKey key, out TValue value);
    bool TryAdd(TValue model);
    void Remove(TKey key);

    Task<TValue> GetOrAddAsync(Expression<Func<TValue, bool>> predicate,
        Func<Task<TValue>> valueFactory);

    TValue GetOrAdd(Expression<Func<TValue, bool>> predicate,
        Func<TValue> valueFactory);

    Task<IList<TValue>> GetOrAddAsync(Expression<Func<TValue, bool>> predicate,
        Func<Task<IEnumerable<TValue>>> valueFactory);

    IList<TValue> GetOrAdd(Expression<Func<TValue, bool>> predicate,
        Func<IEnumerable<TValue>> valueFactory);
}