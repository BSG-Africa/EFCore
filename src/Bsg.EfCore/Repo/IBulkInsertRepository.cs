namespace Bsg.EfCore.Repo
{
    using System.Collections.Generic;
    using Context;
    using Domain;
    using Transactions;
    using Utils;

    public interface IBulkInsertRepository<TEntity, TContext> : IBulkEnabledRepository<TEntity, TContext>
        where TEntity : class, IEntity<TContext>, new()
        where TContext : IDbContext
    {
        int BulkAdd(TEntity item);

        int BulkAdd(IEnumerable<TEntity> items);

        int BulkAdd(IEnumerable<TEntity> items, int bufferSize);

        int BulkAdd(TEntity item, IContextTransaction contextTransaction);

        int BulkAdd(IEnumerable<TEntity> items, IContextTransaction contextTransaction);

        int BulkAdd(IEnumerable<TEntity> items, int bufferSize, IContextTransaction contextTransaction);

        IBulkInserter<TEntity> BuildInserter(int bufferSize, IContextTransaction contextTransaction);
    }
}