namespace Bsg.EfCore.Tests.Data.Repo
{
    using Context;
    using EfCore.Context;
    using EfCore.Domain;
    using EfCore.Repo;

    public class SimplePrimaryRepository<TEntity> : SimpleGenericRepository<TEntity, PrimaryContext>, ISimplePrimaryRepository<TEntity>
        where TEntity : class, IEntity<PrimaryContext>, new()
    {
        public SimplePrimaryRepository(
            IDbContextSession<PrimaryContext> session)
            : base(session)
        {
        }
    }
}
