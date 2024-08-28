namespace AIC.Core.Caching.Implementations;

using AIC.Core.Caching.Contracts;

internal class CacheConfigurator : ICacheConfigurator
{
    public CacheConfigurator(Action<ICache> initAction)
    {
        this.InitAction = initAction;
    }

    public CacheConfigurator(string cacheName, Action<ICache> initAction)
    {
        this.CacheName = cacheName;
        this.InitAction = initAction;
    }

    public string CacheName { get; }

    public Action<ICache> InitAction { get; }
}