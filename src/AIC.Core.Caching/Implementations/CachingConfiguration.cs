namespace AIC.Core.Caching.Implementations;

using System.Collections.Immutable;
using AIC.Core.Caching.Contracts;

internal class CachingConfiguration : ICachingConfiguration
{
    private readonly List<ICacheConfigurator> configurators;

    public CachingConfiguration()
    {
        this.configurators = new List<ICacheConfigurator>();
    }

    public IReadOnlyList<ICacheConfigurator> Configurators => this.configurators.ToImmutableList();

    public void ConfigureAll(Action<ICache> initAction)
    {
        this.configurators.Add(new CacheConfigurator(initAction));
    }

    public void Configure(string cacheName, Action<ICache> initAction)
    {
        this.configurators.Add(new CacheConfigurator(cacheName, initAction));
    }
}