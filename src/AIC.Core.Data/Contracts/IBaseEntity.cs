namespace AIC.Core.Data.Contracts;

using AIC.Core.Data.Models.References.Contracts;
using AIC.Core.Identity.Tenants.Models.References.Contracts;

/// <summary>
///     Interface IBaseEntity
///     Implements the <see cref="IHasId{TId}" />
///     Implements the <see cref="IHasDisplayName" />
/// </summary>
/// <typeparam name="TId">The type of the t identifier.</typeparam>
/// <seealso cref="IHasId{TId}" />
/// <seealso cref="IHasDisplayName" />
public interface IBaseEntity<TId> : IHasId<TId>, IHasDisplayName, IHasOrganisationId, IHasRootUserId, IHasTenantId
    where TId : struct
{
    DateTime Created { get; set; }
    DateTime Updated { get; set; }
    bool? Deleted { get; set; }
}

/// <summary>
///     Interface IBaseEntity
///     Implements the <see cref="Guid" />
/// </summary>
/// <seealso cref="Guid" />
public interface IBaseEntity : IBaseEntity<Guid>
{
}