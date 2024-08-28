namespace AIC.Core.Models.Contracts;

public interface IHasCollection<T>
{
    ICollection<T> Collection { get; set; }
}