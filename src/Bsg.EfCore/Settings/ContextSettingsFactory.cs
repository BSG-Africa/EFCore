namespace Bsg.EfCore.Settings
{
    using Configurations;
    using Context;
    using Dtos;
    using Microsoft.Extensions.Configuration;

    public class ContextSettingsFactory : IContextSettingsFactory
    {
        private readonly IConfigurationCacheService configurationCacheService;

        public ContextSettingsFactory(IConfigurationCacheService configurationCacheService)
        {
            this.configurationCacheService = configurationCacheService;
        }

        public ContextSettingDto BuildSettings<TContext>()
            where TContext : IDbContext
        {
            var contextSettings = this.configurationCacheService.ContextSettingsSection<TContext>();

            var enableLogging = contextSettings.GetValue<bool>(nameof(ContextSettingDto.EnableDbContextConsoleLogging));
            var contextTimeout = contextSettings.GetValue<int>(nameof(ContextSettingDto.ContextTimeout));
            var bulkUpdateTimeout = contextSettings.GetValue<int>(nameof(ContextSettingDto.BulkUpdateTimeout));
            var bulkInsertTimeout = contextSettings.GetValue<int>(nameof(ContextSettingDto.BulkInsertTimeout));
            var connectionString = contextSettings.GetValue<string>(nameof(ContextSettingDto.ConnectionString));
            var providerName = contextSettings.GetValue<string>(nameof(ContextSettingDto.ProviderName));

            return new ContextSettingDto
            {
                EnableDbContextConsoleLogging = enableLogging,
                BulkUpdateTimeout = bulkUpdateTimeout,
                BulkInsertTimeout = bulkInsertTimeout,
                ContextTimeout = contextTimeout,
                ConnectionString = connectionString,
                ProviderName = providerName
            };
        }
    }
}
