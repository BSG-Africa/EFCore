namespace Bsg.EfCore.Repo
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Dynamic.Core;
    using System.Linq.Expressions;
    using Context;
    using Domain;
    using Microsoft.EntityFrameworkCore;

    public class Repository<TEntity, TContext> : IRepository<TEntity, TContext>
        where TEntity : class, IEntity<TContext>, new()
        where TContext : IDbContext
    {
        #region Private Fields

        private readonly IDbContextSession<TContext> session;

        #endregion

        #region Constructors

        public Repository(IDbContextSession<TContext> session)
        {
            this.session = session;
        }

        #endregion

        #region Create

        public virtual void AddOne(TEntity item)
        {
            this.session.Add(item);
        }

        public virtual void AddAll(IEnumerable<TEntity> items)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            foreach (var item in items)
            {
                this.AddOne(item);
            }
        }

        #endregion

        #region Retrieve

        public virtual IQueryable<TEntity> FindAll()
        {
            return this.session.All<TEntity>().AsNoTracking();
        }

        public virtual IQueryable<TEntity> FindAll(Expression<Func<TEntity, bool>> predicate)
        {
            return this.FindAll().Where(predicate);
        }

        public IQueryable<TEntity> FindAll(
            Expression<Func<TEntity, bool>> predicate,
            string orderField,
            string orderDirection,
            int pageSize,
            int pageIndex)
        {
            return this.FindAll().Where(predicate).OrderBy(string.Concat(orderField, " ", orderDirection)).Skip(pageSize * pageIndex).Take(pageSize);
        }

        public virtual TEntity FindOne(Expression<Func<TEntity, bool>> predicate)
        {
            return this.FindAll().FirstOrDefault(predicate);
        }
           
        public virtual TEntity FindOne()
        {
            return this.FindAll().FirstOrDefault();
        }

        public virtual IQueryable<TEntity> FindAllTracked()
        {
            return this.session.All<TEntity>();
        }

        public virtual IQueryable<TEntity> FindAllTracked(Expression<Func<TEntity, bool>> predicate)
        {
            return this.FindAllTracked().Where(predicate);
        }

        public virtual IQueryable<TEntity> FindAllTracked(
            Expression<Func<TEntity, bool>> predicate,
            int pageSize,
            int pageIndex)
        {
            return this.FindAllTracked(predicate, "Id", "asc", pageSize, pageIndex);
        }

        public IQueryable<TEntity> FindAllTracked(
            Expression<Func<TEntity, bool>> predicate,
            string orderField,
            string orderDirection,
            int pageSize,
            int pageIndex)
        {
            return this.FindAllTracked().Where(predicate).OrderBy(string.Concat(orderField, " ", orderDirection)).Skip(pageSize * pageIndex).Take(pageSize);
        }

        public virtual TEntity FindOneTracked(Expression<Func<TEntity, bool>> predicate)
        {
            return this.FindAllTracked().FirstOrDefault(predicate);
        }

        public virtual TEntity FindOneTracked()
        {
            return this.FindAllTracked().FirstOrDefault();
        }

        public int CountAll()
        {
            return this.FindAll().Count();
        }

        public int CountAll(Expression<Func<TEntity, bool>> predicate)
        {
            return this.FindAll().Where(predicate).Count();
        }
        #endregion

        #region Delete

        public virtual void DeleteOne(TEntity item)
        {
            this.session.Delete(item);
        }

        public virtual void DeleteAll(Expression<Func<TEntity, bool>> predicate)
        {
            foreach (var item in this.FindAllTracked(predicate))
            {
                this.DeleteOne(item);
            }
        }

        public virtual void DeleteAll()
        {
            foreach (var item in this.FindAllTracked())
            {
                this.DeleteOne(item);
            }
        }

        public virtual void DeleteAll(IEnumerable<TEntity> items)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            foreach (var item in items)
            {
                this.DeleteOne(item);
            }
        }

        #endregion

        #region Utility Methods

        public void CommitChanges()
        {
            this.session.CommitChanges();
        }

        #endregion
    }
}