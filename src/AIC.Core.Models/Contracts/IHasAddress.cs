namespace AIC.Core.Models.Contracts;

public interface IHasAddress<TAddress>
{
    TAddress Address { get; set; }
}