namespace Bsg.EfCore.Settings
{
    using System;
    using System.Reflection;

    public interface IContextSettingsSetupService
    {
        void BuildAndCacheAllSettings(params Assembly[] assembliesWithContexts);

        void BuildAndCacheAllSettings(params Type[] types);
    }
}