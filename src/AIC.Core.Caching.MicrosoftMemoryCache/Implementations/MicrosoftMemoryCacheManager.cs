namespace AIC.Core.Caching.MicrosoftMemoryCache.Implementations;

using AIC.Core.Caching.Contracts;
using AIC.Core.Caching.Implementations;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

/// <summary>
///     Implements <see cref="ICacheManager" /> to work with MemoryCache.
/// </summary>
public class MicrosoftMemoryCacheManager : CacheManagerBase
{
    /// <summary>
    ///     Constructor.
    /// </summary>
    public MicrosoftMemoryCacheManager(ICachingConfiguration configuration)
        : base(configuration)
    {
        this.Logger = NullLogger.Instance;
    }

    private ILogger Logger { get; }

    protected override ICache CreateCacheImplementation(string name)
    {
        return new MicrosoftMemoryCache(name)
        {
            Logger = this.Logger
        };
    }
}