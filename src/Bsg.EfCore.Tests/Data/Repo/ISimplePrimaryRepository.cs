namespace Bsg.EfCore.Tests.Data.Repo
{
    using Context;
    using EfCore.Domain;
    using EfCore.Repo;

    public interface ISimplePrimaryRepository<TEntity> : ISimpleGenericRepository<TEntity, PrimaryContext>
        where TEntity : class, IEntity<PrimaryContext>, new()
    {
    }
}