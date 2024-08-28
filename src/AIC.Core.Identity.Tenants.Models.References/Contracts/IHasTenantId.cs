namespace AIC.Core.Identity.Tenants.Models.References.Contracts;

public interface IHasTenantId
{
    Guid TenantId { get; set; }
}