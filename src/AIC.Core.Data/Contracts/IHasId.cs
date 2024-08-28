namespace AIC.Core.Data.Contracts;

public interface IHasId<TId>
    where TId : struct
{
    TId Id { get; set; }
}