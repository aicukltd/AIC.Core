namespace AIC.Core.Identity.Data.Contracts;

using AIC.Core.Data.Contracts;
using AIC.Core.Identity.Tenants.Models.References.Contracts;

public interface ITenantAwareBaseEntity<TId> : IBaseEntity<TId>, IHasTenantId where TId : struct
{
}

public interface ITenantAwareBaseEntity : ITenantAwareBaseEntity<Guid>
{
}