namespace Bsg.EfCore.Utils
{
    using Context;
    using Domain;
    using Mapping;
    using Transactions;

    public interface IBulkInserterFactory
    {
        IBulkInserter<TEntity> BuildInserter<TEntity, TContext>(
            TableMapping<TEntity, TContext> mapping,
            IContextTransaction contextTransaction,
            int bufferSize,
            int timeout,
            bool preInitialise)
            where TEntity : class, IEntity<TContext>, new()
            where TContext : IDbContext;
    }
}