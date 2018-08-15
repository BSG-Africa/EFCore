namespace Bsg.EfCore.Tests.Services
{
    using System.Linq;
    using Bsg.EfCore.Tests.Data.Context;
    using Bsg.EfCore.Tests.Data.Domain;
    using Bsg.EfCore.Tests.Data.Repo;
    using Context;
    using Mapping;
    using Settings;
    using Utils;

    public class AlphaRepository : PrimaryRepository<Alpha>, IAlphaRepository
    {
        public AlphaRepository(
            IDbContextSession<PrimaryContext> session,
            IContextSettingsCacheService contextSettingsCacheService,
            IBulkInserterFactory bulkInserterFactory,
            ITableMappingCacheService tableMappingsCacheService)
            : base(session, contextSettingsCacheService, bulkInserterFactory, tableMappingsCacheService)
        {
        }

        public IQueryable<Alpha> Active()
        {
            return this.FindAll(e => e.IsActive);
        }
    }
}