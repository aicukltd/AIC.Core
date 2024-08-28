namespace AIC.Core.Caching.Contracts;

public interface ICacheItem<TModel>
{
    TModel Item { get; }
    DateTime Expiry { get; }
    bool HasExpired { get; }
    void Set(TModel model, TimeSpan lifeSpan);
}