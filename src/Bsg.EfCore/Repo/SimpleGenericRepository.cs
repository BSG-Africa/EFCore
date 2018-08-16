namespace Bsg.EfCore.Repo
{
    using Context;
    using Domain;

    public class SimpleGenericRepository<TEntity, TContext> : Repository<TEntity, TContext>, ISimpleGenericRepository<TEntity, TContext>
        where TEntity : class, IEntity<TContext>, new()
        where TContext : IDbContext
    {
        protected SimpleGenericRepository(
            IDbContextSession<TContext> session)
            : base(session)
        {
        }
    }
}
