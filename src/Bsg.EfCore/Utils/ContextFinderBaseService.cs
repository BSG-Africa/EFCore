namespace Bsg.EfCore.Utils
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Context;

    public abstract class ContextFinderBaseService
    {
        public IList<Type> GetContextTypesFromAssemblies(params Assembly[] assembliesWithContexts)
        {
            if (assembliesWithContexts == null)
            {
                throw new ArgumentNullException(nameof(assembliesWithContexts));
            }

            var allTypes = assembliesWithContexts.SelectMany(a => a.GetTypes()).ToArray();

            return this.GetContextTypesFromTypes(allTypes);
        }

        public IList<Type> GetContextTypesFromTypes(params Type[] types)
        {
            if (types == null)
            {
                throw new ArgumentNullException(nameof(types));
            }

            var dbContextInterfaceType = typeof(IDbContext);

            return types
                 .Where(t => !t.IsAbstract && dbContextInterfaceType.IsAssignableFrom(t))
                 .ToList();
        }
    }
}
