namespace Bsg.EfCore.Mapping
{
    using Context;

    public interface ITableMappingCacheService
    {
        ContextMapping RetrieveContextMapping<TContext>()
            where TContext : IDbContext;

        void StoreContextMapping<TContext>(ContextMapping mappings)
            where TContext : IDbContext;
    }
}