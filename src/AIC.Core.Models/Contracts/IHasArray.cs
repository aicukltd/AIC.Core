namespace AIC.Core.Models.Contracts;

public interface IHasArray<T>
{
    T[] Array { get; set; }
}