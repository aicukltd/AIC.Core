namespace AIC.Core.Identity.Data.Services.Implementations;

using System.Linq.Expressions;
using System.Security;
using AIC.Core.Data.Contracts;
using AIC.Core.Data.Extensions.Expressions;
using AIC.Core.Identity.Data.Services.Contracts;
using AIC.Core.Identity.Models.Contracts;
using AIC.Core.Identity.Models.Implementations;
using AIC.Core.Identity.Tenants.Models.Contracts;
using AIC.Core.Identity.Tenants.Models.References.Contracts;

public abstract class BaseRbacTenantDataService<T, TId> : BaseRbacDataService<T, TId>, IRbacTenantDataService<T, TId>
    where TId : struct
    where T : class, IHasTenantId, IBaseEntity<TId>
{
    protected BaseRbacTenantDataService(IRepository<T, TId> repository) : base(repository)
    {
    }

    public async Task<T> GetByIdAsync(TId id, IUser user, ITenant tenant)
    {
        if (user == null) throw new ArgumentNullException(nameof(user));
        if (tenant == null) throw new ArgumentNullException(nameof(tenant));

        this.ThrowIfNotAuthorised(user, tenant);

        switch (user.Type)
        {
            case UserType.User:
            case UserType.Manager:
            {
                var entity = await base.GetByIdAsync(id, user);

                if (entity.TenantId != tenant.Id)
                    throw new SecurityException("The tenant does not have access to this model.");

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

    public async Task<T> GetByPredicateAsync(Expression<Func<T, bool>>? predicate, IUser user, ITenant tenant)
    {
        if (user == null) throw new ArgumentNullException(nameof(user));
        if (tenant == null) throw new ArgumentNullException(nameof(tenant));

        this.ThrowIfNotAuthorised(user, tenant);

        return user.Type switch
        {
            UserType.User => await base.GetByPredicateAsync(predicate.AndAlso(x => x.TenantId == tenant.Id), user),
            UserType.Manager => await base.GetByPredicateAsync(predicate.AndAlso(x => x.TenantId == tenant.Id), user),
            UserType.Administrator => await base.GetByPredicateAsync(predicate, user),
            UserType.SuperAdministrator => await base.GetByPredicateAsync(predicate, user),
            UserType.Root => await base.GetByPredicateAsync(predicate, user),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    public async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>>? predicate, IUser user, ITenant tenant)
    {
        if (user == null) throw new ArgumentNullException(nameof(user));
        if (tenant == null) throw new ArgumentNullException(nameof(tenant));

        this.ThrowIfNotAuthorised(user, tenant);

        return user.Type switch
        {
            UserType.User => await base.GetAllAsync(predicate.AndAlso(x => x.TenantId == tenant.Id), user),
            UserType.Manager => await base.GetAllAsync(predicate.AndAlso(x => x.TenantId == tenant.Id), user),
            UserType.Administrator => await base.GetAllAsync(predicate, user),
            UserType.SuperAdministrator => await base.GetAllAsync(predicate, user),
            UserType.Root => await base.GetAllAsync(predicate, user),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    public async Task<T> CreateOrUpdateAsync(T entity, IUser user, ITenant tenant)
    {
        if (user == null) throw new ArgumentNullException(nameof(user));
        if (tenant == null) throw new ArgumentNullException(nameof(tenant));

        this.ThrowIfNotAuthorised(user, tenant);

        entity.TenantId = tenant.Id;

        return await base.CreateOrUpdateAsync(entity, user);
    }

    public async Task<T> CreateOrUpdateAsync(T entity, Expression<Func<T, bool>>? predicate, IUser user, ITenant tenant)
    {
        if (user == null) throw new ArgumentNullException(nameof(user));
        if (tenant == null) throw new ArgumentNullException(nameof(tenant));

        this.ThrowIfNotAuthorised(user, tenant);

        entity.TenantId = tenant.Id;

        return await base.CreateOrUpdateAsync(entity, predicate.AndAlso(x => x.TenantId == tenant.Id), user);
    }

    public async Task<bool> DeleteAsync(T entity, IUser user, ITenant tenant)
    {
        if (user == null) throw new ArgumentNullException(nameof(user));
        if (tenant == null) throw new ArgumentNullException(nameof(tenant));

        this.ThrowIfNotAuthorised(user, tenant);

        return await base.DeleteAsync(entity, user);
    }

    public async Task<bool> DeleteByIdAsync(TId id, IUser user, ITenant tenant)
    {
        if (user == null) throw new ArgumentNullException(nameof(user));
        if (tenant == null) throw new ArgumentNullException(nameof(tenant));

        this.ThrowIfNotAuthorised(user, tenant);

        return await base.DeleteByIdAsync(id, user);
    }

    public async Task<long> CountAsync(Expression<Func<T, bool>>? predicate, IUser user, ITenant tenant)
    {
        if (user == null) throw new ArgumentNullException(nameof(user));
        if (tenant == null) throw new ArgumentNullException(nameof(tenant));

        this.ThrowIfNotAuthorised(user, tenant);

        return predicate == null
            ? await this.Repository.CountAsync(x =>
                x.TenantId == tenant.Id)
            : await this.Repository.CountAsync(predicate.AndAlso(x =>
                x.TenantId == tenant.Id));
    }

    public void ThrowIfNotAuthorised(IUser user, ITenant tenant,
        IdentityRole minimumIdentityRole = IdentityRole.Administrator)
    {
        if (user == null) throw new ArgumentNullException(nameof(user));
        if (tenant == null) throw new ArgumentNullException(nameof(tenant));

        if (tenant.Deleted == true)
            throw new UnauthorizedAccessException(
                "That tenant has been deleted or has not been correctly setup.");

        base.ThrowIfNotAuthorised(user, minimumIdentityRole);
    }
}