namespace Bsg.EfCore.Configurations
{
    using Context;
    using Microsoft.Extensions.Configuration;

    public interface IConfigurationCacheService
    {
        IConfigurationSection ContextSettingsSection<TContext>()
            where TContext : IDbContext;
    }
}
