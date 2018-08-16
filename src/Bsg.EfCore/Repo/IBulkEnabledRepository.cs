namespace Bsg.EfCore.Repo
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using Context;
    using Domain;
    using Transactions;

    public interface IBulkEnabledRepository<TEntity, TContext> : IRepository<TEntity, TContext>
        where TEntity : class, IEntity<TContext>, new()
        where TContext : IDbContext
    {
        int Truncate();

        int Truncate(IContextTransaction contextTransaction);

        int TruncateWithForeignKeys();

        int TruncateWithForeignKeys(IContextTransaction contextTransaction);
    }
}
