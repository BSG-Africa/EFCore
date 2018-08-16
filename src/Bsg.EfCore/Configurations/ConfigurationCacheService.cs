namespace Bsg.EfCore.Configurations
{
    using System.IO;
    using Context;
    using Microsoft.Extensions.Configuration;

    public class ConfigurationCacheService : IConfigurationCacheService
    {
        private readonly object lockObj;
        private bool isSetupComplete;
        private IConfigurationRoot configuration;

        public ConfigurationCacheService()
        {
            this.lockObj = new object();
            this.isSetupComplete = false;
        }

        public IConfigurationSection ContextSettingsSection<TContext>()
            where TContext : IDbContext
        {
            var contextName = typeof(TContext).Name;
            return this.SafeConfigurationRoot().GetSection($"ContextSettings:{contextName}");
        }

        private IConfigurationRoot SafeConfigurationRoot()
        {
            if (!this.isSetupComplete)
            {
                lock (this.lockObj)
                {
                    this.configuration = this.BuildConfigurationRoot();
                    this.isSetupComplete = true;
                }
            }

            return this.configuration;
        }

        private IConfigurationRoot BuildConfigurationRoot()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");

            return builder.Build();
        }
    }
}