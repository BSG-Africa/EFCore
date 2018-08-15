namespace Bsg.EfCore.Tests.Data.Repo
{
    using Context;
    using EfCore.Context;
    using EfCore.Domain;
    using EfCore.Repo;
    using Mapping;
    using Settings;
    using Utils;

    public class PrimaryRepository<TEntity> : BulkInsertRepository<TEntity, PrimaryContext>, IPrimaryRepository<TEntity>
        where TEntity : class, IEntity<PrimaryContext>, new()
    {
        public PrimaryRepository(
            IDbContextSession<PrimaryContext> session,
            IContextSettingsCacheService contextSettingsCacheService,
            IBulkInserterFactory bulkInserterFactory,
            ITableMappingCacheService tableMappingCacheService)
            : base(session, contextSettingsCacheService, bulkInserterFactory, tableMappingCacheService)
        {
        }
    }
}
