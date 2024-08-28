namespace AIC.Core.Models.Contracts;

public interface IHasEnumerable<T>
{
    IEnumerable<T> Enumerable { get; set; }
}