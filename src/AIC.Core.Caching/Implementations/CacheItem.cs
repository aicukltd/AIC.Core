namespace AIC.Core.Caching.Implementations;

using AIC.Core.Caching.Contracts;

public class CacheItem<TModel> : ICacheItem<TModel>
{
    public TModel Item { get; private set; }
    public DateTime Expiry { get; private set; }
    public bool HasExpired => this.Expiry < DateTime.UtcNow;

    public void Set(TModel model, TimeSpan lifeSpan)
    {
        this.Item = model;
        this.Expiry = DateTime.UtcNow + lifeSpan;
    }
}