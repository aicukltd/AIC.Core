namespace AIC.Core.Identity.Models.Implementations;

using AIC.Core.Identity.Models.Contracts;

public class Holiday : IHoliday
{
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
    public int Duration { get; set; }
    public UserHolidayStatus Status { get; set; }
}