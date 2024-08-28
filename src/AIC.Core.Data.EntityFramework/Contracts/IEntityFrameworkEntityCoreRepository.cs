namespace AIC.Core.Data.EntityFramework.Contracts;

using AIC.Core.Data.Contracts;
using Microsoft.EntityFrameworkCore;

public interface IEntityFrameworkEntityCoreRepository<TEntity, in TId, TDbo> : IRepository<TEntity, TId>
    where TEntity : class, IHasId<TId>
    where TId : struct
    where TDbo : DbContext, new()
{
}