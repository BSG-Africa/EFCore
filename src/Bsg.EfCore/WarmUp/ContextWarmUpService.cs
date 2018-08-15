namespace Bsg.EfCore.WarmUp
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Context;
    using Utils;

    public class ContextWarmUpService : ContextFinderBaseService, IContextWarmUpService
    {
        private readonly IGenericReflectorService genericReflectorService;
        private readonly IDbContextFactory dbContextFactory;

        public ContextWarmUpService(
            IGenericReflectorService genericReflectorService,
            IDbContextFactory dbContextFactory)
        {
            this.genericReflectorService = genericReflectorService;
            this.dbContextFactory = dbContextFactory;
        }

        public void WarmUpAllContexts(params Assembly[] assembliesWithContexts)
        {
            this.WarmUpAllContexts(this.GetContextTypesFromAssemblies(assembliesWithContexts));
        }

        public void WarmUpAllContexts(params Type[] types)
        {
            this.WarmUpAllContexts(this.GetContextTypesFromTypes(types));
        }

        private void WarmUpAllContexts(IList<Type> contextTypes)
        {
            foreach (var contextType in contextTypes)
            {
                using (var context = this.genericReflectorService.InvokeGenericMethodFromFunc<IDbContext>(
                    () => this.dbContextFactory.BuildContext<IDbContext>(),
                    new[] { contextType },
                    this.dbContextFactory))
                {
                    this.WarmUp(context, contextType);
                }
            }
        }

        private void WarmUp(IDbContext context, Type contextType)
        {
            var firstEntityType = context.Model.GetEntityTypes().First().ClrType;
            var queryableOpenType = typeof(IQueryable<>);
            var extendedQueryableType = typeof(Queryable);

            var dbSet = this.genericReflectorService.InvokeGenericMethodFromMethodName<object>(
                typeof(IDbContext),
                nameof(IDbContext.EntitySet),
                new[] { firstEntityType, contextType },
                context);

            var countMethod =
                extendedQueryableType
                .GetMethods(BindingFlags.Static | BindingFlags.Public)
            .First(m =>
                 m.Name == "Count" &&
                 m.GetParameters().Count() == 1 &&
                 m.GetParameters().First().ParameterType.GetGenericTypeDefinition() == queryableOpenType);

            this.genericReflectorService.InvokeGenericMethodFromMethod<int>(countMethod, new[] { firstEntityType }, dbSet, new[] { dbSet });
        }
    }
}
