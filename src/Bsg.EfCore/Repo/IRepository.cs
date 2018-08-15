namespace Bsg.EfCore.Repo
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using Context;
    using Domain;

    public interface IRepository<TEntity, TContext>
        where TEntity : class, IEntity<TContext>, new()
        where TContext : IDbContext
    {
        void AddOne(TEntity item);

        void AddAll(IEnumerable<TEntity> items);

        IQueryable<TEntity> FindAll();

        IQueryable<TEntity> FindAll(Expression<Func<TEntity, bool>> predicate);

        IQueryable<TEntity> FindAll(
            Expression<Func<TEntity, bool>> predicate,
            string orderField,
            string orderDirection,
            int pageSize,
            int pageIndex);

        TEntity FindOne(Expression<Func<TEntity, bool>> predicate);

        TEntity FindOne();

        IQueryable<TEntity> FindAllTracked();

        IQueryable<TEntity> FindAllTracked(Expression<Func<TEntity, bool>> predicate);

        IQueryable<TEntity> FindAllTracked(
            Expression<Func<TEntity, bool>> predicate,
            int pageSize,
            int pageIndex);

        IQueryable<TEntity> FindAllTracked(
            Expression<Func<TEntity, bool>> predicate,
            string orderField,
            string orderDirection,
            int pageSize,
            int pageIndex);

        TEntity FindOneTracked(Expression<Func<TEntity, bool>> predicate);

        TEntity FindOneTracked();

        int CountAll();

        int CountAll(Expression<Func<TEntity, bool>> predicate);

        void DeleteOne(TEntity item);

        void DeleteAll(Expression<Func<TEntity, bool>> predicate);

        void DeleteAll();

        void DeleteAll(IEnumerable<TEntity> items);
    }
}