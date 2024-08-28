namespace AIC.Core.Caching.Implementations;

using System.Collections.Concurrent;
using System.Collections.Immutable;
using AIC.Core.Caching.Contracts;

/// <summary>
///     Base class for cache managers.
/// </summary>
public abstract class CacheManagerBase : ICacheManager
{
    protected readonly ConcurrentDictionary<string, ICache> Caches;
    protected readonly ICachingConfiguration Configuration;

    /// <summary>
    ///     Constructor.
    /// </summary>
    /// <param name="iocManager"></param>
    /// <param name="configuration"></param>
    protected CacheManagerBase(ICachingConfiguration configuration)
    {
        this.Configuration = configuration;
        this.Caches = new ConcurrentDictionary<string, ICache>();
    }

    public IReadOnlyList<ICache> GetAllCaches()
    {
        return this.Caches.Values.ToImmutableList();
    }

    public virtual ICache GetCache(string name)
    {
        return this.Caches.GetOrAdd(name, cacheName =>
        {
            var cache = this.CreateCacheImplementation(cacheName);

            var configurators =
                this.Configuration.Configurators.Where(c => c.CacheName == null || c.CacheName == cacheName);

            foreach (var configurator in configurators) configurator.InitAction?.Invoke(cache);

            return cache;
        });
    }

    public virtual void Dispose()
    {
        this.Caches.Clear();
    }

    /// <summary>
    ///     Used to create actual cache implementation.
    /// </summary>
    /// <param name="name">Name of the cache</param>
    /// <returns>Cache object</returns>
    protected abstract ICache CreateCacheImplementation(string name);
}