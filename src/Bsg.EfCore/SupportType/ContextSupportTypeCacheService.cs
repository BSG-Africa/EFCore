namespace Bsg.EfCore.SupportType
{
    using System;
    using System.Collections.Generic;
    using Context;
    using SupportType.Dtos;

    public class ContextSupportTypeCacheService : IContextSupportTypeCacheService
    {
        private readonly IDictionary<Type, ContextSupportTypeDto> contextSupportTypeCache;

        public ContextSupportTypeCacheService()
        {
            this.contextSupportTypeCache = new Dictionary<Type, ContextSupportTypeDto>();
        }

        public ContextSupportTypeDto RetrieveTypes<TContext>()
            where TContext : IDbContext
        {
            ContextSupportTypeDto supportTypes;

            if (this.contextSupportTypeCache.TryGetValue(typeof(TContext), out supportTypes))
            {
                return supportTypes;
            }

            throw new InvalidOperationException($"No context support types available for {nameof(TContext)} context.");
        }

        public void StoreTypes<TContext>(ContextSupportTypeDto supportTypes)
            where TContext : IDbContext
        {
            if (supportTypes == null)
            {
                throw new ArgumentNullException(nameof(supportTypes));
            }

            this.contextSupportTypeCache.Add(typeof(TContext), supportTypes);
        }
    }
}
