namespace AIC.Core.Identity.Models.Contracts;

using AIC.Core.Data.Models.References.Contracts;
using AIC.Core.Identity.Models.Implementations;

public interface IHoliday : IHasChronology
{
    UserHolidayStatus Status { get; set; }
}