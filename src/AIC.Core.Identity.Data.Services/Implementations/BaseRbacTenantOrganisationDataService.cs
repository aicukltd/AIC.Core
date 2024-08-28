namespace AIC.Core.Identity.Data.Services.Implementations;

using System.Linq.Expressions;
using System.Security;
using AIC.Core.Data.Contracts;
using AIC.Core.Data.Extensions.Expressions;
using AIC.Core.Identity.Data.Services.Contracts;
using AIC.Core.Identity.Models.Contracts;
using AIC.Core.Identity.Models.Implementations;
using AIC.Core.Identity.Tenants.Models.Contracts;

public abstract class BaseRbacTenantOrganisationDataService<T, TId> : BaseRbacOrganisationDataService<T, TId>,
    IRbacTenantOrganisationDataService<T, TId>
    where TId : struct
    where T : class, IBaseEntity<TId>
{
    protected BaseRbacTenantOrganisationDataService(IRepository<T, TId> repository) : base(repository)
    {
    }

    public async Task<T> GetByIdAsync(TId id, IUser user, IOrganisation organisation, ITenant tenant)
    {
        if (user == null) throw new ArgumentNullException(nameof(user));
        if (organisation == null) throw new ArgumentNullException(nameof(organisation));
        if (tenant == null) throw new ArgumentNullException(nameof(tenant));

        this.ThrowIfNotAuthorised(user, organisation, tenant);

        switch (user.Type)
        {
            case UserType.User:
            case UserType.Manager:
            {
                var entity = await base.GetByIdAsync(id, user);

                if (entity.TenantId != organisation.Id)
                    throw new SecurityException("The tenant does not have access to this model.");

                if (entity.OrganisationId != organisation.Id)
                    throw new SecurityException("The organisation does not have access to this model.");

                return entity;
            }
            case UserType.Administrator:
            case UserType.SuperAdministrator:
            case UserType.Root:
                return await base.GetByIdAsync(id, user);
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public async Task<T> GetByPredicateAsync(Expression<Func<T, bool>>? predicate, IUser user,
        IOrganisation organisation, ITenant tenant)
    {
        if (user == null) throw new ArgumentNullException(nameof(user));
        if (organisation == null) throw new ArgumentNullException(nameof(organisation));
        if (tenant == null) throw new ArgumentNullException(nameof(tenant));

        this.ThrowIfNotAuthorised(user, organisation, tenant);

        return user.Type switch
        {
            UserType.User => await base.GetByPredicateAsync(
                predicate.AndAlso(x => x.OrganisationId == organisation.Id && x.TenantId == tenant.Id),
                user),
            UserType.Manager => await base.GetByPredicateAsync(
                predicate.AndAlso(x => x.OrganisationId == organisation.Id && x.TenantId == tenant.Id), user),
            UserType.Administrator => await base.GetByPredicateAsync(predicate, user),
            UserType.SuperAdministrator => await base.GetByPredicateAsync(predicate, user),
            UserType.Root => await base.GetByPredicateAsync(predicate, user),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    public async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>>? predicate, IUser user,
        IOrganisation organisation, ITenant tenant)
    {
        if (user == null) throw new ArgumentNullException(nameof(user));
        if (organisation == null) throw new ArgumentNullException(nameof(organisation));
        if (tenant == null) throw new ArgumentNullException(nameof(tenant));

        this.ThrowIfNotAuthorised(user, organisation, tenant);

        return user.Type switch
        {
            UserType.User => await base.GetAllAsync(
                predicate.AndAlso(x => x.OrganisationId == organisation.Id && x.TenantId == tenant.Id), user),
            UserType.Manager => await base.GetAllAsync(
                predicate.AndAlso(x => x.OrganisationId == organisation.Id && x.TenantId == tenant.Id),
                user),
            UserType.Administrator => await base.GetAllAsync(predicate, user),
            UserType.SuperAdministrator => await base.GetAllAsync(predicate, user),
            UserType.Root => await base.GetAllAsync(predicate, user),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    public async Task<T> CreateOrUpdateAsync(T entity, IUser user, IOrganisation organisation, ITenant tenant)
    {
        if (user == null) throw new ArgumentNullException(nameof(user));
        if (organisation == null) throw new ArgumentNullException(nameof(organisation));
        if (tenant == null) throw new ArgumentNullException(nameof(tenant));

        this.ThrowIfNotAuthorised(user, organisation, tenant);

        entity.OrganisationId = organisation.Id;
        entity.TenantId = tenant.Id;

        return await base.CreateOrUpdateAsync(entity, user);
    }

    public async Task<T> CreateOrUpdateAsync(T entity, Expression<Func<T, bool>>? predicate, IUser user,
        IOrganisation organisation, ITenant tenant)
    {
        if (user == null) throw new ArgumentNullException(nameof(user));
        if (organisation == null) throw new ArgumentNullException(nameof(organisation));
        if (tenant == null) throw new ArgumentNullException(nameof(tenant));

        this.ThrowIfNotAuthorised(user, organisation, tenant);

        entity.OrganisationId = organisation.Id;
        entity.TenantId = tenant.Id;

        return await base.CreateOrUpdateAsync(entity,
            predicate.AndAlso(x => x.OrganisationId == organisation.Id && x.TenantId == tenant.Id), user);
    }

    public async Task<bool> DeleteAsync(T entity, IUser user, IOrganisation organisation, ITenant tenant)
    {
        if (user == null) throw new ArgumentNullException(nameof(user));
        if (organisation == null) throw new ArgumentNullException(nameof(organisation));
        if (tenant == null) throw new ArgumentNullException(nameof(tenant));

        this.ThrowIfNotAuthorised(user, organisation, tenant);

        return await base.DeleteAsync(entity, user, organisation);
    }

    public async Task<bool> DeleteByIdAsync(TId id, IUser user, IOrganisation organisation, ITenant tenant)
    {
        if (user == null) throw new ArgumentNullException(nameof(user));
        if (organisation == null) throw new ArgumentNullException(nameof(organisation));
        if (tenant == null) throw new ArgumentNullException(nameof(tenant));

        this.ThrowIfNotAuthorised(user, organisation, tenant);

        return await base.DeleteByIdAsync(id, user, organisation);
    }

    public async Task<long> CountAsync(Expression<Func<T, bool>>? predicate, IUser user, IOrganisation organisation,
        ITenant tenant)
    {
        if (user == null) throw new ArgumentNullException(nameof(user));
        if (organisation == null) throw new ArgumentNullException(nameof(organisation));

        this.ThrowIfNotAuthorised(user, organisation);

        return predicate == null
            ? await this.Repository.CountAsync(x =>
                x.OrganisationId == organisation.Id && x.TenantId == tenant.Id)
            : await this.Repository.CountAsync(predicate.AndAlso(x =>
                x.OrganisationId == organisation.Id && x.TenantId == tenant.Id));
    }

    public void ThrowIfNotAuthorised(IUser user, IOrganisation organisation, ITenant tenant,
        IdentityRole minimumIdentityRole = IdentityRole.Administrator)
    {
        if (user == null) throw new ArgumentNullException(nameof(user));
        if (organisation == null) throw new ArgumentNullException(nameof(organisation));
        if (tenant == null) throw new ArgumentNullException(nameof(tenant));

        if (tenant.Deleted == true)
            throw new UnauthorizedAccessException(
                "That tenant has been deleted or has not been correctly setup.");

        base.ThrowIfNotAuthorised(user, organisation, minimumIdentityRole);
    }
}