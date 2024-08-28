namespace AIC.Core.Models.Networking.Contracts;

public interface IHasData<T>
{
    T Data { get; set; }
}