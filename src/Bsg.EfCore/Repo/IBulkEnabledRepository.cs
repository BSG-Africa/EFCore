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
        int BulkDelete(Expression<Func<TEntity, bool>> predicate);

        int BulkDelete(Expression<Func<TEntity, bool>> predicate, IContextTransaction contextTransaction);

        int BulkDelete(IQueryable<TEntity> target);

        int BulkDelete(
            IQueryable<TEntity> target,
            IContextTransaction contextTransaction);

        int BulkUpdate(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TEntity>> updateExpression);

        int BulkUpdate(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TEntity>> updateExpression, IContextTransaction contextTransaction);

        int BulkUpdate(IQueryable<TEntity> target, Expression<Func<TEntity, TEntity>> updateExpression);

        int BulkUpdate(
            IQueryable<TEntity> target,
            Expression<Func<TEntity, TEntity>> updateExpression,
            IContextTransaction contextTransaction);

        int BulkSelectAndUpdate(IQueryable<TEntity> query);

        int BulkSelectAndUpdate(IQueryable<TEntity> query, IContextTransaction contextTransaction);

        int Truncate();

        int Truncate(IContextTransaction contextTransaction);

        int TruncateWithForeignKeys();

        int TruncateWithForeignKeys(IContextTransaction contextTransaction);
    }
}
