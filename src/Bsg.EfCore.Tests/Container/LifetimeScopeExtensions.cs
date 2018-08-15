namespace Bsg.EfCore.Tests.Container
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Autofac;

    public static class LifetimeScopeExtensions
    {
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
        public static TService GetService<TService>(this ILifetimeScope scope)
            where TService : class
        {
            if (scope == null)
            {
                throw new ArgumentNullException(nameof(scope));
            }

            return scope.Resolve<TService>();
        }

        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
        public static object GetService(this ILifetimeScope scope, Type typeToResolve)
        {
            if (scope == null)
            {
                throw new ArgumentNullException(nameof(scope));
            }

            return scope.Resolve(typeToResolve);
        }

        public static ILifetimeScope CreateScopedContainer(this ILifetimeScope scope)
        {
            if (scope == null)
            {
                throw new ArgumentNullException(nameof(scope));
            }

            return scope.BeginLifetimeScope();
        }
    }
}