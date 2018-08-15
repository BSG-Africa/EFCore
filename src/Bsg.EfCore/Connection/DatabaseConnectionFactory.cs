namespace Bsg.EfCore.Connection
{
    using System.Data.Common;
    using Context;
    using Settings;
    using Utils;

    public class DatabaseConnectionFactory : IDatabaseConnectionFactory
    {
        private readonly IMsSqlConnectionFactory msSqlConnectionService;
        private readonly INonMsSqlConnectionFactory nonMsSqlConnectionFactory;
        private readonly IContextSettingsCacheService contextSettingsCacheService;

        public DatabaseConnectionFactory(
            IContextSettingsCacheService contextSettingsCacheService,
            IMsSqlConnectionFactory msSqlConnectionService,
            INonMsSqlConnectionFactory nonMsSqlConnectionFactory)
        {
            this.contextSettingsCacheService = contextSettingsCacheService;
            this.nonMsSqlConnectionFactory = nonMsSqlConnectionFactory;
            this.msSqlConnectionService = msSqlConnectionService;
        }

        public DbConnection BuildConnectionForContext<TContext>()
            where TContext : IDbContext
        {
            var providerName = this.contextSettingsCacheService.Provider<TContext>().ToLowerInvariant();
            var connectionString = this.contextSettingsCacheService.ConnectionString<TContext>();

            switch (providerName)
            {
                case "system.data.sqlclient":
                    return this.msSqlConnectionService.BuildConnection(connectionString);
                default:
                    return this.nonMsSqlConnectionFactory.BuildConnection(connectionString, providerName);
            }
        }
    }
}