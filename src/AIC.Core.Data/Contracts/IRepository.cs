namespace AIC.Core.Data.Contracts;

using System.Linq.Expressions;

/// <summary>
///     A repository interface that is loosley contracted to EF.
/// </summary>
/// <typeparam name="TEntity"></typeparam>
/// <typeparam name="TId">The type of the identifier.</typeparam>
/// <typeparam name="TDbo">The type of the dbo.</typeparam>
public interface IRepository<TEntity, in TId>
    where TEntity : class
    where TId : struct
{
    #region Public Methods

    /// <summary>
    ///     Creates the or update.
    /// </summary>
    /// <param name="entity">The entity.</param>
    /// <returns></returns>
    Task<TEntity> CreateOrUpdateAsync(TEntity entity);

    /// <summary>
    ///     Creates the or update.
    /// </summary>
    /// <param name="entities">The entity.</param>
    /// <returns></returns>
    Task<IEnumerable<TEntity>> CreateOrUpdateAsync(IEnumerable<TEntity> entities);

    /// <summary>
    ///     CreateAsync Or UpdateAsync based on an existing predicate
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="existsPredicate"></param>
    /// <returns></returns>
    Task<TEntity> CreateOrUpdateAsync(TEntity entity, Expression<Func<TEntity, bool>> existsPredicate);

    /// <summary>
    ///     CreateAsync Or UpdateAsync based on an existing predicate
    /// </summary>
    /// <param name="entities"></param>
    /// <param name="existsPredicate"></param>
    /// <returns></returns>
    Task<IEnumerable<TEntity>> CreateOrUpdateAsync(IEnumerable<TEntity> entities,
        Expression<Func<TEntity, bool>> existsPredicate);

    /// <summary>
    ///     Gets the models.
    /// </summary>
    /// <returns></returns>
    Task<IEnumerable<TEntity>> GetModelsAsync();

    /// <summary>
    ///     Gets the model.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <returns></returns>
    Task<TEntity> GetModelAsync(TId id);


    /// <summary>
    ///     Gets or sets a value indicating whether [skip model relationships].
    /// </summary>
    /// <value>
    ///     <c>true</c> if [skip model relationships]; otherwise, <c>false</c>.
    /// </value>
    bool SkipModelRelationships { get; set; }


    /// <summary>
    ///     Gets the model.
    /// </summary>
    /// <param name="findPredicate">The find predicate.</param>
    /// <param name="includes"></param>
    /// <returns></returns>
    Task<TEntity> GetModelAsync(Expression<Func<TEntity, bool>> findPredicate,
        params Expression<Func<TEntity, object>>[] includes);


    /// <summary>
    ///     Gets the models asynchronous.
    /// </summary>
    /// <param name="includes">The includes.</param>
    /// <returns></returns>
    Task<IEnumerable<TEntity>> GetModelsAsync(params Expression<Func<TEntity, object>>[] includes);

    /// <summary>
    ///     Gets the models asynchronous.
    /// </summary>
    /// <param name="findPredicate">The find predicate.</param>
    /// <param name="includes"></param>
    /// <returns></returns>
    Task<IEnumerable<TEntity>> GetModelsAsync(Expression<Func<TEntity, bool>> findPredicate,
        params Expression<Func<TEntity, object>>[] includes);

    /// <summary>
    ///     Gets the models asynchronous.
    /// </summary>
    /// <typeparam name="TOrderBy">The type of the order by.</typeparam>
    /// <param name="page">The page.</param>
    /// <param name="size">The size.</param>
    /// <param name="orderBy">The order by.</param>
    /// <returns></returns>
    Task<IEnumerable<TEntity>> GetModelsAsync<TOrderBy>(int page, int size, Expression<Func<TEntity, TOrderBy>> orderBy,
        bool descending = false);

    /// <summary>
    ///     Gets the models asynchronous.
    /// </summary>
    /// <typeparam name="TOrderBy">The type of the order by.</typeparam>
    /// <param name="page">The page.</param>
    /// <param name="size">The size.</param>
    /// <param name="orderBy">The order by.</param>
    /// <param name="includes">The includes.</param>
    /// <returns></returns>
    Task<IEnumerable<TEntity>> GetModelsAsync<TOrderBy>(int page, int size, Expression<Func<TEntity, TOrderBy>> orderBy,
        bool descending = false, params Expression<Func<TEntity, object>>[] includes);

    /// <summary>
    ///     Gets the models asynchronous.
    /// </summary>
    /// <typeparam name="TOrderBy">The type of the order by.</typeparam>
    /// <param name="page">The page.</param>
    /// <param name="size">The size.</param>
    /// <param name="findPredicate">The find predicate.</param>
    /// <param name="orderBy">The order by.</param>
    /// <param name="includes">The includes.</param>
    /// <returns></returns>
    Task<IEnumerable<TEntity>> GetModelsAsync<TOrderBy>(int page, int size,
        Expression<Func<TEntity, bool>> findPredicate, Expression<Func<TEntity, TOrderBy>> orderBy,
        bool descending = false, params Expression<Func<TEntity, object>>[] includes);

    /// <summary>
    ///     Builds the model relationships.
    /// </summary>
    /// <param name="model">The model.</param>
    /// <returns></returns>
    Task BuildModelRelationshipsAsync(TEntity model);

    /// <summary>
    ///     Searches the specified query.
    /// </summary>
    /// <typeparam name="TQuery">The type of the query.</typeparam>
    /// <typeparam name="TResponse">The type of the response.</typeparam>
    /// <param name="query">The query.</param>
    /// <returns></returns>
    Task<TResponse> SearchAsync<TQuery, TResponse>(TQuery query) where TQuery : class where TResponse : class, new();

    /// <summary>
    ///     Deletes the specified identifier.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <returns></returns>
    Task<bool> DeleteAsync(TId id);

    /// <summary>
    ///     Deletes the specified identifier.
    /// </summary>
    /// <param name="entity">The entity.</param>
    /// <returns>Task&lt;System.Boolean&gt;.</returns>
    Task<bool> DeleteAsync(TEntity entity);

    /// <summary>
    ///     Deletes the specified identifier.
    /// </summary>
    /// <param name="findPredicate">The find predicate.</param>
    /// <returns>Task&lt;System.Boolean&gt;.</returns>
    Task<bool> DeleteAsync(Expression<Func<TEntity, bool>> findPredicate);

    /// <summary>
    ///     Counts this instance.
    /// </summary>
    /// <returns>Task&lt;System.Int32&gt;.</returns>
    Task<int> CountAsync();

    /// <summary>
    ///     Counts the specified find predicate.
    /// </summary>
    /// <param name="findPredicate">The find predicate.</param>
    /// <returns>Task&lt;System.Int32&gt;.</returns>
    Task<int> CountAsync(Expression<Func<TEntity, bool>> findPredicate);

    #endregion
}