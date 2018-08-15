namespace Bsg.EfCore.Tests.Data.Repo
{
    using Context;
    using EfCore.Domain;
    using EfCore.Repo;

    public interface IPrimaryRepository<TEntity> : IBulkInsertRepository<TEntity, PrimaryContext>
        where TEntity : class, IEntity<PrimaryContext>, new()
    {
    }
}