namespace AIC.Core.Identity.Data.Services.Implementations;

using System.Linq.Expressions;
using System.Security;
using AIC.Core.Data.Contracts;
using AIC.Core.Data.Extensions.Expressions;
using AIC.Core.Data.Services.Implementations;
using AIC.Core.Identity.Data.Services.Contracts;
using AIC.Core.Identity.Extensions;
using AIC.Core.Identity.Models.Contracts;
using AIC.Core.Identity.Models.Implementations;

public abstract class BaseRbacDataService<T, TId> : BaseDataService<T, TId>, IRbacDataService<T, TId>
    where TId : struct
    where T : class, IBaseEntity<TId>, IHasId<TId>
{
    protected BaseRbacDataService(IRepository<T, TId> repository) : base(repository)
    {
    }

    public void ThrowIfNotAuthorised(IUser user, IdentityRole minimumIdentityRole = IdentityRole.Administrator)
    {
        if (user.GetIdentityRole() < minimumIdentityRole)
            throw new SecurityException("You are not authorised to create, update or delete data.");
    }

    public async Task<T> GetByIdAsync(TId id, IUser user)
    {
        if (user == null) throw new ArgumentNullException(nameof(user));

        this.ThrowIfNotAuthorised(user);

        switch (user.Type)
        {
            case UserType.User:
            case UserType.Manager:
            {
                var model = await base.GetByIdAsync(id);

                if (model.RootUserId != user.Id)
                    throw new SecurityException("The user is not authorised to access this model.");

                return model;
            }
            case UserType.Administrator:
            case UserType.SuperAdministrator:
            case UserType.Root:
                return await base.GetByIdAsync(id);
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public async Task<T> GetByPredicateAsync(Expression<Func<T, bool>>? predicate, IUser user)
    {
        if (user == null) throw new ArgumentNullException(nameof(user));

        this.ThrowIfNotAuthorised(user);

        return user.Type switch
        {
            UserType.User => await base.GetByPredicateAsync(predicate.AndAlso(x => x.RootUserId == user.Id)),
            UserType.Manager => await base.GetByPredicateAsync(predicate.AndAlso(x => x.RootUserId == user.Id)),
            UserType.Administrator => await base.GetByPredicateAsync(predicate),
            UserType.SuperAdministrator => await base.GetByPredicateAsync(predicate),
            UserType.Root => await base.GetByPredicateAsync(predicate),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    public async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>>? predicate, IUser user)
    {
        if (user == null) throw new ArgumentNullException(nameof(user));

        this.ThrowIfNotAuthorised(user);
        return user.Type switch
        {
            UserType.User => await base.GetAllAsync(predicate.AndAlso(x => x.RootUserId == user.Id)),
            UserType.Manager => await base.GetAllAsync(predicate.AndAlso(x => x.RootUserId == user.Id)),
            UserType.Administrator => await base.GetAllAsync(predicate),
            UserType.SuperAdministrator => await base.GetAllAsync(predicate),
            UserType.Root => await base.GetAllAsync(predicate),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    public async Task<T> CreateOrUpdateAsync(T entity, IUser user)
    {
        if (user == null) throw new ArgumentNullException(nameof(user));

        this.ThrowIfNotAuthorised(user);

        entity.RootUserId = user.Id;

        return await base.CreateOrUpdateAsync(entity);
    }

    public async Task<T> CreateOrUpdateAsync(T entity, Expression<Func<T, bool>>? predicate, IUser user)
    {
        if (user == null) throw new ArgumentNullException(nameof(user));

        this.ThrowIfNotAuthorised(user);

        entity.RootUserId = user.Id;

        return user.Type switch
        {
            UserType.User => await base.CreateOrUpdateAsync(entity, predicate.AndAlso(x => x.RootUserId == user.Id)),
            UserType.Manager => await base.CreateOrUpdateAsync(entity, predicate.AndAlso(x => x.RootUserId == user.Id)),
            UserType.Administrator => await base.CreateOrUpdateAsync(entity, predicate),
            UserType.SuperAdministrator => await base.CreateOrUpdateAsync(entity, predicate),
            UserType.Root => await base.CreateOrUpdateAsync(entity, predicate),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    public async Task<bool> DeleteAsync(T entity, IUser user)
    {
        if (user == null) throw new ArgumentNullException(nameof(user));

        this.ThrowIfNotAuthorised(user);

        switch (user.Type)
        {
            case UserType.User:
            case UserType.Manager:
            {
                entity.Deleted = true;
                await this.CreateOrUpdateAsync(entity, user);
                return true;
            }
            case UserType.Administrator:
            case UserType.SuperAdministrator:
            case UserType.Root:
                return await base.DeleteAsync(entity);
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public async Task<bool> DeleteByIdAsync(TId id, IUser user)
    {
        if (user == null) throw new ArgumentNullException(nameof(user));

        this.ThrowIfNotAuthorised(user);

        switch (user.Type)
        {
            case UserType.User:
            case UserType.Manager:
            {
                var entity = await this.GetByIdAsync(id, user);
                entity.Deleted = true;
                await this.CreateOrUpdateAsync(entity, user);
                return true;
            }
            case UserType.Administrator:
            case UserType.SuperAdministrator:
            case UserType.Root:
                return await base.DeleteByIdAsync(id);
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public async Task<long> CountAsync(Expression<Func<T, bool>>? predicate, IUser user)
    {
        if (user == null) throw new ArgumentNullException(nameof(user));

        this.ThrowIfNotAuthorised(user);

        return predicate == null
            ? await this.Repository.CountAsync(x =>
                x.RootUserId == user.Id)
            : await this.Repository.CountAsync(predicate.AndAlso(x =>
                x.RootUserId == user.Id));
    }
}