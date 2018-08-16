namespace Bsg.EfCore.Repo
{
    using Context;
    using Domain;

    public interface ISimpleGenericRepository<TEntity, TContext> : IRepository<TEntity, TContext>
        where TEntity : class, IEntity<TContext>, new()
        where TContext : IDbContext
    {
    }
}