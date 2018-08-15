namespace Bsg.EfCore.Tests.Container
{
    using System;
    using System.Reflection;
    using Autofac;
    using Bsg.EfCore.Tests.Data.Repo;
    using Configurations;
    using Context;
    using Mapping;
    using Repo;
    using Settings;
    using SupportType;
    using Transactions;
    using WarmUp;

    public class TestContainerBootstrap : AutofacContainerBootStrapperBase
    {
        public override void ConfigureContainerBeforeBuild(ContainerBuilder builder)
        {
            var efCoreAssembly = Assembly.GetAssembly(typeof(IDbContext));
            var testAssembly = Assembly.GetAssembly(typeof(TestContainerBootstrap));

            // Setup Scoped Container factory lamda expression
            builder.Register(ctx => ctx.Resolve<ILifetimeScope>().BeginLifetimeScope() as IServiceProvider).As<IServiceProvider>();

            this.RegisterDefaultConventionTypes(builder, efCoreAssembly, testAssembly);

            // Open Generics
            builder.RegisterGeneric(typeof(GenericRepository<,>)).As(typeof(IGenericRepository<,>));
            builder.RegisterGeneric(typeof(PrimaryRepository<>)).As(typeof(IPrimaryRepository<>));
            builder.RegisterGeneric(typeof(GenericTransactionService<>)).As(typeof(IGenericTransactionService<>));

            // register individual singleton per scope services 
            // or other services which don't fit default conventions
            // will override any previous registrations
            builder.RegisterGeneric(typeof(DbContextSession<>)).As(typeof(IDbContextSession<>)).InstancePerLifetimeScope();

            // Singleton instances across the entire application
            builder.RegisterType<ConfigurationCacheService>().As<IConfigurationCacheService>().SingleInstance();
            builder.RegisterType<ContextSettingsCacheService>().As<IContextSettingsCacheService>().SingleInstance();
            builder.RegisterType<ContextSupportTypeCacheService>().As<IContextSupportTypeCacheService>().SingleInstance();
            builder.RegisterType<TableMappingCacheService>().As<ITableMappingCacheService>().SingleInstance();
        }

        public override void ConfigureContainerAfterBuild(ILifetimeScope container)
        {
            var testAssembly = Assembly.GetAssembly(typeof(TestContainerBootstrap));

            // Cache Timeouts
            var contextSettingsSetupService = container.GetService<IContextSettingsSetupService>();
            contextSettingsSetupService.BuildAndCacheAllSettings(testAssembly);

            // Cache Support Types
            var supportTypeSetupService = container.GetService<IContextSupportTypeSetupService>();
            supportTypeSetupService.BuildAndCacheAllContextSupportingTypes(testAssembly);

            // Cache Mappings
            var mappingSetupService = container.GetService<ITableMappingSetupService>();
            mappingSetupService.BuildAndCacheAllTableMappings(testAssembly);

            // WarmUp Contexts 
            // var warmUpService = container.GetService<IContextWarmUpService>();
            // warmUpService.WarmUpAllContexts(testAssembly);
        }
    }
}