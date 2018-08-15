namespace Bsg.EfCore.Context
{
    using System;
    using Domain;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Infrastructure;
    using Microsoft.EntityFrameworkCore.Metadata;

    public interface IDbContext : IDisposable
    {
        DatabaseFacade Database { get; }

        IModel Model { get; }

        DbSet<TEntity> EntitySet<TEntity, TContext>()
            where TEntity : class, IEntity<TContext>, new()
            where TContext : IDbContext;

        bool HasChanges();

        int SaveChanges();
    }
}
