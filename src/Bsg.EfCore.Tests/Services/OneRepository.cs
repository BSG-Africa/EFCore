namespace Bsg.EfCore.Tests.Services
{
    using System.Linq;
    using Bsg.EfCore.Tests.Data.Context;
    using Bsg.EfCore.Tests.Data.Domain;
    using Context;
    using Mapping;
    using Repo;
    using Settings;
    using Utils;

    public class OneRepository : BulkInsertRepository<One, SecondaryContext>, IOneRepository
    {
        public OneRepository(
            IDbContextSession<SecondaryContext> session,
            IContextSettingsCacheService contextSettingsCacheService,
            IBulkInserterFactory bulkInserterFactory,
            ITableMappingCacheService tableMappingsCacheService)
            : base(session, contextSettingsCacheService, bulkInserterFactory, tableMappingsCacheService)
        {
        }

        public IQueryable<One> AllJohns()
        {
            return this.FindAll(e => e.Name.ToLower() == "john");
        } 
    }
}
