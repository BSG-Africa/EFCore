namespace Bsg.EfCore.Mapping
{
    using System;
    using System.Reflection;

    public interface ITableMappingSetupService
    {
        void BuildAndCacheAllTableMappings(params Assembly[] assembliesWithContexts);

        void BuildAndCacheAllTableMappings(params Type[] types);
    }
}