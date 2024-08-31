// ***********************************************************************
// Assembly         : AIC.Data.Core
// Author           : OliverChristieAICUKL
// Created          : 11-25-2020
//
// Last Modified By : OliverChristieAICUKL
// Last Modified On : 11-25-2020
// ***********************************************************************
// <copyright file="BaseEntityFrameworkCoreRepository.cs" company="AIC.Data.Core">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************


/// <summary>
/// The Models namespace.
/// </summary>

namespace AIC.Core.Data.EntityFramework.Implementations;

using System.Linq.Expressions;
using System.Text.RegularExpressions;
using AIC.Core.Caching.Contracts;
using AIC.Core.Caching.Implementations;
using AIC.Core.Caching.InMemory.Implementations;
using AIC.Core.Data.Contracts;
using AIC.Core.Data.EntityFramework.Contracts;
using Microsoft.EntityFrameworkCore;

/// <summary>
///     Definition for <see cref="BaseEntityFrameworkCoreRepository{TEntity,TId,TDbo}" /> class.
/// </summary>
/// <typeparam name="TEntity">The type of the t entity.</typeparam>
/// <typeparam name="TId">The type of the tid.</typeparam>
/// <typeparam name="TDbo">The type of the tdbo.</typeparam>
public abstract class BaseEntityFrameworkCoreRepository<TEntity, TId, TDbo> :
    IEntityFrameworkEntityCoreRepository<TEntity, TId, TDbo>
    where TEntity : class, IHasId<TId>
    where TDbo : DbContext, new()
    where TId : struct
{
    private readonly ITypedCache<TId, TEntity> cache;

    #region Constructors

    /// <summary>
    ///     Initializes a new instance of the <see cref="BaseManagerManager{TEntity,TId,TDbo}" /> class.
    /// </summary>
    public BaseEntityFrameworkCoreRepository(TDbo context)
    {
        this.cache = new TypedCacheWrapper<TId, TEntity>(new InMemoryCache(nameof(TEntity)));
        this.ProtectEntities = false;
        this.Context = context;
    }

    #endregion

    #region Public Methods

    /// <summary>
    ///     Gets the identifier from entity.
    /// </summary>
    /// <param name="entity">The entity.</param>
    /// <returns>TId.</returns>
    /// <exception cref="ArgumentNullException">entity</exception>
    public TId GetIdFromEntity(TEntity entity)
    {
        if (entity == null) throw new ArgumentNullException(nameof(entity));

        var id =
            typeof(TEntity).GetProperties()
                .FirstOrDefault(x => x.PropertyType == typeof(Guid) && x.Name.ToLower() == "id");
        if (id == null) return new TId();
        return (TId)id.GetValue(entity, null);
    }

    #endregion

    #region Properties

    /// <summary>
    ///     Gets or sets a value indicating whether [protect entities].
    /// </summary>
    /// <value><c>true</c> if [protect entities]; otherwise, <c>false</c>.</value>
    public bool ProtectEntities { get; set; }

    public TDbo Context { get; set; }

    #endregion

    #region Private Methods

    /// <summary>
    ///     Wases the save changes task successful.
    /// </summary>
    /// <param name="dbContext">The database context.</param>
    /// <returns>Task&lt;System.Boolean&gt;.</returns>
    /// <exception cref="ArgumentNullException"></exception>
    private static async Task<bool> WasSaveChangesTaskSuccessful(TDbo dbContext)
    {
        if (dbContext == null)
            throw new ArgumentNullException();

        return await dbContext.SaveChangesAsync() != 0;
    }

    /// <summary>
    ///     Protects the entity.
    /// </summary>
    /// <param name="entity">The entity.</param>
    /// <returns>TEntity.</returns>
    private TEntity ProtectEntity(TEntity entity)
    {
        if (!this.ProtectEntities) return entity;

        if (entity == null) return null;

        var properties = entity.GetType().GetProperties();

        foreach (var prop in properties)
        {
            if (prop.PropertyType != typeof(string)) continue;
            var insecureValue = prop.GetValue(entity, null) as string;
        }

        return entity;
    }

    /// <summary>
    ///     Unprotects the entity.
    /// </summary>
    /// <param name="entity">The entity.</param>
    /// <returns>TEntity.</returns>
    private TEntity UnprotectEntity(TEntity entity)
    {
        if (!this.ProtectEntities) return entity;

        if (entity == null) return null;

        var properties = entity.GetType().GetProperties();

        foreach (var prop in properties)
        {
            if (prop.PropertyType != typeof(string)) continue;
            var insecureValue = prop.GetValue(entity, null) as string;
        }

        return entity;
    }

    /// <summary>
    ///     Determines whether [is string base64] [the specified input].
    /// </summary>
    /// <param name="input">The input.</param>
    /// <returns><c>true</c> if [is string base64] [the specified input]; otherwise, <c>false</c>.</returns>
    /// <exception cref="ArgumentNullException">input</exception>
    private bool IsStringBase64(string input)
    {
        if (string.IsNullOrEmpty(input)) throw new ArgumentNullException(nameof(input));
        var regex = new Regex("^([A-Za-z0-9+/]{4})*([A-Za-z0-9+/]{4}|[A-Za-z0-9+/]{3}=|[A-Za-z0-9+/]{2}==)$");
        return regex.IsMatch(input);
    }

    /// <summary>
    ///     Protects the expression.
    /// </summary>
    /// <param name="expression">The expression.</param>
    /// <returns>Expression&lt;Func&lt;TEntity, System.Boolean&gt;&gt;.</returns>
    /// <exception cref="ArgumentNullException">expression</exception>
    private Expression<Func<TEntity, bool>> ProtectExpression(Expression<Func<TEntity, bool>> expression)
    {
        if (expression == null) throw new ArgumentNullException(nameof(expression));

        return expression;
    }

    #endregion

    #region Interfaces

    /// <summary>
    ///     Creates the specified entity.
    /// </summary>
    /// <param name="entity">The entity.</param>
    /// <returns>Task&lt;TModel&gt;.</returns>
    /// <exception cref="ArgumentNullException"></exception>
    public virtual async Task<TEntity> CreateAsync(TEntity entity)
    {
        if (entity == null)
            throw new ArgumentNullException();

        var unmodifiedEntity = entity;

        var protectedEntity = this.ProtectEntity(entity);

        await using var dbContext = this.Context ?? new TDbo();
        dbContext.Entry(protectedEntity).State = EntityState.Added;

        var success =
            await BaseEntityFrameworkCoreRepository<TEntity, TId, TDbo>.WasSaveChangesTaskSuccessful(dbContext);

        if (!success) return null;

        await this.cache.SetAsync(entity.Id, unmodifiedEntity);

        return unmodifiedEntity;
    }

    /// <summary>
    ///     Creates the specified entity.
    /// </summary>
    /// <param name="entity">The entity.</param>
    /// <param name="existsPredicate">The exists predicate.</param>
    /// <returns>TEntity.</returns>
    /// <exception cref="ArgumentNullException"></exception>
    public virtual async Task<TEntity> CreateAsync(TEntity entity, Expression<Func<TEntity, bool>> existsPredicate)
    {
        if (entity == null)
            throw new ArgumentNullException();

        var existingItems = await this.ReadAllAsync(existsPredicate);

        if (existingItems.Any()) return null;

        return await this.CreateAsync(entity);
    }

    /// <summary>
    ///     Creates the specified entities.
    /// </summary>
    /// <param name="entities">The entities.</param>
    /// <returns>IEnumerable&lt;TEntity&gt;.</returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="System.ArgumentNullException"></exception>
    public virtual async Task<IEnumerable<TEntity>> CreateAsync(IEnumerable<TEntity> entities)
    {
        if (entities == null)
            throw new ArgumentNullException();

        var protectedEntities = entities.Select(this.ProtectEntity);

        await using var dbContext = this.Context ?? new TDbo();
        await dbContext.Set<TEntity>().AddRangeAsync(protectedEntities);

        var success =
            await BaseEntityFrameworkCoreRepository<TEntity, TId, TDbo>.WasSaveChangesTaskSuccessful(dbContext);

        if (!success) return null;

        var pairs = protectedEntities.Select(x => new KeyValuePair<TId, TEntity>(x.Id, x)).ToList();

        await this.cache.SetAsync(pairs.ToArray());

        return protectedEntities;
    }

    /// <summary>
    ///     Retrieves the specified identifier.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <returns>Task&lt;TModel&gt;.</returns>
    public virtual async Task<TEntity> ReadAsync(TId id)
    {
        await using var dbContext = this.Context ?? new TDbo();

        var result = await this.cache.GetOrDefaultAsync(id) ?? await dbContext.Set<TEntity>().FindAsync(id);
        return result == null ? result : this.UnprotectEntity(result);
    }

    /// <summary>
    ///     Retrieves the specified identifier.
    /// </summary>
    /// <param name="findPredicate">The find predicate.</param>
    /// <returns>Task&lt;TModel&gt;.</returns>
    /// <exception cref="ArgumentNullException"></exception>
    public virtual async Task<TEntity> ReadAsync(Expression<Func<TEntity, bool>> findPredicate)
    {
        if (findPredicate == null)
            throw new ArgumentNullException();

        await using var dbContext = this.Context ?? new TDbo();
        var result = await dbContext.Set<TEntity>().Where(findPredicate).FirstOrDefaultAsync();
        return result == null ? result : this.UnprotectEntity(result);
    }

    /// <summary>
    ///     Retrieves the specified identifier.
    /// </summary>
    /// <param name="findPredicate">The find predicate.</param>
    /// <param name="includes">The includes.</param>
    /// <returns>Task&lt;TModel&gt;.</returns>
    /// <exception cref="ArgumentNullException"></exception>
    public virtual async Task<TEntity> ReadAsync(Expression<Func<TEntity, bool>> findPredicate,
        params Expression<Func<TEntity, object>>[] includes)
    {
        if (findPredicate == null || includes.Any(i => i == null))
            throw new ArgumentNullException();

        await using var dbContext = this.Context ?? new TDbo();
        var query = dbContext.Set<TEntity>().Where(findPredicate).AsQueryable();

        query = includes.Aggregate(query, (current, include) => current.Include(include));

        var result = await query.FirstOrDefaultAsync();
        if (result == null) return result;
        return this.UnprotectEntity(result);
    }

    /// <summary>
    ///     Retrieves this instance.
    /// </summary>
    /// <returns>Task&lt;IEnumerable&lt;TModel&gt;&gt;.</returns>
    public virtual async Task<IEnumerable<TEntity>> ReadAllAsync()
    {
        await using var dbContext = this.Context ?? new TDbo();
        var resultSet = await dbContext.Set<TEntity>().ToListAsync();
        foreach (var entity in resultSet) this.UnprotectEntity(entity);
        return resultSet;
    }

    /// <summary>
    ///     Retrieves this instance.
    /// </summary>
    /// <param name="findPredicate">The find predicate.</param>
    /// <returns>Task&lt;IEnumerable&lt;TModel&gt;&gt;.</returns>
    /// <exception cref="ArgumentNullException"></exception>
    public virtual async Task<IEnumerable<TEntity>> ReadAllAsync(Expression<Func<TEntity, bool>> findPredicate)
    {
        if (findPredicate == null)
            throw new ArgumentNullException();

        await using var dbContext = this.Context ?? new TDbo();
        var resultSet = await dbContext.Set<TEntity>().Where(findPredicate).ToListAsync();
        foreach (var entity in resultSet) this.UnprotectEntity(entity);
        return resultSet;
    }

    /// <summary>
    ///     Reads all when they match the func.
    /// </summary>
    /// <param name="findPredicate">The find predicate.</param>
    /// <param name="includes">The includes.</param>
    /// <returns>Task&lt;IEnumerable&lt;TModel&gt;&gt;.</returns>
    /// <exception cref="ArgumentNullException"></exception>
    public virtual async Task<IEnumerable<TEntity>> ReadAllAsync(Expression<Func<TEntity, bool>> findPredicate,
        params Expression<Func<TEntity, object>>[] includes)
    {
        if (findPredicate == null || includes.Any(i => i == null))
            throw new ArgumentNullException();

        await using var dbContext = this.Context ?? new TDbo();
        var query = dbContext.Set<TEntity>().Where(findPredicate).AsQueryable();

        query = includes.Aggregate(query, (current, include) => current.Include(include));

        var resultSet = await query.ToListAsync();
        foreach (var entity in resultSet) this.UnprotectEntity(entity);
        return resultSet;
    }

    /// <summary>
    ///     Reads all when they match the func.
    /// </summary>
    /// <param name="includes">The includes.</param>
    /// <returns>Task&lt;IEnumerable&lt;TModel&gt;&gt;.</returns>
    /// <exception cref="ArgumentNullException"></exception>
    public virtual async Task<IEnumerable<TEntity>> ReadAllAsync(params Expression<Func<TEntity, object>>[] includes)
    {
        if (includes.Any(i => i == null))
            throw new ArgumentNullException();

        await using var dbContext = this.Context ?? new TDbo();
        var query = dbContext.Set<TEntity>().AsQueryable();

        query = includes.Aggregate(query, (current, include) => current.Include(include));

        var resultSet = await query.ToListAsync();
        foreach (var entity in resultSet) this.UnprotectEntity(entity);
        return resultSet;
    }

    /// <summary>
    ///     Reads all.
    /// </summary>
    /// <param name="page">The page.</param>
    /// <param name="count">The count.</param>
    /// <returns>IEnumerable&lt;TEntity&gt;.</returns>
    public virtual async Task<IEnumerable<TEntity>> ReadAllAsync(int page, int count)
    {
        await using var dbContext = this.Context ?? new TDbo();
        var skip = page > 1 ? page * count : 0;
        var resultSet = await dbContext.Set<TEntity>().Skip(skip).Take(count).ToListAsync();
        foreach (var entity in resultSet) this.UnprotectEntity(entity);
        return resultSet;
    }

    /// <summary>
    ///     Reads all.
    /// </summary>
    /// <param name="page">The page.</param>
    /// <param name="count">The count.</param>
    /// <param name="findPredicate">The find predicate.</param>
    /// <returns>IEnumerable&lt;TEntity&gt;.</returns>
    public virtual async Task<IEnumerable<TEntity>> ReadAllAsync(int page, int count,
        Expression<Func<TEntity, bool>> findPredicate)
    {
        await using var dbContext = this.Context ?? new TDbo();
        var skip = page > 1 ? page * count : 0;
        var resultSet = await dbContext.Set<TEntity>().Where(findPredicate).Skip(skip).Take(count).ToListAsync();
        foreach (var entity in resultSet) this.UnprotectEntity(entity);
        return resultSet;
    }

    /// <summary>
    ///     Reads all.
    /// </summary>
    /// <param name="page">The page.</param>
    /// <param name="count">The count.</param>
    /// <param name="includes">The includes.</param>
    /// <returns>IEnumerable&lt;TEntity&gt;.</returns>
    /// <exception cref="ArgumentNullException"></exception>
    public virtual async Task<IEnumerable<TEntity>> ReadAllAsync(int page, int count,
        params Expression<Func<TEntity, object>>[] includes)
    {
        if (includes.Any(i => i == null))
            throw new ArgumentNullException();

        await using var dbContext = this.Context ?? new TDbo();
        var query = dbContext.Set<TEntity>().AsQueryable();

        query = includes.Aggregate(query, (current, include) => current.Include(include));
        var skip = page > 1 ? page * count : 0;
        var resultSet = await query.Skip(skip).Take(count).ToListAsync();
        foreach (var entity in resultSet) this.UnprotectEntity(entity);
        return resultSet;
    }

    /// <summary>
    ///     Reads all.
    /// </summary>
    /// <param name="page">The page.</param>
    /// <param name="count">The count.</param>
    /// <param name="findPredicate">The find predicate.</param>
    /// <param name="includes">The includes.</param>
    /// <returns>IEnumerable&lt;TEntity&gt;.</returns>
    /// <exception cref="ArgumentNullException"></exception>
    public virtual async Task<IEnumerable<TEntity>> ReadAllAsync(int page, int count,
        Expression<Func<TEntity, bool>> findPredicate, params Expression<Func<TEntity, object>>[] includes)
    {
        if (findPredicate == null || includes.Any(i => i == null))
            throw new ArgumentNullException();

        await using var dbContext = this.Context ?? new TDbo();
        var query = dbContext.Set<TEntity>().Where(findPredicate).AsQueryable();

        query = includes.Aggregate(query, (current, include) => current.Include(include));
        var skip = page > 1 ? page * count : 0;
        var resultSet = await query.Skip(skip).Take(count).ToListAsync();
        foreach (var entity in resultSet) this.UnprotectEntity(entity);
        return resultSet;
    }

    /// <summary>
    ///     Reads all.
    /// </summary>
    /// <typeparam name="TOrderBy">The type of the t order by.</typeparam>
    /// <param name="page">The page.</param>
    /// <param name="count">The count.</param>
    /// <param name="orderBy">The order by.</param>
    /// <param name="descending">if set to <c>true</c> [descending].</param>
    /// <param name="includes">The includes.</param>
    /// <returns>IEnumerable&lt;TEntity&gt;.</returns>
    /// <exception cref="ArgumentNullException"></exception>
    public virtual async Task<IEnumerable<TEntity>> ReadAllAsync<TOrderBy>(int page, int count,
        Expression<Func<TEntity, TOrderBy>> orderBy, bool descending = false,
        params Expression<Func<TEntity, object>>[] includes)
    {
        if (includes.Any(i => i == null))
            throw new ArgumentNullException();

        await using var dbContext = this.Context ?? new TDbo();
        var query = dbContext.Set<TEntity>().AsQueryable();

        query = includes.Aggregate(query, (current, include) => current.Include(include));
        var skip = page > 1 ? page * count : 0;
        query = descending ? query.OrderByDescending(orderBy) : query.OrderBy(orderBy);
        var resultSet = await query.Skip(skip).Take(count).ToListAsync();
        foreach (var entity in resultSet) this.UnprotectEntity(entity);
        return resultSet;
    }

    /// <summary>
    ///     Reads all.
    /// </summary>
    /// <typeparam name="TOrderBy">The type of the t order by.</typeparam>
    /// <param name="page">The page.</param>
    /// <param name="count">The count.</param>
    /// <param name="findPredicate">The find predicate.</param>
    /// <param name="orderBy">The order by.</param>
    /// <param name="descending">if set to <c>true</c> [descending].</param>
    /// <param name="includes">The includes.</param>
    /// <returns>IEnumerable&lt;TEntity&gt;.</returns>
    /// <exception cref="ArgumentNullException"></exception>
    public virtual async Task<IEnumerable<TEntity>> ReadAllAsync<TOrderBy>(int page, int count,
        Expression<Func<TEntity, bool>> findPredicate, Expression<Func<TEntity, TOrderBy>> orderBy,
        bool descending = false,
        params Expression<Func<TEntity, object>>[] includes
    )
    {
        if (findPredicate == null || includes.Any(i => i == null))
            throw new ArgumentNullException();

        await using var dbContext = this.Context ?? new TDbo();
        var query = dbContext.Set<TEntity>().Where(findPredicate).AsQueryable();

        query = includes.Aggregate(query, (current, include) => current.Include(include));
        var skip = page > 1 ? page * count : 0;
        query = descending ? query.OrderByDescending(orderBy) : query.OrderBy(orderBy);
        var resultSet = await query.Skip(skip).Take(count).ToListAsync();
        foreach (var entity in resultSet) this.UnprotectEntity(entity);
        return resultSet;
    }

    /// <summary>
    ///     Reads all.
    /// </summary>
    /// <typeparam name="TOrderBy">The type of the t order by.</typeparam>
    /// <param name="page">The page.</param>
    /// <param name="count">The count.</param>
    /// <param name="findPredicate">The find predicate.</param>
    /// <param name="orderBy">The order by.</param>
    /// <param name="descending">if set to <c>true</c> [descending].</param>
    /// <returns>IEnumerable&lt;TEntity&gt;.</returns>
    /// <exception cref="ArgumentNullException"></exception>
    public virtual async Task<IEnumerable<TEntity>> ReadAllAsync<TOrderBy>(int page, int count,
        Expression<Func<TEntity, bool>> findPredicate,
        Expression<Func<TEntity, TOrderBy>> orderBy, bool descending = false)
    {
        if (findPredicate == null)
            throw new ArgumentNullException();

        await using var dbContext = this.Context ?? new TDbo();
        var query = dbContext.Set<TEntity>().Where(findPredicate).AsQueryable();
        query = descending ? query.OrderByDescending(orderBy) : query.OrderBy(orderBy);
        var skip = page > 1 ? page * count : 0;
        var resultSet = await query.Skip(skip).Take(count).ToListAsync();
        foreach (var entity in resultSet) this.UnprotectEntity(entity);
        return resultSet;
    }

    /// <summary>
    ///     Reads all when they match the func.
    /// </summary>
    /// <param name="findPredicate">The find predicate.</param>
    /// <param name="includes">The includes.</param>
    /// <returns>Task&lt;IEnumerable&lt;TModel&gt;&gt;.</returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="System.ArgumentNullException"></exception>
    public virtual async Task<IEnumerable<TEntity>> WhereAsync(Expression<Func<TEntity, bool>> findPredicate,
        params Expression<Func<TEntity, object>>[] includes)
    {
        if (includes.Any(i => i == null) || findPredicate == null)
            throw new ArgumentNullException();

        await using var dbContext = this.Context ?? new TDbo();
        var query = dbContext.Set<TEntity>().Where(findPredicate).AsQueryable();

        query = includes.Aggregate(query, (current, include) => current.Include(include));

        var resultSet = await query.ToListAsync();
        foreach (var entity in resultSet) this.UnprotectEntity(entity);
        return resultSet;
    }

    /// <summary>
    ///     Reads all when they match the func.
    /// </summary>
    /// <param name="findPredicate">The find predicate.</param>
    /// <returns>Task&lt;IEnumerable&lt;TModel&gt;&gt;.</returns>
    /// <exception cref="ArgumentNullException"></exception>
    public virtual async Task<IEnumerable<TEntity>> WhereAsync(Expression<Func<TEntity, bool>> findPredicate)
    {
        if (findPredicate == null)
            throw new ArgumentNullException();

        await using var dbContext = this.Context ?? new TDbo();
        var query = dbContext.Set<TEntity>().Where(findPredicate).AsQueryable();

        var resultSet = await query.ToListAsync();
        foreach (var entity in resultSet) this.UnprotectEntity(entity);
        return resultSet;
    }

    /// <summary>
    ///     Updates the specified entity.
    /// </summary>
    /// <param name="entity">The entity.</param>
    /// <returns>Task&lt;TModel&gt;.</returns>
    /// <exception cref="ArgumentNullException"></exception>
    public virtual async Task<TEntity> UpdateAsync(TEntity entity)
    {
        if (entity == null)
            throw new ArgumentNullException();

        var unmodifiedEntity = entity;

        var protectedEntity = this.ProtectEntity(entity);

        await using var dbContext = this.Context ?? new TDbo();
        dbContext.Entry(protectedEntity).State = EntityState.Modified;

        return await BaseEntityFrameworkCoreRepository<TEntity, TId, TDbo>.WasSaveChangesTaskSuccessful(dbContext)
            ? unmodifiedEntity
            : null;
    }

    /// <summary>
    ///     Creates the or update.
    /// </summary>
    /// <param name="entity">The entity.</param>
    /// <returns>Task&lt;TEntity&gt;.</returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task<TEntity> CreateOrUpdateAsync(TEntity entity)
    {
        var exists = await this.CountAsync(x => object.Equals(x.Id, entity.Id)) > 0;

        if (exists) return await this.UpdateAsync(entity);

        return await this.CreateAsync(entity);
    }

    /// <summary>
    ///     Creates the or update.
    /// </summary>
    /// <param name="entities">The entity.</param>
    /// <returns>Task&lt;IEnumerable&lt;TEntity&gt;&gt;.</returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task<IEnumerable<TEntity>> CreateOrUpdateAsync(IEnumerable<TEntity> entities)
    {
        var result = new List<TEntity>();

        foreach (var entity in entities) result.Add(await this.CreateOrUpdateAsync(entity));

        return result;
    }

    /// <summary>
    ///     CreateAsync Or UpdateAsync based on an existing predicate
    /// </summary>
    /// <param name="entity">The entity.</param>
    /// <param name="existsPredicate">The exists predicate.</param>
    /// <returns>Task&lt;TEntity&gt;.</returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task<TEntity> CreateOrUpdateAsync(TEntity entity, Expression<Func<TEntity, bool>> existsPredicate)
    {
        var exists = await this.ReadAsync(existsPredicate);

        if (exists == null) return await this.CreateAsync(entity);

        entity.Id = exists.Id;
        return await this.UpdateAsync(entity);
    }

    /// <summary>
    ///     CreateAsync Or UpdateAsync based on an existing predicate
    /// </summary>
    /// <param name="entities">The entities.</param>
    /// <param name="existsPredicate">The exists predicate.</param>
    /// <returns>Task&lt;IEnumerable&lt;TEntity&gt;&gt;.</returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task<IEnumerable<TEntity>> CreateOrUpdateAsync(IEnumerable<TEntity> entities,
        Expression<Func<TEntity, bool>> existsPredicate)
    {
        var result = new List<TEntity>();

        foreach (var entity in entities) result.Add(await this.CreateOrUpdateAsync(entity, existsPredicate));

        return result;
    }

    /// <summary>
    ///     Deletes the specified identifier.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    public virtual async Task<bool> DeleteAsync(TId id)
    {
        await using var dbContext = this.Context ?? new TDbo();
        var originalEntity = await dbContext.Set<TEntity>().FindAsync(new[] { id });
        if (originalEntity == null) return false;
        dbContext.Entry(originalEntity).State = EntityState.Deleted;
        return await BaseEntityFrameworkCoreRepository<TEntity, TId, TDbo>.WasSaveChangesTaskSuccessful(dbContext);
    }

    /// <summary>
    ///     Deletes the specified identifier.
    /// </summary>
    /// <param name="entity">The entity.</param>
    /// <returns>Task&lt;System.Boolean&gt;.</returns>
    public virtual async Task<bool> DeleteAsync(TEntity entity)
    {
        await using var dbContext = this.Context ?? new TDbo();
        dbContext.Entry(entity).State = EntityState.Deleted;
        return await BaseEntityFrameworkCoreRepository<TEntity, TId, TDbo>.WasSaveChangesTaskSuccessful(dbContext);
    }

    /// <summary>
    ///     Deletes the specified identifier.
    /// </summary>
    /// <param name="findPredicate">The find predicate.</param>
    /// <returns>Task&lt;System.Boolean&gt;.</returns>
    /// <exception cref="ArgumentNullException"></exception>
    public virtual async Task<bool> DeleteAsync(Expression<Func<TEntity, bool>> findPredicate)
    {
        if (findPredicate == null)
            throw new ArgumentNullException();

        await using var dbContext = this.Context ?? new TDbo();
        var originalEntitys = await dbContext.Set<TEntity>().Where(findPredicate).ToListAsync();
        if (originalEntitys == null || originalEntitys.Count <= 0) return false;
        foreach (var originalEntity in originalEntitys) dbContext.Entry(originalEntity).State = EntityState.Deleted;
        return await BaseEntityFrameworkCoreRepository<TEntity, TId, TDbo>.WasSaveChangesTaskSuccessful(dbContext);
    }

    /// <summary>
    ///     Gets the models.
    /// </summary>
    /// <returns>IEnumerable&lt;TEntity&gt;.</returns>
    public virtual async Task<IEnumerable<TEntity>> GetModelsAsync()
    {
        var models = (await this.ReadAllAsync()).ToList();

        if (!models.Any()) return models;

        foreach (var model in models) await this.BuildModelRelationshipsAsync(model);

        return models;
    }

    /// <summary>
    ///     Gets the models asynchronous.
    /// </summary>
    /// <param name="includes">The includes.</param>
    /// <returns>IEnumerable&lt;TEntity&gt;.</returns>
    public virtual async Task<IEnumerable<TEntity>> GetModelsAsync(params Expression<Func<TEntity, object>>[] includes)
    {
        var models = (await this.ReadAllAsync(includes)).ToList();

        if (!models.Any()) return models;

        foreach (var model in models) await this.BuildModelRelationshipsAsync(model);

        return models;
    }

    /// <summary>
    ///     Gets the models asynchronous.
    /// </summary>
    /// <param name="findPredicate">The find predicate.</param>
    /// <param name="includes">The includes.</param>
    /// <returns>IEnumerable&lt;TEntity&gt;.</returns>
    public virtual async Task<IEnumerable<TEntity>> GetModelsAsync(Expression<Func<TEntity, bool>> findPredicate,
        params Expression<Func<TEntity, object>>[] includes)
    {
        var models = (await this.ReadAllAsync(findPredicate, includes)).ToList();

        if (!models.Any()) return models;

        foreach (var model in models) await this.BuildModelRelationshipsAsync(model);

        return models;
    }

    /// <summary>
    ///     Gets the models asynchronous.
    /// </summary>
    /// <typeparam name="TOrderBy">The type of the order by.</typeparam>
    /// <param name="page">The page.</param>
    /// <param name="size">The size.</param>
    /// <param name="orderBy">The order by.</param>
    /// <param name="descending">if set to <c>true</c> [descending].</param>
    /// <returns>IEnumerable&lt;TEntity&gt;.</returns>
    public virtual async Task<IEnumerable<TEntity>> GetModelsAsync<TOrderBy>(int page, int size,
        Expression<Func<TEntity, TOrderBy>> orderBy, bool descending = false)
    {
        var models = (await this.ReadAllAsync(page, size, orderBy, descending)).ToList();

        if (!models.Any()) return models;

        foreach (var model in models) await this.BuildModelRelationshipsAsync(model);

        return models;
    }

    /// <summary>
    ///     Gets the models asynchronous.
    /// </summary>
    /// <typeparam name="TOrderBy">The type of the order by.</typeparam>
    /// <param name="page">The page.</param>
    /// <param name="size">The size.</param>
    /// <param name="orderBy">The order by.</param>
    /// <param name="descending">if set to <c>true</c> [descending].</param>
    /// <param name="includes">The includes.</param>
    /// <returns>IEnumerable&lt;TEntity&gt;.</returns>
    public virtual async Task<IEnumerable<TEntity>> GetModelsAsync<TOrderBy>(int page, int size,
        Expression<Func<TEntity, TOrderBy>> orderBy, bool descending = false,
        params Expression<Func<TEntity, object>>[] includes)
    {
        var models = (await this.ReadAllAsync(page, size, orderBy, descending, includes)).ToList();

        if (!models.Any()) return models;

        foreach (var model in models) await this.BuildModelRelationshipsAsync(model);

        return models;
    }

    /// <summary>
    ///     Gets the models asynchronous.
    /// </summary>
    /// <typeparam name="TOrderBy">The type of the order by.</typeparam>
    /// <param name="page">The page.</param>
    /// <param name="size">The size.</param>
    /// <param name="findPredicate">The find predicate.</param>
    /// <param name="orderBy">The order by.</param>
    /// <param name="descending">if set to <c>true</c> [descending].</param>
    /// <param name="includes">The includes.</param>
    /// <returns>IEnumerable&lt;TEntity&gt;.</returns>
    public virtual async Task<IEnumerable<TEntity>> GetModelsAsync<TOrderBy>(int page, int size,
        Expression<Func<TEntity, bool>> findPredicate, Expression<Func<TEntity, TOrderBy>> orderBy,
        bool descending = false, params Expression<Func<TEntity, object>>[] includes)
    {
        var models = (await this.ReadAllAsync(page, size, findPredicate, orderBy, descending, includes)).ToList();

        if (!models.Any()) return models;

        foreach (var model in models) await this.BuildModelRelationshipsAsync(model);

        return models;
    }

    /// <summary>
    ///     Gets the model.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <returns>TEntity.</returns>
    public virtual async Task<TEntity> GetModelAsync(TId id)
    {
        var model = await this.ReadAsync(id);

        if (model == null) return null;

        await this.BuildModelRelationshipsAsync(model);

        return model;
    }

    /// <summary>
    ///     Gets the model.
    /// </summary>
    /// <param name="findPredicate">The find predicate.</param>
    /// <param name="includes">The includes.</param>
    /// <returns>TEntity.</returns>
    public virtual async Task<TEntity> GetModelAsync(Expression<Func<TEntity, bool>> findPredicate,
        params Expression<Func<TEntity, object>>[] includes)
    {
        var model = await this.ReadAsync(findPredicate, includes);

        if (model == null) return null;

        await this.BuildModelRelationshipsAsync(model);

        return model;
    }

    /// <summary>
    ///     Gets or sets a value indicating whether [skip model relationships].
    /// </summary>
    /// <value><c>true</c> if [skip model relationships]; otherwise, <c>false</c>.</value>
    public bool SkipModelRelationships { get; set; }

    /// <summary>
    ///     Builds the model relationships.
    /// </summary>
    /// <param name="model">The model.</param>
    /// <returns>Task.</returns>
    /// <exception cref="NotImplementedException"></exception>
    public virtual async Task BuildModelRelationshipsAsync(TEntity model)
    {
    }

    /// <summary>
    ///     Searches the specified query.
    /// </summary>
    /// <typeparam name="TQuery">The type of the query.</typeparam>
    /// <typeparam name="TResponse">The type of the response.</typeparam>
    /// <param name="query">The query.</param>
    /// <returns>Task&lt;TResponse&gt;.</returns>
    /// <exception cref="NotImplementedException"></exception>
    public virtual async Task<TResponse> SearchAsync<TQuery, TResponse>(TQuery query)
        where TQuery : class where TResponse : class, new()
    {
        return new TResponse();
    }

    /// <summary>
    ///     Counts this instance.
    /// </summary>
    /// <returns>Task&lt;System.Int32&gt;.</returns>
    public virtual async Task<int> CountAsync()
    {
        await using var dbContext = this.Context ?? new TDbo();
        return await dbContext.Set<TEntity>().CountAsync();
    }

    /// <summary>
    ///     Counts the entity set using the specified find predicate.
    /// </summary>
    /// <param name="findPredicate">The find predicate.</param>
    /// <returns>Task&lt;System.Int32&gt;.</returns>
    /// <exception cref="ArgumentNullException"></exception>
    public virtual async Task<int> CountAsync(Expression<Func<TEntity, bool>> findPredicate)
    {
        if (findPredicate == null)
            throw new ArgumentNullException();

        await using var dbContext = this.Context ?? new TDbo();
        return await dbContext.Set<TEntity>().CountAsync(findPredicate);
    }

    #endregion
}