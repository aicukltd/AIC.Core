namespace AIC.Core.Identity.Data.Services.Implementations;

using System.Linq.Expressions;
using System.Security;
using AIC.Core.Data.Contracts;
using AIC.Core.Data.Extensions.Expressions;
using AIC.Core.Data.Models.References.Contracts;
using AIC.Core.Identity.Data.Services.Contracts;
using AIC.Core.Identity.Models.Contracts;
using AIC.Core.Identity.Models.Implementations;

public abstract class BaseRbacOrganisationDataService<T, TId> : BaseRbacDataService<T, TId>,
    IRbacOrganisationDataService<T, TId>
    where TId : struct
    where T : class, IHasOrganisationId, IBaseEntity<TId>
{
    protected BaseRbacOrganisationDataService(IRepository<T, TId> repository) : base(repository)
    {
    }

    public async Task<T> GetByIdAsync(TId id, IUser user, IOrganisation organisation)
    {
        if (user == null) throw new ArgumentNullException(nameof(user));
        if (organisation == null) throw new ArgumentNullException(nameof(organisation));

        this.ThrowIfNotAuthorised(user, organisation);

        switch (user.Type)
        {
            case UserType.User:
            case UserType.Manager:
            {
                var entity = await base.GetByIdAsync(id, user);

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
        IOrganisation organisation)
    {
        if (user == null) throw new ArgumentNullException(nameof(user));
        if (organisation == null) throw new ArgumentNullException(nameof(organisation));

        this.ThrowIfNotAuthorised(user, organisation);

        return user.Type switch
        {
            UserType.User => await base.GetByPredicateAsync(predicate.AndAlso(x => x.OrganisationId == organisation.Id),
                user),
            UserType.Manager => await base.GetByPredicateAsync(
                predicate.AndAlso(x => x.OrganisationId == organisation.Id), user),
            UserType.Administrator => await base.GetByPredicateAsync(predicate, user),
            UserType.SuperAdministrator => await base.GetByPredicateAsync(predicate, user),
            UserType.Root => await base.GetByPredicateAsync(predicate, user),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    public async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>>? predicate, IUser user,
        IOrganisation organisation)
    {
        if (user == null) throw new ArgumentNullException(nameof(user));
        if (organisation == null) throw new ArgumentNullException(nameof(organisation));

        this.ThrowIfNotAuthorised(user, organisation);

        return user.Type switch
        {
            UserType.User => await base.GetAllAsync(predicate.AndAlso(x => x.OrganisationId == organisation.Id), user),
            UserType.Manager => await base.GetAllAsync(predicate.AndAlso(x => x.OrganisationId == organisation.Id),
                user),
            UserType.Administrator => await base.GetAllAsync(predicate, user),
            UserType.SuperAdministrator => await base.GetAllAsync(predicate, user),
            UserType.Root => await base.GetAllAsync(predicate, user),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    public async Task<T> CreateOrUpdateAsync(T entity, IUser user, IOrganisation organisation)
    {
        if (user == null) throw new ArgumentNullException(nameof(user));
        if (organisation == null) throw new ArgumentNullException(nameof(organisation));

        this.ThrowIfNotAuthorised(user, organisation);

        entity.OrganisationId = organisation.Id;

        return await base.CreateOrUpdateAsync(entity, user);
    }

    public async Task<T> CreateOrUpdateAsync(T entity, Expression<Func<T, bool>>? predicate, IUser user,
        IOrganisation organisation)
    {
        if (user == null) throw new ArgumentNullException(nameof(user));
        if (organisation == null) throw new ArgumentNullException(nameof(organisation));

        this.ThrowIfNotAuthorised(user, organisation);

        entity.OrganisationId = organisation.Id;

        return await base.CreateOrUpdateAsync(entity, predicate.AndAlso(x => x.OrganisationId == organisation.Id),
            user);
    }

    public async Task<bool> DeleteAsync(T entity, IUser user, IOrganisation organisation)
    {
        if (user == null) throw new ArgumentNullException(nameof(user));
        if (organisation == null) throw new ArgumentNullException(nameof(organisation));

        this.ThrowIfNotAuthorised(user, organisation);

        return await base.DeleteAsync(entity, user);
    }

    public async Task<bool> DeleteByIdAsync(TId id, IUser user, IOrganisation organisation)
    {
        if (user == null) throw new ArgumentNullException(nameof(user));
        if (organisation == null) throw new ArgumentNullException(nameof(organisation));

        this.ThrowIfNotAuthorised(user, organisation);

        return await base.DeleteByIdAsync(id, user);
    }

    public async Task<long> CountAsync(Expression<Func<T, bool>>? predicate, IUser user, IOrganisation organisation)
    {
        if (user == null) throw new ArgumentNullException(nameof(user));
        if (organisation == null) throw new ArgumentNullException(nameof(organisation));

        this.ThrowIfNotAuthorised(user, organisation);

        return predicate == null
            ? await this.Repository.CountAsync(x =>
                x.OrganisationId == organisation.Id)
            : await this.Repository.CountAsync(predicate.AndAlso(x =>
                x.OrganisationId == organisation.Id));
    }

    public void ThrowIfNotAuthorised(IUser user, IOrganisation organisation,
        IdentityRole minimumIdentityRole = IdentityRole.Administrator)
    {
        if (user == null) throw new ArgumentNullException(nameof(user));
        if (organisation == null) throw new ArgumentNullException(nameof(organisation));

        if (!organisation.HasBeenSetup || organisation.Deleted == true)
            throw new UnauthorizedAccessException(
                "That organisation has been deleted or has not been correctly setup.");

        base.ThrowIfNotAuthorised(user, minimumIdentityRole);
    }
}