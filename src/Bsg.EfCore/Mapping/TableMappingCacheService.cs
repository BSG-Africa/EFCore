namespace Bsg.EfCore.Mapping
{
    using System;
    using System.Collections.Generic;
    using Context;

    public class TableMappingCacheService : ITableMappingCacheService
    {
        private readonly IDictionary<Type, ContextMapping> tableMappingCache;

        public TableMappingCacheService()
        {
            this.tableMappingCache = new Dictionary<Type, ContextMapping>();
        }

        public ContextMapping RetrieveContextMapping<TContext>() 
            where TContext : IDbContext
        {
            if (this.tableMappingCache.TryGetValue(typeof(TContext), out var contextMappings))
            {
                return contextMappings;
            }

            throw new InvalidOperationException($"No mappings available for {nameof(TContext)} context.");
        }

        public void StoreContextMapping<TContext>(ContextMapping mappings) 
            where TContext : IDbContext
        {
            if (mappings == null)
            {
                throw new ArgumentNullException(nameof(mappings));
            }

            this.tableMappingCache.Add(typeof(TContext), mappings);
        }
    }
}
