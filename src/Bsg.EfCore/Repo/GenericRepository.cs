namespace Bsg.EfCore.Repo
{
    using Context;
    using Domain;
    using Mapping;
    using Settings;
    using Utils;

    public class GenericRepository<TEntity, TContext> : BulkInsertRepository<TEntity, TContext>, IGenericRepository<TEntity, TContext> 
        where TEntity : class, IEntity<TContext>, new()
        where TContext : IDbContext
    {
        public GenericRepository(
            IDbContextSession<TContext> session, 
            IContextSettingsCacheService contextSettingsCacheService, 
            IBulkInserterFactory bulkInserterFactory,
            ITableMappingCacheService tableMappingCacheService)
            : base(session, contextSettingsCacheService, bulkInserterFactory, tableMappingCacheService)
        {
        }
    }
}