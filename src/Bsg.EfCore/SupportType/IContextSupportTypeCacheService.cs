namespace Bsg.EfCore.SupportType
{
    using Context;
    using SupportType.Dtos;

    public interface IContextSupportTypeCacheService
    {
        ContextSupportTypeDto RetrieveTypes<TContext>() 
            where TContext : IDbContext;

        void StoreTypes<TContext>(ContextSupportTypeDto supportTypes) 
            where TContext : IDbContext;
    }
}