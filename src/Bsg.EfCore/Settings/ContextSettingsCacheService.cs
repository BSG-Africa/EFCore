namespace Bsg.EfCore.Settings
{
    using System;
    using System.Collections.Generic;
    using Context;
    using Dtos;

    public class ContextSettingsCacheService : IContextSettingsCacheService
    {
        private readonly IDictionary<Type, ContextSettingDto> settingsCache;

        public ContextSettingsCacheService()
        {
            this.settingsCache = new Dictionary<Type, ContextSettingDto>();
        }

        public int ContextTimeout<TContext>()
            where TContext : IDbContext
        {
            return this.settingsCache[typeof(TContext)].ContextTimeout;
        }

        public int BulkInsertTimeout<TContext>()
            where TContext : IDbContext
        {
            return this.settingsCache[typeof(TContext)].BulkInsertTimeout;
        }

        public int BulkUpdateTimeout<TContext>()
            where TContext : IDbContext
        {
            return this.settingsCache[typeof(TContext)].BulkUpdateTimeout;
        }

        public string ConnectionString<TContext>() 
            where TContext : IDbContext
        {
            return this.settingsCache[typeof(TContext)].ConnectionString;
        }

        public string Provider<TContext>() 
            where TContext : IDbContext
        {
            return this.settingsCache[typeof(TContext)].ProviderName;
        }

        public bool EnableDbContextConsoleLogging<TContext>()
            where TContext : IDbContext
        {
            return this.settingsCache[typeof(TContext)].EnableDbContextConsoleLogging;
        }

        public void StoreSettings<TContext>(ContextSettingDto settings) 
            where TContext : IDbContext
        {
            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            this.settingsCache.Add(typeof(TContext), settings);
        }
    }
}