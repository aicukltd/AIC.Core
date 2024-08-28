namespace AIC.Core.Caching.MicrosoftMemoryCache.Implementations;

using AIC.Core.Caching.Contracts;
using AIC.Core.Caching.Implementations;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

/// <summary>
///     Implements <see cref="ICache" /> to work with <see cref="MemoryCache" />.
/// </summary>
public class MicrosoftMemoryCache : CacheBase
{
    private IMemoryCache memoryCache;

    /// <summary>
    ///     Constructor.
    /// </summary>
    /// <param name="name">Unique name of the cache</param>
    public MicrosoftMemoryCache(string name)
        : base(name)
    {
        this.memoryCache = new MemoryCache(new OptionsWrapper<MemoryCacheOptions>(new MemoryCacheOptions()));
    }

    public override object GetOrDefault(string key)
    {
        return this.memoryCache.Get(key);
    }

    public override object GetOrDefault(Func<object, bool> predicate)
    {
        throw new NotImplementedException();
    }

    public override object[] GetMultipleOrDefault(Func<object, bool> predicate)
    {
        throw new NotImplementedException();
    }

    public override void Set(string key, object value, TimeSpan? slidingExpireTime = null,
        TimeSpan? absoluteExpireTime = null)
    {
        if (value == null) throw new Exception("Can not insert null values to the cache!");

        if (absoluteExpireTime != null)
            this.memoryCache.Set(key, value, DateTimeOffset.Now.Add(absoluteExpireTime.Value));
        else if (slidingExpireTime != null)
            this.memoryCache.Set(key, value, slidingExpireTime.Value);
        else if (this.DefaultAbsoluteExpireTime != null)
            this.memoryCache.Set(key, value, DateTimeOffset.Now.Add(this.DefaultAbsoluteExpireTime.Value));
        else
            this.memoryCache.Set(key, value, this.DefaultSlidingExpireTime);
    }

    public override void Remove(string key)
    {
        this.memoryCache.Remove(key);
    }

    public override void Clear()
    {
        this.memoryCache.Dispose();
        this.memoryCache = new MemoryCache(new OptionsWrapper<MemoryCacheOptions>(new MemoryCacheOptions()));
    }

    public override void Dispose()
    {
        this.memoryCache.Dispose();
        base.Dispose();
    }
}