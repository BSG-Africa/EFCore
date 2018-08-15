namespace Bsg.EfCore.Mapping
{
    using Context;

    public interface ITableMappingFactory
    {
        ContextMapping BuildContextMapping<TContext>()
            where TContext : IDbContext;
    }
}