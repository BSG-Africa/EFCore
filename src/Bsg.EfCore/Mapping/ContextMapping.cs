namespace Bsg.EfCore.Mapping
{
    using System;
    using System.Collections.Generic;
    using Context;
    using Domain;

    public class ContextMapping
    {
        public ContextMapping()
        {
            this.TableMappings = new Dictionary<Type, object>();
        }

        private IDictionary<Type, object> TableMappings { get; }

        public void AddMapping<TEntity, TContext>(TableMapping<TEntity, TContext> tableMapping) 
            where TEntity : class, IEntity<TContext>, new()
            where TContext : IDbContext
        {
            var entry = new KeyValuePair<Type, object>(typeof(TEntity), tableMapping);
            this.TableMappings.Add(entry);
        }

        public TableMapping<TEntity, TContext> GetMapping<TEntity, TContext>() 
            where TEntity : class, IEntity<TContext>, new()
            where TContext : IDbContext
        {
            return (TableMapping<TEntity, TContext>)this.TableMappings[typeof(TEntity)];
        }
    }
}
