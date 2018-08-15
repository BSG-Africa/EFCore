namespace Bsg.EfCore.WarmUp
{
    using System;
    using System.Reflection;

    public interface IContextWarmUpService
    {
        void WarmUpAllContexts(params Assembly[] assembliesWithContexts);

        void WarmUpAllContexts(params Type[] types);
    }
}