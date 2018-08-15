namespace Bsg.EfCore.Mapping
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using Context;
    using Utils;

    public class TableMappingSetupService : ContextFinderBaseService, ITableMappingSetupService
    {
        private readonly IGenericReflectorService genericReflectorService;
        private readonly ITableMappingCacheService tableMappingCacheService;
        private readonly ITableMappingFactory tableMappingFactory;

        public TableMappingSetupService(
            IGenericReflectorService genericReflectorService,
            ITableMappingCacheService tableMappingCacheService,
            ITableMappingFactory tableMappingFactory)
        {
            this.tableMappingFactory = tableMappingFactory;
            this.genericReflectorService = genericReflectorService;
            this.tableMappingCacheService = tableMappingCacheService;
            this.tableMappingCacheService = tableMappingCacheService;
        }

        public void BuildAndCacheAllTableMappings(params Assembly[] assembliesWithContexts)
        {
            this.BuildAndCacheAllTableMappings(this.GetContextTypesFromAssemblies(assembliesWithContexts));
        }

        public void BuildAndCacheAllTableMappings(params Type[] types)
        {
            this.BuildAndCacheAllTableMappings(this.GetContextTypesFromTypes(types));
        }

        private void BuildAndCacheAllTableMappings(IList<Type> contextTypes)
        {
            foreach (var contextType in contextTypes)
            {
                // generate mappings from factory in a generic (using reflection) manner
                var tableMappings =
                    this.genericReflectorService.InvokeGenericMethodFromFunc<ContextMapping>(
                    () => this.tableMappingFactory.BuildContextMapping<IDbContext>(),
                    new[] { contextType },
                    this.tableMappingFactory);

                // store mappings in cache in a generic (using reflection) manner
                this.genericReflectorService.InvokeGenericMethodFromAction(
                    e => this.tableMappingCacheService.StoreContextMapping<IDbContext>(tableMappings),
                    new[] { contextType },
                    this.tableMappingCacheService,
                    new[] { tableMappings });
            }
        }
    }
}
