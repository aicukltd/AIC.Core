namespace AIC.Core.Identity.Tenants.Models.Contracts;

using AIC.Core.Data.Contracts;
using AIC.Core.Identity.Tenants.Models.Implementations;

public interface ITenant : IBaseEntity
{
    string Name { get; set; }
    Guid OwnerUserId { get; set; }
    TenantStatus Status { get; set; }
    TenantType Type { get; set; }
}