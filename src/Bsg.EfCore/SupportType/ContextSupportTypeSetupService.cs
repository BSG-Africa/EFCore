namespace Bsg.EfCore.SupportType
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using Context;
    using Dtos;
    using Utils;

    public class ContextSupportTypeSetupService : ContextFinderBaseService, IContextSupportTypeSetupService
    {
        private readonly IGenericReflectorService genericReflectorService;
        private readonly IContextSupportTypeCacheService contextSupportTypeCacheService;
        private readonly IContextSupportTypeFactory contextSupportTypeFactory;

        public ContextSupportTypeSetupService(
            IGenericReflectorService genericReflectorService,
            IContextSupportTypeCacheService contextSupportTypeCacheService,
            IContextSupportTypeFactory contextSupportTypeFactory)
        {
            this.contextSupportTypeFactory = contextSupportTypeFactory;
            this.genericReflectorService = genericReflectorService;
            this.contextSupportTypeCacheService = contextSupportTypeCacheService;
        }

        public void BuildAndCacheAllContextSupportingTypes(params Assembly[] assembliesWithContexts)
        {
            this.BuildAndCacheAllContextSupportingTypes(this.GetContextTypesFromAssemblies(assembliesWithContexts));
        }

        public void BuildAndCacheAllContextSupportingTypes(params Type[] types)
        {
            this.BuildAndCacheAllContextSupportingTypes(this.GetContextTypesFromTypes(types));
        }

        private void BuildAndCacheAllContextSupportingTypes(IList<Type> contextTypes)
        {
            foreach (var contextType in contextTypes)
            {
                // generate supporting types from factory in a generic (using reflection) manner
                var supportingTypes =
                    this.genericReflectorService.InvokeGenericMethodFromFunc<ContextSupportTypeDto>(
                    () => this.contextSupportTypeFactory.BuildContextTypes<IDbContext>(),
                    new[] { contextType },
                    this.contextSupportTypeFactory);

                // store supporting types in cache in a generic (using reflection) manner
                this.genericReflectorService.InvokeGenericMethodFromAction(
                    e => this.contextSupportTypeCacheService.StoreTypes<IDbContext>(supportingTypes),
                    new[] { contextType },
                    this.contextSupportTypeCacheService,
                    new object[] { supportingTypes });
            }
        }
    }
}
