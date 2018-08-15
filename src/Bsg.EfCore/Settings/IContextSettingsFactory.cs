namespace Bsg.EfCore.Settings
{
    using Context;
    using Dtos;

    public interface IContextSettingsFactory
    {
        ContextSettingDto BuildSettings<TContext>()
            where TContext : IDbContext;
    }
}