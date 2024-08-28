namespace AIC.Core.Caching.Implementations;

using System.Reactive.Concurrency;
using AIC.Core.Caching.Contracts;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

/// <summary>
///     Base class for caches.
///     It's used to simplify implementing <see cref="ICache" />.
/// </summary>
public abstract class CacheBase : ICache
{
    private readonly AsyncLock asyncLock = new();

    private readonly object syncObj = new();

    /// <summary>
    ///     Constructor.
    /// </summary>
    /// <param name="name"></param>
    protected CacheBase(string name)
    {
        this.Name = name;
        this.DefaultSlidingExpireTime = TimeSpan.FromHours(1);

        this.Logger = NullLogger.Instance;
    }

    public ILogger Logger { get; init; }

    public string Name { get; }

    public TimeSpan DefaultSlidingExpireTime { get; set; }

    public TimeSpan? DefaultAbsoluteExpireTime { get; set; }

    public virtual object Get(string key, Func<string, object> factory)
    {
        object item = null;

        try
        {
            item = this.GetOrDefault(key);
        }
        catch (Exception ex)
        {
            this.Logger.LogError(ex.ToString(), ex);
        }

        if (item != null) return item;
        {
            lock (this.syncObj)
            {
                try
                {
                    item = this.GetOrDefault(key);
                }
                catch (Exception ex)
                {
                    this.Logger.LogError(ex.ToString(), ex);
                }

                if (item != null) return item;
                {
                    item = factory(key);

                    if (item == null) return null;

                    try
                    {
                        this.Set(key, item);
                    }
                    catch (Exception ex)
                    {
                        this.Logger.LogError(ex.ToString(), ex);
                    }
                }
            }
        }

        return item;
    }

    public virtual object[] Get(string[] keys, Func<string, object> factory)
    {
        object[] items = null;

        try
        {
            items = this.GetOrDefault(keys);
        }
        catch (Exception ex)
        {
            this.Logger.LogError(ex.ToString(), ex);
        }

        items ??= new object[keys.Length];

        if (!items.Any(i => i == null)) return items;
        {
            lock (this.syncObj)
            {
                try
                {
                    items = this.GetOrDefault(keys);
                }
                catch (Exception ex)
                {
                    this.Logger.LogError(ex.ToString(), ex);
                }

                var fetched = new List<KeyValuePair<string, object>>();
                for (var i = 0; i < items.Length; i++)
                {
                    var key = keys[i];
                    var value = items[i] ?? factory(key);

                    if (value != null) fetched.Add(new KeyValuePair<string, object>(key, value));
                }

                if (!fetched.Any()) return items;
                {
                    try
                    {
                        this.Set(fetched.ToArray());
                    }
                    catch (Exception ex)
                    {
                        this.Logger.LogError(ex.ToString(), ex);
                    }
                }
            }
        }

        return items;
    }

    public virtual async Task<object> GetAsync(string key, Func<string, Task<object>> factory)
    {
        object item = null;

        try
        {
            item = await this.GetOrDefaultAsync(key);
        }
        catch (Exception ex)
        {
            this.Logger.LogError(ex.ToString(), ex);
        }

        if (item == null)
            using (this.asyncLock)
            {
                try
                {
                    item = await this.GetOrDefaultAsync(key);
                }
                catch (Exception ex)
                {
                    this.Logger.LogError(ex.ToString(), ex);
                }

                if (item == null)
                {
                    item = await factory(key);

                    if (item == null) return null;

                    try
                    {
                        await this.SetAsync(key, item);
                    }
                    catch (Exception ex)
                    {
                        this.Logger.LogError(ex.ToString(), ex);
                    }
                }
            }

        return item;
    }

    public virtual async Task<object[]> GetAsync(string[] keys, Func<string, Task<object>> factory)
    {
        object[] items = null;

        try
        {
            items = await this.GetOrDefaultAsync(keys);
        }
        catch (Exception ex)
        {
            this.Logger.LogError(ex.ToString(), ex);
        }

        if (items == null) items = new object[keys.Length];

        if (items.Any(i => i == null))
            using (this.asyncLock)
            {
                try
                {
                    items = await this.GetOrDefaultAsync(keys);
                }
                catch (Exception ex)
                {
                    this.Logger.LogError(ex.ToString(), ex);
                }

                var fetched = new List<KeyValuePair<string, object>>();
                for (var i = 0; i < items.Length; i++)
                {
                    var key = keys[i];
                    var value = items[i];
                    if (value == null) value = factory(key);

                    if (value != null) fetched.Add(new KeyValuePair<string, object>(key, value));
                }

                if (fetched.Any())
                    try
                    {
                        await this.SetAsync(fetched.ToArray());
                    }
                    catch (Exception ex)
                    {
                        this.Logger.LogError(ex.ToString(), ex);
                    }
            }

        return items;
    }

    public abstract object GetOrDefault(string key);
    public abstract object GetOrDefault(Func<object, bool> predicate);

    public virtual object[] GetOrDefault(string[] keys)
    {
        return keys.Select(this.GetOrDefault).ToArray();
    }

    public abstract object[] GetMultipleOrDefault(Func<object, bool> predicate);

    public virtual Task<object> GetOrDefaultAsync(string key)
    {
        return Task.FromResult(this.GetOrDefault(key));
    }

    public virtual Task<object[]> GetOrDefaultAsync(string[] keys)
    {
        return Task.FromResult(this.GetOrDefault(keys));
    }

    public abstract void Set(string key, object value, TimeSpan? slidingExpireTime = null,
        TimeSpan? absoluteExpireTime = null);

    public virtual void Set(KeyValuePair<string, object>[] pairs, TimeSpan? slidingExpireTime = null,
        TimeSpan? absoluteExpireTime = null)
    {
        foreach (var pair in pairs) this.Set(pair.Key, pair.Value, slidingExpireTime, absoluteExpireTime);
    }

    public virtual Task SetAsync(string key, object value, TimeSpan? slidingExpireTime = null,
        TimeSpan? absoluteExpireTime = null)
    {
        this.Set(key, value, slidingExpireTime, absoluteExpireTime);
        return Task.FromResult(0);
    }

    public virtual Task SetAsync(KeyValuePair<string, object>[] pairs, TimeSpan? slidingExpireTime = null,
        TimeSpan? absoluteExpireTime = null)
    {
        return Task.WhenAll(pairs.Select(p => this.SetAsync(p.Key, p.Value, slidingExpireTime, absoluteExpireTime)));
    }

    public abstract void Remove(string key);

    public virtual void Remove(string[] keys)
    {
        foreach (var key in keys) this.Remove(key);
    }

    public virtual Task RemoveAsync(string key)
    {
        this.Remove(key);
        return Task.FromResult(0);
    }

    public virtual Task RemoveAsync(string[] keys)
    {
        return Task.WhenAll(keys.Select(this.RemoveAsync));
    }

    public abstract void Clear();

    public virtual Task ClearAsync()
    {
        this.Clear();
        return Task.FromResult(0);
    }

    public virtual void Dispose()
    {
    }
}