namespace AIC.Core.Caching.InMemory.Implementations;

using AIC.Core.Caching.Contracts;
using AIC.Core.Caching.Implementations;
using Microsoft.Extensions.Logging.Abstractions;

public class InMemoryCacheManager : CacheManagerBase
{
    public InMemoryCacheManager(ICachingConfiguration configuration) : base(configuration)
    {
        this.Logger = NullLogger.Instance;
    }

    private NullLogger Logger { get; }

    protected override ICache CreateCacheImplementation(string name)
    {
        return new InMemoryCache(name)
        {
            Logger = this.Logger
        };
    }
}