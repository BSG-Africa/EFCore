namespace Bsg.EfCore.Settings
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using Context;
    using Dtos;
    using Utils;

    public class ContextSettingsSetupService : ContextFinderBaseService, IContextSettingsSetupService
    {
        private readonly IGenericReflectorService genericReflectorService;
        private readonly IContextSettingsCacheService contextSettingCacheService;
        private readonly IContextSettingsFactory contextSettingFactory;

        public ContextSettingsSetupService(
            IGenericReflectorService genericReflectorService,
            IContextSettingsCacheService contextSettingCacheService,
            IContextSettingsFactory contextSettingFactory)
        {
            this.contextSettingFactory = contextSettingFactory;
            this.genericReflectorService = genericReflectorService;
            this.contextSettingCacheService = contextSettingCacheService;
        }

        public void BuildAndCacheAllSettings(params Assembly[] assembliesWithContexts)
        {
            this.BuildAndCacheAllSettings(this.GetContextTypesFromAssemblies(assembliesWithContexts));
        }

        public void BuildAndCacheAllSettings(params Type[] types)
        {
            this.BuildAndCacheAllSettings(this.GetContextTypesFromTypes(types));
        }

        private void BuildAndCacheAllSettings(IList<Type> contextTypes)
        {
            foreach (var contextType in contextTypes)
            {
                // generate contextSettings from factory in a generic (using reflection) manner
                var contextSettings =
                    this.genericReflectorService.InvokeGenericMethodFromFunc<ContextSettingDto>(
                    () => this.contextSettingFactory.BuildSettings<IDbContext>(),
                    new[] { contextType },
                    this.contextSettingFactory);

                // store contextSettings in cache in a generic (using reflection) manner
                this.genericReflectorService.InvokeGenericMethodFromAction(
                    e => this.contextSettingCacheService.StoreSettings<IDbContext>(contextSettings),
                    new[] { contextType },
                    this.contextSettingCacheService,
                    new[] { contextSettings });
            }
        }
    }
}
