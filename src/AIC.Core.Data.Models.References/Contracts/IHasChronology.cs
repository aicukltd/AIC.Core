namespace AIC.Core.Data.Models.References.Contracts;

public interface IHasChronology
{
    DateTime Start { get; set; }
    DateTime End { get; set; }
    int Duration { get; set; }
}