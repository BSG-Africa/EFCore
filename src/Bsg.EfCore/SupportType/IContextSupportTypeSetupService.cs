namespace Bsg.EfCore.SupportType
{
    using System;
    using System.Reflection;

    public interface IContextSupportTypeSetupService
    {
        void BuildAndCacheAllContextSupportingTypes(params Assembly[] assembliesWithContexts);

        void BuildAndCacheAllContextSupportingTypes(params Type[] types);
    }
}