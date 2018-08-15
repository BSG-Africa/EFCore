namespace Bsg.EfCore.Context
{
    using System;
    using System.Data.Common;
    using System.Diagnostics.CodeAnalysis;
    using Connection;
    using Logging;
    using Microsoft.EntityFrameworkCore;
    using Settings;
    using SupportType;

    public class DbContextFactory : IDbContextFactory
    {
        private readonly IContextSupportTypeCacheService contextSupportTypeCacheService;
        private readonly IContextLoggerFactoryService contextLoggerFactoryService;
        private readonly IContextSettingsCacheService contextSettingsCacheService;
        private readonly IDatabaseConnectionFactory connectionFactory;

        public DbContextFactory(
            IContextSupportTypeCacheService contextSupportTypeCacheService,
            IContextLoggerFactoryService contextLoggerFactoryService,
            IContextSettingsCacheService contextSettingsCacheService,
            IDatabaseConnectionFactory databaseConnectionFactory)
        {
            this.connectionFactory = databaseConnectionFactory;
            this.contextSettingsCacheService = contextSettingsCacheService;
            this.contextSupportTypeCacheService = contextSupportTypeCacheService;
            this.contextLoggerFactoryService = contextLoggerFactoryService;
        }

        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        public IDbContext BuildContext<TContext>()
            where TContext : IDbContext
        {
            var connection = this.connectionFactory.BuildConnectionForContext<TContext>();
            return this.ActualBuildContext<TContext>(connection);
        }

        private TContext ActualBuildContext<TContext>(DbConnection dbConnection)
            where TContext : IDbContext
        {
            var contextTimeout = this.contextSettingsCacheService.ContextTimeout<TContext>();

            var mustAttachLogger = this.contextSettingsCacheService.EnableDbContextConsoleLogging<TContext>();

            // TODO check - If connection is open will it be managed externally. If closed will be managed internally?
            // TODO extract this to not be dependant on SQL Server (similar to BuildConnection)
            var dbContextOptionsBuilder = new DbContextOptionsBuilder()
                .UseSqlServer(dbConnection, opts => opts.CommandTimeout(contextTimeout));

            if (mustAttachLogger)
            {
                dbContextOptionsBuilder.UseLoggerFactory(this.contextLoggerFactoryService.GetLoggerFactory());
            }

            var options = dbContextOptionsBuilder.Options;
            var contextSupportTypeDto = this.contextSupportTypeCacheService.RetrieveTypes<TContext>();

            return (TContext)Activator.CreateInstance(typeof(TContext), options, contextSupportTypeDto);
        }
    }
}